using LifeCalculator.Framework.Enums;
using System;

namespace LifeCalculator.Framework.LifeEvents
{
    public class InvestmentLifeEvent : ILifeEvent
    {
        public event EventHandler ValueChanged;

        public string Name { get; set; }
        public LifeEnum LifeEventType { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public double CurrentValue { get; set; }
        public double InterestRate { get; set; }
        public bool FinalEvent { get; set; }
    }
}
