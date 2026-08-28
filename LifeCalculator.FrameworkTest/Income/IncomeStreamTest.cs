using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Income;
using NUnit.Framework;
using Should;
using System;

namespace LifeCalcuator.FrameworkTest.Income
{
    /// <summary>
    /// Covers turning a pay rate into annual and monthly figures. The bi-weekly vs semi-monthly
    /// distinction is the one people most often get wrong by hand, so it's asserted explicitly.
    /// </summary>
    [TestFixture]
    public class IncomeStreamTest
    {
        [TestCase(PayFrequency.Annual, 60000, 60000)]
        [TestCase(PayFrequency.Monthly, 5000, 60000)]
        [TestCase(PayFrequency.SemiMonthly, 2500, 60000)]   // 24 cheques
        [TestCase(PayFrequency.BiWeekly, 2000, 52000)]      // 26 cheques
        [TestCase(PayFrequency.Weekly, 1000, 52000)]
        public void AnnualiseRate_ConvertsEachFrequency(PayFrequency frequency, double rate, double expectedAnnual)
        {
            IncomeStream.AnnualiseRate(rate, frequency, 40).ShouldEqual(expectedAnnual);
        }

        [Test]
        public void AnnualiseRate_Hourly_UsesHoursPerWeek()
        {
            // $25/hr at 40 hrs over 52 weeks.
            IncomeStream.AnnualiseRate(25, PayFrequency.Hourly, 40).ShouldEqual(52000.0);

            // Part-time at the same rate scales down proportionally.
            IncomeStream.AnnualiseRate(25, PayFrequency.Hourly, 20).ShouldEqual(26000.0);
        }

        /// <summary>
        /// The pair that trips people up: same per-cheque amount, $4,000 a year apart.
        /// </summary>
        [Test]
        public void AnnualiseRate_BiWeeklyAndSemiMonthly_AreNotTheSame()
        {
            double biWeekly = IncomeStream.AnnualiseRate(2000, PayFrequency.BiWeekly, 40);
            double semiMonthly = IncomeStream.AnnualiseRate(2000, PayFrequency.SemiMonthly, 40);

            biWeekly.ShouldEqual(52000.0);
            semiMonthly.ShouldEqual(48000.0);
            (biWeekly - semiMonthly).ShouldEqual(4000.0);
        }

        [Test]
        public void SettingPayRate_DerivesMonthlyAmount()
        {
            var stream = new IncomeStream
            {
                PayFrequency = PayFrequency.BiWeekly,
                PayRate = 2000
            };

            stream.AnnualAmount.ShouldEqual(52000.0);
            stream.MonthlyAmount.ShouldEqual(52000.0 / 12);
        }

        /// <summary>
        /// Changing frequency alone must re-derive the monthly figure — otherwise switching a
        /// $2,000 cheque from bi-weekly to monthly would leave the old annualisation in place.
        /// </summary>
        [Test]
        public void ChangingFrequency_RederivesMonthlyAmount()
        {
            var stream = new IncomeStream
            {
                PayFrequency = PayFrequency.BiWeekly,
                PayRate = 2000
            };

            stream.PayFrequency = PayFrequency.Monthly;

            stream.AnnualAmount.ShouldEqual(24000.0);
            stream.MonthlyAmount.ShouldEqual(2000.0);
        }

        /// <summary>
        /// Guards the hydration hazard: the data service assigns public properties in an
        /// unspecified order, so a row saved before pay frequency existed arrives with
        /// PayRate = 0. That must not wipe the MonthlyAmount loaded from the database.
        /// </summary>
        [Test]
        public void LegacyRowWithNoPayRate_KeepsItsStoredMonthlyAmount()
        {
            var stream = new IncomeStream();

            stream.MonthlyAmount = 4200;
            stream.PayFrequency = PayFrequency.Monthly;
            stream.HoursPerWeek = 40;
            stream.PayRate = 0;

            stream.MonthlyAmount.ShouldEqual(4200.0);
        }

        /// <summary>
        /// Gross salary is derived from the pay rate now, so a 401(k) match cap can be sized
        /// without asking the user to state their salary a second time.
        /// </summary>
        [Test]
        public void GrossAnnualSalary_DerivesFromPayRate_WhenEnteredGross()
        {
            var stream = new IncomeStream
            {
                IsGross = true,
                PayFrequency = PayFrequency.Hourly,
                HoursPerWeek = 40,
                PayRate = 30
            };

            stream.GrossAnnualSalary.ShouldEqual(62400.0);
        }

        /// <summary>
        /// For take-home streams gross genuinely isn't known, so the stored value stands.
        /// </summary>
        [Test]
        public void GrossAnnualSalary_UsesStoredValue_WhenEnteredNet()
        {
            var stream = new IncomeStream
            {
                IsGross = false,
                PayFrequency = PayFrequency.Monthly,
                PayRate = 4000,
                GrossAnnualSalary = 75000
            };

            stream.GrossAnnualSalary.ShouldEqual(75000.0);
        }

        [Test]
        public void NewStream_DefaultsToGross()
        {
            new IncomeStream().IsGross.ShouldBeTrue();
        }
    }
}
