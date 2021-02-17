using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.AccountDataServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.FrameworkTest.Services.CompoundAccountDataServices
{
    [TestFixture]
    class CompoundAccountDataServiceTest
    {
        protected List<IAccountEvent> AccountLifeEventsExpected;
        public readonly double InitialAmountExpected = 100.1;
        public readonly double FinalAmountExpected = 200.2;

        public CompoundAccount CreateCompoundAccount(string name)
        {
            var AccountLifeEventsExpected = new List<IAccountEvent>();
            AccountLifeEventsExpected.Add(new InvestmentAccountEvent() { Name = "start" });
            AccountLifeEventsExpected.Add(new InvestmentAccountEvent() { Name = "stop" });

            return new CompoundAccount()
            {
                Name = name,
                InitialAmount = InitialAmountExpected,
                FinalAmount = FinalAmountExpected,
                AccountLifeEvents = AccountLifeEventsExpected
            };
        }

        [Test]
        //not fully working list need to be compared in equals method.
        public void DBGenericTest()
        {
            //will insert 2 chris
            var GusAccount = CreateCompoundAccount("Gus");
            var RoulphAccount = CreateCompoundAccount("Roulph");

            var dataService = new CompoundAccountDataService("CompoundAccounts");


            GusAccount = dataService.Insert(GusAccount).Result;
            var RoulphInsertedAccount = RoulphAccount.Insert(RoulphAccount).Result;

            Assert.IsFalse(RoulphInsertedAccount.Equals(RoulphAccount));

            var RoulphLoadedAccount = RoulphAccount.Load(RoulphInsertedAccount.Id).Result;

            Assert.IsTrue(RoulphInsertedAccount.Equals(RoulphLoadedAccount));

        }
    }
}
