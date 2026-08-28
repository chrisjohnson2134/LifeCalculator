using System;

namespace LifeCalculator.Framework.ColumnDefinitions
{
    public class MonthlyColumn
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public double Gain { get; set; }

        /// <summary>
        /// Every account's Calculation() starts its list with a bare placeholder row whose Date
        /// is never set (so it defaults to year 1). Callers that plot or aggregate a real
        /// timeline must skip these, or the series stretches back to year 1 and the whole date
        /// axis becomes meaningless. The placeholder can't simply be removed from Calculation()
        /// because existing tests index into the results by position.
        /// </summary>
        public bool IsPlaceholder => Date == default(DateTime);
    }
}
