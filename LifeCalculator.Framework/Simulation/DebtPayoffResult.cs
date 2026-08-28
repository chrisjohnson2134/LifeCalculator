using LifeCalculator.Framework.ColumnDefinitions;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Simulation
{
    public class DebtPayoffResult
    {
        public Dictionary<string, List<MonthlyColumn>> BalancesByDebtName { get; } = new Dictionary<string, List<MonthlyColumn>>();
        public Dictionary<string, DateTime?> PayoffDateByDebtName { get; } = new Dictionary<string, DateTime?>();

        /// <summary>Total amount actually applied across all debts that month (interest + principal), keyed by date.</summary>
        public Dictionary<DateTime, double> TotalPaymentByDate { get; } = new Dictionary<DateTime, double>();
    }
}
