using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.LifeEvents;
using NUnit.Framework;
using Should;
using System;
using LifeCalculator.Framework.Managers;

namespace LifeCalcuator.FrameworkTest.SimulatedAccount
{
    //calculator I trust to prove calculations
    //https://www.bankrate.com/calculators/savings/compound-savings-calculator-tool.aspx

    [TestFixture]
    public class InvestmentAccountTest
    {
        IAccountsEventsManager accountEventManager;

        [Test]
        public void BasicCompoundInterestCalculation()
        {
            accountEventManager = new AccountsEventsManager();
            CompoundAccount investmentAccount = new CompoundAccount(accountEventManager);

            investmentAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddYears(1),
            10, 100, 10);

            investmentAccount.Calculation();
            investmentAccount.FinalAmount.ShouldBeInRange(237.17, 237.18);
        }

        [Test]
        public void BasicMonthByMonthReturnTest()
        {
            IAccountsEventsManager accountEventManager = new AccountsEventsManager();
            CompoundAccount investmentAccount = new CompoundAccount(accountEventManager);

            investmentAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddYears(1),
            1, 100, 10);

            var monthlyList = investmentAccount.Calculation();

            monthlyList[6].Gain.ShouldBeInRange(160.67, 160.68);
            monthlyList[12].Gain.ShouldBeInRange(221.65, 221.66);
        }

        [Test]
        public void AddedLifeEventComputation()
        {
            IAccountsEventsManager accountEventManager = new AccountsEventsManager();
            CompoundAccount investmentAccount = new CompoundAccount(accountEventManager);

            investmentAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddYears(2),
            10, 100, 10);

            investmentAccount.AddLifeEvent(new AccountEvent
            {
                Name = "addition",
                Amount = 1000,
                StartDate = DateTime.Now.AddYears(1),
                LifeEventType = LifeCalculator.Framework.Enums.LifeEnum.OneTime
            }); 

            var midCalculationCheck = investmentAccount.Calculation();
            midCalculationCheck[13].Gain.ShouldEqual(1257.57);
            investmentAccount.FinalAmount.ShouldEqual(1493.43);

        }

        [Test]
        public void AddMonthlyEvents()
        {
            IAccountsEventsManager accountEventManager = new AccountsEventsManager();
            CompoundAccount investmentAccount = new CompoundAccount(accountEventManager);

            investmentAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddYears(2),
            10, 100, 10);

            investmentAccount.AddLifeEvent(new AccountEvent
            {
                Name = "addition",
                Amount = 10,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(2),
                LifeEventType = LifeCalculator.Framework.Enums.LifeEnum.MonthlyContribute
            });

            // Two $10 monthly contributions run here: the one SetupBasicCalculation creates and
            // the one added above, so $20 goes in every month from the first.
            //
            // These figures were 352.83 / 643.18 while event matching compared exact timestamps.
            // Both events start at "now", but the two DateTime.Now calls differ by microseconds,
            // so the second event tested as "not started yet" against the account's own start
            // instant and skipped its first month. Matching on whole months fixes that, and
            // $100 seed + $20/month at 10% compounded monthly gives these values.
            var midCalculationCheck = investmentAccount.Calculation();
            midCalculationCheck[12].Gain.ShouldEqual(363.88);
            investmentAccount.FinalAmount.ShouldEqual(655.39);

        }
    }
}
