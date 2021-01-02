using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using NUnit.Framework;
using Should;
using System;

namespace LifeCalcuator.FrameworkTest.Account
{
    //Calculator Used to check Calculations https://www.bankrate.com/calculators/mortgages/mortgage-calculator.aspx

    [TestFixture]
    public class MortgageAccountTest
    {
        LoanAccount mortgageAccount;

        public MortgageAccountTest()
        {
            mortgageAccount = new LoanAccount(DateTime.Now, 2.75,327000,65400);
        }

        [Test]
        public void BasicCalculation()
        {
            mortgageAccount.MonthlyPayment.ShouldEqual(1067);
        }

        [Test]
        public void CalculationTest()
        {
            var calcs = mortgageAccount.Calculation();

            calcs[10].Amount.ShouldBeInRange(5201.83, 5201.84);
            calcs[22].Amount.ShouldBeInRange(11027.95, 11027.96);
            calcs[34].Amount.ShouldBeInRange(17016.32, 17016.33);
            calcs[34].Amount.ShouldBeInRange(17016.32, 17016.33);
            calcs[178].Amount.ShouldBeInRange(103310.61, 103310.62);
        }
    }
}
