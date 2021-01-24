using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using NUnit.Framework;
using Should;
using System;

namespace LifeCalcuator.FrameworkTest.Account
{
    //Calculator Used to check Calculations https://www.bankrate.com/calculators/mortgages/mortgage-calculator.aspx

    [TestFixture]
    public class LoanAccountTest
    {
        LoanAccount mortgageAccount;

        public LoanAccountTest()
        {
            mortgageAccount = new LoanAccount("mortgage",DateTime.Now,360, 2.75,327000,65400);
        }

        [Test]
        public void BasicCalculation()
        {
            mortgageAccount.MonthlyPayment.ShouldEqual(1067);

            mortgageAccount.AccountLifeEvents[0].Name.ShouldEqual("Start - "+ mortgageAccount.Name);
            mortgageAccount.AccountLifeEvents[1].Name.ShouldEqual("Stop - " + mortgageAccount.Name);


        }

        [Test]
        public void CalculationTest()
        {
            var calcs = mortgageAccount.Calculation();

            calcs[11].Gain.ShouldBeInRange(5201.83, 5201.84);
            calcs[23].Gain.ShouldBeInRange(11027.95, 11027.96);
            calcs[35].Gain.ShouldBeInRange(17016.32, 17016.33);
            calcs[35].Gain.ShouldBeInRange(17016.32, 17016.33);
            calcs[179].Gain.ShouldBeInRange(103310.61, 103310.62);
            (calcs[360].Gain).ShouldBeInRange(261599.99,
                261600.01);

             calcs = mortgageAccount.Calculation();

            calcs[11].Gain.ShouldBeInRange(5201.83, 5201.84);
            calcs[23].Gain.ShouldBeInRange(11027.95, 11027.96);
            calcs[35].Gain.ShouldBeInRange(17016.32, 17016.33);
            calcs[35].Gain.ShouldBeInRange(17016.32, 17016.33);
            calcs[179].Gain.ShouldBeInRange(103310.61, 103310.62);
            (calcs[360].Gain).ShouldBeInRange(261599.99,
                261600.01);
        }
    }
}
