using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using System;

namespace LifeCalculator.Control.ViewModels
{
    public class ModifyEventLoanViewModel : ViewModelBase, IAccountEvent
    {
        #region Fields

        private IAccountEvent lifeEvent;

        #endregion


        #region Constructors

        public ModifyEventLoanViewModel(IAccountEvent lifeEvent)
        {
            this.lifeEvent = lifeEvent;
        }

        #endregion


        #region Properties

        public string Name
        {
            get
            {
                return lifeEvent.Name;
            }
            set
            {
                lifeEvent.Name = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("Name");
            }
        }

        public LifeEnum LifeEventType
        {
            get
            {
                return lifeEvent.LifeEventType;
            }
            set
            {
                lifeEvent.LifeEventType = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("LifeEventType");
            }
        }

        public AccountTypes AccountType
        {
            get
            {
                return lifeEvent.AccountType;
            }
            set
            {
                lifeEvent.AccountType = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("AccountType");
            }
        }

        public double CurrentValue
        {
            get
            {
                return lifeEvent.CurrentValue;
            }
            set
            {
                lifeEvent.CurrentValue = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("StartDate");
            }
        }

        public double Amount
        {
            get
            {
                return lifeEvent.Amount;
            }
            set
            {
                lifeEvent.Amount = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("StartDate");
            }
        }

        public double InterestRate
        {
            get
            {
                return lifeEvent.InterestRate;
            }
            set
            {
                lifeEvent.InterestRate = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("StartDate");
            }
        }

        public DateTime StartDate
        {
            get
            {
                return lifeEvent.StartDate;
            }
            set
            {
                lifeEvent.StartDate = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("StartDate");
            }
        }

        public DateTime EndDate
        {
            get
            {
                return lifeEvent.EndDate;
            }
            set
            {
                lifeEvent.EndDate = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("StartDate");
            }
        }

        public int Id
        {
            get
            {
                return lifeEvent.Id;
            }
            set
            {
            }
        }

        public int AccountId
        {
            get
            {
                return lifeEvent.AccountId;
            }
            set
            {
                lifeEvent.AccountId = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("StartDate");
            }
        }

        public event EventHandler ValueChanged;

        #endregion


    }
}
