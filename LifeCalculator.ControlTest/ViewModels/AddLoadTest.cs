﻿using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.AccountManager;
using LifeCalculator.Tools.Common.Converters;
using NUnit.Framework;
using System;

namespace LifeCalculator.ControlTest.ViewModels
{
    [TestFixture]
    class AddLoanViewModelTest
    {
        public AddLoanViewModel addLoanviewModel;
        public AccountManager accountManager;


        [Test]
        public void addMortgageAccountTest()
        {
            init();

            addLoanviewModel.AccountName = "Mortgage";
            addLoanviewModel.StartDate = DateTime.Now;
            addLoanviewModel.InitialLoanAmount = "327000";
            addLoanviewModel.InterestRate = "2.75";
            addLoanviewModel.LoanLength = "30 Year";
            addLoanviewModel.DownPayment = "60000";

            addLoanviewModel.AddAccountCommand.Execute();

        }

        private void init()
        {
            accountManager = new AccountManager();

            addLoanviewModel = new AddLoanViewModel(accountManager);
        }

    }
}
