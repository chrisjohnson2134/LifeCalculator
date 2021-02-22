using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.EventsDataService;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.FrameworkTest.Services.EventsDataService
{
    [TestFixture]
    public class CompoundAccountEventDataServiceTest
    {
        private List<IAccountEvent> AccountLifeEventsExpected;
        private List<IAccountEvent> AccountLifeEventsList;

        public CompoundAccount CreateCompoundAccount(string name)
        {
            AccountLifeEventsExpected = new List<IAccountEvent>();
            AccountLifeEventsList = new List<IAccountEvent>();
            AccountLifeEventsList.Add(new AccountEvent()
            {
                Name = "start",
                AccountId = 100,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(1),
                InterestRate = .1,
                Amount = 100,
                CurrentValue = 100,
                LifeEventType = Framework.Enums.LifeEnum.OneTime
            });
            AccountLifeEventsList.Add(new AccountEvent() { Name = "stop" , AccountId = 100});
            AccountLifeEventsList.Add(new AccountEvent() { Name = "stop", AccountId = 200 });

            AccountLifeEventsExpected.Add(AccountLifeEventsList[0]);
            AccountLifeEventsExpected.Add(AccountLifeEventsList[1]);

            return new CompoundAccount()
            {
                Name = name,
                AccountLifeEvents = AccountLifeEventsExpected
            };
        }

        [Test]
        public void GenericDBTest()
        {
            var AccountLifeEvents = CreateCompoundAccount("Chris");
            var dataService = new CompoundAccountEventDataService();

            //INSERT
            for (int i = 0; i < AccountLifeEventsExpected.Count; i++)
            {
                AccountLifeEventsExpected[i] = dataService.Insert((AccountEvent)AccountLifeEventsExpected[i]).Result;
            }

            foreach(var accEvent in AccountLifeEventsExpected)
            {
                Assert.That(accEvent.Id != 0);
            }

            //LOAD
            List<AccountEvent> AccountLifeEventsLoaded = (List<AccountEvent>)dataService.LoadFromAccountID(100).Result;

            if ( AccountLifeEventsLoaded.Count != AccountLifeEventsExpected.Count )
                Assert.Fail();

            foreach(var accEvent in AccountLifeEventsExpected)
            {
                if (accEvent.AccountId == 200)
                    continue;

                var findEvent = ((List<AccountEvent>)AccountLifeEventsLoaded).Find(i => i.Id == accEvent.Id);
                Assert.That(findEvent.Equals(accEvent));
            }

            AccountLifeEventsLoaded[0].Amount = 321.321;
            AccountLifeEventsLoaded[1].Amount = 123.123;

            //UPDATE
            dataService.Save(AccountLifeEventsLoaded[0].Id,AccountLifeEventsLoaded[0]);
            dataService.Save(AccountLifeEventsLoaded[1].Id, AccountLifeEventsLoaded[1]);

            AccountLifeEventsLoaded = (List<AccountEvent>)dataService.LoadFromAccountID(100).Result;

            Assert.That(AccountLifeEventsLoaded[0].Amount == 321.321);
            Assert.That(AccountLifeEventsLoaded[1].Amount == 123.123);


            //DELETE
            foreach (var item in AccountLifeEventsLoaded)
            {
                dataService.Delete(item.Id);
            }

            AccountLifeEventsLoaded = (List<AccountEvent>)dataService.LoadFromAccountID(100).Result;

            if (AccountLifeEventsLoaded.Count != 0)
                Assert.Fail();

            //CLEAR List
            foreach (var item in AccountLifeEventsList)
            {
                dataService.Delete(item.Id);
            }

        }

    }
}
