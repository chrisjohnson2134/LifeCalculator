using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Simulation;
using LifeCalculator.Framework.SimulatedAccount;
using NUnit.Framework;
using Should;
using System;
using System.Collections.Generic;

namespace LifeCalcuator.FrameworkTest.Simulation
{
    [TestFixture]
    public class NetWorthAggregatorTest
    {
        [Test]
        public void Aggregate_CombinesAssetsAndDebtsAcrossCheckpoints()
        {
            var jan = new DateTime(2026, 1, 1);
            var feb = new DateTime(2026, 2, 1);
            var mar = new DateTime(2026, 3, 1);

            var debtResult = new DebtPayoffResult();
            debtResult.BalancesByDebtName["CarLoan"] = new List<MonthlyColumn>
            {
                new MonthlyColumn { Date = jan, Gain = 1000 },
                new MonthlyColumn { Date = feb, Gain = 500 },
                new MonthlyColumn { Date = mar, Gain = 0 } // paid off at the March checkpoint
            };

            var assets = new Dictionary<string, List<MonthlyColumn>>
            {
                ["Brokerage"] = new List<MonthlyColumn>
                {
                    new MonthlyColumn { Date = jan, Gain = 2000 },
                    new MonthlyColumn { Date = feb, Gain = 2200 },
                    new MonthlyColumn { Date = mar, Gain = 2400 }
                }
            };

            var result = NetWorthAggregator.Aggregate(debtResult, assets);

            result.Count.ShouldEqual(3);

            result[0].TotalAssets.ShouldEqual(2000.0);
            result[0].TotalDebt.ShouldEqual(1000.0);
            result[0].NetWorth.ShouldEqual(1000.0);

            result[1].NetWorth.ShouldEqual(1700.0); // 2200 - 500

            // Payoff-completion checkpoint: debt drops to 0, net worth equals total assets.
            result[2].TotalDebt.ShouldEqual(0.0);
            result[2].NetWorth.ShouldEqual(2400.0);
        }

        [Test]
        public void Aggregate_DateBeyondSeriesEnd_HoldsLastKnownValue()
        {
            var jan = new DateTime(2026, 1, 1);
            var feb = new DateTime(2026, 2, 1);

            var debtResult = new DebtPayoffResult();
            debtResult.BalancesByDebtName["ShortLoan"] = new List<MonthlyColumn>
            {
                new MonthlyColumn { Date = jan, Gain = 0 } // fully paid off, only one entry
            };

            var assets = new Dictionary<string, List<MonthlyColumn>>
            {
                ["Savings"] = new List<MonthlyColumn>
                {
                    new MonthlyColumn { Date = jan, Gain = 500 },
                    new MonthlyColumn { Date = feb, Gain = 550 }
                }
            };

            var result = NetWorthAggregator.Aggregate(debtResult, assets);

            // At the Feb checkpoint, ShortLoan has no Feb entry — its Jan (final) value of 0 should carry forward.
            var febColumn = result.Find(c => c.Date == feb);
            febColumn.TotalDebt.ShouldEqual(0.0);
            febColumn.TotalAssets.ShouldEqual(550.0);
        }

        [Test]
        public void Aggregate_NoDebtsOrAssets_ReturnsEmpty()
        {
            var result = NetWorthAggregator.Aggregate(new DebtPayoffResult(), new Dictionary<string, List<MonthlyColumn>>());

            result.Count.ShouldEqual(0);
        }

        [Test]
        public void Aggregate_AlignsSeriesRecordedAtDifferentTimesOfDay()
        {
            // Accounts created minutes apart produce series whose timestamps differ by
            // time-of-day. These are MONTHLY projections, so a debt row at 10:32 must still
            // see an asset whose series starts at 10:33 the same month — otherwise the assets
            // read as "not started yet" and the summary shows $0.
            var debtStamp = new DateTime(2026, 1, 15, 10, 32, 0);
            var assetStamp = new DateTime(2026, 1, 15, 10, 33, 0);

            var debtResult = new DebtPayoffResult();
            debtResult.BalancesByDebtName["Car"] = new List<MonthlyColumn>
            {
                new MonthlyColumn { Date = debtStamp, Gain = 5000 }
            };

            var assets = new Dictionary<string, List<MonthlyColumn>>
            {
                ["Brokerage"] = new List<MonthlyColumn>
                {
                    new MonthlyColumn { Date = assetStamp, Gain = 2000 }
                }
            };

            var result = NetWorthAggregator.Aggregate(debtResult, assets);

            // One month bucket, with both sides counted.
            result.Count.ShouldEqual(1);
            result[0].TotalDebt.ShouldEqual(5000.0);
            result[0].TotalAssets.ShouldEqual(2000.0);
            result[0].NetWorth.ShouldEqual(-3000.0);
        }

        [Test]
        public void Aggregate_IgnoresPlaceholderRowsFromCalculation()
        {
            // Every account's Calculation() prepends a bare MonthlyColumn whose Date is never
            // set, so it defaults to year 1. If that leaks into the timeline the whole chart
            // axis spans two millennia.
            var jan = new DateTime(2026, 1, 1);

            var assets = new Dictionary<string, List<MonthlyColumn>>
            {
                ["Brokerage"] = new List<MonthlyColumn>
                {
                    new MonthlyColumn(),                                     // placeholder, Date == year 1
                    new MonthlyColumn { Date = jan, Gain = 1000 }
                }
            };

            var result = NetWorthAggregator.Aggregate(new DebtPayoffResult(), assets);

            result.Count.ShouldEqual(1);
            result[0].Date.ShouldEqual(jan);
            result[0].TotalAssets.ShouldEqual(1000.0);
        }

        [Test]
        public void RealCompoundAccountCalculation_ProducesNoPreYear1900Dates()
        {
            // End-to-end guard: run a real account calculation through the aggregator and make
            // sure nothing lands before 1900 (i.e. the placeholder really is filtered).
            var eventsManager = new AccountsEventsManager();
            var account = new CompoundAccount(eventsManager) { Name = "Brokerage" };
            account.SetupBasicCalculation(new DateTime(2026, 1, 1), new DateTime(2028, 1, 1), 5, 1000, 100);

            var assets = new Dictionary<string, List<MonthlyColumn>>
            {
                ["Brokerage"] = account.Calculation()
            };

            var result = NetWorthAggregator.Aggregate(new DebtPayoffResult(), assets);

            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.TrueForAll(c => c.Date.Year >= 1900), "A pre-1900 date leaked into the net worth timeline.");
        }
    }
}
