using LifeCalculator.Framework.Enums;
using System;

namespace LifeCalculator.Framework.LifeEvents
{
    public interface ILifeEvent
    {
        event EventHandler ValueChanged;
        string Name { get; set; }
        LifeEnum LifeEventType { get; set; }
        DateTime Date { get; set; }
        DateTime EndDate { get; set; }
        bool FinalEvent { get; set; }
        double CurrentValue { get; set; }
        double Amount { get; set; }
        double InterestRate { get; set; }
        
    }
}
