using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using NUnit.Framework;
using System;

namespace LifeCalculator.Framework.Test.Account
{
    [TestFixture]
    class InvestmentAccountTest
    {

        [Test]
        public void AddAccountStartEndEventsTest()
        {
            string savingsAccountName = "Savings Account";

            InvestmentLifeEvent investmentLifeEventStart = new InvestmentLifeEvent()
            {
                InterestRate = .1,
                Date = DateTime.Now,
                CurrentValue = 100,
                Amount = 10
            };

            InvestmentLifeEvent investmentLifeEventStop = new InvestmentLifeEvent()
            {
                InterestRate = .1,
                Date = DateTime.Now.AddYears(1),
                CurrentValue = 100,
                Amount = 10
            };

            InvestmentAccount investmentAccount = new InvestmentAccount()
            {
                Name = savingsAccountName,
                InitialAmount = 100
            };

            investmentAccount.Calculation();
            Console.WriteLine(investmentAccount.FinalAmount);

            Assert.Fail();
        }
    }
}
