using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.SimulatedAccount
{
    public class CompoundAccount : ISimulatedAccount
    {

        #region Events

        public event EventHandler<IAccountEvent> LifeEventAdded;
        public event EventHandler<IAccount> ValueChanged;

        #endregion

        #region Fields

        IAccountsEventsManager _accountEventsManager;

        #endregion

        #region Constructors

        public CompoundAccount()
        {

        }

        public CompoundAccount(IAccountsEventsManager accountEventManager)
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
                if(_id == -1)
                {
                    _id = value;
                }
            }
        }

        private int _userId;
        public int UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
                ValueChanged?.Invoke(this, this);
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                ValueChanged?.Invoke(this, this);
            }
        }
        private double _initialAmount;
        public double InitialAmount
        {
            get
            {
                return _initialAmount;
            }
            set
            {
                _initialAmount = value;
                ValueChanged?.Invoke(this, this);
            }
        }
        private double _interestRate;
        public double InterestRate
        {
            get
            {
                return _interestRate;
            }
            set
            {
                _interestRate = value;
                ValueChanged?.Invoke(this, this);
            }
        }
        private DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                ValueChanged?.Invoke(this, this);
            }
        }
        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _finalAmount;
        public double FinalAmount
        {
            get
            {
                return _finalAmount;
            }
            set
            {
                _finalAmount = value;
                ValueChanged?.Invoke(this, this);
            }
        }


        [IgnoreDatabase]
        public List<IAccountEvent> AccountLifeEvents => _accountEventsManager.GetAllAccountEventsByAccountId(Id,AccountTypes.CompoundInterest);
        
        #endregion

        #region Methods

        public void SetupBasicCalculation(DateTime startDate, DateTime endDate, double interestRate,
            double initialAmount, double additionalAmount)
        {

            _initialAmount = initialAmount;
            _interestRate = interestRate;
            _startDate = startDate;
            _endDate = endDate;

            var newEvent = new AccountEvent() { Name = "Additional Monthly Contribute", StartDate = startDate, EndDate = endDate, Amount = additionalAmount,LifeEventType = LifeEnum.MonthlyContribute };

            AddLifeEvent(newEvent);

            Calculation();
        }

        public List<MonthlyColumn> Calculation()
        {
            double currValue = InitialAmount;
            double monthlyContribute = 0;
            List<MonthlyColumn> monthlies = new List<MonthlyColumn>();
            int monthDiff = 0;
            _finalAmount = 0;

            AccountLifeEvents.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            monthlies.Add(new MonthlyColumn());

            if(AccountLifeEvents.Count > 0)
                monthlyContribute = AccountLifeEvents[0].Amount;
            else
                monthlyContribute = 0;
                monthDiff = Math.Abs((_startDate.Year * 12 + (_startDate.Month - 1))
                - (_endDate.Year * 12 + (_endDate.Month - 1)));
            
            for (int j = 0; j < monthDiff; j++)
            {

                currValue = (currValue + additionalPriPaymentCalculation(_startDate.AddMonths(j))) * (1 + (InterestRate / 100) / 12);
                monthlies.Add(new MonthlyColumn() { Name = Name, Gain = Math.Round(currValue,2), Date = _startDate.AddMonths(j) });
            }

            if (monthDiff != 0)
                _finalAmount = monthlies[monthlies.Count-1].Gain;

            return monthlies;
        }

        private double additionalPriPaymentCalculation(DateTime dateTime)
        {
            double additonalAmount = 0;

            AccountLifeEvents.FindAll(i => i.StartDate <= dateTime && dateTime <= i.EndDate && i.LifeEventType == LifeEnum.MonthlyContribute)
                .ForEach(i => additonalAmount += i.Amount);

            AccountLifeEvents.FindAll(i => i.StartDate.Year == dateTime.Year && dateTime.Month == i.StartDate.Month && i.LifeEventType == LifeEnum.OneTime)
                .ForEach(i => additonalAmount += i.Amount);

            return additonalAmount;
        }

        public void SetEventsManager(IAccountsEventsManager accountsEventsManager)
        {
            _accountEventsManager = accountsEventsManager;
        }

        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
            lifeEvent.AccountId = Id;
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
            var other = obj as CompoundAccount;

            return obj == null ? false :
                Id.Equals(other.Id) &&
                InitialAmount.Equals(other.InitialAmount) &&
                InterestRate.Equals(other.InterestRate) &&
                Name.Equals(other.Name) &&
                StartDate.Equals(other.StartDate) &&
                EndDate.Equals(other.EndDate) &&
                UserId.Equals(other.UserId) &&
                FinalAmount.Equals(other.FinalAmount);
        }

        #endregion

    }
}
