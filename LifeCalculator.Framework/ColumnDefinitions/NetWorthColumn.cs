using System;

namespace LifeCalculator.Framework.ColumnDefinitions
{
    public class NetWorthColumn
    {
        public DateTime Date { get; set; }
        public double TotalDebt { get; set; }
        public double TotalAssets { get; set; }
        public double NetWorth { get; set; }
    }
}
