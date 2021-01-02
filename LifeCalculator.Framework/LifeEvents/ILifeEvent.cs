using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.LifeEvents
{
    public interface ILifeEvent
    {
        event EventHandler ValueChanged;
        string Name { get; set; }
        LifeEnum LifeEventType { get; set; }
        DateTime Date { get; set; }
        bool FinalEvent { get; set; }


        double CurrentValue { get; set; }
        double Amount { get; set; }
        double InterestRate { get; set; }
        
    }
}
