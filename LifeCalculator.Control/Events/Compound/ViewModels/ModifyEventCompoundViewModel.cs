using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using System;

namespace LifeCalculator.Control.ViewModels
{
    public class ModifyEventCompoundViewModel : ViewModelBase , IAccountEvent
    {
        #region Fields

        private IAccountEvent _lifeEvent;

        #endregion

        #region Constructors

        public ModifyEventCompoundViewModel()
        {
        }

        public ModifyEventCompoundViewModel(IAccountEvent e)
        {
            _lifeEvent = e;
        }

        #endregion

        #region Properties

        public string Name
        {
            get => _lifeEvent.Name;
            set
            {
                _lifeEvent.Name = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged(nameof(Name));
            }
        }

        public LifeEnum LifeEventType { get; set; }

        public DateTime Date
        {
            get => _lifeEvent.StartDate;
            set
            {
                _lifeEvent.StartDate = value;

                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged(nameof(Date));
            }
        }

        public bool EndDateEnabled => _lifeEvent.LifeEventType == LifeEnum.MonthlyContribute ? true : false;

        public DateTime EndDate
        {
            get => _lifeEvent.EndDate;
            set
            {
                _lifeEvent.EndDate = value;

                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public double Amount
        {
            get => _lifeEvent.Amount;
            set
            {
                _lifeEvent.Amount = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged(nameof(Amount));
            }
        }

        public double CurrentValue
        {
            get => _lifeEvent.CurrentValue;
            set
            {
                _lifeEvent.CurrentValue = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged(nameof(CurrentValue));
            }
        }

        public double InterestRate
        {
            get => _lifeEvent.InterestRate;
            set
            {
                _lifeEvent.InterestRate = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged(nameof(InterestRate));
            }
        }

        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public AccountTypes AccountType { get; set; }

        public event EventHandler ValueChanged;

        #endregion

    }
    
}
