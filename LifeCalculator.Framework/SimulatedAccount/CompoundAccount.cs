using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Database;
using LifeCalculator.Framework.Database.Queries;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LifeCalculator.Framework.SimulatedAccount
{
    public class CompoundAccount : ISimulatedAccount
    {

        #region Events

        public event EventHandler<IAccountEvent> LifeEventAdded;
        public event EventHandler<ISimulatedAccount> ValueChanged;

        #endregion

        #region Constructors

        public CompoundAccount()
        {
            AccountLifeEvents = new List<IAccountEvent>();
        }

        public CompoundAccount(string AccountName)
        {
            Name = AccountName;
            AccountLifeEvents = new List<IAccountEvent>();
        }

        public CompoundAccount(CompoundAccount compoundAccount)
        {
            Name = compoundAccount.Name;
            AccountLifeEvents = new List<IAccountEvent>();
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
        public List<IAccountEvent> AccountLifeEvents { get; set; }
        
        #endregion

        #region Methods

        public void SetupBasicCalculation(DateTime startDate, DateTime endDate, double interestRate,
            double initialAmount, double additionalAmount)
        {

            _initialAmount = initialAmount;
            _interestRate = interestRate;
            _startDate = startDate;
            _endDate = endDate;

            var newEvent = new AccountEvent() { Name = "Additional Monthly Contribute", StartDate = startDate, EndDate = endDate, Amount = additionalAmount };

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

            monthlyContribute = AccountLifeEvents[0].Amount;
                monthDiff = Math.Abs((_startDate.Year * 12 + (_startDate.Month - 1))
                - (_endDate.Year * 12 + (_endDate.Month - 1)));
            
            for (int j = 0; j < monthDiff; j++)
                {
                currValue = (currValue + monthlyContribute) * (1 + (InterestRate / 100) / 12) + additionalPriPaymentCalculation(_startDate.AddMonths(j));
                monthlies.Add(new MonthlyColumn() { Name = Name, Gain = currValue, Date = _startDate.AddMonths(j) });
            }

            if (monthDiff != 0)
                _finalAmount = monthlies[monthlies.Count - 1].Gain;

            return monthlies;
        }

        private double additionalPriPaymentCalculation(DateTime dateTime)
        {
            double additonalAmount = 0;

            AccountLifeEvents.FindAll(i => i.StartDate < dateTime && dateTime < i.EndDate && i.LifeEventType == LifeEnum.MonthlyContribute)
                .ForEach(i => additonalAmount += i.Amount);

            AccountLifeEvents.FindAll(i => i.StartDate.Year == dateTime.Year && dateTime.Month == i.StartDate.Month && i.LifeEventType == LifeEnum.OneTime)
                .ForEach(i => additonalAmount += i.Amount);

            return additonalAmount;
        }

        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
            lifeEvent.ValueChanged += LifeEvent_ValueChanged;
            AccountLifeEvents.Add(lifeEvent);
            ValueChanged?.Invoke(this, this);
        }

        private void LifeEvent_ValueChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, this);
        }

        #endregion

        #region Overrident Methods

        public override bool Equals(object obj)
        {
            var temp = obj as CompoundAccount;

            if (temp == null)
                return false;
            else if (temp.Id == Id )
            {
                foreach (var item in AccountLifeEvents)
                {
                    var accEvent = temp.AccountLifeEvents.Find(t => t.Id == item.Id);
                    if (!accEvent.Equals(item))
                        return false;
                }
                return true;
            }
            else
                return false;

        }

        #endregion
    }
}
