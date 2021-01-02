using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.LifeEventManager
{

    /// <summary>
    /// Not Needed
    /// </summary>
    public class LifeEventManager : ILifeEventManager
    {
        public List<ILifeEvent> LifeEventList { get; set; }

        public event EventHandler<ILifeEvent> LifeEventAdded;

        public LifeEventManager()
        {
            LifeEventList = new List<ILifeEvent>();
        }

        public void AddLifeEvent(ILifeEvent lifeEvent)
        {
            LifeEventList.Add(lifeEvent);
            LifeEventAdded?.Invoke(this, lifeEvent);
        }
    }
}


/// <summary>
/// Not Needed
/// </summary>