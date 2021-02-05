using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.LifeEventManager
{
    public interface ILifeEventManager
    {
        List<IAccountEvent> LifeEventList { get; set; }
        event EventHandler<IAccountEvent> LifeEventAdded;
        void AddLifeEvent(IAccountEvent lifeEvent);
    }
}
