using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.SimulatedAccount;
using NUnit.Framework;
using Should;
using System;

namespace LifeCalcuator.FrameworkTest.Account
{
    /// <summary>
    /// Covers the emergency fund's two jobs: projecting when the goal is reached, and turning a
    /// balance into months of expenses covered.
    /// </summary>
    [TestFixture]
    public class EmergencyFundAccountTest
    {
        private static EmergencyFundAccount BuildFund(
            double balance = 0,
            double goal = 0,
            double monthly = 0,
            double rate = 0)
        {
            return new EmergencyFundAccount(new AccountsEventsManager())
            {
                Name = "Emergency Fund",
                InitialAmount = balance,
                GoalAmount = goal,
                MonthlyContribution = monthly,
                InterestRate = rate,
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2036, 1, 1)
            };
        }

        /// <summary>
        /// With no interest the arithmetic is exact: $500 a month toward a $6,000 goal is twelve
        /// months, landing on the twelfth month from the start.
        /// </summary>
        [Test]
        public void ProjectedGoalDate_NoInterest_IsPlainDivision()
        {
            var fund = BuildFund(balance: 0, goal: 6000, monthly: 500);

            DateTime? date = fund.ProjectedGoalDate();

            date.HasValue.ShouldBeTrue();

            // Month 0 deposits the first $500, so the 12th deposit lands on month index 11.
            date.Value.ShouldEqual(new DateTime(2026, 12, 1));
        }

        [Test]
        public void ProjectedGoalDate_ExistingBalance_ShortensTheTimeline()
        {
            var withNothing = BuildFund(balance: 0, goal: 6000, monthly: 500);
            var withHalf = BuildFund(balance: 3000, goal: 6000, monthly: 500);

            withHalf.ProjectedGoalDate().Value.ShouldBeLessThan(withNothing.ProjectedGoalDate().Value);
        }

        /// <summary>
        /// Interest should pull the date in, never push it out.
        /// </summary>
        [Test]
        public void ProjectedGoalDate_Interest_ReachesGoalNoLaterThanWithout()
        {
            var noInterest = BuildFund(balance: 1000, goal: 20000, monthly: 400);
            var withInterest = BuildFund(balance: 1000, goal: 20000, monthly: 400, rate: 0.045);

            withInterest.ProjectedGoalDate().Value.ShouldBeLessThanOrEqualTo(noInterest.ProjectedGoalDate().Value);
        }

        [Test]
        public void ProjectedGoalDate_AlreadyFunded_IsTheStartDate()
        {
            var fund = BuildFund(balance: 10000, goal: 6000, monthly: 100);

            fund.ProjectedGoalDate().ShouldEqual(fund.StartDate);
            fund.IsGoalMet.ShouldBeTrue();
        }

        [Test]
        public void ProjectedGoalDate_NoGoalSet_IsNull()
        {
            BuildFund(balance: 500, goal: 0, monthly: 100).ProjectedGoalDate().HasValue.ShouldBeFalse();
        }

        /// <summary>
        /// Nothing going in and no interest means the goal is never reached — the UI needs a null
        /// here so it can say so rather than showing a bogus date.
        /// </summary>
        [Test]
        public void ProjectedGoalDate_NoContributionAndNoInterest_IsNull()
        {
            BuildFund(balance: 500, goal: 6000, monthly: 0).ProjectedGoalDate().HasValue.ShouldBeFalse();
        }

        /// <summary>
        /// Interest alone can carry a fund over the line, so a zero contribution is not by itself
        /// unreachable.
        /// </summary>
        [Test]
        public void ProjectedGoalDate_InterestAloneCanReachGoal()
        {
            var fund = BuildFund(balance: 10000, goal: 11000, monthly: 0, rate: 0.05);

            fund.ProjectedGoalDate().HasValue.ShouldBeTrue();
        }

        /// <summary>
        /// A contribution too small to ever get there must terminate rather than loop forever.
        /// </summary>
        [Test]
        public void ProjectedGoalDate_UnreachableWithinFiftyYears_IsNull()
        {
            var fund = BuildFund(balance: 0, goal: 10000000, monthly: 5);

            fund.ProjectedGoalDate().HasValue.ShouldBeFalse();
        }

        [Test]
        public void MonthsOfExpensesCovered_DividesBalanceByMonthlySpend()
        {
            var fund = BuildFund(balance: 12000);

            fund.MonthsOfExpensesCovered(3000).ShouldEqual(4.0);
        }

        /// <summary>No budget entered yet — must not divide by zero.</summary>
        [Test]
        public void MonthsOfExpensesCovered_NoExpenses_IsZero()
        {
            BuildFund(balance: 12000).MonthsOfExpensesCovered(0).ShouldEqual(0.0);
        }

        [TestCase(3, 9000)]
        [TestCase(6, 18000)]
        [TestCase(12, 36000)]
        public void SetGoalFromMonthsOfExpenses_MultipliesMonthlySpend(int months, double expectedGoal)
        {
            var fund = BuildFund();

            fund.SetGoalFromMonthsOfExpenses(months, 3000);

            fund.GoalAmount.ShouldEqual(expectedGoal);
            fund.GoalMonthsOfExpenses.ShouldEqual(months);
        }

        [Test]
        public void ProgressFraction_IsClampedToOne_WhenOverfunded()
        {
            BuildFund(balance: 9000, goal: 6000).ProgressFraction.ShouldEqual(1.0);
        }

        [Test]
        public void ProgressFraction_NoGoal_IsZeroRatherThanDivideByZero()
        {
            BuildFund(balance: 9000, goal: 0).ProgressFraction.ShouldEqual(0.0);
        }

        [Test]
        public void RemainingToGoal_NeverGoesNegative()
        {
            BuildFund(balance: 9000, goal: 6000).RemainingToGoal.ShouldEqual(0.0);
            BuildFund(balance: 2000, goal: 6000).RemainingToGoal.ShouldEqual(4000.0);
        }

        /// <summary>
        /// A one-off deposit should move the projection forward, confirming that events still
        /// layer on top of the standing monthly contribution.
        /// </summary>
        [Test]
        public void OneTimeDeposit_PullsGoalDateForward()
        {
            var eventsManager = new AccountsEventsManager();

            var plain = BuildFund(balance: 0, goal: 6000, monthly: 500);

            var boosted = new EmergencyFundAccount(eventsManager)
            {
                Id = 1,
                Name = "Emergency Fund",
                InitialAmount = 0,
                GoalAmount = 6000,
                MonthlyContribution = 500,
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2036, 1, 1)
            };

            boosted.AddLifeEvent(new AccountEvent
            {
                Name = "Tax refund",
                Amount = 2000,
                StartDate = new DateTime(2026, 3, 1),
                EndDate = new DateTime(2026, 3, 1),
                LifeEventType = LifeEnum.OneTime,
                AccountType = AccountTypes.EmergencyFund
            });

            boosted.ProjectedGoalDate().Value.ShouldBeLessThan(plain.ProjectedGoalDate().Value);
        }

        /// <summary>
        /// The fund is a target, not a pot you feed forever. Once the goal is reached the
        /// monthly contribution stops, so with no interest the balance goes flat instead of
        /// climbing as though you kept paying in.
        /// </summary>
        [Test]
        public void Calculation_StopsContributing_OnceGoalIsMet()
        {
            var fund = BuildFund(balance: 0, goal: 6000, monthly: 500, rate: 0);
            fund.EndDate = new DateTime(2029, 1, 1);

            var columns = fund.Calculation();

            double finalBalance = columns[columns.Count - 1].Gain;

            // Three years of unchecked $500 deposits would be $18,000; it must stop at the goal.
            finalBalance.ShouldEqual(6000.0);
        }

        /// <summary>
        /// Past the goal the only thing that moves the balance is interest.
        /// </summary>
        [Test]
        public void Calculation_AfterGoalIsMet_OnlyInterestGrowsTheBalance()
        {
            var fund = BuildFund(balance: 6000, goal: 6000, monthly: 500, rate: 0.12);
            fund.StartDate = new DateTime(2026, 1, 1);
            fund.EndDate = new DateTime(2027, 1, 1);

            var columns = fund.Calculation();

            double finalBalance = columns[columns.Count - 1].Gain;

            // Starts funded, so no contributions at all: 12 months of 1%/mo on $6,000.
            double interestOnly = 6000 * Math.Pow(1 + 0.12 / 12, 12);

            finalBalance.ShouldBeInRange(interestOnly - 1, interestOnly + 1);
        }

        /// <summary>
        /// With no goal there's nothing to stop at, so contributions continue.
        /// </summary>
        [Test]
        public void Calculation_NoGoal_KeepsContributing()
        {
            var fund = BuildFund(balance: 0, goal: 0, monthly: 500, rate: 0);
            fund.EndDate = new DateTime(2027, 1, 1);

            var columns = fund.Calculation();

            columns[columns.Count - 1].Gain.ShouldEqual(6000.0);
        }

        [Test]
        public void IsContributingOn_IsFalse_AfterTheGoalDate()
        {
            var fund = BuildFund(balance: 0, goal: 6000, monthly: 500);

            // Goal lands December 2026.
            fund.IsContributingOn(new DateTime(2026, 6, 1)).ShouldBeTrue();
            fund.IsContributingOn(new DateTime(2026, 12, 1)).ShouldBeTrue();
            fund.IsContributingOn(new DateTime(2027, 1, 1)).ShouldBeFalse();
        }

        [Test]
        public void IsContributingOn_AlreadyFunded_IsFalse()
        {
            var fund = BuildFund(balance: 10000, goal: 6000, monthly: 500);

            fund.IsContributingOn(new DateTime(2027, 1, 1)).ShouldBeFalse();
        }

        /// <summary>
        /// A goal that is never reached keeps taking contributions rather than stopping early.
        /// </summary>
        [Test]
        public void IsContributingOn_UnreachableGoal_StaysTrue()
        {
            var fund = BuildFund(balance: 0, goal: 10000000, monthly: 5);

            fund.IsContributingOn(new DateTime(2040, 1, 1)).ShouldBeTrue();
        }

        /// <summary>
        /// Growth still compounds monthly like every other account, so the balance after a year
        /// of contributions exceeds the contributions alone.
        /// </summary>
        [Test]
        public void Calculation_AppliesInterestOnTopOfContributions()
        {
            var fund = BuildFund(balance: 0, goal: 100000, monthly: 500, rate: 0.05);
            fund.EndDate = new DateTime(2027, 1, 1);

            var columns = fund.Calculation();

            double finalBalance = columns[columns.Count - 1].Gain;

            finalBalance.ShouldBeGreaterThan(6000.0);
        }
    }
}
