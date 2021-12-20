using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.AccountDataServices;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeCalculator.Framework.Managers;
using System.Threading;

namespace LifeCalculator.FrameworkTest.Services.CompoundAccountDataServices
{
    [TestFixture]
    public class CompoundAccountDataServiceTest
    {
        IAccountsEventsManager accountsEventsManager;

        protected List<IAccountEvent> AccountLifeEventsExpected;
        public readonly double InitialAmountExpected = 100.1;
        public readonly double FinalAmountExpected = 200.2;

        public CompoundAccount CreateCompoundAccount(string name)
        {
            accountsEventsManager = new AccountsEventsManager();
            Thread.Sleep(1000);

            return new CompoundAccount(accountsEventsManager)
            {
                Name = name,
                UserId = 1234,
                InitialAmount = InitialAmountExpected,
                FinalAmount = FinalAmountExpected
            };
        }

        [Test]
        //not fully working list need to be compared in equals method.
        public async Task DBGenericTest()
        {
            var GusAccount = CreateCompoundAccount("Gus");
            var RoulphAccount = CreateCompoundAccount("Roulph");

            var dataService = new CompoundAccountDataService();

            //INSERT
            GusAccount = await dataService.Insert(GusAccount);
            var RoulphInsertedAccount = await dataService.Insert(RoulphAccount);

            Assert.IsFalse(RoulphInsertedAccount.Equals(RoulphAccount));

            //LOAD
            var RoulphLoadedAccount = await dataService.Load(RoulphInsertedAccount.Id);

            Assert.AreEqual(RoulphInsertedAccount,RoulphLoadedAccount);

            RoulphInsertedAccount.FinalAmount = 123.1;
            RoulphInsertedAccount.InitialAmount = 321.2;
            RoulphInsertedAccount.Name = "newName";

            //SAVE
            await dataService.Save(RoulphInsertedAccount.Id, RoulphInsertedAccount);

            RoulphLoadedAccount = await dataService.Load(RoulphInsertedAccount.Id);

            Assert.AreEqual(RoulphInsertedAccount,RoulphLoadedAccount);

            //DELETE
            dataService.Delete(RoulphLoadedAccount.Id);

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
