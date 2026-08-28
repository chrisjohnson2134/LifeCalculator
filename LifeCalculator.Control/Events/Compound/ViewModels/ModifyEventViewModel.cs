using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Managers;
using System;

namespace LifeCalculator.Control.ViewModels
{
    public class ModifyEventViewModel : ValidatableViewModelBase , IAccountEvent
    {
        #region Fields

        private IAccountEvent _lifeEvent;
        private IAccountsEventsManager _eventsManager;

        #endregion

        #region Events

        /// <summary>Raised after deletion so the owning account can drop this row from its list.</summary>
        public event EventHandler<IAccountEvent> EventDeleted;

        #endregion

        #region Constructors

        public ModifyEventViewModel()
        {
        }

        public ModifyEventViewModel(IAccountEvent e, IAccountsEventsManager eventsManager = null)
        {
            _lifeEvent = e;
            _eventsManager = eventsManager;
            DeleteEventCommand = new RelayCommand(DeleteEvent, () => _eventsManager != null);
            ValidateAll();
        }

        #endregion

        #region Commands

        public IRelayCommand DeleteEventCommand { get; }

        private void DeleteEvent()
        {
            _eventsManager.DeleteAccountEvent(_lifeEvent);
            EventDeleted?.Invoke(this, _lifeEvent);
        }

        #endregion

        #region Properties

        public string Name
        {
            get => _lifeEvent.Name;
            set
            {
                Validate(nameof(Name), () => !string.IsNullOrWhiteSpace(value), "Event name is required.");

                if (!string.IsNullOrWhiteSpace(value))
                {
                    _lifeEvent.Name = value;
                    ValueChanged?.Invoke(this, _lifeEvent);
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public LifeEnum LifeEventType { get; set; }

        public DateTime Date
        {
            get => _lifeEvent.StartDate;
            set
            {
                _lifeEvent.StartDate = value;
                ValidateDateRange();

                ValueChanged?.Invoke(this, _lifeEvent);
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
                ValidateDateRange();

                ValueChanged?.Invoke(this, _lifeEvent);
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public double Amount
        {
            get => _lifeEvent.Amount;
            set
            {
                Validate(nameof(Amount), () => value > 0, "Amount must be greater than 0.");

                if (value > 0)
                {
                    _lifeEvent.Amount = value;
                    ValueChanged?.Invoke(this, _lifeEvent);
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public double CurrentValue
        {
            get => _lifeEvent.CurrentValue;
            set
            {
                _lifeEvent.CurrentValue = value;
                ValueChanged?.Invoke(this, _lifeEvent);
                OnPropertyChanged(nameof(CurrentValue));
            }
        }

        public double InterestRate
        {
            get => _lifeEvent.InterestRate;
            set
            {
                _lifeEvent.InterestRate = value;
                ValueChanged?.Invoke(this, _lifeEvent);
                OnPropertyChanged(nameof(InterestRate));
            }
        }

        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public AccountTypes AccountType { get; set; }

        public event EventHandler<IAccountEvent> ValueChanged;

        #endregion

        #region Validation

        private void ValidateAll()
        {
            Validate(nameof(Name), () => !string.IsNullOrWhiteSpace(_lifeEvent.Name), "Event name is required.");
            Validate(nameof(Amount), () => _lifeEvent.Amount > 0, "Amount must be greater than 0.");
            ValidateDateRange();
        }

        private void ValidateDateRange()
        {
            if (!EndDateEnabled)
            {
                RemoveError(nameof(EndDate), "End date must be after the start date.");
                return;
            }

            Validate(nameof(EndDate), () => _lifeEvent.EndDate > _lifeEvent.StartDate, "End date must be after the start date.");
        }

        #endregion

    }

}
