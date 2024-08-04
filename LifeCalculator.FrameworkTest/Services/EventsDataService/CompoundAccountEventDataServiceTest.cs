using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.EventsDataService;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifeCalculator.Framework.Managers;

namespace LifeCalculator.FrameworkTest.Services.EventsDataService
{
    [TestFixture]
    public class CompoundAccountEventDataServiceTest
    {
        private List<IAccountEvent> AccountLifeEventsExpected;
        private List<IAccountEvent> AccountLifeEventsList;
        IAccountsEventsManager acocuntsEventManager;

        public IAccountEvent CreateAccountEvent()
        {
            return new AccountEvent();
        }

        [Test,Ignore("needs re-implementing")]
        public void GenericDBTest()
        {
        }

    }
}
