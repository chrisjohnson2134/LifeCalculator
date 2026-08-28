using LifeCalculator.Control.Accounts;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LifeCalculator.Control.ViewModels
{
    public class ModifyCompoundViewModel : ValidatableViewModelBase, IModifyAccount
    {
        #region Fields

        public event EventHandler<IAccountEvent> LifeEventAdded;

        private CompoundAccount _account;
        private AccountManager _accountManager;
        private IAccountsEventsManager _accountsEventsManager;

        #endregion

        #region Constructors

        public ModifyCompoundViewModel()
        {

        }

        public ModifyCompoundViewModel(CompoundAccount account, AccountManager accountManager, IAccountsEventsManager eventsManager = null)
        {
            _account = account;
            _accountManager = accountManager;
            _accountsEventsManager = eventsManager;
            _account.ValueChanged += Account_ValueChanged;
            AccountLifeEventsVMs = new BindingList<ModifyEventViewModel>();
            DeleteAccountCommand = new RelayCommand(DeleteAccount);
            ToggleAddEventCommand = new RelayCommand(ToggleAddEvent);

            foreach (var item in _account.AccountLifeEvents)
            {
                if (item is AccountEvent accEvent)
                {
                    AccountLifeEventsVMs.Add(CreateEventRow(accEvent));
                }
            }

            ValidateAll();
        }

        /// <summary>Builds an event row wired for deletion, so removing one updates this list.</summary>
        private ModifyEventViewModel CreateEventRow(AccountEvent accEvent)
        {
            var vm = new ModifyEventViewModel(accEvent, _accountsEventsManager);
            vm.ValueChanged += EventValueChangedHandler;
            vm.EventDeleted += (s, e) => AccountLifeEventsVMs.Remove(vm);
            return vm;
        }

        #endregion

        #region Properties

        public int Id => _account.Id;

        public IRelayCommand ToggleAddEventCommand { get; set; }

        private bool _isAddEventOpen;
        public bool IsAddEventOpen
        {
            get => _isAddEventOpen;
            private set { _isAddEventOpen = value; OnPropertyChanged(nameof(IsAddEventOpen)); }
        }

        private AddEventViewModel _addEventViewModel;
        public AddEventViewModel AddEventViewModel
        {
            get => _addEventViewModel;
            private set { _addEventViewModel = value; OnPropertyChanged(nameof(AddEventViewModel)); }
        }

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
                bool isDuplicate = _accountManager != null && _accountManager.GetAllAccounts()
                    .Any(a => a.Id != _account.Id && a.Name != null && a.Name.Equals(value, StringComparison.OrdinalIgnoreCase));

                Validate(nameof(Name), () => !string.IsNullOrWhiteSpace(value), "Account name is required.");
                Validate(nameof(Name), () => !isDuplicate, "An account with this name already exists.");

                if (!string.IsNullOrWhiteSpace(value) && !isDuplicate)
                {
                    _account.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private System.Windows.Media.Brush _seriesColor;
        public System.Windows.Media.Brush SeriesColor
        {
            get => _seriesColor;
            set
            {
                _seriesColor = value;
                OnPropertyChanged(nameof(SeriesColor));
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
                Validate(nameof(InitialAmount), () => value >= 0, "Initial amount cannot be negative.");

                if (value >= 0)
                {
                    _account.InitialAmount = value;
                    OnPropertyChanged("InitialAmount");
                }
            }
        }

        public double InterestRate
        {
            get
            {
                return _account.InterestRate * 100;
            }
            set
            {
                Validate(nameof(InterestRate), () => value >= 0 && value <= 100, "Interest rate must be between 0 and 100.");

                if (value >= 0 && value <= 100)
                {
                    _account.InterestRate = value / 100;
                    OnPropertyChanged("InterestRate");
                }
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
                ValidateDateRange();
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
                ValidateDateRange();
                OnPropertyChanged("EndDate");
            }
        }

        public List<IAccountEvent> AccountLifeEvents { get; set; }
        public BindingList<ModifyEventViewModel> AccountLifeEventsVMs { get; set; }
        public CompoundAccount Account => _account;
        public IRelayCommand DeleteAccountCommand { get; set; }


        public List<MonthlyColumn> Calculation()
        {
            return _account.Calculation();
        }

        #endregion

        #region Validation

        private void ValidateAll()
        {
            Validate(nameof(InitialAmount), () => _account.InitialAmount >= 0, "Initial amount cannot be negative.");
            Validate(nameof(InterestRate), () => _account.InterestRate * 100 >= 0 && _account.InterestRate * 100 <= 100, "Interest rate must be between 0 and 100.");
            ValidateDateRange();
        }

        private void ValidateDateRange()
        {
            Validate(nameof(EndDate), () => _account.EndDate > _account.StartDate, "Stop date must be after the start date.");
        }

        #endregion

        #region Commands

        public void DeleteAccount()
        {
            _accountManager.DeleteAccount(_account);
        }

        private void ToggleAddEvent()
        {
            IsAddEventOpen = !IsAddEventOpen;

            if (!IsAddEventOpen)
                return;

            var vm = new AddEventViewModel(_account);
            vm.EventAdded += (s, e) => IsAddEventOpen = false;
            AddEventViewModel = vm;
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
                    AccountLifeEventsVMs.Add(CreateEventRow(accEvent));
                }

            }
        }

        private void EventValueChangedHandler(object sender, IAccountEvent e)
        {
            if(AccountLifeEventsVMs.Count != Account.AccountLifeEvents.Count)
            {
                AccountLifeEventsVMs.Clear();
                foreach (var item in _account.AccountLifeEvents)
                {
                    if (item is AccountEvent accEvent)
                    {
                        AccountLifeEventsVMs.Add(CreateEventRow(accEvent));
                    }

                }

            }

            OnPropertyChanged(string.Empty);


        }

        #endregion

    }
}
