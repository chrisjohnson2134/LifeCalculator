using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.LifeEvents;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LifeCalculator.Framework.Services.AccDataService;
using LifeCalculator.Framework.Enums;

namespace LifeCalculator.FrameworkTest.Managers
{
    [TestFixture]
    public class AccountManagerTest
    {
        public AccountManagerTest()
        {
        }

        private AccountManager createAccountManager()
        {
            return new AccountManager(); 
        }

        [Test]
        public void AddGetDeleteAccount()
        {
            IAccountsEventsManager accountEventManager = new AccountsEventsManager();
            IAccount account = new CompoundAccount(accountEventManager) { Name= "Chris"};

            int addedEventFired = 0;
            int deleteEventFired = 0;
            var testObject = createAccountManager();

            testObject.AccountAdded += (object obj, IAccount e) => { Assert.That(e.Equals(account)); addedEventFired++; };
            testObject.AccountAdded += (object obj, IAccount e) => { Assert.That(e.Equals(account)); deleteEventFired++; };

            testObject.AddAccount(account);

            IAccount getAccount = testObject.GetAccount(account.Id);
            Assert.That(getAccount.Equals(account));

            getAccount = testObject.GetAccount(account.Name);
            Assert.That(getAccount.Equals(account));

            testObject.DeleteAccount(account);

            Assert.That(addedEventFired == 1);
            Assert.That(deleteEventFired == 1);
        }

        [Test]
        public void ModifyingCompoundAccountTriggersValueChangedEvent()
        {
            IAccountsEventsManager accountEventManager = new AccountsEventsManager();
            CompoundAccount account = new CompoundAccount(accountEventManager) { Name = "Chris" };
            int valueChangedCount = 0;

            var testObject = createAccountManager();
            testObject.AccountChanged += (object obj, IAccount e) => { Assert.That(e.Equals(account)); valueChangedCount++; };

            testObject.AddAccount(account);

            //should trigger 7 events
            account.UserId = 125;
            account.Name = "Jeremy";
            account.InitialAmount = 30;
            account.InterestRate = .34;
            account.FinalAmount = 13;
            account.StartDate = DateTime.Today;
            account.EndDate = DateTime.Today.AddDays(1);

            //should trigger 8 events
            account.AddLifeEvent(new AccountEvent() { AccountId = account.Id});

            account.AccountLifeEvents[0].Name = "newName";
            account.AccountLifeEvents[0].AccountId = account.Id;
            account.AccountLifeEvents[0].StartDate = DateTime.Now;
            account.AccountLifeEvents[0].EndDate = DateTime.Now.AddYears(5);
            account.AccountLifeEvents[0].CurrentValue = 45;
            account.AccountLifeEvents[0].Amount = 150;
            account.AccountLifeEvents[0].InterestRate = .04;

            Assert.AreEqual(15,valueChangedCount);
        }

        [Test]
        public void ModifyingLoanAccountTriggersValueChangedEvent()
        {
            IAccountsEventsManager accountEventManager = new AccountsEventsManager();
            LoanAccount account = new LoanAccount("Chris",DateTime.Now,36,.03,5000,500);
            account.SetEventsManager(accountEventManager);
            int valueChangedCount = 0;

            var testObject = createAccountManager();
            testObject.AccountChanged += (object obj, IAccount e) => { Assert.That(e.Equals(account)); valueChangedCount++; };

            testObject.AddAccount(account);

            //should trigger 8 events
            account.UserId = 125;
            account.Name = "Jeremy";
            account.LoanAmount = 3000;
            account.DownPayment = 100;
            account.InterestRate = .03;
            account.StartDate = DateTime.Today;

            account.Calculation();

            //should trigger 8 events
            var priPayment = new AccountEvent() { AccountId = account.Id,AccountType = AccountTypes.LoanAccount};
            account.AddLifeEvent(priPayment);

            account.AccountLifeEvents[0].Name = "newName";
            account.AccountLifeEvents[0].AccountId = account.Id;
            account.AccountLifeEvents[0].StartDate = DateTime.Now;
            account.AccountLifeEvents[0].EndDate = DateTime.Now.AddYears(5);
            account.AccountLifeEvents[0].CurrentValue = 45;
            account.AccountLifeEvents[0].Amount = 150;
            account.AccountLifeEvents[0].InterestRate = .04;

            Assert.AreEqual(14, valueChangedCount);
        }

        [Test]
        public async Task AddingAndUpdatingAccountSavesToDatabase()
        {
            LoanAccount account = new LoanAccount("Chris", DateTime.Now, 36, .03, 5000, 500);
            int valueChangedCount = 0;

            IAccount addedLoanAccount = new LoanAccount() ;
            IAccount accountUpdated = new LoanAccount();

            var testObject = createAccountManager();
            testObject.AccountAdded += (sender, args) => { addedLoanAccount = args; };
            testObject.AccountChanged += (sender, args) => { accountUpdated = args; };

            testObject.AddAccount(account);

            AccountDataService dataService = new AccountDataService();

            var acc = await dataService.Load(account);

            Assert.AreEqual(addedLoanAccount, acc);

            account.Name = "newName";

            acc = await dataService.Load(account);

            Assert.AreEqual(accountUpdated, acc);

        }


    }
}
