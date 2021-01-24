using LifeCalculator.Framework.Enums;
using System;

namespace LifeCalculator.Framework.LifeEvents
{
    public class MortgageLifeEvent : ILifeEvent
    {
        public event EventHandler ValueChanged;

        public string Name { get; set; }
        public LifeEnum LifeEventType { get; set; }
        public DateTime Date { get; set; }
        public bool FinalEvent { get; set; }

        //mortgage things
        public double CurrentValue { get; set; }
        public double InterestPaid { get; set; }
        public double PrincipalPaid { get; set; }
        public double InterestRate { get; set; }


        public double Amount { get; set; }
    }
}
