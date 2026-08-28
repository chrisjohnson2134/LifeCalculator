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

            // Income is entered gross, so it has to be taxed before it can be spent — otherwise
            // every surplus is overstated by the entire tax bill. The net figure per stream is
            // computed once across the whole household (brackets are progressive, so streams
            // can't be taxed independently) and then looked up per month below.
            var netMonthlyByStreamId = ResolveNetMonthlyByStream(account, incomeStreams);

            // Resolved once per fund: working out the goal date walks up to 600 months, so doing
            // it inside the month loop would make a long projection quadratic.
            var emergencyFunds = growthAccounts
                .OfType<EmergencyFundAccount>()
                .Select(fund => new KeyValuePair<EmergencyFundAccount, DateTime?>(fund, fund.ProjectedGoalDate()))
                .ToList();

            int monthDiff = Math.Abs((start.Year * 12 + (start.Month - 1)) - (end.Year * 12 + (end.Month - 1)));

            for (int i = 0; i <= monthDiff; i++)
            {
                DateTime date = start.AddMonths(i);

                double totalIncome = incomeStreams
                    .Where(s => s.IsActiveDuring(date))
                    .Sum(s => netMonthlyByStreamId.TryGetValue(s.Id, out var net) ? net : s.MonthlyAmount);

                // Payment totals are bucketed by month, so look them up the same way rather
                // than by exact timestamp.
                DateTime monthBucket = new DateTime(date.Year, date.Month, 1);

                double totalDebtPayments = debtPayoffResult != null && debtPayoffResult.TotalPaymentByDate.TryGetValue(monthBucket, out var payment)
                    ? payment
                    : 0;

                // Event-driven contributions, plus the emergency fund's standing monthly amount.
                // That one is a plain field rather than an event, so it would otherwise be
                // invisible here and the surplus would count money already committed to savings.
                // It stops once the fund hits its goal, at which point that money is free again.
                double totalContributions =
                    growthAccounts.Sum(a => AccountEventResolver.ResolveAdditionalAmount(a.AccountLifeEvents, date))
                    + emergencyFunds
                        .Where(pair => pair.Key.IsContributingOn(date, pair.Value))
                        .Sum(pair => pair.Key.MonthlyContribution);

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
        /// Take-home per stream, taxed as one household.
        ///
        /// Streams flagged as already-net pass through untouched. Gross streams get their share
        /// of household take-home, which is why this can't be done per stream in isolation: a
        /// second job is taxed at the marginal rate the first already pushed you into, so taxing
        /// each alone would understate the bill and overstate the surplus.
        /// </summary>
        private static Dictionary<int, double> ResolveNetMonthlyByStream(
            FinancialAccount.FinancialAccount account,
            List<Income.IncomeStream> incomeStreams)
        {
            if (incomeStreams.Count == 0)
                return new Dictionary<int, double>();

            var estimate = Tax.HouseholdTaxEstimator.Estimate(
                incomeStreams,
                account.FilingStatus,
                account.PreTaxDeductionsAnnual,
                account.StateTaxRatePercent);

            return estimate.NetMonthlyByStreamId ?? new Dictionary<int, double>();
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
