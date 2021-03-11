using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.FrameworkTest.Services
{
    [TestFixture]
    public class LoanAccountDataServiceTest
    {

        protected List<IAccountEvent> AccountLifeEventsExpected;
        public LoanAccount CreateCompoundAccount(string name)
        {
            var AccountLifeEventsExpected = new List<IAccountEvent>();
            AccountLifeEventsExpected.Add(new AccountEvent() { Name = "start", AccountType = AccountTypes.LoanAccount });
            AccountLifeEventsExpected.Add(new AccountEvent() { Name = "stop", AccountType = AccountTypes.LoanAccount });

            return new LoanAccount()
            {
                Name = name,
                AccountLifeEvents = AccountLifeEventsExpected
            };
        }

        [Test]
        //not fully working list need to be compared in equals method.
        public async Task GeneralCRUDTest()
        {
            var GusAccount = CreateCompoundAccount("Gus");
            var RoulphAccount = CreateCompoundAccount("Roulph");

            var dataService = new LoanAccountDataService();

            //INSERT
            GusAccount = await dataService.Insert(GusAccount);
            var RoulphInsertedAccount = await dataService.Insert(RoulphAccount);

            Assert.IsFalse(RoulphInsertedAccount.Equals(RoulphAccount));

            //LOAD
            var RoulphLoadedAccount = await dataService.Load(RoulphInsertedAccount.Id);

            Assert.That(RoulphInsertedAccount.Equals(RoulphLoadedAccount));

            RoulphInsertedAccount.Name = "newName";

            //SAVE
            await dataService.Save(RoulphInsertedAccount.Id, RoulphInsertedAccount);

            RoulphLoadedAccount = await dataService.Load(RoulphInsertedAccount.Id);

            Assert.That(RoulphInsertedAccount.Equals(RoulphLoadedAccount));

            //DELETE
            dataService.Delete(RoulphLoadedAccount.Id);
            dataService.Delete(GusAccount.Id);

            try
            {
                RoulphLoadedAccount = await dataService.Load(RoulphLoadedAccount.Id);
            }
            catch
            {
                RoulphLoadedAccount = null;
            }

            Assert.IsFalse(RoulphInsertedAccount.Equals(RoulphLoadedAccount));

        }

        

    }
}
