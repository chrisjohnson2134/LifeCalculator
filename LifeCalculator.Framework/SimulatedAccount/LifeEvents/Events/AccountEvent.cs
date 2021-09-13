using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.LifeEvents
{
    public class AccountEvent : IAccountEvent
    {
        public event EventHandler ValueChanged;
        public int Id { get; set; }


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
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private int _accountId;
        public int AccountId 
        {
            get
            {
                return _accountId;
            }
            set
            {
                _accountId = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public LifeEnum LifeEventType { get; set; }
        public AccountTypes AccountType { get; set; }

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
                ValueChanged?.Invoke(this, EventArgs.Empty);
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
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private double _currentValue;
        public double CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                _currentValue = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private double _amount;
        public double Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
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
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public AccountEvent()
        {
        }

        public AccountEvent(IAccountEvent entity)
        {
            _name = entity.Name;
            Id = entity.Id;
            _accountId = entity.AccountId;
            LifeEventType = entity.LifeEventType;
            AccountType = entity.AccountType;
            _startDate = entity.StartDate;
            _endDate = entity.EndDate;
            _currentValue = entity.CurrentValue;
            _amount = entity.Amount;
            _interestRate = entity.InterestRate;
        }


        //rethink
        public override bool Equals(object obj)
        {
            AccountEvent accountEvent = obj as AccountEvent;
            if (accountEvent == null)
                return false;

            if (Id == accountEvent.Id )
                    return true;

            return false;
        }
    }

    
}
