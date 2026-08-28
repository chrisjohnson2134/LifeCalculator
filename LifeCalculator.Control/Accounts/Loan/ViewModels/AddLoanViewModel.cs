using LifeCalculator.Control.Accounts;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using LifeCalculator.Framework.Managers;

namespace LifeCalculator.Control.ViewModels
{
    public class AddLoanViewModel : ValidatableViewModelBase, IControlAccount
    {

        #region Fields

        private IAccountStore _accountStore;
        public event EventHandler<IAccount> AccountAdded;
        public event EventHandler<IAccount> AccountModified;

        #endregion

        #region Constructor

        public AddLoanViewModel()
        {
            AddAccountCommand = new RelayCommand(AddAccountCommandHandler, () => !HasErrors);
            LinkCommandToValidation(AddAccountCommand);
            StartDate = DateTime.Now;
            ValidateAll();
        }

        public AddLoanViewModel(IAccountStore accountStore)
        {
            AddAccountCommand = new RelayCommand(AddAccountCommandHandler, () => !HasErrors);
            LinkCommandToValidation(AddAccountCommand);
            _accountStore = accountStore;
            StartDate = DateTime.Now;
            ValidateAll();
        }

        #endregion

        #region Properties

        public IRelayCommand AddAccountCommand { get; set; }

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

        public DateTime StartDate { get; set; }

        private double _initialLoanAmount;
        public double InitialLoanAmount
        {
            get => _initialLoanAmount;
            set
            {
                _initialLoanAmount = value;
                Validate(nameof(InitialLoanAmount), () => _initialLoanAmount > 0, "Loan amount must be greater than 0.");
                ValidateDownPayment();
                OnPropertyChanged(nameof(InitialLoanAmount));
            }
        }

        private double _interestRate;
        public double InterestRate
        {
            get => _interestRate;
            set
            {
                _interestRate = value;
                Validate(nameof(InterestRate), () => _interestRate >= 0 && _interestRate <= 100, "Interest rate must be between 0 and 100.");
                OnPropertyChanged(nameof(InterestRate));
            }
        }

        private int _loanLength;
        public int LoanLength
        {
            get => _loanLength;
            set
            {
                _loanLength = value;
                Validate(nameof(LoanLength), () => _loanLength > 0, "Loan length must be greater than 0 years.");
                OnPropertyChanged(nameof(LoanLength));
            }
        }

        private double _downPayment;
        public double DownPayment
        {
            get => _downPayment;
            set
            {
                _downPayment = value;
                ValidateDownPayment();
                OnPropertyChanged(nameof(DownPayment));
            }
        }

        public List<string> LoanLengths
        {
            get
            {
                return new List<string>() { "30 years"};
            }
        }



        #endregion

        #region Validation

        private void ValidateAll()
        {
            Validate(nameof(InitialLoanAmount), () => _initialLoanAmount > 0, "Loan amount must be greater than 0.");
            Validate(nameof(InterestRate), () => _interestRate >= 0 && _interestRate <= 100, "Interest rate must be between 0 and 100.");
            Validate(nameof(LoanLength), () => _loanLength > 0, "Loan length must be greater than 0 years.");
            ValidateAccountName();
            ValidateDownPayment();
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

        private void ValidateDownPayment()
        {
            Validate(nameof(DownPayment), () => _downPayment >= 0, "Down payment cannot be negative.");
            Validate(nameof(DownPayment), () => _downPayment <= _initialLoanAmount, "Down payment cannot exceed the loan amount.");
        }

        #endregion

        #region Command Handler

        private void AddAccountCommandHandler()
        {

            var acc = new LoanAccount(_accountStore.CurrentAccount.AccountsEventsManager,AccountName, StartDate,LoanLength * 12, InterestRate,InitialLoanAmount,
            DownPayment);
            acc.UserId = _accountStore.CurrentAccount.Id;

            _accountStore.CurrentAccount.SimulatedAccountManager.AddAccount(acc);

            AccountAdded?.Invoke(this, acc);
        }

        #endregion

    }
}
