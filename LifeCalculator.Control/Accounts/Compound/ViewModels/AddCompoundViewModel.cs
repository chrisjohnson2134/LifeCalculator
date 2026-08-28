using LifeCalculator.Framework.SimulatedAccount;
using System;
using System.Linq;
using LifeCalculator.Framework.LifeEvents;
using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Control.Accounts;
using LifeCalculator.Framework.Managers;

namespace LifeCalculator.Control.ViewModels
{
    //Compound Interest View Model
    public class AddCompoundViewModel : ValidatableViewModelBase , IControlAccount
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

            AddAccountCommand = new RelayCommand(AddAccountCommandHandler, () => !HasErrors);
            LinkCommandToValidation(AddAccountCommand);

            StartDate = DateTime.Now;
            StopDate = DateTime.Now.AddYears(1);

            ValidateAll();
        }

        #endregion

        #region Properties

        private string _accountName;
        public string AccountName
        {
            get => _accountName;
            set
            {
                _accountName = value;
                ValidateAccountName();
                OnPropertyChanged(nameof(AccountName));
            }
        }

        public string DescriptionText { get; set; }

        private double _initialValue;
        public double InitialValue
        {
            get => _initialValue;
            set
            {
                _initialValue = value;
                Validate(nameof(InitialValue), () => _initialValue >= 0, "Initial amount cannot be negative.");
                OnPropertyChanged(nameof(InitialValue));
            }
        }

        private double _interest;
        public double Interest
        {
            get => _interest;
            set
            {
                _interest = value;
                Validate(nameof(Interest), () => _interest >= 0 && _interest <= 100, "Interest rate must be between 0 and 100.");
                OnPropertyChanged(nameof(Interest));
            }
        }

        public double Contribute { get; set; }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                ValidateDateRange();
                OnPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime _stopDate;
        public DateTime StopDate
        {
            get => _stopDate;
            set
            {
                _stopDate = value;
                ValidateDateRange();
                OnPropertyChanged(nameof(StopDate));
            }
        }

        public IRelayCommand AddAccountCommand { get; set; }


        #endregion

        #region Validation

        private void ValidateAll()
        {
            ValidateAccountName();
            Validate(nameof(InitialValue), () => _initialValue >= 0, "Initial amount cannot be negative.");
            Validate(nameof(Interest), () => _interest >= 0 && _interest <= 100, "Interest rate must be between 0 and 100.");
            ValidateDateRange();
        }

        private void ValidateAccountName()
        {
            Validate(nameof(AccountName), () => !string.IsNullOrWhiteSpace(_accountName), "Account name is required.");

            if (string.IsNullOrWhiteSpace(_accountName) || _accountStore?.CurrentAccount == null)
                return;

            bool isDuplicate = _accountStore.CurrentAccount.SimulatedAccountManager.GetAllAccounts()
                .Any(a => a.Name != null && a.Name.Equals(_accountName, StringComparison.OrdinalIgnoreCase));

            Validate(nameof(AccountName), () => !isDuplicate, "An account with this name already exists.");
        }

        private void ValidateDateRange()
        {
            Validate(nameof(StopDate), () => _stopDate > _startDate, "Stop date must be after the start date.");
        }

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

