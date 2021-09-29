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
        public void MontlyPaymentCalculated()
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
                EndDate = DateTime.Now.AddYears(10),
                Amount = 400,
                LifeEventType = LifeEnum.MonthlyContribute
            };

            localLoanAccount.AddLifeEvent(monthlyContribute);

            var calcs = localLoanAccount.Calculation();

            calcs[11].Gain.ShouldEqual(27725.99);
            calcs[23].Gain.ShouldEqual(19578.99);
            calcs[35].Gain.ShouldEqual(11205.11);
            calcs[46].Gain.ShouldEqual(3324.35);
            calcs[50].Gain.ShouldEqual(409.06);
            calcs[51].Gain.ShouldEqual(-323.94);
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
                Amount = 10000,
                LifeEventType = LifeEnum.OneTime
            };

            localLoanAccount.AddLifeEvent(oneTimeContribute);

            var calcs = localLoanAccount.Calculation();
            calcs[11].Gain.ShouldEqual(32176.75);
            calcs[23].Gain.ShouldEqual(18759.68);
            calcs[35].Gain.ShouldEqual(15223.95);
            calcs[46].Gain.ShouldEqual(11896.43);
            calcs[83].Gain.ShouldEqual(68.54);
            calcs[84].Gain.ShouldEqual(0);
            calcs[119].Gain.ShouldEqual(0);
            calcs = LoanAccount.Calculation();

            calcs[11].Gain.ShouldEqual(32176.75);
            calcs[23].Gain.ShouldEqual(29014.67);
            calcs[35].Gain.ShouldEqual(25764.53);
            calcs[46].Gain.ShouldEqual(22705.79);
            calcs[119].Gain.ShouldEqual(332.98);
        }

        
    }
}
