using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Database.Queries;
using LifeCalculator.Framework.LifeEvents;
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
            AccountLifeEventsExpected.Add(new InvestmentAccountEvent() {Name="start" });
            AccountLifeEventsExpected.Add(new InvestmentAccountEvent() { Name = "stop" });

            return new CompoundAccount()
            {
                Name = accountNameExpected,
                InitialAmount = InitialAmountExpected,
                FinalAmount = FinalAmountExpected,
                AccountLifeEvents = AccountLifeEventsExpected
            };
        }

        [Test]
        public void CRUDMethodsDatabaseTest()
        {
            //Save Data to Database
            var createdAccount = CreateCompoundAccount();
            CompoundQueries.Save(createdAccount);

            //Load Data From Database
            var loadLoanAccounts = CompoundQueries.LoadByName("Chris");
            var accountCreatedLoaded = loadLoanAccounts.Find(x => x.Name.Equals(createdAccount.Name));
            Assert.That(loadLoanAccounts.Find(x => x.Name.Equals("josh")) == null);
            Assert.That(accountCreatedLoaded.Name.Equals(createdAccount.Name));

            //UpdateData
            accountCreatedLoaded.Name = "Josh";
            accountCreatedLoaded.InitialAmount = 321.1;
            accountCreatedLoaded.FinalAmount = 123.2;

            CompoundQueries.Update(accountCreatedLoaded);

            var loadUpdatedAccount = CompoundQueries.Load(accountCreatedLoaded);
            Assert.That(!loadUpdatedAccount.Name.Equals(accountNameExpected));
            Assert.That(!loadUpdatedAccount.InitialAmount.Equals(InitialAmountExpected));
            Assert.That(!loadUpdatedAccount.FinalAmount.Equals(FinalAmountExpected));

            //Delete Account

            CompoundQueries.Delete(loadUpdatedAccount);

            loadLoanAccounts = CompoundQueries.LoadByName("Chris");

            Assert.That(loadLoanAccounts.Find(x => x.Name.Equals("Chris")) == null);
        }
    }
}
