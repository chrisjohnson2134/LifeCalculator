using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Managers;
using NUnit.Framework;
using Should;
using System;

namespace LifeCalcuator.FrameworkTest.SimulatedAccount
{
    [TestFixture]
    public class RetirementAccountTest
    {
        [Test]
        public void NoEmployerMatch_MatchesCompoundAccountMath()
        {
            // 0% interest isolates the contribution/match mechanics from compounding.
            var retirementEventsManager = new AccountsEventsManager();
            var retirementAccount = new RetirementAccount(retirementEventsManager);
            retirementAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddMonths(2), 0, 0, 100);

            var compoundEventsManager = new AccountsEventsManager();
            var compoundAccount = new CompoundAccount(compoundEventsManager);
            compoundAccount.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddMonths(2), 0, 0, 100);

            var retirementCalc = retirementAccount.Calculation();
            var compoundCalc = compoundAccount.Calculation();

            retirementCalc[1].Gain.ShouldEqual(compoundCalc[1].Gain);
            retirementCalc[2].Gain.ShouldEqual(compoundCalc[2].Gain);
            retirementAccount.FinalAmount.ShouldEqual(compoundAccount.FinalAmount);
        }

        [Test]
        public void EmployerMatch_CappedWhenContributionExceedsCap()
        {
            var eventsManager = new AccountsEventsManager();
            var account = new RetirementAccount(eventsManager)
            {
                EmployerMatchPercent = 0.5,
                EmployerMatchCapPercentOfSalary = 0.06
            };

            // Contributing 500/mo against a 5000/mo salary reference: cap = 5000*0.06 = 300,
            // so only 300 of the 500 is matchable => match = 0.5*300 = 150/mo.
            account.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddMonths(3), 0, 0, 500);

            var calc = account.Calculation(5000);

            calc[1].Gain.ShouldEqual(650.0);
            calc[2].Gain.ShouldEqual(1300.0);
            calc[3].Gain.ShouldEqual(1950.0);
            account.FinalAmount.ShouldEqual(1950.0);
        }

        [Test]
        public void EmployerMatch_FullyAppliedWhenContributionUnderCap()
        {
            var eventsManager = new AccountsEventsManager();
            var account = new RetirementAccount(eventsManager)
            {
                EmployerMatchPercent = 0.5,
                EmployerMatchCapPercentOfSalary = 0.06
            };

            // Contributing 200/mo against a 5000/mo salary reference: cap = 300, contribution is
            // under the cap, so the full 200 is matchable => match = 0.5*200 = 100/mo.
            account.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddMonths(2), 0, 0, 200);

            var calc = account.Calculation(5000);

            calc[1].Gain.ShouldEqual(300.0);
            calc[2].Gain.ShouldEqual(600.0);
        }

        [Test]
        public void NoSalaryReference_AppliesNoMatchEvenWithMatchPercentSet()
        {
            var eventsManager = new AccountsEventsManager();
            var account = new RetirementAccount(eventsManager)
            {
                EmployerMatchPercent = 0.5,
                EmployerMatchCapPercentOfSalary = 0.06
            };

            account.SetupBasicCalculation(DateTime.Now, DateTime.Now.AddMonths(1), 0, 0, 500);

            var calc = account.Calculation(0);

            calc[1].Gain.ShouldEqual(500.0);
        }
    }
}
