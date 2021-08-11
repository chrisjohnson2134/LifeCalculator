using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Account.Manager;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.FrameworkTest.Account.Manager
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

       
    }
}
