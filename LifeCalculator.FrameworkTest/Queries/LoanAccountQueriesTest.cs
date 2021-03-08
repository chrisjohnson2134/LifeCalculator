using NUnit.Framework;
using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Queries;

namespace LifeCalculator.FrameworkTest.Queries
{
    [TestFixture]
    public class LoanAccountQueriesTest
    {
        public readonly string accountNameExpected = "Chris";
        public readonly double DownPaymentExpected = 100.1;
        public readonly double InterestPaidExpected = 200.2;
        public readonly double InterestRateExpected = 2.75;
        public readonly double LoanAmountExpeected = 300.3;
        public readonly int LoanLengthMonthsExpected = 36;
        public readonly double MonthlyPaymentExpected = 400.4;
        public readonly double PrincipalPaidExpected = 500.5;

        public LoanAccount CreateLoanAccount()
        {
            return new LoanAccount()
            {
                Name = accountNameExpected,
                DownPayment = DownPaymentExpected,
                InterestPaid = InterestPaidExpected,
                InterestRate = InterestRateExpected,
                LoanAmount = LoanAmountExpeected,
                LoanLengthMonths = LoanLengthMonthsExpected,
                MonthlyPayment = MonthlyPaymentExpected,
                PrincipalPaid = PrincipalPaidExpected
            };
        }

        //[Test]
        //public void CRUDMethodsDatabaseTest()
        //{
        //    //Save Data to Database
        //    var createdAccount = CreateLoanAccount();
        //    LoanQueries.Save(createdAccount);

        //    //Load Data From Database
        //    var loadLoanAccounts = LoanQueries.LoadByName("Chris");
        //    var accountCreatedLoaded = loadLoanAccounts.Find(x => x.Name.Equals(createdAccount.Name));
        //    Assert.That(loadLoanAccounts.Find(x => x.Name.Equals("josh")) == null);
        //    Assert.That(accountCreatedLoaded.Name.Equals(createdAccount.Name));

        //    //UpdateData
        //    accountCreatedLoaded.Name = "Josh";
        //    accountCreatedLoaded.MonthlyPayment = 123.3;
        //    accountCreatedLoaded.LoanAmount = 321.1;
        //    accountCreatedLoaded.DownPayment = 213.2;
        //    accountCreatedLoaded.InterestPaid = 111.1;
        //    accountCreatedLoaded.InterestRate = 222.2;
        //    accountCreatedLoaded.PrincipalPaid = 333.3;
        //    accountCreatedLoaded.LoanLengthMonths = 64;

        //    LoanQueries.Update(accountCreatedLoaded);

        //    var loadUpdatedAccount = LoanQueries.Load(accountCreatedLoaded);
        //    Assert.That(!loadUpdatedAccount.Name.Equals(accountNameExpected));
        //    Assert.That(!loadUpdatedAccount.MonthlyPayment.Equals(DownPaymentExpected));
        //    Assert.That(!loadUpdatedAccount.LoanAmount.Equals(LoanAmountExpeected));
        //    Assert.That(!loadUpdatedAccount.DownPayment.Equals(DownPaymentExpected));
        //    Assert.That(!loadUpdatedAccount.InterestPaid.Equals(InterestPaidExpected));
        //    Assert.That(!loadUpdatedAccount.InterestRate.Equals(InterestRateExpected));
        //    Assert.That(!loadUpdatedAccount.PrincipalPaid.Equals(PrincipalPaidExpected));
        //    Assert.That(!loadUpdatedAccount.LoanLengthMonths.Equals(LoanLengthMonthsExpected));

        //    //Delete Account

        //    LoanQueries.Delete(loadUpdatedAccount);

        //    loadLoanAccounts = LoanQueries.LoadByName("Chris");

        //    Assert.That(loadLoanAccounts.Find(x => x.Name.Equals("Chris")) == null);
        //}
    }
}
