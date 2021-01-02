using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.AccountManager;
using LifeCalculator.Tools.Common.Converters;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Control.ViewModels
{
    public class AddLoanViewModel : BindableBase, INavigationAware
    {

        private IAccountManager _accountManager;

        #region Constructor

        public AddLoanViewModel()
        {
            AddAccountCommand = new DelegateCommand(AddAccountCommandHandler);
        }

        public AddLoanViewModel(IAccountManager accountManager)
        {
            AddAccountCommand = new DelegateCommand(AddAccountCommandHandler);
            _accountManager = accountManager;
        }

        #endregion


        #region Properties

        public DelegateCommand AddAccountCommand { get; set; }

        public string AccountName { get; set; }
        public DateTime StartDate { get; set; }
        public string InitialLoanAmount { get; set; }
        public string InterestRate { get; set; }
        public string LoanLength { get; set; }
        public string DownPayment { get; set; }

        public List<string> LoanLengths
        {
            get
            {
                return new List<string>() { "30 years", "15 years" };
            }
        }

        #endregion

        #region Event Handler

        private void AddAccountCommandHandler()
        {

            var acc = new LoanAccount(StartDate, ConvertString.ToDouble(InterestRate), Convert.ToDouble(InitialLoanAmount),
            ConvertString.ToDouble(DownPayment))
            {
                LoanAmount = ConvertString.ToDouble(InitialLoanAmount),
                Name = AccountName
            };

            _accountManager.AddAccount(acc);
        }

        #endregion


        #region Navigation Events

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _accountManager = navigationContext.Parameters["accountManager"] as IAccountManager;
        }

        #endregion

    }
}

