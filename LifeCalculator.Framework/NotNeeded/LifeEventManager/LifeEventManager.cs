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
        public List<IAccountEvent> LifeEventList { get; set; }

        public event EventHandler<IAccountEvent> LifeEventAdded;

        public LifeEventManager()
        {
            LifeEventList = new List<IAccountEvent>();
        }

        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
            LifeEventList.Add(lifeEvent);
            LifeEventAdded?.Invoke(this, lifeEvent);
        }
    }
}


/// <summary>
/// Not Needed
/// </summary>