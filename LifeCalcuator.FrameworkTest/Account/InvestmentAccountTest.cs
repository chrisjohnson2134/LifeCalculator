using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using NUnit.Framework;
using Should;
using System;

namespace LifeCalcuator.FrameworkTest.Account
{
    //calculator I trust to prove calculations
    //https://www.bankrate.com/calculators/savings/compound-savings-calculator-tool.aspx

    [TestFixture]
    public class InvestmentAccountTest
    {
        [Test]
        public void BasicCompoundInterestCalculation()
        {
            InvestmentAccount investmentAccount = new InvestmentAccount();

            investmentAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddYears(1),
            .1, 100, 10);

            investmentAccount.Calculation();
            investmentAccount.FinalAmount.ShouldBeInRange(549.06, 549.07);
        }

        [Test]
        public void BasicMonthByMonthReturnTest()
        {
            InvestmentAccount investmentAccount = new InvestmentAccount();

            investmentAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddYears(1),
            .1, 100, 10);

            var monthlyList = investmentAccount.Calculation();

            monthlyList[5].Amount.ShouldBeInRange(262.02, 262.03);
            monthlyList[11].Amount.ShouldBeInRange(549.06, 549.07);
        }

        [Test]
        public void AddedLifeEventComputation()
        {
            InvestmentAccount investmentAccount = new InvestmentAccount();

            investmentAccount.AddLifeEvent(new InvestmentLifeEvent
            {
                Name = "addition",
                Amount = 10,
                Date = DateTime.Now,
                InterestRate = .1
            });

            investmentAccount.AddLifeEvent(new InvestmentLifeEvent
            {
                Name = "addition",
                Amount = 1000,
                Date = DateTime.Now.AddYears(1),
                InterestRate = .1
            });

            var midCalculationCheck = investmentAccount.Calculation();
            midCalculationCheck[11].Amount.ShouldBeInRange(235.22, 235.23);
            investmentAccount.FinalAmount.ShouldBeInRange(235.22, 235.23);

            investmentAccount.AddLifeEvent(new InvestmentLifeEvent
            {
                Name = "addition",
                Amount = 0,
                Date = DateTime.Now.AddYears(2),
                InterestRate = .1
            });

            var a = investmentAccount.Calculation();
            a[11].Amount.ShouldBeInRange(235.22, 235.23);
            a[23].Amount.ShouldBeInRange(24260.95, 24260.96);
            investmentAccount.FinalAmount.ShouldBeInRange(24260.95, 24260.96);
        }
    }
}
