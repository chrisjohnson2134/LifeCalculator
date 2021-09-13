using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Database;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.SimulatedAccount
{
    public class LoanAccount : ISimulatedAccount, IDatabaseable
    {
        public event EventHandler<IAccountEvent> LifeEventAdded;
        public event EventHandler<ISimulatedAccount> ValueChanged;

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

        private double _monthlyPayment;
        public double MonthlyPayment
        {
            get
            {
                return _monthlyPayment;
            }
            private set
            {
                _monthlyPayment = value;
            }
        }

        private double _loanAmount;
        public double LoanAmount
        {
            get
            {
                return _loanAmount;
            }
            set
            {
                _loanAmount = value;
                updateMonthlyPayment();
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _downPayment;
        public double DownPayment
        {
            get
            {
                return _downPayment;
            }
            set
            {
                _downPayment = value;
                updateMonthlyPayment();
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
                updateMonthlyPayment();
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _interestPaid;
        public double InterestPaid
        {
            get
            {
                return _interestPaid;
            }
            private set
            {
                _interestPaid = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _principalPaid;
        public double PrincipalPaid
        {
            get
            {
                return _principalPaid;
            }
            private set
            {
                _principalPaid = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private int _loanLengthMonths;
        public int LoanLengthMonths
        {
            get
            {
                return _loanLengthMonths;
            }
            set
            {
                _loanLengthMonths = value;
                updateMonthlyPayment();
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

        [IgnoreDatabase]
        public List<IAccountEvent> AccountLifeEvents { get; set; }

        public LoanAccount()
        {
            AccountLifeEvents = new List<IAccountEvent>();
        }

        public LoanAccount(string name, DateTime date, int loanLengthMonths, double interestRate, double loanAmount, double downPayment)
        {
            _name = name;
            _interestRate = interestRate / 100;
            _loanAmount = loanAmount;
            _downPayment = downPayment;
            _loanLengthMonths = loanLengthMonths;
            _startDate = date;


            updateMonthlyPayment();
            AccountLifeEvents = new List<IAccountEvent>();

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

        private void updateMonthlyPayment()
        {
            _monthlyPayment = (_loanAmount - _downPayment) * (Math.Pow((1 + (_interestRate / 12)), _loanLengthMonths) * _interestRate)
                / (12 * (Math.Pow((1 + (_interestRate / 12)), _loanLengthMonths) - 1));

            _monthlyPayment = Math.Round(_monthlyPayment,2);
        }

        public List<MonthlyColumn> Calculation()
        {
            double currValue = _loanAmount - _downPayment;
            double interestPay;
            double principalPay;
            _interestPaid = 0;
            _principalPaid = 0;
            List<MonthlyColumn> monthlies = new List<MonthlyColumn>();

            monthlies.Add(new MonthlyColumn());
            int monthDiff = 0;

            AccountLifeEvents.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            DateTime stopDate = _startDate.AddMonths(LoanLengthMonths);

            monthDiff = Math.Abs(_startDate.Year * 12 + (_startDate.Month - 1)
                    - (stopDate.Year * 12 + (stopDate.Month - 1)));


            for (int j = 0; j < monthDiff; j++)
            {
                interestPay = currValue * _interestRate / 12;

                if (_monthlyPayment < currValue)
                    principalPay = _monthlyPayment - interestPay + additionalPriPaymentCalculation(StartDate.AddMonths(1 + j));
                else if (currValue > 0)
                    principalPay = currValue;
                else
                    principalPay = 0;

                _interestPaid += interestPay;
                _principalPaid += principalPay;
                currValue = currValue - principalPay;
                monthlies.Add(new MonthlyColumn()
                {
                    Name = _name,
                    Gain = Math.Round((_loanAmount - _downPayment) - _principalPaid,2),
                    Date = _startDate.AddMonths(1 + j)
                });

            }

            monthlies[monthlies.Count - 1].Gain = Math.Round(monthlies[monthlies.Count - 1].Gain + currValue,2);
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

        public override bool Equals(object obj)
        {
            var entity = obj as LoanAccount;

            if (obj == null || entity == null)
                return false;

            if (entity.Id == Id)
            {
                foreach (var item in AccountLifeEvents)
                {
                    var accEvent = entity.AccountLifeEvents.Find(t => t.Id == item.Id);
                    if (!accEvent.Equals(item))
                        return false;
                }
                return true;
            }

            return false;
        }
    }
}
