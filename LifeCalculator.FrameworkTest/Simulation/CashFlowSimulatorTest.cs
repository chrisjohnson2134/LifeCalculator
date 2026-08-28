using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Income;
using LifeCalculator.Framework.LifeEvents;
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
    public class CashFlowSimulatorTest
    {
        [Test]
        public void Calculate_SurplusReflectsIncomeMinusBillsMinusDebtMinusContributions()
        {
            var financialAccount = new LifeCalculator.Framework.FinancialAccount.FinancialAccount
            {
                Rent = 1000,
                Groceries = 300
            };

            DateTime date = new DateTime(2026, 1, 1);

            financialAccount.IncomeStreamManager.AddIncomeStream(new IncomeStream
            {
                Name = "Job",
                // Pinned to take-home so this test measures the surplus formula, not the tax engine.
                IsGross = false,
                MonthlyAmount = 3000,
                StartDate = date,
                StreamType = IncomeStreamType.Salary
            });

            var debtPayoffResult = new DebtPayoffResult();
            debtPayoffResult.TotalPaymentByDate[date] = 500;

            var compoundEventsManager = new AccountsEventsManager();
            var compoundAccount = new CompoundAccount(compoundEventsManager) { Name = "Brokerage" };
            compoundAccount.AddLifeEvent(new AccountEvent
            {
                Name = "Monthly contribution",
                Amount = 200,
                StartDate = date,
                EndDate = date.AddYears(1),
                LifeEventType = LifeEnum.MonthlyContribute,
                AccountType = AccountTypes.CompoundInterest
            });

            var columns = CashFlowSimulator.Calculate(
                financialAccount, date, date, debtPayoffResult, new List<ISimulatedAccount> { compoundAccount });

            columns.Count.ShouldEqual(1);
            var column = columns[0];

            column.TotalIncome.ShouldEqual(3000.0);
            column.TotalBills.ShouldEqual(1300.0);
            column.TotalDebtPayments.ShouldEqual(500.0);
            column.TotalContributions.ShouldEqual(200.0);
            column.Surplus.ShouldEqual(1000.0);
        }

        [Test]
        public void Calculate_NegativeSurplus_FlagsUnaffordablePlan()
        {
            var financialAccount = new LifeCalculator.Framework.FinancialAccount.FinancialAccount
            {
                Rent = 2500,
                CarPayments = 600
            };

            DateTime date = new DateTime(2026, 1, 1);

            financialAccount.IncomeStreamManager.AddIncomeStream(new IncomeStream
            {
                Name = "Job",
                // Pinned to take-home so this test measures the surplus formula, not the tax engine.
                IsGross = false,
                MonthlyAmount = 2000,
                StartDate = date,
                StreamType = IncomeStreamType.Salary
            });

            var debtPayoffResult = new DebtPayoffResult();

            var columns = CashFlowSimulator.Calculate(
                financialAccount, date, date, debtPayoffResult, new List<ISimulatedAccount>());

            columns[0].Surplus.ShouldBeInRange(-1100.01, -1099.99);
            Assert.Less(columns[0].Surplus, 0);
        }

        [Test]
        public void Calculate_SubtractsDebtPaymentsForLoansAddedThisMonth()
        {
            // Loans carry the day-of-month and time they were created, but cash flow asks
            // about the 1st. The payment map must be keyed by month or the debt payments
            // silently read as $0 and the leftover figure is overstated.
            var financialAccount = new LifeCalculator.Framework.FinancialAccount.FinancialAccount();
            var eventsManager = new AccountsEventsManager();

            DateTime createdMidMonth = new DateTime(2026, 8, 27, 14, 33, 0);

            var carLoan = new LoanAccount(eventsManager, "Car", createdMidMonth, 60, 5, 30000, 0) { Id = 1 };
            var studentLoan = new LoanAccount(eventsManager, "Student", createdMidMonth, 120, 4, 40000, 0) { Id = 2 };

            var debtResult = DebtPayoffSimulator.Simulate(
                new List<LoanAccount> { carLoan, studentLoan }, DebtPayoffStrategy.Avalanche);

            DateTime firstOfMonth = new DateTime(2026, 8, 1);

            var columns = CashFlowSimulator.Calculate(
                financialAccount, firstOfMonth, firstOfMonth, debtResult, new List<ISimulatedAccount>());

            double expected = Math.Round(carLoan.MonthlyPayment + studentLoan.MonthlyPayment, 2);

            columns[0].TotalDebtPayments.ShouldEqual(expected);
            Assert.Greater(columns[0].TotalDebtPayments, 0);
        }

        [Test]
        public void Calculate_StreamStartingMidMonth_CountsForThatMonth()
        {
            // Cash flow is bucketed monthly and asks about the 1st, but people create streams
            // on whatever day it happens to be. A stream started on the 27th must still count
            // as income for that month rather than silently reading as $0.
            var financialAccount = new LifeCalculator.Framework.FinancialAccount.FinancialAccount();

            financialAccount.IncomeStreamManager.AddIncomeStream(new IncomeStream
            {
                Name = "New job",
                // Pinned to take-home so this test measures the surplus formula, not the tax engine.
                IsGross = false,
                MonthlyAmount = 5000,
                StartDate = new DateTime(2026, 8, 27),
                StreamType = IncomeStreamType.Salary
            });

            DateTime firstOfMonth = new DateTime(2026, 8, 1);

            var columns = CashFlowSimulator.Calculate(
                financialAccount, firstOfMonth, firstOfMonth, new DebtPayoffResult(), new List<ISimulatedAccount>());

            columns[0].TotalIncome.ShouldEqual(5000.0);
        }

        [Test]
        public void Calculate_StreamEndingMidMonth_StillCountsForThatMonth()
        {
            var financialAccount = new LifeCalculator.Framework.FinancialAccount.FinancialAccount();

            financialAccount.IncomeStreamManager.AddIncomeStream(new IncomeStream
            {
                Name = "Contract ending",
                // Pinned to take-home so this test measures the surplus formula, not the tax engine.
                IsGross = false,
                MonthlyAmount = 2000,
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 8, 10),
                StreamType = IncomeStreamType.Freelance
            });

            var columns = CashFlowSimulator.Calculate(
                financialAccount, new DateTime(2026, 8, 1), new DateTime(2026, 8, 1),
                new DebtPayoffResult(), new List<ISimulatedAccount>());

            columns[0].TotalIncome.ShouldEqual(2000.0);
        }

        [Test]
        public void Calculate_IncomeStreamOutsideActiveRange_IsExcluded()
        {
            var financialAccount = new LifeCalculator.Framework.FinancialAccount.FinancialAccount();
            DateTime date = new DateTime(2026, 6, 1);

            financialAccount.IncomeStreamManager.AddIncomeStream(new IncomeStream
            {
                Name = "ExpiredContract",
                // Pinned to take-home so this test measures the surplus formula, not the tax engine.
                IsGross = false,
                MonthlyAmount = 1000,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2026, 1, 1),
                StreamType = IncomeStreamType.Freelance
            });

            var columns = CashFlowSimulator.Calculate(
                financialAccount, date, date, new DebtPayoffResult(), new List<ISimulatedAccount>());

            columns[0].TotalIncome.ShouldEqual(0.0);
        }

        /// <summary>
        /// Income is entered gross, so cash flow has to tax it before treating it as spendable.
        /// Counting gross as available money would overstate every surplus by the whole tax
        /// bill and make unaffordable plans look affordable.
        /// </summary>
        [Test]
        public void Calculate_GrossIncome_IsTaxedBeforeItCountsAsSpendable()
        {
            var financialAccount = new LifeCalculator.Framework.FinancialAccount.FinancialAccount
            {
                FilingStatus = FilingStatus.Single,
                StateTaxRatePercent = 0
            };

            DateTime date = new DateTime(2026, 1, 1);

            financialAccount.IncomeStreamManager.AddIncomeStream(new IncomeStream
            {
                Name = "Salaried job",
                PayFrequency = PayFrequency.Annual,
                PayRate = 90000,
                IsGross = true,
                TaxTreatment = IncomeTaxTreatment.W2Wages,
                StartDate = date,
                StreamType = IncomeStreamType.Salary
            });

            var columns = CashFlowSimulator.Calculate(
                financialAccount, date, date, new DebtPayoffResult(), new List<ISimulatedAccount>());

            double grossMonthly = 90000.0 / 12;

            columns[0].TotalIncome.ShouldBeLessThan(grossMonthly);

            // Sanity-check the magnitude rather than pin an exact figure, so the assertion
            // survives a bracket-table update: federal + FICA on $90k lands well inside this.
            columns[0].TotalIncome.ShouldBeGreaterThan(grossMonthly * 0.6);
            columns[0].TotalIncome.ShouldBeLessThan(grossMonthly * 0.9);
        }

        /// <summary>
        /// Take-home income is spent as-is: no second round of tax on money already withheld.
        /// </summary>
        [Test]
        public void Calculate_AlreadyNetIncome_IsNotTaxedAgain()
        {
            var financialAccount = new LifeCalculator.Framework.FinancialAccount.FinancialAccount
            {
                FilingStatus = FilingStatus.Single
            };

            DateTime date = new DateTime(2026, 1, 1);

            financialAccount.IncomeStreamManager.AddIncomeStream(new IncomeStream
            {
                Name = "Gift",
                IsGross = false,
                MonthlyAmount = 1500,
                StartDate = date,
                StreamType = IncomeStreamType.Other
            });

            var columns = CashFlowSimulator.Calculate(
                financialAccount, date, date, new DebtPayoffResult(), new List<ISimulatedAccount>());

            columns[0].TotalIncome.ShouldEqual(1500.0);
        }
    }
}
