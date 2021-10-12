﻿using LifeCalculator.Framework.SimulatedAccount;
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
            midCalculationCheck[13].Gain.ShouldEqual(1249.23);
            investmentAccount.FinalAmount.ShouldEqual(1484.3);

        }
    }
}
