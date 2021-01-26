using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Enums;
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
        LoanAccount LoanAccount;

        public LoanAccountTest()
        {
            LoanAccount = new LoanAccount("mortgage",DateTime.Now,360, 2.75,327000,65400);
        }

        [Test]
        public void BasicCalculation()
        {
            LoanAccount.MonthlyPayment.ShouldEqual(1067);

            LoanAccount.AccountLifeEvents[0].Name.ShouldEqual("Start - "+ LoanAccount.Name);
            LoanAccount.AccountLifeEvents[1].Name.ShouldEqual("Stop - " + LoanAccount.Name);


        }

        [Test]
        public void CalculationTest()
        {
            var calcs = LoanAccount.Calculation();

            calcs[11].Gain.ShouldBeInRange(5201.83, 5201.84);
            calcs[23].Gain.ShouldBeInRange(11027.95, 11027.96);
            calcs[35].Gain.ShouldBeInRange(17016.32, 17016.33);
            calcs[35].Gain.ShouldBeInRange(17016.32, 17016.33);
            calcs[179].Gain.ShouldBeInRange(103310.61, 103310.62);
            (calcs[360].Gain).ShouldBeInRange(261599.99,
                261600.01);

             calcs = LoanAccount.Calculation();

            calcs[11].Gain.ShouldBeInRange(5201.83, 5201.84);
            calcs[23].Gain.ShouldBeInRange(11027.95, 11027.96);
            calcs[35].Gain.ShouldBeInRange(17016.32, 17016.33);
            calcs[35].Gain.ShouldBeInRange(17016.32, 17016.33);
            calcs[179].Gain.ShouldBeInRange(103310.61, 103310.62);
            (calcs[360].Gain).ShouldBeInRange(261599.99,
                261600.01);
        }

        [Test]
        public void AddMonthlyPriPaymentsCalculationTest()
        {
            ILifeEvent monthlyContribute = new LoanLifeEvent()
            {
                Date = DateTime.Now,
                EndDate = DateTime.Now.AddYears(30),
                Amount = 400,
                LifeEventType = LifeEnum.MonthlyContribute
            };

            LoanAccount.AddLifeEvent(monthlyContribute);

            var calcs = LoanAccount.Calculation();

            calcs[11].Gain.ShouldBeInRange(9652.59, 9652.60);
            calcs[23].Gain.ShouldBeInRange(20463.63, 20463.64);
            calcs[35].Gain.ShouldBeInRange(31575.74, 31575.75);
            calcs[179].Gain.ShouldBeInRange(191704.72, 191704.73);
            calcs[230].Gain.ShouldBeInRange(261600,261600.01);
        }
    }
}
