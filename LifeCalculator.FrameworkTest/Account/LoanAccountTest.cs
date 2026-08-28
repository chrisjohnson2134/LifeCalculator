using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using NUnit.Framework;
using Should;
using System;
using LifeCalculator.Framework.Managers;

namespace LifeCalcuator.FrameworkTest.SimulatedAccount
{
    //Calculator Used to check Calculations https://www.bankrate.com/calculators/mortgages/mortgage-calculator.aspx
    //Contacted company about 

    [TestFixture]
    public class LoanAccountTest
    {
        LoanAccount LoanAccount;
        private LoanAccount setupLoanAccount()
        {
            AccountsEventsManager _eventsManager = new AccountsEventsManager();
            return new LoanAccount(_eventsManager,"mortgage", DateTime.Now, 120, 2.75, 40000, 5000) { Id = 0};
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
        public void SettingMonthlyPaymentDirectly_SolvesForLoanLength()
        {
            var testLoanAccount = setupLoanAccount();

            // Round-trip against the bankrate-verified case above: the same payment on the
            // same loan should solve back to (approximately) the same 120-month term.
            testLoanAccount.MonthlyPayment = 333.94;

            testLoanAccount.LoanLengthMonths.ShouldEqual(120);
        }

        [Test]
        public void SettingHigherMonthlyPayment_ShortensLoanLength()
        {
            var testLoanAccount = setupLoanAccount();

            testLoanAccount.MonthlyPayment = 600;

            (testLoanAccount.LoanLengthMonths < 120).ShouldBeTrue();
        }

        [Test]
        public void SettingMonthlyPaymentBelowInterestOnly_IsIgnored()
        {
            var testLoanAccount = setupLoanAccount();
            int originalLength = testLoanAccount.LoanLengthMonths;

            // 35000 principal @ 2.75%/12 accrues ~80.2/mo in interest; anything at or below
            // that would never pay down principal, so the loan length should be unaffected.
            testLoanAccount.MonthlyPayment = 50;

            testLoanAccount.LoanLengthMonths.ShouldEqual(originalLength);
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
                LifeEventType = LifeEnum.MonthlyContribute,
                AccountType = AccountTypes.LoanAccount
            };

            localLoanAccount.AddLifeEvent(monthlyContribute);

            var calcs = localLoanAccount.Calculation();

            calcs[11].Gain.ShouldEqual(27725.99);
            calcs[23].Gain.ShouldEqual(19578.99);
            calcs[35].Gain.ShouldEqual(11205.11);
            calcs[46].Gain.ShouldEqual(3324.35);
            calcs[50].Gain.ShouldEqual(409.06);
            calcs[51].Gain.ShouldEqual(0);

            calcs.Count.ShouldEqual(52);
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
                LifeEventType = LifeEnum.OneTime, 
                AccountType = AccountTypes.LoanAccount
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
