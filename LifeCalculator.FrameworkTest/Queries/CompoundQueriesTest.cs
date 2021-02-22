using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Database.Queries;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.AccountDataServices;
using NUnit.Framework;
using System.Collections.Generic;

namespace LifeCalculator.FrameworkTest.Queries
{
    [TestFixture]
    public class CompoundQueriesTest
    {
        public readonly string accountNameExpected = "Chris";
        public readonly double InitialAmountExpected = 100.1;
        public readonly double FinalAmountExpected = 200.2;
        public List<IAccountEvent> AccountLifeEventsExpected;

        public CompoundAccount CreateCompoundAccount()
        {
            AccountLifeEventsExpected = new List<IAccountEvent>();
            AccountLifeEventsExpected.Add(new AccountEvent() {Name="start" });
            AccountLifeEventsExpected.Add(new AccountEvent() { Name = "stop" });

            return new CompoundAccount()
            {
                Name = accountNameExpected,
                InitialAmount = InitialAmountExpected,
                FinalAmount = FinalAmountExpected,
                AccountLifeEvents = AccountLifeEventsExpected
            };
        }

        public CompoundAccount CreateCompoundAccount(string name)
        {
            AccountLifeEventsExpected = new List<IAccountEvent>();
            AccountLifeEventsExpected.Add(new AccountEvent() { Name = "start" });
            AccountLifeEventsExpected.Add(new AccountEvent() { Name = "stop" });

            return new CompoundAccount()
            {
                Name = name,
                InitialAmount = InitialAmountExpected,
                FinalAmount = FinalAmountExpected,
                AccountLifeEvents = AccountLifeEventsExpected
            };
        }

        [Test]
        public void DBGenericTest()
        {
            //will insert 2 chris
            var GusAccount = CreateCompoundAccount("Gus");
            var RoulphAccount = CreateCompoundAccount("Roulph");

            var dataService = new CompoundAccountDataService("CompoundAccounts");


            GusAccount = dataService.Insert(GusAccount).Result;
            var y = dataService.Insert(RoulphAccount).Result;

            Assert.That(!y.Equals(RoulphAccount));

            var RoulphLoadedAccount = dataService.Load(RoulphAccount.Id).Result;

            Assert.That(RoulphAccount.Equals(RoulphLoadedAccount));

        }
    }
}
