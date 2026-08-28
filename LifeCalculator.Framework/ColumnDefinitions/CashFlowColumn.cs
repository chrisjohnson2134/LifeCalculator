using System;

namespace LifeCalculator.Framework.ColumnDefinitions
{
    public class CashFlowColumn
    {
        public DateTime Date { get; set; }
        public double TotalIncome { get; set; }
        public double TotalBills { get; set; }
        public double TotalDebtPayments { get; set; }
        public double TotalContributions { get; set; }
        public double Surplus { get; set; }
    }
}
