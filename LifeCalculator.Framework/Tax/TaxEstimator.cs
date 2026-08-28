using LifeCalculator.Framework.Enums;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Tax
{
    /// <summary>
    /// Estimates US federal income tax, FICA, and (via a flat user-supplied rate) state tax,
    /// to turn a gross salary into an approximate take-home figure.
    ///
    /// This is an ESTIMATE, not tax advice. It deliberately models only the common case:
    /// wage income, the standard deduction, and ordinary brackets. It does not handle itemized
    /// deductions, credits (child tax credit, EITC...), other income types, local/city taxes,
    /// or state-specific rules — hence the flat state rate input.
    ///
    /// Figures are for the <see cref="TaxYear"/> below; update the tables when the IRS
    /// publishes new numbers.
    /// </summary>
    public static class TaxEstimator
    {
        public const int TaxYear = 2025;

        private class Bracket
        {
            public double UpTo { get; set; }
            public double Rate { get; set; }

            public Bracket(double upTo, double rate)
            {
                UpTo = upTo;
                Rate = rate;
            }
        }

        // 2025 Social Security wage base and Medicare rates.
        private const double SocialSecurityWageBase = 176100;
        private const double SocialSecurityRate = 0.062;
        private const double MedicareRate = 0.0145;
        private const double AdditionalMedicareRate = 0.009;

        public static double GetStandardDeduction(FilingStatus status)
        {
            switch (status)
            {
                case FilingStatus.MarriedFilingJointly: return 30000;
                case FilingStatus.HeadOfHousehold: return 22500;
                case FilingStatus.MarriedFilingSeparately: return 15000;
                default: return 15000; // Single
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

        private static List<Bracket> GetFederalBrackets(FilingStatus status)
        {
            switch (status)
            {
                case FilingStatus.MarriedFilingJointly:
                    return new List<Bracket>
                    {
                        new Bracket(23850, 0.10),
                        new Bracket(96950, 0.12),
                        new Bracket(206700, 0.22),
                        new Bracket(394600, 0.24),
                        new Bracket(501050, 0.32),
                        new Bracket(751600, 0.35),
                        new Bracket(double.MaxValue, 0.37)
                    };

                case FilingStatus.MarriedFilingSeparately:
                    return new List<Bracket>
                    {
                        new Bracket(11925, 0.10),
                        new Bracket(48475, 0.12),
                        new Bracket(103350, 0.22),
                        new Bracket(197300, 0.24),
                        new Bracket(250525, 0.32),
                        new Bracket(375800, 0.35),
                        new Bracket(double.MaxValue, 0.37)
                    };

                case FilingStatus.HeadOfHousehold:
                    return new List<Bracket>
                    {
                        new Bracket(17000, 0.10),
                        new Bracket(64850, 0.12),
                        new Bracket(103350, 0.22),
                        new Bracket(197300, 0.24),
                        new Bracket(250500, 0.32),
                        new Bracket(626350, 0.35),
                        new Bracket(double.MaxValue, 0.37)
                    };

                default: // Single
                    return new List<Bracket>
                    {
                        new Bracket(11925, 0.10),
                        new Bracket(48475, 0.12),
                        new Bracket(103350, 0.22),
                        new Bracket(197300, 0.24),
                        new Bracket(250525, 0.32),
                        new Bracket(626350, 0.35),
                        new Bracket(double.MaxValue, 0.37)
                    };
            }
        }

        /// <param name="grossAnnual">Salary before any tax or deductions.</param>
        /// <param name="status">Federal filing status.</param>
        /// <param name="preTaxDeductionsAnnual">
        /// Traditional 401(k), HSA, health premiums etc. These reduce income subject to federal
        /// and state tax. They do NOT reduce Social Security/Medicare wages here — true for
        /// 401(k) contributions; Section 125 health premiums would also escape FICA, so FICA
        /// may be slightly overstated if this figure includes them.
        /// </param>
        /// <param name="stateTaxRatePercent">Flat effective state rate, e.g. 5 for 5%. Use 0 for no-income-tax states.</param>
        public static TaxEstimate Estimate(double grossAnnual, FilingStatus status, double preTaxDeductionsAnnual, double stateTaxRatePercent)
        {
            if (grossAnnual < 0) grossAnnual = 0;
            if (preTaxDeductionsAnnual < 0) preTaxDeductionsAnnual = 0;
            if (preTaxDeductionsAnnual > grossAnnual) preTaxDeductionsAnnual = grossAnnual;
            if (stateTaxRatePercent < 0) stateTaxRatePercent = 0;

            double incomeAfterPreTax = grossAnnual - preTaxDeductionsAnnual;
            double taxableIncome = Math.Max(0, incomeAfterPreTax - GetStandardDeduction(status));

            var estimate = new TaxEstimate
            {
                GrossAnnual = grossAnnual,
                PreTaxDeductions = preTaxDeductionsAnnual,
                FederalTax = Math.Round(CalculateBracketedTax(taxableIncome, GetFederalBrackets(status)), 2),
                SocialSecurityTax = Math.Round(Math.Min(grossAnnual, SocialSecurityWageBase) * SocialSecurityRate, 2),
                MedicareTax = Math.Round(CalculateMedicare(grossAnnual, status), 2),
                StateTax = Math.Round(taxableIncome * (stateTaxRatePercent / 100), 2)
            };

            return estimate;
        }

        /// <summary>Applies marginal rates band by band — only income above each threshold is taxed at the higher rate.</summary>
        private static double CalculateBracketedTax(double taxableIncome, List<Bracket> brackets)
        {
            double tax = 0;
            double lowerBound = 0;

            foreach (var bracket in brackets)
            {
                if (taxableIncome <= lowerBound)
                    break;

                double amountInBracket = Math.Min(taxableIncome, bracket.UpTo) - lowerBound;
                tax += amountInBracket * bracket.Rate;
                lowerBound = bracket.UpTo;
            }

            return tax;
        }

        private static double CalculateMedicare(double grossAnnual, FilingStatus status)
        {
            double medicare = grossAnnual * MedicareRate;

            double threshold = GetAdditionalMedicareThreshold(status);
            if (grossAnnual > threshold)
                medicare += (grossAnnual - threshold) * AdditionalMedicareRate;

            return medicare;
        }
    }
}
