﻿using LifeCalculator.Framework.SimulatedAccount;
using System;
using LifeCalculator.Framework.LifeEvents;
using Microsoft.VisualStudio.PlatformUI;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Control.Accounts;
using LifeCalculator.Framework.Managers;

namespace LifeCalculator.Control.ViewModels
{
    //Compound Interest View Model
    public class AddCompoundViewModel : ViewModelBase , IControlAccount
    {
        #region Events

        public event EventHandler<IAccount> AccountAdded;
        public event EventHandler<IAccount> AccountModified;

        #endregion

        #region Fields

        IAccountsEventsManager _accountsEventsManager;
        private IAccountStore _accountStore;


        #endregion

        #region Constructors

        public AddCompoundViewModel(IAccountStore accountStore)
        {
            _accountStore = accountStore;
            _accountsEventsManager = accountStore.CurrentAccount.AccountsEventsManager;

            AddAccountCommand = new DelegateCommand(AddAccountCommandHandler);

            StartDate = DateTime.Now;
            StopDate = DateTime.Now.AddYears(1);
        }

        #endregion

        #region Properties

        public string AccountName { get; set; }
        public string DescriptionText { get; set; }
        public double InitialValue { get; set; }
        public double Interest { get; set; }
        public double Contribute { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StopDate { get; set; }
        public DelegateCommand AddAccountCommand { get; set; }


        #endregion

        #region Command Handlers

        /// <summary>
        /// Creates an Account with a Start/Stop Event
        /// </summary>
        private void AddAccountCommandHandler()
        {

            var investmentAccount = new CompoundAccount(_accountsEventsManager)
            {
                Name = AccountName,
                InitialAmount = InitialValue,
                UserId = _accountStore.CurrentAccount.Id
            };

            _accountStore.CurrentAccount.SimulatedAccountManager.AddAccount(investmentAccount);


            investmentAccount.SetupBasicCalculation(StartDate, StopDate, Interest, InitialValue, Contribute);

        }

        #endregion

    }
}

