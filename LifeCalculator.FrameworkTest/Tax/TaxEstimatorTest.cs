using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Tax;
using NUnit.Framework;
using Should;

namespace LifeCalcuator.FrameworkTest.Tax
{
    /// <summary>
    /// Figures hand-computed from the 2025 federal brackets, standard deductions, and FICA
    /// rates encoded in TaxEstimator. If the IRS tables are updated, these expectations must
    /// be recomputed alongside them.
    /// </summary>
    [TestFixture]
    public class TaxEstimatorTest
    {
        [Test]
        public void ZeroIncome_ProducesZeroTax()
        {
            var estimate = TaxEstimator.Estimate(0, FilingStatus.Single, 0, 0);

            estimate.TotalTax.ShouldEqual(0.0);
            estimate.NetAnnual.ShouldEqual(0.0);
        }

        [Test]
        public void IncomeBelowStandardDeduction_OwesNoFederalTaxButStillOwesFica()
        {
            // $12,000 single: standard deduction ($15,000) wipes out taxable income entirely,
            // but FICA is levied on gross wages regardless.
            var estimate = TaxEstimator.Estimate(12000, FilingStatus.Single, 0, 0);

            estimate.FederalTax.ShouldEqual(0.0);
            estimate.SocialSecurityTax.ShouldEqual(744.0);   // 12,000 * 6.2%
            estimate.MedicareTax.ShouldEqual(174.0);         // 12,000 * 1.45%
        }

        [Test]
        public void Single60k_MatchesHandComputedBrackets()
        {
            // Gross 60,000; taxable = 60,000 - 15,000 = 45,000.
            //   10% on first 11,925                      = 1,192.50
            //   12% on 45,000 - 11,925 = 33,075          = 3,969.00
            //   Federal total                            = 5,161.50
            var estimate = TaxEstimator.Estimate(60000, FilingStatus.Single, 0, 0);

            estimate.FederalTax.ShouldEqual(5161.50);
            estimate.SocialSecurityTax.ShouldEqual(3720.0); // 60,000 * 6.2%
            estimate.MedicareTax.ShouldEqual(870.0);        // 60,000 * 1.45%
            estimate.StateTax.ShouldEqual(0.0);

            estimate.TotalTax.ShouldEqual(9751.50);
            estimate.NetAnnual.ShouldEqual(50248.50);
        }

        [Test]
        public void PreTaxDeductions_ReduceIncomeTaxButNotFica()
        {
            // 60,000 gross with 10,000 pre-tax: taxable = 60,000 - 10,000 - 15,000 = 35,000.
            //   10% on 11,925                            = 1,192.50
            //   12% on 23,075                            = 2,769.00
            //   Federal total                            = 3,961.50
            var estimate = TaxEstimator.Estimate(60000, FilingStatus.Single, 10000, 0);

            estimate.FederalTax.ShouldEqual(3961.50);

            // FICA still computed on the full gross wage.
            estimate.SocialSecurityTax.ShouldEqual(3720.0);
            estimate.MedicareTax.ShouldEqual(870.0);
        }

        [Test]
        public void StateRate_AppliesToTaxableIncome()
        {
            // taxable = 45,000; 5% state = 2,250.
            var estimate = TaxEstimator.Estimate(60000, FilingStatus.Single, 0, 5);

            estimate.StateTax.ShouldEqual(2250.0);
        }

        [Test]
        public void MarriedFilingJointly_UsesWiderBracketsThanSingle()
        {
            var single = TaxEstimator.Estimate(120000, FilingStatus.Single, 0, 0);
            var joint = TaxEstimator.Estimate(120000, FilingStatus.MarriedFilingJointly, 0, 0);

            Assert.Less(joint.FederalTax, single.FederalTax);
        }

        [Test]
        public void SocialSecurity_StopsAtWageBase()
        {
            // 2025 wage base is 176,100 → SS caps at 176,100 * 6.2% = 10,918.20
            var estimate = TaxEstimator.Estimate(300000, FilingStatus.Single, 0, 0);

            estimate.SocialSecurityTax.ShouldEqual(10918.20);
        }

        [Test]
        public void AdditionalMedicare_AppliesOverThreshold()
        {
            // Single threshold is 200,000. At 250,000:
            //   1.45% on 250,000            = 3,625.00
            //   +0.9% on 50,000             =   450.00
            //   total                       = 4,075.00
            var estimate = TaxEstimator.Estimate(250000, FilingStatus.Single, 0, 0);

            estimate.MedicareTax.ShouldEqual(4075.0);
        }

        [Test]
        public void NetMonthly_IsNetAnnualDividedByTwelve()
        {
            var estimate = TaxEstimator.Estimate(60000, FilingStatus.Single, 0, 0);

            estimate.NetMonthly.ShouldEqual(estimate.NetAnnual / 12);
        }

        [Test]
        public void PreTaxDeductionsExceedingGross_AreClampedToGross()
        {
            var estimate = TaxEstimator.Estimate(50000, FilingStatus.Single, 999999, 0);

            // The deduction is clamped to gross, so there's no income tax left to owe...
            estimate.PreTaxDeductions.ShouldEqual(50000.0);
            estimate.FederalTax.ShouldEqual(0.0);

            // ...but FICA is still levied on gross wages. Deferring 100% of salary therefore
            // implies negative take-home, which is arithmetically correct and practically
            // impossible — IRS contribution limits prevent it, and the UI caps the input.
            estimate.SocialSecurityTax.ShouldEqual(3100.0); // 50,000 * 6.2%
            estimate.MedicareTax.ShouldEqual(725.0);        // 50,000 * 1.45%
            estimate.NetAnnual.ShouldEqual(-3825.0);
        }
    }
}
