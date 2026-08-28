using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Income;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Framework.Tax
{
    /// <summary>
    /// Estimates tax across ALL income streams together.
    ///
    /// Why combined rather than per-stream: income tax is progressive over total income. A
    /// freelance gig on top of a salary is taxed at the marginal rate that salary already
    /// pushed you into — not at its own standalone rate starting from the 10% bracket with a
    /// fresh standard deduction. Taxing each stream in isolation would badly understate tax.
    ///
    /// What DOES vary per stream is payroll tax:
    ///   - W-2 wages:        6.2% Social Security + 1.45% Medicare (employee half only)
    ///   - Self-employment:  both halves (~15.3%), on 92.35% of net earnings per IRS rules
    ///   - Rental/investment: no payroll tax at all
    ///
    /// Same caveats as <see cref="TaxEstimator"/>: an estimate, standard deduction only, no
    /// credits, flat state rate.
    /// </summary>
    public static class HouseholdTaxEstimator
    {
        private const double SocialSecurityWageBase = 176100;
        private const double SocialSecurityRate = 0.062;
        private const double MedicareRate = 0.0145;
        private const double AdditionalMedicareRate = 0.009;

        // Self-employment tax applies to 92.35% of net earnings, and is the combined
        // employer+employee rate.
        private const double SelfEmploymentIncomeFactor = 0.9235;
        private const double SelfEmploymentSocialSecurityRate = 0.124;
        private const double SelfEmploymentMedicareRate = 0.029;

        /// <summary>Half of SE tax is deductible against income tax.</summary>
        private const double SelfEmploymentDeductibleShare = 0.5;

        public static HouseholdTaxEstimate Estimate(
            IEnumerable<IncomeStream> streams,
            FilingStatus status,
            double preTaxDeductionsAnnual,
            double stateTaxRatePercent)
        {
            var estimate = new HouseholdTaxEstimate();

            var allStreams = (streams ?? Enumerable.Empty<IncomeStream>()).ToList();
            if (preTaxDeductionsAnnual < 0) preTaxDeductionsAnnual = 0;
            if (stateTaxRatePercent < 0) stateTaxRatePercent = 0;

            var grossStreams = allStreams.Where(s => s.IsGross).ToList();
            var netStreams = allStreams.Where(s => !s.IsGross).ToList();

            double w2Wages = grossStreams
                .Where(s => s.TaxTreatment == IncomeTaxTreatment.W2Wages)
                .Sum(s => s.MonthlyAmount * 12);

            double selfEmploymentIncome = grossStreams
                .Where(s => s.TaxTreatment == IncomeTaxTreatment.SelfEmployment)
                .Sum(s => s.MonthlyAmount * 12);

            double untaxedPayrollIncome = grossStreams
                .Where(s => s.TaxTreatment == IncomeTaxTreatment.NoPayrollTax)
                .Sum(s => s.MonthlyAmount * 12);

            double totalGross = w2Wages + selfEmploymentIncome + untaxedPayrollIncome;

            if (preTaxDeductionsAnnual > totalGross)
                preTaxDeductionsAnnual = totalGross;

            estimate.GrossAnnual = totalGross;
            estimate.PreTaxDeductions = preTaxDeductionsAnnual;
            estimate.AlreadyNetAnnual = netStreams.Sum(s => s.MonthlyAmount * 12);

            // --- Payroll taxes, per treatment ---

            // Social Security is capped at the wage base across W-2 and SE earnings combined;
            // W-2 wages consume the base first.
            estimate.SocialSecurityTax = Math.Round(Math.Min(w2Wages, SocialSecurityWageBase) * SocialSecurityRate, 2);
            estimate.MedicareTax = Math.Round(w2Wages * MedicareRate, 2);

            double seTaxableEarnings = selfEmploymentIncome * SelfEmploymentIncomeFactor;
            double remainingSsBase = Math.Max(0, SocialSecurityWageBase - w2Wages);
            double seSocialSecurity = Math.Min(seTaxableEarnings, remainingSsBase) * SelfEmploymentSocialSecurityRate;
            double seMedicare = seTaxableEarnings * SelfEmploymentMedicareRate;
            estimate.SelfEmploymentTax = Math.Round(seSocialSecurity + seMedicare, 2);

            // Additional Medicare surtax applies across combined earned income.
            double earnedIncome = w2Wages + seTaxableEarnings;
            double surtaxThreshold = GetAdditionalMedicareThreshold(status);
            if (earnedIncome > surtaxThreshold)
            {
                double surtax = Math.Round((earnedIncome - surtaxThreshold) * AdditionalMedicareRate, 2);
                estimate.MedicareTax = Math.Round(estimate.MedicareTax + surtax, 2);
            }

            // --- Income tax, on combined income ---

            double halfSelfEmploymentTax = estimate.SelfEmploymentTax * SelfEmploymentDeductibleShare;

            double incomeSubjectToTax = totalGross
                - preTaxDeductionsAnnual
                - halfSelfEmploymentTax
                - TaxEstimator.GetStandardDeduction(status);

            double taxableIncome = Math.Max(0, incomeSubjectToTax);

            estimate.FederalTax = Math.Round(CalculateFederalTax(taxableIncome, status), 2);
            estimate.StateTax = Math.Round(taxableIncome * (stateTaxRatePercent / 100), 2);

            BuildPerStreamBreakdown(estimate, allStreams, totalGross);

            return estimate;
        }

        /// <summary>
        /// Splits the household take-home back across the gross streams in proportion to their
        /// share of gross, so each row can show a meaningful per-stream figure. Streams already
        /// entered as take-home pass through unchanged.
        /// </summary>
        private static void BuildPerStreamBreakdown(HouseholdTaxEstimate estimate, List<IncomeStream> allStreams, double totalGross)
        {
            foreach (var stream in allStreams)
            {
                double monthly = stream.MonthlyAmount;

                if (!stream.IsGross)
                {
                    estimate.NetMonthlyByStreamId[stream.Id] = Math.Round(monthly, 2);
                    continue;
                }

                if (totalGross <= 0)
                {
                    estimate.NetMonthlyByStreamId[stream.Id] = 0;
                    continue;
                }

                double share = (monthly * 12) / totalGross;
                estimate.NetMonthlyByStreamId[stream.Id] = Math.Round((estimate.NetFromGrossAnnual * share) / 12, 2);
            }
        }

        private static double GetAdditionalMedicareThreshold(FilingStatus status)
        {
            switch (status)
            {
                case FilingStatus.MarriedFilingJointly: return 250000;
                case FilingStatus.MarriedFilingSeparately: return 125000;
                default: return 200000;
            }
        }

        /// <summary>
        /// Reuses TaxEstimator's bracket tables by asking it for tax on an equivalent
        /// wage-only scenario, keeping a single source of truth for the rate schedule.
        /// </summary>
        private static double CalculateFederalTax(double taxableIncome, FilingStatus status)
        {
            // TaxEstimator.Estimate subtracts the standard deduction itself, so add it back
            // here: we've already applied it (plus the SE-tax and pre-tax adjustments).
            double grossEquivalent = taxableIncome + TaxEstimator.GetStandardDeduction(status);
            return TaxEstimator.Estimate(grossEquivalent, status, 0, 0).FederalTax;
        }
    }
}
