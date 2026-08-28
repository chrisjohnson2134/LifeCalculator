using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.SimulatedAccount;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Framework.Simulation
{
    /// <summary>
    /// Computes monthly cash-flow surplus/deficit: income minus bills minus debt payments minus
    /// planned contributions to growth accounts. This is what connects FinancialAccount's
    /// (previously display-only) bill fields and IncomeStreams into the Life Calculator
    /// projection for the first time.
    /// </summary>
    public static class CashFlowSimulator
    {
        public static List<CashFlowColumn> Calculate(
            FinancialAccount.FinancialAccount account,
            DateTime start,
            DateTime end,
            DebtPayoffResult debtPayoffResult,
            List<ISimulatedAccount> growthAccounts)
        {
            var columns = new List<CashFlowColumn>();

            if (account == null)
                return columns;

            double totalBills = SumBills(account);
            var incomeStreams = account.IncomeStreamManager?.GetAllIncomeStreams() ?? new List<Income.IncomeStream>();
            growthAccounts = growthAccounts ?? new List<ISimulatedAccount>();

            int monthDiff = Math.Abs((start.Year * 12 + (start.Month - 1)) - (end.Year * 12 + (end.Month - 1)));

            for (int i = 0; i <= monthDiff; i++)
            {
                DateTime date = start.AddMonths(i);

                double totalIncome = incomeStreams.Where(s => s.IsActiveDuring(date)).Sum(s => s.MonthlyAmount);

                // Payment totals are bucketed by month, so look them up the same way rather
                // than by exact timestamp.
                DateTime monthBucket = new DateTime(date.Year, date.Month, 1);

                double totalDebtPayments = debtPayoffResult != null && debtPayoffResult.TotalPaymentByDate.TryGetValue(monthBucket, out var payment)
                    ? payment
                    : 0;

                double totalContributions = growthAccounts.Sum(a => AccountEventResolver.ResolveAdditionalAmount(a.AccountLifeEvents, date));

                double surplus = totalIncome - totalBills - totalDebtPayments - totalContributions;

                columns.Add(new CashFlowColumn
                {
                    Date = date,
                    TotalIncome = Math.Round(totalIncome, 2),
                    TotalBills = Math.Round(totalBills, 2),
                    TotalDebtPayments = Math.Round(totalDebtPayments, 2),
                    TotalContributions = Math.Round(totalContributions, 2),
                    Surplus = Math.Round(surplus, 2)
                });
            }

            return columns;
        }

        /// <summary>
        /// Monthly expenses come from the Budget screen's ExpenseItems — the single source of
        /// truth for planned spending — so editing the Budget immediately moves the Life
        /// Calculator's surplus. Falls back to the legacy fixed bill columns only if a profile
        /// predates the migration and has no expense rows yet.
        /// </summary>
        private static double SumBills(FinancialAccount.FinancialAccount account)
        {
            double expenseTotal = account.ExpenseManager?.GetTotalMonthlyExpenses() ?? 0;

            if (expenseTotal > 0)
                return expenseTotal;

            return account.Rent + account.WaterBill + account.ElectricBill + account.InternetBill
                + account.CableBill + account.Subscriptions + account.Groceries + account.EmergencyFundContributions
                + account.Gas + account.CarInsurance + account.HomeInsurance + account.CarPayments
                + account.OtherDebts + account.MiscellaneousPayments;
        }
    }
}
