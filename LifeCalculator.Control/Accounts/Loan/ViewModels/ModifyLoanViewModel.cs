using LifeCalculator.Control.Accounts;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LifeCalculator.Control.ViewModels
{
    public class ModifyLoanViewModel : ViewModelBase, IModifyAccount
    {
        #region Fields

        private LoanAccount _account;
        private AccountManager _accountManager;
        public event EventHandler ValueChanged;

        #endregion

        #region Constructors

        public ModifyLoanViewModel()
        {
            DeleteAccountCommand = new DelegateCommand(DeleteAccount);
        }

        public ModifyLoanViewModel(LoanAccount account, AccountManager accountManager)
        {
            _account = account;
            _accountManager = accountManager;

            AccountLifeEventsVMs = new BindingList<ModifyEventLoanViewModel>();
            foreach (var item in _account.AccountLifeEvents)
            {
                if (item is AccountEvent accEvent)
                {
                    var accVM = new ModifyEventLoanViewModel(accEvent);
                    AccountLifeEventsVMs.Add(accVM);
                }
            }

            _account.ValueChanged += ValueChangedHandler;
            DeleteAccountCommand = new DelegateCommand(DeleteAccount);

        }

        #endregion

        #region Properties

        public int Id => _account.Id;

        public DelegateCommand DeleteAccountCommand { get; set; }

        public int UserId
        {
            get
            {
                return _account.UserId;
            }
            set
            {
                _account.UserId = value;
                OnPropertyChanged("UserId");
            }
        }

        public string Name
        {
            get
            {
                return _account.Name;
            }
            set
            {
                _account.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public double MonthlyPayment
        {
            get
            {
                return _account.MonthlyPayment;
            }
        }

        public double LoanAmount
        {
            get
            {
                return _account.LoanAmount;
            }
            set
            {
                _account.LoanAmount = value;
                OnPropertyChanged("LoanAmount");
            }
        }

        public double DownPayment
        {
            get
            {
                return _account.DownPayment;
            }
            set
            {
                _account.DownPayment = value;
                OnPropertyChanged("DownPayment");
            }
        }

        public double InterestRate
        {
            get
            {
                return _account.InterestRate;
            }
            set
            {
                _account.InterestRate = value;
                OnPropertyChanged("InterestRate");
            }
        }

        public double InterestPaid
        {
            get
            {
                return _account.InterestPaid;
            }
        }

        public double PrincipalPaid
        {
            get
            {
                return _account.PrincipalPaid;
            }
        }

        public int LoanLengthMonths
        {
            get
            {
                return _account.LoanLengthMonths;
            }
            set
            {
                _account.LoanLengthMonths = value;
                OnPropertyChanged("LoanLengthMonths");
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _account.StartDate;
            }
            set
            {
                _account.StartDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        public BindingList<ModifyEventLoanViewModel> AccountLifeEventsVMs { get; set; }

        public LoanAccount Account => _account;

        public List<IAccountEvent> AccountLifeEvents
        {
            get
            {
                return _account.AccountLifeEvents;
            }
            set
            {
                _account.AccountLifeEvents = value;
                OnPropertyChanged("AccountLifeEvents");
            }
        }

        #endregion

        #region Commands

        public void DeleteAccount()
        {
            _accountManager.DeleteAccount(_account);
        }

        #endregion

        #region Event Handlers

        private void ValueChangedHandler(object sender, IAccount e)
        {
            if (AccountLifeEventsVMs.Count != Account.AccountLifeEvents.Count)
            {
                AccountLifeEventsVMs.Clear();
                foreach (var item in _account.AccountLifeEvents)
                {
                    if (item is AccountEvent accEvent)
                    {
                        var modifyLoanVM = new ModifyEventLoanViewModel(accEvent);
                        AccountLifeEventsVMs.Add(modifyLoanVM);
                    }
                }
            }

            OnPropertyChanged(String.Empty);
        }

        #endregion

    }
}
