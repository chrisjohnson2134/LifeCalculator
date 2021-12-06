using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.LifeEvents;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.FrameworkTest.SimulatedAccount.Manager
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
            IAccount account = new CompoundAccount("Chris");

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
            CompoundAccount account = new CompoundAccount("Chris");
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
            account.AddLifeEvent(new AccountEvent());

            account.AccountLifeEvents[0].Name = "newName";
            account.AccountLifeEvents[0].AccountId = 23;
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
            LoanAccount account = new LoanAccount("Chris",DateTime.Now,36,.03,5000,500);
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
            var priPayment = new AccountEvent();
            account.AddLifeEvent(priPayment);

            account.AccountLifeEvents[0].Name = "newName";
            account.AccountLifeEvents[0].AccountId = 23;
            account.AccountLifeEvents[0].StartDate = DateTime.Now;
            account.AccountLifeEvents[0].EndDate = DateTime.Now.AddYears(5);
            account.AccountLifeEvents[0].CurrentValue = 45;
            account.AccountLifeEvents[0].Amount = 150;
            account.AccountLifeEvents[0].InterestRate = .04;

            Assert.AreEqual(14, valueChangedCount);
        }


    }
}
