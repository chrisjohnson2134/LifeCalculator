using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using NUnit.Framework;
using Should;
using System;

namespace LifeCalcuator.FrameworkTest.Account
{
    //Calculator Used to check Calculations https://www.bankrate.com/calculators/mortgages/mortgage-calculator.aspx
    //Contacted company about 

    [TestFixture]
    public class LoanAccountTest
    {
        LoanAccount LoanAccount;

        private LoanAccount setupLoanAccount()
        {
            return new LoanAccount("mortgage", DateTime.Now, 120, 2.75, 40000, 5000);
        }

        public LoanAccountTest()
        {
            LoanAccount = setupLoanAccount();
        }

        [Test]
        public void BasicCalculation()
        {
            var testLoanAccount = setupLoanAccount();

            testLoanAccount.MonthlyPayment.ShouldEqual(333.94);
        }

        [Test]
        public void CalculationTest()
        {
            LoanAccount localLoanAccount = setupLoanAccount();

            var calcs = localLoanAccount.Calculation();
            calcs[11].Gain.ShouldEqual(32176.75);
            calcs[23].Gain.ShouldEqual(29014.67);
            calcs[35].Gain.ShouldEqual(25764.53);
            calcs[46].Gain.ShouldEqual(22705.79);
            calcs[119].Gain.ShouldEqual(332.98);
             calcs = LoanAccount.Calculation();

            calcs[11].Gain.ShouldEqual(32176.75);
            calcs[23].Gain.ShouldEqual(29014.67);
            calcs[35].Gain.ShouldEqual(25764.53);
            calcs[46].Gain.ShouldEqual(22705.79);
            calcs[119].Gain.ShouldEqual(332.98);

        }

        [Test]
        public void AddMonthlyPriPaymentsCalculationTest()
        {
            var localLoanAccount = setupLoanAccount();

            IAccountEvent monthlyContribute = new AccountEvent()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(30),
                Amount = 400,
                LifeEventType = LifeEnum.MonthlyContribute
            };

            localLoanAccount.AddLifeEvent(monthlyContribute);

            var calcs = localLoanAccount.Calculation();

            calcs[230-11].Gain.ShouldBeInRange(9652.59, 9652.60);
            calcs[23].Gain.ShouldBeInRange(20463.63, 20463.64);
            calcs[35].Gain.ShouldBeInRange(31575.74, 31575.75);
            calcs[179].Gain.ShouldBeInRange(191704.72, 191704.73);
            calcs[230].Gain.ShouldBeInRange(261600,261600.01);
        }

        /// <summary>
        /// I believe this is correct, Calculations done by hand.
        /// </summary>
        [Test]
        public void AddOneTimePriPaymentsCalculationTest()
        {
            LoanAccount localLoanAccount =  setupLoanAccount();

            IAccountEvent oneTimeContribute = new AccountEvent()
            {
                StartDate = DateTime.Now.AddYears(1),
                Amount = 100000,
                LifeEventType = LifeEnum.OneTime
            };

            localLoanAccount.AddLifeEvent(oneTimeContribute);

            var calcs = localLoanAccount.Calculation();

            calcs[11].Gain.ShouldBeInRange(261600-5201.83, 5201.84);
            calcs[12].Gain.ShouldBeInRange(105681.25, 105681.26);
            calcs[23].Gain.ShouldBeInRange(113577.86, 113577.87);
            calcs[35].Gain.ShouldBeInRange(122422.18, 122422.19);
            calcs[47].Gain.ShouldBeInRange(131512.80, 131512.81);
            (calcs[191].Gain).ShouldBeInRange(261599.99,
                261600.01);
        }

        
    }
}
