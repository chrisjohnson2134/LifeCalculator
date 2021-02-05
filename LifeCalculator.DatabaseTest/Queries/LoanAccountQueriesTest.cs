using NUnit.Framework;
using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Queries;

namespace LifeCalculator.DatabaseTest
{
    [TestFixture]
    public class LoanAccountQueriesTest
    {
        [Test]
        public void LoadAndSaveDataFromDatabase()
        {
            //Save Data to Database
            LoanQueries.SaveLoanAccount(new LoanAccount() { Name = "chris" });

            //Load Data From Database
            var a = LoanQueries.LoadLoanAccounts();
            Assert.That(a.Exists(x => x.Name.Equals("Chris")));
        }
    }
}
