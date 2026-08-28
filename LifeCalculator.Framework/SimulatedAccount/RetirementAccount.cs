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
    /// A tax-advantaged retirement account (401k, Roth/Traditional IRA). Grows the same way as
    /// <see cref="CompoundAccount"/> (monthly compounding + life-event contributions), plus an
    /// optional employer match capped at a percentage of salary. No tax/withdrawal-phase modeling.
    /// </summary>
    public class RetirementAccount : ISimulatedAccount
    {
        #region Events

        public event EventHandler<IAccountEvent> LifeEventAdded;
        public event EventHandler<IAccount> ValueChanged;

        #endregion

        #region Fields

        IAccountsEventsManager _accountEventsManager;

        #endregion

        #region Constructors

        public RetirementAccount()
        {
        }

        public RetirementAccount(IAccountsEventsManager accountEventManager)
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
            set
            {
                _userId = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _initialAmount;
        public double InitialAmount
        {
            get => _initialAmount;
            set
            {
                _initialAmount = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        /// <summary>Stored as a fraction (0.05 = 5%), matching CompoundAccount/LoanAccount.</summary>
        private double _interestRate;
        public double InterestRate
        {
            get => _interestRate;
            set
            {
                _interestRate = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _finalAmount;
        public double FinalAmount
        {
            get => _finalAmount;
            set
            {
                _finalAmount = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private RetirementAccountType _accountKind;
        public RetirementAccountType AccountKind
        {
            get => _accountKind;
            set
            {
                _accountKind = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        /// <summary>Fraction of employee contribution the employer matches (0.5 = 50%).</summary>
        private double _employerMatchPercent;
        public double EmployerMatchPercent
        {
            get => _employerMatchPercent;
            set
            {
                _employerMatchPercent = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        /// <summary>
        /// The income stream this account's employer match is based on (-1 when unlinked).
        /// A 401(k) belongs to a specific job, so the match cap is computed against that job's
        /// salary rather than a separately-entered figure.
        /// </summary>
        private int _linkedIncomeStreamId = -1;
        public int LinkedIncomeStreamId
        {
            get => _linkedIncomeStreamId;
            set
            {
                _linkedIncomeStreamId = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        /// <summary>Cap on the match, as a fraction of monthly salary (0.06 = 6% of salary).</summary>
        private double _employerMatchCapPercentOfSalary;
        public double EmployerMatchCapPercentOfSalary
        {
            get => _employerMatchCapPercentOfSalary;
            set
            {
                _employerMatchCapPercentOfSalary = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        [IgnoreDatabase]
        public List<IAccountEvent> AccountLifeEvents => _accountEventsManager.GetAllAccountEventsByAccountId(Id, AccountTypes.RetirementAccount);

        #endregion

        #region Methods

        public void SetupBasicCalculation(DateTime startDate, DateTime endDate, double interestRate,
            double initialAmount, double additionalAmount)
        {
            _initialAmount = initialAmount;
            _interestRate = interestRate / 100;
            _startDate = startDate;
            _endDate = endDate;

            var newEvent = new AccountEvent()
            {
                Name = "Additional Monthly Contribute",
                StartDate = startDate,
                EndDate = endDate,
                Amount = additionalAmount,
                LifeEventType = LifeEnum.MonthlyContribute
            };

            AddLifeEvent(newEvent);

            Calculation();
        }

        /// <summary>Calculation with no employer match applied (ISimulatedAccount contract).</summary>
        public List<MonthlyColumn> Calculation()
        {
            return Calculation(0);
        }

        /// <summary>
        /// Projects month-by-month growth. <paramref name="monthlySalaryReference"/> is the salary
        /// basis the employer match cap is computed against (0 = no employer match applied); the
        /// caller (the overall Life Calculator projection) supplies it from FinancialAccount.Salary.
        /// </summary>
        public List<MonthlyColumn> Calculation(double monthlySalaryReference)
        {
            double currValue = InitialAmount;
            List<MonthlyColumn> monthlies = new List<MonthlyColumn>();
            int monthDiff;
            _finalAmount = 0;

            AccountLifeEvents.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            monthlies.Add(new MonthlyColumn());

            monthDiff = Math.Abs((_startDate.Year * 12 + (_startDate.Month - 1))
                - (_endDate.Year * 12 + (_endDate.Month - 1)));

            for (int j = 0; j < monthDiff; j++)
            {
                double employeeContribution = AccountEventResolver.ResolveAdditionalAmount(AccountLifeEvents, _startDate.AddMonths(j));
                double employerMatch = CalculateEmployerMatch(employeeContribution, monthlySalaryReference);

                currValue = (currValue + employeeContribution + employerMatch) * (1 + InterestRate / 12);
                monthlies.Add(new MonthlyColumn() { Name = Name, Gain = Math.Round(currValue, 2), Date = _startDate.AddMonths(j) });
            }

            if (monthDiff != 0)
                _finalAmount = monthlies[monthlies.Count - 1].Gain;

            return monthlies;
        }

        private double CalculateEmployerMatch(double employeeContribution, double monthlySalaryReference)
        {
            if (EmployerMatchPercent <= 0 || monthlySalaryReference <= 0)
                return 0;

            double matchableContribution = Math.Min(employeeContribution, monthlySalaryReference * EmployerMatchCapPercentOfSalary);

            return matchableContribution > 0 ? EmployerMatchPercent * matchableContribution : 0;
        }

        public void SetEventsManager(IAccountsEventsManager accountsEventsManager)
        {
            _accountEventsManager = accountsEventsManager;
        }

        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
            lifeEvent.AccountId = Id;
            lifeEvent.AccountType = AccountTypes.RetirementAccount;
            lifeEvent.ValueChanged += LifeEvent_ValueChanged;
            _accountEventsManager.AddAccountEvent(lifeEvent);
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
            var other = obj as RetirementAccount;

            return other == null ? false :
                Id.Equals(other.Id) &&
                InitialAmount.Equals(other.InitialAmount) &&
                InterestRate.Equals(other.InterestRate) &&
                Name.Equals(other.Name) &&
                StartDate.Equals(other.StartDate) &&
                EndDate.Equals(other.EndDate) &&
                UserId.Equals(other.UserId) &&
                FinalAmount.Equals(other.FinalAmount) &&
                AccountKind.Equals(other.AccountKind);
        }

        #endregion
    }
}
