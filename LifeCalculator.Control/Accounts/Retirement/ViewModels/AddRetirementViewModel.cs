using LifeCalculator.Framework.SimulatedAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using LifeCalculator.Framework.LifeEvents;
using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Control.Accounts;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Managers;

namespace LifeCalculator.Control.ViewModels
{
    public class AddRetirementViewModel : ValidatableViewModelBase, IControlAccount
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

        public AddRetirementViewModel(IAccountStore accountStore)
        {
            _accountStore = accountStore;
            _accountsEventsManager = accountStore.CurrentAccount.AccountsEventsManager;

            AddAccountCommand = new RelayCommand(AddAccountCommandHandler, () => !HasErrors);
            LinkCommandToValidation(AddAccountCommand);

            StartDate = DateTime.Now;
            StopDate = DateTime.Now.AddYears(30);
            AccountKind = RetirementAccountType.FourOhOneK;

            ValidateAll();
        }

        #endregion

        #region Properties

        public List<RetirementAccountType> AccountKinds { get; } = Enum.GetValues(typeof(RetirementAccountType)).Cast<RetirementAccountType>().ToList();

        /// <summary>The job this 401(k) belongs to — its salary is what the match cap applies to.</summary>
        public List<LifeCalculator.Framework.Income.IncomeStream> IncomeStreams =>
            _accountStore.CurrentAccount.IncomeStreamManager.GetAllIncomeStreams();

        private LifeCalculator.Framework.Income.IncomeStream _linkedIncomeStream;
        public LifeCalculator.Framework.Income.IncomeStream LinkedIncomeStream
        {
            get => _linkedIncomeStream;
            set
            {
                _linkedIncomeStream = value;
                ResyncContributionToSalary();
                OnPropertyChanged(nameof(LinkedIncomeStream));
                OnPropertyChanged(nameof(MonthlySalary));
                OnPropertyChanged(nameof(ContributionEquivalentText));
                OnPropertyChanged(nameof(ContributionBasisHint));
            }
        }

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

        public RetirementAccountType AccountKind { get; set; }

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

        #region Contribution

        /// <summary>
        /// Contributions can be entered either way round because that is how people actually hold
        /// the number: a payroll deferral is set as a percent of pay, while a personal transfer is
        /// a fixed sum. Whichever basis is selected is the figure the user owns; the other is
        /// derived from it, so the pair can never drift apart.
        /// </summary>
        private ContributionBasis _contributionBasis = ContributionBasis.PercentOfSalary;
        public ContributionBasis ContributionBasis
        {
            get => _contributionBasis;
            set
            {
                _contributionBasis = value;
                OnPropertyChanged(nameof(ContributionBasis));
                OnPropertyChanged(nameof(IsPercentBasis));
                OnPropertyChanged(nameof(IsDollarBasis));
                OnPropertyChanged(nameof(ContributionEquivalentText));
            }
        }

        public bool IsPercentBasis => _contributionBasis == ContributionBasis.PercentOfSalary;
        public bool IsDollarBasis => _contributionBasis == ContributionBasis.DollarAmount;

        /// <summary>
        /// Gross monthly pay of the linked job — the basis both conversions run through. Monthly
        /// rather than annual because <see cref="Contribute"/> is a monthly figure, and because
        /// it is what the employer match cap is measured against too.
        /// </summary>
        public double MonthlySalary => (LinkedIncomeStream?.GrossAnnualSalary ?? 0) / 12;

        private double _contributionPercent;
        public double ContributionPercent
        {
            get => _contributionPercent;
            set
            {
                _contributionPercent = value;
                Validate(nameof(ContributionPercent), () => _contributionPercent >= 0 && _contributionPercent <= 100,
                    "Contribution must be between 0 and 100.");

                _contribute = DollarsFromPercent(_contributionPercent);

                OnPropertyChanged(nameof(ContributionPercent));
                OnPropertyChanged(nameof(Contribute));
                OnPropertyChanged(nameof(ContributionEquivalentText));
            }
        }

        /// <summary>The monthly dollar contribution — what the projection is actually built from.</summary>
        private double _contribute;
        public double Contribute
        {
            get => _contribute;
            set
            {
                _contribute = value;
                Validate(nameof(Contribute), () => _contribute >= 0, "Contribution cannot be negative.");

                _contributionPercent = PercentFromDollars(_contribute);

                OnPropertyChanged(nameof(Contribute));
                OnPropertyChanged(nameof(ContributionPercent));
                OnPropertyChanged(nameof(ContributionEquivalentText));
            }
        }

        /// <summary>The greyed-out counterpart shown beside the field being typed into.</summary>
        public string ContributionEquivalentText
        {
            get
            {
                if (MonthlySalary <= 0)
                    return string.Empty;

                return IsPercentBasis
                    ? $"= {_contribute:C2}/mo"
                    : $"= {_contributionPercent:0.##}% of salary";
            }
        }

        public string ContributionBasisHint => MonthlySalary > 0
            ? "Percent is of the linked job's gross pay; dollars are per month."
            : "Pick the income stream below to convert between percent and dollars.";

        private double DollarsFromPercent(double percent) => Math.Round(MonthlySalary * (percent / 100), 2);

        private double PercentFromDollars(double dollars) =>
            MonthlySalary > 0 ? Math.Round(dollars / MonthlySalary * 100, 2) : 0;

        /// <summary>
        /// Re-derives the non-authoritative side against a newly selected job. Which side survives
        /// depends on the basis: a 6% deferral is still 6% at a different salary, whereas a $500
        /// standing transfer is still $500.
        /// </summary>
        private void ResyncContributionToSalary()
        {
            if (IsPercentBasis)
                _contribute = DollarsFromPercent(_contributionPercent);
            else
                _contributionPercent = PercentFromDollars(_contribute);

            // The derived side is assigned to its field directly rather than through its setter,
            // so its rule has to be re-run here: a fixed dollar amount against a smaller salary
            // can push the derived percent past 100.
            Validate(nameof(ContributionPercent), () => _contributionPercent >= 0 && _contributionPercent <= 100,
                "Contribution must be between 0 and 100.");
            Validate(nameof(Contribute), () => _contribute >= 0, "Contribution cannot be negative.");

            OnPropertyChanged(nameof(Contribute));
            OnPropertyChanged(nameof(ContributionPercent));
        }

        #endregion

        private double _employerMatchPercent;
        public double EmployerMatchPercent
        {
            get => _employerMatchPercent;
            set
            {
                _employerMatchPercent = value;
                Validate(nameof(EmployerMatchPercent), () => _employerMatchPercent >= 0 && _employerMatchPercent <= 100, "Employer match must be between 0 and 100.");
                OnPropertyChanged(nameof(EmployerMatchPercent));
            }
        }

        private double _employerMatchCapPercentOfSalary;
        public double EmployerMatchCapPercentOfSalary
        {
            get => _employerMatchCapPercentOfSalary;
            set
            {
                _employerMatchCapPercentOfSalary = value;
                Validate(nameof(EmployerMatchCapPercentOfSalary), () => _employerMatchCapPercentOfSalary >= 0 && _employerMatchCapPercentOfSalary <= 100, "Match cap must be between 0 and 100.");
                OnPropertyChanged(nameof(EmployerMatchCapPercentOfSalary));
            }
        }

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
            Validate(nameof(ContributionPercent), () => _contributionPercent >= 0 && _contributionPercent <= 100, "Contribution must be between 0 and 100.");
            Validate(nameof(Contribute), () => _contribute >= 0, "Contribution cannot be negative.");
            Validate(nameof(EmployerMatchPercent), () => _employerMatchPercent >= 0 && _employerMatchPercent <= 100, "Employer match must be between 0 and 100.");
            Validate(nameof(EmployerMatchCapPercentOfSalary), () => _employerMatchCapPercentOfSalary >= 0 && _employerMatchCapPercentOfSalary <= 100, "Match cap must be between 0 and 100.");
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

        private void AddAccountCommandHandler()
        {
            var retirementAccount = new RetirementAccount(_accountsEventsManager)
            {
                Name = AccountName,
                InitialAmount = InitialValue,
                UserId = _accountStore.CurrentAccount.Id,
                AccountKind = AccountKind,
                EmployerMatchPercent = EmployerMatchPercent / 100,
                EmployerMatchCapPercentOfSalary = EmployerMatchCapPercentOfSalary / 100,
                LinkedIncomeStreamId = LinkedIncomeStream?.Id ?? -1
            };

            _accountStore.CurrentAccount.SimulatedAccountManager.AddAccount(retirementAccount);

            retirementAccount.SetupBasicCalculation(StartDate, StopDate, Interest, InitialValue, Contribute);

            AccountAdded?.Invoke(this, retirementAccount);
        }

        #endregion
    }
}
