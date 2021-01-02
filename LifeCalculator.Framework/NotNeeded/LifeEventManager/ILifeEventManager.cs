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
        List<ILifeEvent> LifeEventList { get; set; }
        event EventHandler<ILifeEvent> LifeEventAdded;
        void AddLifeEvent(ILifeEvent lifeEvent);
    }
}
