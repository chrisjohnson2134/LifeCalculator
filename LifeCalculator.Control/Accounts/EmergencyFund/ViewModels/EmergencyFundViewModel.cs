using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.SimulatedAccount;
using System;
using System.Linq;

namespace LifeCalculator.Control.ViewModels
{
    /// <summary>
    /// Drives the Life Calculator's Emergency Fund section.
    ///
    /// Deliberately singular: you have one emergency fund, and the question it answers — "am I
    /// covered yet?" — only has a meaningful answer against one pot of money. Splitting it across
    /// several accounts would make the months-of-expenses figure ambiguous, which is the whole
    /// point of the section.
    ///
    /// Changes save themselves: AccountManager persists on the account's ValueChanged, so setting
    /// a property here is the save.
    /// </summary>
    public class EmergencyFundViewModel : ValidatableViewModelBase
    {
        #region Fields

        private readonly IAccountStore _accountStore;
        private readonly AccountManager _accountManager;
        private readonly IAccountsEventsManager _eventsManager;
        private readonly IExpenseManager _expenseManager;

        private EmergencyFundAccount _fund;

        #endregion

        #region Constructors

        public EmergencyFundViewModel(
            IAccountStore accountStore,
            AccountManager accountManager,
            IAccountsEventsManager eventsManager,
            IExpenseManager expenseManager)
        {
            _accountStore = accountStore;
            _accountManager = accountManager;
            _eventsManager = eventsManager;
            _expenseManager = expenseManager;

            CreateFundCommand = new RelayCommand(CreateFund, () => !HasFund);
            DeleteFundCommand = new RelayCommand(DeleteFund, () => HasFund);
            SetGoalMonthsCommand = new RelayCommand<object>(SetGoalMonths);
            ClearGoalCommand = new RelayCommand(ClearGoal, () => HasFund);

            _fund = accountManager.GetAllAccounts().OfType<EmergencyFundAccount>().FirstOrDefault();

            if (_fund != null)
                _fund.SetEventsManager(_eventsManager);
        }

        #endregion

        #region Properties

        public EmergencyFundAccount Fund => _fund;

        public bool HasFund => _fund != null;

        public string Name
        {
            get => _fund?.Name;
            set
            {
                if (_fund == null) return;

                Validate(nameof(Name), () => !string.IsNullOrWhiteSpace(value), "Name is required.");

                if (!string.IsNullOrWhiteSpace(value))
                {
                    _fund.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>What's saved so far. Named "current balance" in the UI — "initial amount" is
        /// the model's word for it and means nothing to someone tracking a savings account.</summary>
        public double CurrentBalance
        {
            get => _fund?.InitialAmount ?? 0;
            set
            {
                if (_fund == null) return;

                Validate(nameof(CurrentBalance), () => value >= 0, "Balance cannot be negative.");

                if (value >= 0)
                {
                    _fund.InitialAmount = value;
                    RaiseProjectionChanged();
                }
            }
        }

        /// <summary>Percent at the view-model boundary, fraction in the model — same convention
        /// as every other account.</summary>
        public double InterestRate
        {
            get => (_fund?.InterestRate ?? 0) * 100;
            set
            {
                if (_fund == null) return;

                Validate(nameof(InterestRate), () => value >= 0 && value <= 100, "Rate must be between 0 and 100.");

                if (value >= 0 && value <= 100)
                {
                    _fund.InterestRate = value / 100;
                    RaiseProjectionChanged();
                }
            }
        }

        public double MonthlyContribution
        {
            get => _fund?.MonthlyContribution ?? 0;
            set
            {
                if (_fund == null) return;

                Validate(nameof(MonthlyContribution), () => value >= 0, "Contribution cannot be negative.");

                if (value >= 0)
                {
                    _fund.MonthlyContribution = value;
                    RaiseProjectionChanged();
                }
            }
        }

        public double GoalAmount
        {
            get => _fund?.GoalAmount ?? 0;
            set
            {
                if (_fund == null) return;

                Validate(nameof(GoalAmount), () => value >= 0, "Goal cannot be negative.");

                if (value >= 0)
                {
                    // Typing a figure by hand detaches it from the preset, so the label stops
                    // claiming it's still "6 months of expenses" once it no longer is.
                    _fund.GoalMonthsOfExpenses = 0;
                    _fund.GoalAmount = value;
                    RaiseProjectionChanged();
                }
            }
        }

        #endregion

        #region Budget-derived figures

        /// <summary>Monthly spend from the Budget screen — the basis for the preset goals.</summary>
        public double MonthlyExpenses => _expenseManager?.GetTotalMonthlyExpenses() ?? 0;

        public bool HasExpenses => MonthlyExpenses > 0;

        public double ThreeMonthGoal => Math.Round(MonthlyExpenses * 3, 2);
        public double SixMonthGoal => Math.Round(MonthlyExpenses * 6, 2);
        public double TwelveMonthGoal => Math.Round(MonthlyExpenses * 12, 2);

        /// <summary>
        /// How long the current balance would actually last. This is the number that gives the
        /// balance meaning — "$12,000" says nothing until you know it's four months of rent.
        /// </summary>
        public double MonthsCovered => _fund?.MonthsOfExpensesCovered(MonthlyExpenses) ?? 0;

        public string MonthsCoveredText
        {
            get
            {
                if (!HasFund)
                    return string.Empty;

                if (!HasExpenses)
                    return "Add your expenses on the Budget screen to see how long this would last.";

                return $"Covers {MonthsCovered:0.#} months of expenses";
            }
        }

        #endregion

        #region Goal progress

        public double ProgressFraction => _fund?.ProgressFraction ?? 0;

        public double ProgressPercent => ProgressFraction * 100;

        public bool IsGoalMet => _fund?.IsGoalMet ?? false;

        public bool HasGoal => (_fund?.GoalAmount ?? 0) > 0;

        public double RemainingToGoal => _fund?.RemainingToGoal ?? 0;

        /// <summary>Names the preset when one was used, so the goal reads as a decision rather
        /// than an arbitrary number.</summary>
        public string GoalDescription
        {
            get
            {
                if (!HasGoal)
                    return "No goal set yet";

                int months = _fund.GoalMonthsOfExpenses;

                return months > 0
                    ? $"{months} months of expenses"
                    : "Custom goal";
            }
        }

        public DateTime? ProjectedGoalDate => _fund?.ProjectedGoalDate();

        /// <summary>
        /// Answers the section's actual question in a sentence. Each branch is a genuinely
        /// different situation for the user, so none of them collapse into a generic message.
        /// </summary>
        public string ProjectionText
        {
            get
            {
                if (!HasFund)
                    return string.Empty;

                if (!HasGoal)
                    return "Set a goal to see when you'll reach it.";

                if (IsGoalMet)
                    return "Goal reached — you're fully funded.";

                DateTime? date = ProjectedGoalDate;

                if (date == null)
                {
                    return MonthlyContribution <= 0
                        ? "Set a monthly contribution to see when you'll reach your goal."
                        : "At this rate you won't reach the goal within 50 years.";
                }

                int months = MonthsUntil(date.Value);

                return months <= 0
                    ? $"On track to hit {GoalAmount:C0} this month."
                    : $"On track to hit {GoalAmount:C0} in {DescribeDuration(months)} — {date.Value:MMMM yyyy}.";
            }
        }

        private int MonthsUntil(DateTime date)
        {
            DateTime now = DateTime.Now;
            return ((date.Year - now.Year) * 12) + (date.Month - now.Month);
        }

        private static string DescribeDuration(int months)
        {
            if (months < 12)
                return months == 1 ? "1 month" : $"{months} months";

            int years = months / 12;
            int remainder = months % 12;

            string yearPart = years == 1 ? "1 year" : $"{years} years";

            if (remainder == 0)
                return yearPart;

            string monthPart = remainder == 1 ? "1 month" : $"{remainder} months";

            return $"{yearPart} {monthPart}";
        }

        #endregion

        #region Commands

        public IRelayCommand CreateFundCommand { get; }
        public IRelayCommand DeleteFundCommand { get; }
        public IRelayCommand<object> SetGoalMonthsCommand { get; }
        public IRelayCommand ClearGoalCommand { get; }

        private void CreateFund()
        {
            var fund = new EmergencyFundAccount(_eventsManager)
            {
                Name = "Emergency Fund",
                InitialAmount = 0,
                // A high-yield savings account is where an emergency fund belongs, so seed a
                // plausible rate rather than 0% — the user can correct it.
                InterestRate = 0.04,
                MonthlyContribution = 0,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(10),
                UserId = _accountStore.CurrentAccount.Id
            };

            _accountManager.AddAccount(fund);

            _fund = fund;

            // Default to six months: the standard recommendation, and a concrete starting point
            // beats an empty box. Only when we actually know what a month costs.
            if (HasExpenses)
                _fund.SetGoalFromMonthsOfExpenses(6, MonthlyExpenses);

            RaiseAllChanged();
        }

        private void DeleteFund()
        {
            if (_fund == null)
                return;

            _accountManager.DeleteAccount(_fund);
            _fund = null;

            RaiseAllChanged();
        }

        /// <summary>
        /// The 3/6/12-month presets. Takes the month count as a command parameter so one command
        /// serves all three buttons; the parameter arrives as a string from XAML.
        /// </summary>
        private void SetGoalMonths(object parameter)
        {
            if (_fund == null || parameter == null)
                return;

            if (!int.TryParse(parameter.ToString(), out int months) || months <= 0)
                return;

            _fund.SetGoalFromMonthsOfExpenses(months, MonthlyExpenses);

            RaiseProjectionChanged();
        }

        private void ClearGoal()
        {
            if (_fund == null)
                return;

            _fund.GoalMonthsOfExpenses = 0;
            _fund.GoalAmount = 0;

            RaiseProjectionChanged();
        }

        #endregion

        #region Change notification

        /// <summary>
        /// Re-reads the budget-derived figures too: the preset amounts and months-covered move
        /// whenever expenses change on the Budget screen, and this view has no other trigger.
        /// </summary>
        public void RefreshFromBudget()
        {
            OnPropertyChanged(nameof(MonthlyExpenses));
            OnPropertyChanged(nameof(HasExpenses));
            OnPropertyChanged(nameof(ThreeMonthGoal));
            OnPropertyChanged(nameof(SixMonthGoal));
            OnPropertyChanged(nameof(TwelveMonthGoal));
            OnPropertyChanged(nameof(MonthsCovered));
            OnPropertyChanged(nameof(MonthsCoveredText));
        }

        private void RaiseProjectionChanged()
        {
            OnPropertyChanged(nameof(CurrentBalance));
            OnPropertyChanged(nameof(InterestRate));
            OnPropertyChanged(nameof(MonthlyContribution));
            OnPropertyChanged(nameof(GoalAmount));
            OnPropertyChanged(nameof(GoalDescription));
            OnPropertyChanged(nameof(HasGoal));
            OnPropertyChanged(nameof(IsGoalMet));
            OnPropertyChanged(nameof(ProgressFraction));
            OnPropertyChanged(nameof(ProgressPercent));
            OnPropertyChanged(nameof(RemainingToGoal));
            OnPropertyChanged(nameof(ProjectedGoalDate));
            OnPropertyChanged(nameof(ProjectionText));
            OnPropertyChanged(nameof(MonthsCovered));
            OnPropertyChanged(nameof(MonthsCoveredText));

            GoalChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseAllChanged()
        {
            OnPropertyChanged(nameof(HasFund));
            OnPropertyChanged(nameof(Name));
            CreateFundCommand.NotifyCanExecuteChanged();
            DeleteFundCommand.NotifyCanExecuteChanged();
            ClearGoalCommand.NotifyCanExecuteChanged();
            RefreshFromBudget();
            RaiseProjectionChanged();
        }

        /// <summary>Lets the Calculator page rebuild its charts when the fund changes.</summary>
        public event EventHandler GoalChanged;

        #endregion
    }
}
