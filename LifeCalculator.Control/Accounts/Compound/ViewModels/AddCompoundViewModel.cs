using LifeCalculator.Framework.Account;
using System;
using LifeCalculator.Framework.LifeEvents;
using Microsoft.VisualStudio.PlatformUI;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Control.Accounts;

namespace LifeCalculator.Control.ViewModels
{
    //Compound Interest View Model
    public class AddCompoundViewModel : ViewModelBase , IControlAccount
    {
        #region Events

        public event EventHandler<IAccount> AccountAdded;

        #endregion

        #region Constructors

        public AddCompoundViewModel(IAccountStore accountStore)
        {
            _accountStore = accountStore;

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

        private IAccountStore _accountStore;

        #endregion

        #region Command Handlers

        /// <summary>
        /// Creates an Account with a Start/Stop Event
        /// </summary>
        private void AddAccountCommandHandler()
        {

            var investmentAccount = new CompoundAccount(AccountName)
            {
                InitialAmount = InitialValue,
                UserId = _accountStore.CurrentAccount.Id
            };

            AccountEvent startEvent = new AccountEvent()
            {
                Name = "Start - " + AccountName,
                StartDate = StartDate,
                InterestRate = Interest,
                Amount = Contribute
            };

            AccountEvent stopEvent = new AccountEvent()
            {
                Name = "Stop - " + AccountName,
                StartDate = StopDate,
                InterestRate = 0,
                Amount = 0
            };

            investmentAccount.AddLifeEvent(startEvent);
            investmentAccount.AddLifeEvent(stopEvent);

            _accountStore.CurrentAccount.Accounts.Add(investmentAccount);

            AccountAdded?.Invoke(this, investmentAccount);
        }

        #endregion

    }
}

