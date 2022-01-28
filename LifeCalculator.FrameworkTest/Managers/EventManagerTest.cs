using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Services.EventsDataService;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.FrameworkTest.Managers
{
    [TestFixture]
    public class EventManagerTest
    {
        #region Setup
        public AccountsEventsManager CreateEventManager()
        {
            return new AccountsEventsManager();
        }

        public AccountEventDataService CreateDataService()
        {
            return new AccountEventDataService();
        }

        public AccountEvent CreateBasicAccountEvent(string name = "default",int accountId = -1)
        {
            return new AccountEvent { Name = name, AccountId = accountId };
        }

        #endregion

        #region Test

        [Test]
        //1.) Event Should be added to the list 2.) saved to the database
        //3.) add event should fire
        public async Task AddingEventSavesToDatabaseAndFiresEvent()
        {
            var manager = CreateEventManager();
            var databaseService = new AccountEventDataService();

            IAccountEvent newEvent = CreateBasicAccountEvent(nameof(AddingEventSavesToDatabaseAndFiresEvent));
            IAccountEvent addedEvent = new AccountEvent();

            manager.AccountEventAdded += (obj,eventAdded)  => { addedEvent = eventAdded; };

            manager.AddAccountEvent(newEvent);

            Assert.AreEqual(newEvent,addedEvent); 

            var loadedDatabaseEntryEvent = await databaseService.Load(addedEvent.Id); 

            Assert.AreEqual(newEvent,loadedDatabaseEntryEvent);

            Assert.AreEqual(loadedDatabaseEntryEvent, manager.GetAccountEvent(newEvent.Id));
        }

        [Test]
        //1.) changing an event value should fire the ValueChanged Event
        //2.) Changin event value should save to the database.
        public async Task ChangingEventValueSavesToDatabase()
        {
            var manager = CreateEventManager();
            var databaseService = new AccountEventDataService();

            IAccountEvent newEvent = CreateBasicAccountEvent(nameof(ChangingEventValueSavesToDatabase));
            IAccountEvent addedEvent = new AccountEvent();
            IAccountEvent changedEvent = new AccountEvent();

            manager.AccountEventAdded += (obj, eventAdded) => { addedEvent = eventAdded; };
            manager.AccountEventChanged += (obj, eventAdded) => { changedEvent = eventAdded; };

            manager.AddAccountEvent(newEvent);

            var getEvent = manager.GetAccountEvent(newEvent.Id);

            getEvent.Name = "newName";

            Assert.AreEqual(getEvent, changedEvent);

            var databaseEvent = await databaseService.Load(getEvent.Id);

            Assert.AreEqual(getEvent, databaseEvent);
        }

        [Test]
        //1.) User Should be able to load all the accounts associated with a accountId
        public void LoadEventsByUserAccountId()
        {
            AccountEventDataService _accountEventDataService = new AccountEventDataService();

            var manager = CreateEventManager();

            manager.AddAccountEvent(CreateBasicAccountEvent(accountId: 1111));
            manager.AddAccountEvent(CreateBasicAccountEvent(accountId: 1111));
            manager.AddAccountEvent(CreateBasicAccountEvent(accountId: 2222));
            manager.AddAccountEvent(CreateBasicAccountEvent(accountId: 2222));
            manager.AddAccountEvent(CreateBasicAccountEvent(accountId: 3333));

            Assert.AreEqual(manager.GetAllAccountEventsByAccountId(1111,AccountTypes.CompoundInterest).Count, 2);
            Assert.AreEqual(manager.GetAllAccountEventsByAccountId(3333,AccountTypes.CompoundInterest).Count, 1);

            _accountEventDataService.DeleteByAccountID(1111);
            _accountEventDataService.DeleteByAccountID(2222);
            _accountEventDataService.DeleteByAccountID(3333);
        }

        [Test, Ignore("notImplemented")]
        //1.) Listed event properies should fire value changed.
        public async Task ChangingSpecifiedValuesWillFireValueChangeEvent()
        {

        }

        [Test,Ignore("not implemented.")]
        public void CorrectEventsLoadedForUserAccount()
        {

        }

        #endregion

    }
}
