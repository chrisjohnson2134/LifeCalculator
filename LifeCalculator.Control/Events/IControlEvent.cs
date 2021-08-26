using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Control.Events
{
    public interface IControlEvent
    {
        event EventHandler<IAccountEvent> EventAdded;
    }
}
