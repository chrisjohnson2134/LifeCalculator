using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.SimulatedAccount
{
    /// <summary>
    /// Cash savings held against an emergency, kept separate from investments and retirement
    /// because it answers a different question. Investments ask "how much will this be worth in
    /// thirty years"; an emergency fund asks "when do I have enough, and how long would it last".
    ///
    /// It still grows by monthly compounding like <see cref="CompoundAccount"/> — savings accounts
    /// pay interest — but adds a target and a first-class monthly contribution. The contribution
    /// is a plain field rather than a life event: putting a fixed amount aside every month is the
    /// entire point of the account, not an exception to model, and making people add an "event"
    /// to express it would be backwards. Events still layer on top for one-off deposits.
    /// </summary>
    public class EmergencyFundAccount : ISimulatedAccount
    {
        #region Events

        public event EventHandler<IAccountEvent> LifeEventAdded;
        public event EventHandler<IAccount> ValueChanged;

        #endregion

        #region Fields

        /// <summary>
        /// Projections run this far out when a goal is unreachable within a sane horizon —
        /// same 50-year ceiling the debt payoff simulator uses, so an unreachable goal reports
        /// "not within 50 years" instead of looping forever.
        /// </summary>
        public const int MaxProjectionMonths = 600;

        private IAccountsEventsManager _accountEventsManager;

        #endregion

        #region Constructors

        public EmergencyFundAccount()
        {
        }

        public EmergencyFundAccount(IAccountsEventsManager accountEventManager)
        {
            _accountEventsManager = accountEventManager;
        }

        #endregion

        #region Properties

        private int _id = -1;
        public int Id
        {
            get => _id;
            set
            {
                if (_id == -1)
                {
                    _id = value;
                }
            }
        }

        private int _userId;
        public int UserId
        {
            get => _userId;
            set { _userId = value; ValueChanged?.Invoke(this, this); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>What's already saved.</summary>
        private double _initialAmount;
        public double InitialAmount
        {
            get => _initialAmount;
            set { _initialAmount = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>Stored as a fraction (0.04 = 4% APY), matching every other account.</summary>
        private double _interestRate;
        public double InterestRate
        {
            get => _interestRate;
            set { _interestRate = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>The target balance. Zero means no goal set yet.</summary>
        private double _goalAmount;
        public double GoalAmount
        {
            get => _goalAmount;
            set { _goalAmount = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>Amount set aside each month.</summary>
        private double _monthlyContribution;
        public double MonthlyContribution
        {
            get => _monthlyContribution;
            set { _monthlyContribution = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>
        /// Remembers that the goal was set from a months-of-expenses preset, so the UI can show
        /// "6 months of expenses" rather than a bare figure, and offer to refresh it when the
        /// budget changes. Zero means the goal was typed in directly.
        /// </summary>
        private int _goalMonthsOfExpenses;
        public int GoalMonthsOfExpenses
        {
            get => _goalMonthsOfExpenses;
            set { _goalMonthsOfExpenses = value; ValueChanged?.Invoke(this, this); }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; ValueChanged?.Invoke(this, this); }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; ValueChanged?.Invoke(this, this); }
        }

        private double _finalAmount;
        public double FinalAmount
        {
            get => _finalAmount;
            set { _finalAmount = value; ValueChanged?.Invoke(this, this); }
        }

        [IgnoreDatabase]
        public List<IAccountEvent> AccountLifeEvents =>
            _accountEventsManager?.GetAllAccountEventsByAccountId(Id, AccountTypes.EmergencyFund) ?? new List<IAccountEvent>();

        /// <summary>Progress toward the goal, clamped to 0–1 so a funded-past-goal account doesn't overflow a progress bar.</summary>
        [IgnoreDatabase]
        public double ProgressFraction =>
            GoalAmount <= 0 ? 0 : Math.Min(1, Math.Max(0, InitialAmount / GoalAmount));

        [IgnoreDatabase]
        public bool IsGoalMet => GoalAmount > 0 && InitialAmount >= GoalAmount;

        [IgnoreDatabase]
        public double RemainingToGoal => Math.Max(0, GoalAmount - InitialAmount);

        #endregion

        #region Methods

        /// <summary>
        /// Month-by-month balance, contributions plus interest. Mirrors CompoundAccount's shape
        /// (leading placeholder column included) so this account drops into the same charting and
        /// net-worth aggregation as every other asset.
        /// </summary>
        public List<MonthlyColumn> Calculation()
        {
            double currValue = InitialAmount;
            var monthlies = new List<MonthlyColumn>();
            _finalAmount = 0;

            var events = AccountLifeEvents;
            events.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            monthlies.Add(new MonthlyColumn());

            // Projected over a fixed long horizon rather than to EndDate. Unlike a loan or a
            // fixed-term investment, an emergency fund has no natural end — you keep it for as
            // long as you might need it. Honouring a stored end date made the balance simply
            // stop partway along the chart, which reads as the money vanishing rather than as
            // the projection running out.
            int monthDiff = MaxProjectionMonths;

            for (int j = 0; j < monthDiff; j++)
            {
                DateTime month = _startDate.AddMonths(j);

                // Contributions stop once the goal is met. An emergency fund is a target to
                // reach, not a pot to feed forever — past the goal the money goes somewhere
                // else, so projecting it as if you kept paying in would overstate both this
                // balance and your net worth. Interest keeps compounding on what's there.
                double contribution = IsFundedAt(currValue)
                    ? 0
                    : MonthlyContribution + AccountEventResolver.ResolveAdditionalAmount(events, month);

                currValue = (currValue + contribution) * (1 + InterestRate / 12);

                monthlies.Add(new MonthlyColumn
                {
                    Name = Name,
                    Gain = Math.Round(currValue, 2),
                    Date = month
                });
            }

            if (monthDiff != 0)
                _finalAmount = monthlies[monthlies.Count - 1].Gain;

            return monthlies;
        }

        /// <summary>
        /// The month the balance first reaches the goal, or null if it never does within
        /// <see cref="MaxProjectionMonths"/>. Null covers the two cases worth telling the user
        /// about: no goal set, and a contribution too small to ever get there.
        ///
        /// Walks its own loop rather than reading <see cref="Calculation"/> so the answer doesn't
        /// depend on where the user happened to set the projection end date.
        /// </summary>
        public DateTime? ProjectedGoalDate()
        {
            if (GoalAmount <= 0)
                return null;

            if (InitialAmount >= GoalAmount)
                return StartDate;

            // Interest alone can carry a fund over the line, so a zero contribution isn't
            // automatically unreachable — but a zero contribution AND zero rate is.
            if (MonthlyContribution <= 0 && InterestRate <= 0)
                return null;

            double balance = InitialAmount;
            var events = AccountLifeEvents;

            for (int month = 0; month < MaxProjectionMonths; month++)
            {
                DateTime date = StartDate.AddMonths(month);

                double contribution = MonthlyContribution
                    + AccountEventResolver.ResolveAdditionalAmount(events, date);

                balance = (balance + contribution) * (1 + InterestRate / 12);

                if (balance >= GoalAmount)
                    return date;
            }

            return null;
        }

        /// <summary>
        /// Whether a balance has reached the target. No goal set means never funded, so an
        /// unbounded fund keeps taking contributions rather than silently stopping at zero.
        /// </summary>
        private bool IsFundedAt(double balance) => GoalAmount > 0 && balance >= GoalAmount;

        /// <summary>
        /// Whether money is still going in as of the given month. Used by the cash-flow
        /// projection so a fully funded emergency fund stops eating into monthly surplus.
        /// </summary>
        public bool IsContributingOn(DateTime date) => IsContributingOn(date, ProjectedGoalDate());

        /// <summary>
        /// Overload taking an already-computed goal date. <see cref="ProjectedGoalDate"/> walks up
        /// to 600 months, so a caller simulating many months should resolve it once rather than
        /// per month, which would make a long projection quadratic.
        /// </summary>
        public bool IsContributingOn(DateTime date, DateTime? projectedGoalDate)
        {
            if (MonthlyContribution <= 0)
                return false;

            if (GoalAmount <= 0)
                return true;

            // Already funded before a single deposit, so nothing is going in. Checked separately
            // because ProjectedGoalDate reports the start date in that case, which the
            // comparison below would otherwise read as "still contributing this month".
            if (IsFundedAt(InitialAmount))
                return false;

            // Never reaches the goal, so contributions continue indefinitely.
            if (projectedGoalDate == null)
                return true;

            // Inclusive: the goal month is the one whose deposit tips the balance over.
            return MonthOf(date) <= MonthOf(projectedGoalDate.Value);
        }

        private static DateTime MonthOf(DateTime date) => new DateTime(date.Year, date.Month, 1);

        /// <summary>
        /// How many months of spending the current balance covers. This is the number that makes
        /// an emergency fund meaningful — "$12,000" means nothing until you know it's four months.
        /// </summary>
        public double MonthsOfExpensesCovered(double monthlyExpenses)
        {
            if (monthlyExpenses <= 0)
                return 0;

            return InitialAmount / monthlyExpenses;
        }

        /// <summary>Sets the goal to N months of the given monthly spend, remembering the preset used.</summary>
        public void SetGoalFromMonthsOfExpenses(int months, double monthlyExpenses)
        {
            GoalMonthsOfExpenses = months;
            GoalAmount = Math.Round(months * Math.Max(0, monthlyExpenses), 2);
        }

        public void SetEventsManager(IAccountsEventsManager accountsEventsManager)
        {
            _accountEventsManager = accountsEventsManager;
        }

        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
            lifeEvent.AccountId = Id;
            lifeEvent.AccountType = AccountTypes.EmergencyFund;
            lifeEvent.ValueChanged += LifeEvent_ValueChanged;
            _accountEventsManager.AddAccountEvent(lifeEvent);
            LifeEventAdded?.Invoke(this, lifeEvent);
            ValueChanged?.Invoke(this, this);
        }

        private void LifeEvent_ValueChanged(object sender, IAccountEvent e)
        {
            ValueChanged?.Invoke(this, this);
        }

        #endregion

        #region OverridenMethods

        public override bool Equals(object obj)
        {
            var other = obj as EmergencyFundAccount;

            return other == null ? false :
                Id.Equals(other.Id) &&
                InitialAmount.Equals(other.InitialAmount) &&
                InterestRate.Equals(other.InterestRate) &&
                GoalAmount.Equals(other.GoalAmount) &&
                MonthlyContribution.Equals(other.MonthlyContribution) &&
                Name.Equals(other.Name) &&
                StartDate.Equals(other.StartDate) &&
                EndDate.Equals(other.EndDate) &&
                UserId.Equals(other.UserId);
        }

        public override int GetHashCode() => Id.GetHashCode();

        #endregion
    }
}
