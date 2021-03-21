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
            CompoundAccount investmentAccount = new CompoundAccount();

            investmentAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddYears(1),
            10, 100, 10);

            investmentAccount.Calculation();
            investmentAccount.FinalAmount.ShouldBeInRange(237.17, 237.18);
        }

        [Test]
        public void BasicMonthByMonthReturnTest()
        {
            CompoundAccount investmentAccount = new CompoundAccount();

            investmentAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddYears(1),
            1, 100, 10);

            var monthlyList = investmentAccount.Calculation();

            monthlyList[6].Gain.ShouldBeInRange(160.67, 160.68);
            monthlyList[12].Gain.ShouldBeInRange(221.65, 221.66);
        }

        [Test]
        public void AddedLifeEventComputation()
        {
            CompoundAccount investmentAccount = new CompoundAccount();

            investmentAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddYears(1),
            10, 100, 10);

            investmentAccount.AddLifeEvent(new AccountEvent
            {
                Name = "addition",
                Amount = 1000,
                StartDate = DateTime.Now.AddYears(1),
                InterestRate = 10
            });

            var midCalculationCheck = investmentAccount.Calculation();
            midCalculationCheck[12].Gain.ShouldBeInRange(126.70, 126.71);
            investmentAccount.FinalAmount.ShouldBeInRange(126.70, 126.71);

            investmentAccount.AddLifeEvent(new AccountEvent
            {
                Name = "addition",
                Amount = 0,
                StartDate = DateTime.Now.AddYears(2),
                InterestRate = 10
            });

            var a = investmentAccount.Calculation();
            a[12].Gain.ShouldBeInRange(126.70, 126.71);
            a[24].Gain.ShouldBeInRange(12810.25, 12810.26);
            investmentAccount.FinalAmount.ShouldBeInRange(12810.25, 12810.26);
        }
    }
}
