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
    public class ModifyCompoundViewModel : ViewModelBase, IModifyAccount
    {
        #region Fields

        public event EventHandler<IAccountEvent> LifeEventAdded;

        private CompoundAccount _account;
        private AccountManager _accountManager;

        #endregion

        #region Constructors

        public ModifyCompoundViewModel()
        {

        }

        public ModifyCompoundViewModel(CompoundAccount account, AccountManager accountManager)
        {
            _account = account;
            _accountManager = accountManager;
            _account.ValueChanged += Account_ValueChanged;
            AccountLifeEventsVMs = new BindingList<ModifyEventCompoundViewModel>();
            DeleteAccountCommand = new DelegateCommand(DeleteAccount);

            foreach (var item in _account.AccountLifeEvents)
            {
                if (item is AccountEvent accEvent)
                {
                    var accVM = new ModifyEventCompoundViewModel(accEvent);
                    accVM.ValueChanged += EventValueChangedHandler;
                    AccountLifeEventsVMs.Add(accVM);
                }
            }
        }

        #endregion

        #region Properties

        public int Id => _account.Id;

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

        public double InitialAmount
        {
            get
            {
                return _account.InitialAmount;
            }
            set
            {
                _account.InitialAmount = value;
                OnPropertyChanged("InitialAmount");
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

        public DateTime EndDate
        {
            get
            {
                return _account.EndDate;
            }
            set
            {
                _account.EndDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        public List<IAccountEvent> AccountLifeEvents { get; set; }
        public BindingList<ModifyEventCompoundViewModel> AccountLifeEventsVMs { get; set; }
        public CompoundAccount Account => _account;
        public DelegateCommand DeleteAccountCommand { get; set; }


        public List<MonthlyColumn> Calculation()
        {
            return _account.Calculation();
        }

        #endregion

        #region Commands

        public void DeleteAccount()
        {
            _accountManager.DeleteAccount(_account);
        }

        #endregion

        #region Event Handler

        private void Account_ValueChanged(object sender, IAccount e)
        {
            AccountLifeEventsVMs.Clear();
            foreach (var item in _account.AccountLifeEvents)
            {
                if (item is AccountEvent accEvent)
                {
                    var compoundModifyVM = new ModifyEventCompoundViewModel(accEvent);
                    compoundModifyVM.ValueChanged += EventValueChangedHandler;
                    AccountLifeEventsVMs.Add(compoundModifyVM);
                }

            }
        }

        private void EventValueChangedHandler(object sender, EventArgs e)
        {
            if(AccountLifeEventsVMs.Count != Account.AccountLifeEvents.Count)
            {
                AccountLifeEventsVMs.Clear();
                foreach (var item in _account.AccountLifeEvents)
                {
                    if (item is AccountEvent accEvent)
                    {
                        var compoundModifyVM = new ModifyEventCompoundViewModel(accEvent);
                        compoundModifyVM.ValueChanged += EventValueChangedHandler;
                        AccountLifeEventsVMs.Add(compoundModifyVM);
                    }

                }

            }

            OnPropertyChanged(string.Empty);


        }

        #endregion

    }
}
