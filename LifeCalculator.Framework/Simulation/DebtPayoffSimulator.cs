using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.SimulatedAccount;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Framework.Simulation
{
    /// <summary>
    /// Orchestrates multiple LoanAccounts together, applying an avalanche (highest interest
    /// rate first) or snowball (smallest balance first) payoff strategy: once a debt is paid
    /// off, its monthly payment permanently rolls over to the next unpaid debt in priority
    /// order. This does not modify LoanAccount.Calculation() itself, which remains the
    /// independent, no-rollover projection for a single debt viewed in isolation.
    /// </summary>
    public static class DebtPayoffSimulator
    {
        // Projection horizon. A debt whose payment barely covers interest would otherwise run
        // effectively forever; we stop at 50 years and report it as never paying off rather
        // than drawing a century-long flat line.
        private const int MaxMonths = 600;

        /// <summary>
        /// Projections are monthly, so payment totals are keyed by the first of the month.
        /// Keying by the raw simulation date would carry the loan's original day-of-month and
        /// time, and callers asking about "this month" would never find a match.
        /// </summary>
        private static DateTime ToMonthBucket(DateTime date) => new DateTime(date.Year, date.Month, 1);

        public static DebtPayoffResult Simulate(List<LoanAccount> debts, DebtPayoffStrategy strategy)
        {
            var result = new DebtPayoffResult();

            if (debts == null || debts.Count == 0)
                return result;

            var ordered = OrderDebts(debts, strategy);

            var balances = new Dictionary<LoanAccount, double>();
            var paidOff = new Dictionary<LoanAccount, bool>();

            DateTime simulationStart = ordered.Min(d => d.StartDate);

            foreach (var debt in ordered)
            {
                double startingBalance = debt.LoanAmount - debt.DownPayment;
                balances[debt] = startingBalance;
                paidOff[debt] = startingBalance <= 0;

                result.BalancesByDebtName[debt.Name] = new List<MonthlyColumn>
                {
                    new MonthlyColumn { Name = debt.Name, Date = simulationStart, Gain = Math.Round(startingBalance, 2) }
                };
                result.PayoffDateByDebtName[debt.Name] = paidOff[debt] ? (DateTime?)simulationStart : null;
            }

            // Seed the opening month with what's actually owed right now. The payment loop
            // below starts at month 1 (the first payment date), so without this a freshly
            // added loan reports no obligation for the current month.
            result.TotalPaymentByDate[ToMonthBucket(simulationStart)] = Math.Round(
                ordered.Where(d => !paidOff[d]).Sum(d => d.MonthlyPayment), 2);

            double rolloverPool = 0;

            for (int month = 1; month <= MaxMonths; month++)
            {
                if (ordered.All(d => paidOff[d]))
                    break;

                DateTime currentDate = simulationStart.AddMonths(month);
                double freedThisMonth = 0;
                double totalPaymentThisMonth = 0;
                bool rolloverApplied = false;

                foreach (var debt in ordered)
                {
                    if (paidOff[debt])
                        continue;

                    if (currentDate < debt.StartDate)
                    {
                        result.BalancesByDebtName[debt.Name].Add(new MonthlyColumn { Name = debt.Name, Date = currentDate, Gain = Math.Round(balances[debt], 2) });
                        continue;
                    }

                    double currBalance = balances[debt];
                    double interestPay = currBalance * debt.InterestRate / 12;
                    double extraFromEvents = AccountEventResolver.ResolveAdditionalAmount(debt.AccountLifeEvents, currentDate);

                    double payment = debt.MonthlyPayment + extraFromEvents;
                    if (!rolloverApplied && rolloverPool > 0)
                    {
                        payment += rolloverPool;
                        rolloverApplied = true;
                    }

                    double principalPay = payment - interestPay;
                    if (principalPay < 0)
                        principalPay = 0;
                    if (principalPay > currBalance)
                        principalPay = currBalance;

                    currBalance -= principalPay;
                    totalPaymentThisMonth += interestPay + principalPay;

                    if (currBalance <= 0.005)
                    {
                        currBalance = 0;
                        paidOff[debt] = true;
                        result.PayoffDateByDebtName[debt.Name] = currentDate;
                        freedThisMonth += debt.MonthlyPayment;
                    }

                    balances[debt] = currBalance;
                    result.BalancesByDebtName[debt.Name].Add(new MonthlyColumn { Name = debt.Name, Date = currentDate, Gain = Math.Round(currBalance, 2) });
                }

                result.TotalPaymentByDate[ToMonthBucket(currentDate)] = Math.Round(totalPaymentThisMonth, 2);
                rolloverPool += freedThisMonth;
            }

            return result;
        }

        private static List<LoanAccount> OrderDebts(List<LoanAccount> debts, DebtPayoffStrategy strategy)
        {
            if (strategy == DebtPayoffStrategy.Avalanche)
                return debts.OrderByDescending(d => d.InterestRate).ToList();

            return debts.OrderBy(d => d.LoanAmount - d.DownPayment).ToList();
        }
    }
}
