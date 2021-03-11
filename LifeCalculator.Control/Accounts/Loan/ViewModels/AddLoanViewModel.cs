using LifeCalculator.Control.Accounts;
using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Managers.Interfaces;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Control.ViewModels
{
    public class AddLoanViewModel : ViewModelBase, IControlAccount
    {

        private IAccountStore _accountStore;
        public event EventHandler<IAccount> AccountAdded;

        #region Constructor

        public AddLoanViewModel()
        {
            AddAccountCommand = new DelegateCommand(AddAccountCommandHandler);
            StartDate = DateTime.Now;
        }

        public AddLoanViewModel(IAccountStore accountStore)
        {
            AddAccountCommand = new DelegateCommand(AddAccountCommandHandler);
            _accountStore = accountStore;
            StartDate = DateTime.Now;
        }

        #endregion

        #region Properties

        public DelegateCommand AddAccountCommand { get; set; }

        public string AccountName { get; set; }
        public DateTime StartDate { get; set; }
        public double InitialLoanAmount { get; set; }
        public double InterestRate { get; set; }
        public int LoanLength { get; set; }
        public double DownPayment { get; set; }

        public List<string> LoanLengths
        {
            get
            {
                return new List<string>() { "30 years"};
            }
        }

        

        #endregion

        #region Command Handler

        private void AddAccountCommandHandler()
        {

            var acc = new LoanAccount(AccountName, StartDate,LoanLength * 12, InterestRate,InitialLoanAmount,
            DownPayment);
            acc.UserId = _accountStore.CurrentAccount.Id;

            _accountStore.CurrentAccount.Accounts.Add(acc);

            AccountAdded?.Invoke(this, acc);
        }

        #endregion

    }
}

