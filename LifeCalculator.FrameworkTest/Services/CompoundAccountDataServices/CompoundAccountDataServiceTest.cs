using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.AccountDataServices;
using LifeCalculator.Framework.Services.EventsDataService;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.FrameworkTest.Services.CompoundAccountDataServices
{
    [TestFixture]
    public class CompoundAccountDataServiceTest
    {
        protected List<IAccountEvent> AccountLifeEventsExpected;
        public readonly double InitialAmountExpected = 100.1;
        public readonly double FinalAmountExpected = 200.2;

        public CompoundAccount CreateCompoundAccount(string name)
        {
            var AccountLifeEventsExpected = new List<IAccountEvent>();
            AccountLifeEventsExpected.Add(new AccountEvent() { Name = "start" });
            AccountLifeEventsExpected.Add(new AccountEvent() { Name = "stop" });

            return new CompoundAccount()
            {
                Name = name,
                UserId = 1234,
                InitialAmount = InitialAmountExpected,
                FinalAmount = FinalAmountExpected,
                AccountLifeEvents = AccountLifeEventsExpected
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

            Assert.That(RoulphInsertedAccount.Equals(RoulphLoadedAccount));

            RoulphInsertedAccount.FinalAmount = 123.1;
            RoulphInsertedAccount.InitialAmount = 321.2;
            RoulphInsertedAccount.Name = "newName";

            //SAVE
            await dataService.Save(RoulphInsertedAccount.Id, RoulphInsertedAccount);

            RoulphLoadedAccount = await dataService.Load(RoulphInsertedAccount.Id);

            Assert.That(RoulphInsertedAccount.Equals(RoulphLoadedAccount));

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
