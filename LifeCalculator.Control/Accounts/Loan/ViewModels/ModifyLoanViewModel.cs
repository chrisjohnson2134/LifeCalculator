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
using LifeCalculator.Framework.Enums;

namespace LifeCalculator.Control.ViewModels
{
    public class ModifyLoanViewModel : ValidatableViewModelBase, IModifyAccount
    {
        #region Fields

        private LoanAccount _account;
        private AccountManager _accountManager;
        private IAccountsEventsManager _accountsEventsManager;
        public event EventHandler ValueChanged;

        #endregion

        #region Constructors

        public ModifyLoanViewModel()
        {
            DeleteAccountCommand = new RelayCommand(DeleteAccount);
        }

        public ModifyLoanViewModel(LoanAccount account, AccountManager accountManager, IAccountsEventsManager eventsManager = null)
        {
            _account = account;
            _accountManager = accountManager;
            _accountsEventsManager = eventsManager;

            AccountLifeEventsVMs = new BindingList<ModifyEventViewModel>();
            foreach (var item in _account.AccountLifeEvents)
            {
                if (item is AccountEvent accEvent)
                {
                    AccountLifeEventsVMs.Add(CreateEventRow(accEvent));
                }
            }

            _account.ValueChanged += ValueChangedHandler;
            DeleteAccountCommand = new RelayCommand(DeleteAccount);
            ToggleAddEventCommand = new RelayCommand(ToggleAddEvent);

            ValidateAll();
        }

        #endregion

        #region Properties

        public int Id => _account.Id;

        public IRelayCommand DeleteAccountCommand { get; set; }
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

        public double MonthlyPayment
        {
            get
            {
                return _account.MonthlyPayment;
            }
            set
            {
                double principal = _account.LoanAmount - _account.DownPayment;
                double interestOnlyPayment = principal * (_account.InterestRate / 12);

                Validate(nameof(MonthlyPayment), () => value > interestOnlyPayment, "Payment must be more than the interest accruing each month, or the loan will never pay off.");

                if (value > interestOnlyPayment)
                {
                    _account.MonthlyPayment = value;
                    OnPropertyChanged("MonthlyPayment");
                    OnPropertyChanged(nameof(LoanLengthMonths));
                }
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
                Validate(nameof(LoanAmount), () => value > 0, "Loan amount must be greater than 0.");

                if (value > 0)
                {
                    _account.LoanAmount = value;
                    OnPropertyChanged("LoanAmount");
                    OnPropertyChanged(nameof(MonthlyPayment));
                }

                ValidateDownPayment(_account.DownPayment);
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
                ValidateDownPayment(value);

                if (value >= 0 && value <= _account.LoanAmount)
                {
                    _account.DownPayment = value;
                    OnPropertyChanged("DownPayment");
                    OnPropertyChanged(nameof(MonthlyPayment));
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
                    OnPropertyChanged(nameof(MonthlyPayment));
                }
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
                Validate(nameof(LoanLengthMonths), () => value > 0, "Loan length must be greater than 0 months.");

                if (value > 0)
                {
                    _account.LoanLengthMonths = value;
                    OnPropertyChanged("LoanLengthMonths");
                    OnPropertyChanged(nameof(MonthlyPayment));
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
                OnPropertyChanged("StartDate");
            }
        }

        public BindingList<ModifyEventViewModel> AccountLifeEventsVMs { get; set; }

        public LoanAccount Account => _account;

        public List<IAccountEvent> AccountLifeEvents => _accountsEventsManager.GetAllAccountEventsByAccountId(Account.Id,AccountTypes.LoanAccount);

        #endregion

        #region Validation

        private void ValidateAll()
        {
            Validate(nameof(LoanAmount), () => _account.LoanAmount > 0, "Loan amount must be greater than 0.");
            Validate(nameof(InterestRate), () => _account.InterestRate * 100 >= 0 && _account.InterestRate * 100 <= 100, "Interest rate must be between 0 and 100.");
            Validate(nameof(LoanLengthMonths), () => _account.LoanLengthMonths > 0, "Loan length must be greater than 0 months.");
            ValidateDownPayment(_account.DownPayment);
        }

        private void ValidateDownPayment(double downPayment)
        {
            Validate(nameof(DownPayment), () => downPayment >= 0, "Down payment cannot be negative.");
            Validate(nameof(DownPayment), () => downPayment <= _account.LoanAmount, "Down payment cannot exceed the loan amount.");
        }

        #endregion

        #region Commands

        public void DeleteAccount()
        {
            _accountManager.DeleteAccount(_account);
        }

        /// <summary>Builds an event row wired for deletion, so removing one updates this list.</summary>
        private ModifyEventViewModel CreateEventRow(AccountEvent accEvent)
        {
            var vm = new ModifyEventViewModel(accEvent, _accountsEventsManager);
            vm.EventDeleted += (s, e) => AccountLifeEventsVMs.Remove(vm);
            return vm;
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
                        AccountLifeEventsVMs.Add(CreateEventRow(accEvent));
                    }
                }
            }

            OnPropertyChanged(String.Empty);
        }

        #endregion

    }
}
