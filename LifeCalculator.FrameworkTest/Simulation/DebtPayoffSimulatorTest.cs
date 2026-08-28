using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Simulation;
using LifeCalculator.Framework.SimulatedAccount;
using NUnit.Framework;
using Should;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalcuator.FrameworkTest.Simulation
{
    [TestFixture]
    public class DebtPayoffSimulatorTest
    {
        private LoanAccount MakeLoan(string name, double loanAmount, double interestRate, int lengthMonths, DateTime startDate)
        {
            var eventsManager = new AccountsEventsManager();
            return new LoanAccount(eventsManager, name, startDate, lengthMonths, interestRate, loanAmount, 0) { Id = 0 };
        }

        [Test]
        public void NoDebts_ReturnsEmptyResult()
        {
            var result = DebtPayoffSimulator.Simulate(new List<LoanAccount>(), DebtPayoffStrategy.Avalanche);

            result.BalancesByDebtName.Count.ShouldEqual(0);
        }

        [Test]
        public void RolloverAcceleratesPayoff_ExactZeroInterestArithmetic()
        {
            // 0% interest keeps the arithmetic linear and exactly hand-verifiable.
            var start = new DateTime(2026, 1, 1);
            var small = MakeLoan("Small", 600, 0, 12, start);   // pays 50/mo, paid off exactly at month 12
            var big = MakeLoan("Big", 5000, 0, 50, start);      // pays 100/mo on its own

            var result = DebtPayoffSimulator.Simulate(new List<LoanAccount> { small, big }, DebtPayoffStrategy.Snowball);

            var bigBalances = result.BalancesByDebtName["Big"];

            // Month 12: Small just paid off (600 - 12*50 = 0); Big has only received its own payment so far.
            bigBalances.Single(c => c.Date == start.AddMonths(12)).Gain.ShouldEqual(5000 - 12 * 100.0);

            // Month 13 onward: Small's freed $50/mo rolls onto Big, so Big now drops by 150/mo instead of 100/mo.
            double balanceAtMonth12 = bigBalances.Single(c => c.Date == start.AddMonths(12)).Gain;
            double balanceAtMonth13 = bigBalances.Single(c => c.Date == start.AddMonths(13)).Gain;
            double balanceAtMonth14 = bigBalances.Single(c => c.Date == start.AddMonths(14)).Gain;

            (balanceAtMonth12 - balanceAtMonth13).ShouldEqual(150.0);
            (balanceAtMonth13 - balanceAtMonth14).ShouldEqual(150.0);

            result.PayoffDateByDebtName["Small"].ShouldEqual((DateTime?)start.AddMonths(12));
        }

        [Test]
        public void Avalanche_PrioritizesHigherInterestRate_OverSnowball()
        {
            var start = new DateTime(2026, 1, 1);

            // Avalanche should route the trigger's freed payment to HighRate (20%) even though it has the bigger balance.
            // Snowball should route it to LowRate instead, since LowRate has the smaller balance.
            Func<List<LoanAccount>> makeDebts = () => new List<LoanAccount>
            {
                MakeLoan("HighRateBigBalance", 5000, 20, 60, start),
                MakeLoan("LowRateSmallBalance", 1000, 2, 60, start),
                MakeLoan("Trigger", 100, 0, 1, start)
            };

            var avalancheResult = DebtPayoffSimulator.Simulate(makeDebts(), DebtPayoffStrategy.Avalanche);
            var snowballResult = DebtPayoffSimulator.Simulate(makeDebts(), DebtPayoffStrategy.Snowball);

            DateTime checkpoint = start.AddMonths(6);

            double highRateBalanceUnderAvalanche = avalancheResult.BalancesByDebtName["HighRateBigBalance"].Single(c => c.Date == checkpoint).Gain;
            double highRateBalanceUnderSnowball = snowballResult.BalancesByDebtName["HighRateBigBalance"].Single(c => c.Date == checkpoint).Gain;

            double lowRateBalanceUnderAvalanche = avalancheResult.BalancesByDebtName["LowRateSmallBalance"].Single(c => c.Date == checkpoint).Gain;
            double lowRateBalanceUnderSnowball = snowballResult.BalancesByDebtName["LowRateSmallBalance"].Single(c => c.Date == checkpoint).Gain;

            // Avalanche should have paid HighRateBigBalance down further (it got the rollover) than Snowball did.
            Assert.Less(highRateBalanceUnderAvalanche, highRateBalanceUnderSnowball);

            // Snowball should have paid LowRateSmallBalance down further (it got the rollover) than Avalanche did.
            Assert.Less(lowRateBalanceUnderSnowball, lowRateBalanceUnderAvalanche);
        }
    }
}
