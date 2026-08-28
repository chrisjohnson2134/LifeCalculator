using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Income;
using LifeCalculator.Framework.Tax;
using NUnit.Framework;
using Should;
using System;
using System.Collections.Generic;

namespace LifeCalcuator.FrameworkTest.Tax
{
    [TestFixture]
    public class HouseholdTaxEstimatorTest
    {
        private static IncomeStream Gross(int id, double monthly, IncomeTaxTreatment treatment)
        {
            return new IncomeStream
            {
                Id = id,
                MonthlyAmount = monthly,
                IsGross = true,
                TaxTreatment = treatment,
                StartDate = new DateTime(2026, 1, 1)
            };
        }

        private static IncomeStream TakeHome(int id, double monthly)
        {
            return new IncomeStream
            {
                Id = id,
                MonthlyAmount = monthly,
                IsGross = false,
                StartDate = new DateTime(2026, 1, 1)
            };
        }

        [Test]
        public void NoStreams_ProducesEmptyEstimate()
        {
            var estimate = HouseholdTaxEstimator.Estimate(new List<IncomeStream>(), FilingStatus.Single, 0, 0);

            estimate.GrossAnnual.ShouldEqual(0.0);
            estimate.TotalTax.ShouldEqual(0.0);
            estimate.TotalNetMonthly.ShouldEqual(0.0);
        }

        [Test]
        public void TakeHomeStreams_PassThroughUntaxed()
        {
            var streams = new List<IncomeStream> { TakeHome(1, 4000) };

            var estimate = HouseholdTaxEstimator.Estimate(streams, FilingStatus.Single, 0, 0);

            estimate.GrossAnnual.ShouldEqual(0.0);
            estimate.TotalTax.ShouldEqual(0.0);
            estimate.AlreadyNetAnnual.ShouldEqual(48000.0);
            estimate.TotalNetMonthly.ShouldEqual(4000.0);
        }

        [Test]
        public void SingleW2Stream_MatchesStandaloneSalaryEstimate()
        {
            // A lone W-2 stream should agree with the simpler per-salary estimator.
            var streams = new List<IncomeStream> { Gross(1, 5000, IncomeTaxTreatment.W2Wages) };

            var household = HouseholdTaxEstimator.Estimate(streams, FilingStatus.Single, 0, 0);
            var standalone = TaxEstimator.Estimate(60000, FilingStatus.Single, 0, 0);

            household.GrossAnnual.ShouldEqual(60000.0);
            household.FederalTax.ShouldEqual(standalone.FederalTax);
            household.SocialSecurityTax.ShouldEqual(standalone.SocialSecurityTax);
            household.MedicareTax.ShouldEqual(standalone.MedicareTax);
        }

        [Test]
        public void SecondStream_IsTaxedAtMarginalRate_NotAsFreshStandaloneIncome()
        {
            // The whole reason for a combined model: taxing two $60k streams separately would
            // give each its own standard deduction and restart each at the 10% bracket,
            // understating the real bill.
            var combined = HouseholdTaxEstimator.Estimate(
                new List<IncomeStream>
                {
                    Gross(1, 5000, IncomeTaxTreatment.W2Wages),
                    Gross(2, 5000, IncomeTaxTreatment.W2Wages)
                },
                FilingStatus.Single, 0, 0);

            double taxedSeparately = TaxEstimator.Estimate(60000, FilingStatus.Single, 0, 0).FederalTax * 2;

            Assert.Greater(combined.FederalTax, taxedSeparately);

            // And it should match a single $120k earner.
            var asOneSalary = TaxEstimator.Estimate(120000, FilingStatus.Single, 0, 0);
            combined.FederalTax.ShouldEqual(asOneSalary.FederalTax);
        }

        [Test]
        public void SelfEmploymentStream_PaysBothHalvesOfFica()
        {
            // $60k self-employed: SE tax on 92.35% of earnings = 55,410
            //   Social Security 12.4% on 55,410 = 6,870.84
            //   Medicare        2.9%  on 55,410 = 1,606.89
            //   total                            = 8,477.73
            var streams = new List<IncomeStream> { Gross(1, 5000, IncomeTaxTreatment.SelfEmployment) };

            var estimate = HouseholdTaxEstimator.Estimate(streams, FilingStatus.Single, 0, 0);

            estimate.SelfEmploymentTax.ShouldEqual(8477.73);

            // No employee-side payroll tax, since there are no W-2 wages.
            estimate.SocialSecurityTax.ShouldEqual(0.0);
            estimate.MedicareTax.ShouldEqual(0.0);
        }

        [Test]
        public void SelfEmployment_IsTaxedMoreHeavilyThanEquivalentW2Wages()
        {
            var w2 = HouseholdTaxEstimator.Estimate(
                new List<IncomeStream> { Gross(1, 5000, IncomeTaxTreatment.W2Wages) },
                FilingStatus.Single, 0, 0);

            var freelance = HouseholdTaxEstimator.Estimate(
                new List<IncomeStream> { Gross(1, 5000, IncomeTaxTreatment.SelfEmployment) },
                FilingStatus.Single, 0, 0);

            Assert.Greater(freelance.TotalTax, w2.TotalTax);
        }

        [Test]
        public void RentalStream_PaysNoPayrollTaxButStillOwesIncomeTax()
        {
            var streams = new List<IncomeStream> { Gross(1, 5000, IncomeTaxTreatment.NoPayrollTax) };

            var estimate = HouseholdTaxEstimator.Estimate(streams, FilingStatus.Single, 0, 0);

            estimate.SocialSecurityTax.ShouldEqual(0.0);
            estimate.MedicareTax.ShouldEqual(0.0);
            estimate.SelfEmploymentTax.ShouldEqual(0.0);
            Assert.Greater(estimate.FederalTax, 0);
        }

        [Test]
        public void SocialSecurityWageBase_IsSharedAcrossW2AndSelfEmployment()
        {
            // $170k W-2 leaves only 6,100 of the 176,100 base for SE earnings.
            var streams = new List<IncomeStream>
            {
                Gross(1, 170000 / 12.0, IncomeTaxTreatment.W2Wages),
                Gross(2, 50000 / 12.0, IncomeTaxTreatment.SelfEmployment)
            };

            var estimate = HouseholdTaxEstimator.Estimate(streams, FilingStatus.Single, 0, 0);

            // SE Social Security portion is capped by the remaining base, not charged in full.
            double seEarnings = 50000 * 0.9235;
            double uncappedSeSocialSecurity = seEarnings * 0.124;

            Assert.Less(estimate.SelfEmploymentTax, uncappedSeSocialSecurity + (seEarnings * 0.029));
        }

        [Test]
        public void MixedGrossAndTakeHome_CombinesBothIntoTotalNet()
        {
            var streams = new List<IncomeStream>
            {
                Gross(1, 5000, IncomeTaxTreatment.W2Wages),
                TakeHome(2, 1000)
            };

            var estimate = HouseholdTaxEstimator.Estimate(streams, FilingStatus.Single, 0, 0);

            estimate.AlreadyNetAnnual.ShouldEqual(12000.0);
            estimate.TotalNetAnnual.ShouldEqual(estimate.NetFromGrossAnnual + 12000.0);
        }

        [Test]
        public void PerStreamBreakdown_CoversEveryStream()
        {
            var streams = new List<IncomeStream>
            {
                Gross(1, 5000, IncomeTaxTreatment.W2Wages),
                Gross(2, 2000, IncomeTaxTreatment.SelfEmployment),
                TakeHome(3, 1000)
            };

            var estimate = HouseholdTaxEstimator.Estimate(streams, FilingStatus.Single, 0, 0);

            estimate.NetMonthlyByStreamId.Count.ShouldEqual(3);

            // Take-home stream is reported unchanged.
            estimate.NetMonthlyByStreamId[3].ShouldEqual(1000.0);

            // Gross streams are reported net, so below what was entered.
            Assert.Less(estimate.NetMonthlyByStreamId[1], 5000);
            Assert.Greater(estimate.NetMonthlyByStreamId[1], 0);
        }

        [Test]
        public void PreTaxDeductions_ReduceIncomeTaxAcrossHousehold()
        {
            var streams = new List<IncomeStream> { Gross(1, 5000, IncomeTaxTreatment.W2Wages) };

            var without = HouseholdTaxEstimator.Estimate(streams, FilingStatus.Single, 0, 0);
            var with = HouseholdTaxEstimator.Estimate(streams, FilingStatus.Single, 10000, 0);

            Assert.Less(with.FederalTax, without.FederalTax);

            // FICA is unaffected by 401(k)-style deferrals.
            with.SocialSecurityTax.ShouldEqual(without.SocialSecurityTax);
        }
    }
}
