using LifeCalculator.Control.Events;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Control.ViewModels
{
    public class AddEventViewModel : ValidatableViewModelBase, IControlEvent
    {
        public event EventHandler<IAccountEvent> EventAdded;

        private ISimulatedAccount _account;

        public AddEventViewModel(ISimulatedAccount account)
        {
            _account = account;

            EventTypes = new List<string> { "One-Time", "Monthly" };

            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddYears(1);

            AddEventCommand = new RelayCommand(AddEventCommandHandler, () => !HasErrors);
            LinkCommandToValidation(AddEventCommand);

            ValidateAll();
        }

        #region Properties

        public List<string> EventTypes { get; set; }
        private string _eventSelected;
        public string EventSelected
        {
            get
            {
                return _eventSelected;
            }
            set
            {
                _eventSelected = value;
                if (_eventSelected != null && _eventSelected.Equals("One-Time"))
                    NeedsEndDate = false;
                else
                    NeedsEndDate = true;

                Validate(nameof(EventSelected), () => !string.IsNullOrWhiteSpace(_eventSelected), "Select an event type.");
                ValidateDateRange();
                OnPropertyChanged("NeedsEndDate");
            }
        }

        private string _eventName;
        public string EventName
        {
            get => _eventName;
            set
            {
                _eventName = value;
                Validate(nameof(EventName), () => !string.IsNullOrWhiteSpace(_eventName), "Event name is required.");
                OnPropertyChanged(nameof(EventName));
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                ValidateDateRange();
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public bool NeedsEndDate { get; set; }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                ValidateDateRange();
                OnPropertyChanged(nameof(EndDate));
            }
        }

        private double _contribute;
        public double Contribute
        {
            get => _contribute;
            set
            {
                _contribute = value;
                Validate(nameof(Contribute), () => _contribute > 0, "Amount must be greater than 0.");
                OnPropertyChanged(nameof(Contribute));
            }
        }

        public IRelayCommand AddEventCommand { get; set; }


        #endregion

        #region Validation

        private void ValidateAll()
        {
            Validate(nameof(EventSelected), () => !string.IsNullOrWhiteSpace(_eventSelected), "Select an event type.");
            Validate(nameof(EventName), () => !string.IsNullOrWhiteSpace(_eventName), "Event name is required.");
            Validate(nameof(Contribute), () => _contribute > 0, "Amount must be greater than 0.");
            ValidateDateRange();
        }

        private void ValidateDateRange()
        {
            if (!NeedsEndDate)
            {
                RemoveError(nameof(EndDate), "End date must be after the start date.");
                return;
            }

            Validate(nameof(EndDate), () => _endDate > _startDate, "End date must be after the start date.");
        }

        #endregion

        #region Command Handlers

        private void AddEventCommandHandler()
        {
            var accountEvent = new AccountEvent()
            {
                LifeEventType = EventSelectedToLifeEnum(EventSelected),
                Name = EventName,
                StartDate = StartDate,
                EndDate = EndDate,
                Amount = Contribute,
                AccountId = _account.Id,
                AccountType = _account is CompoundAccount ? AccountTypes.CompoundInterest : AccountTypes.LoanAccount,
            };

            _account.AddLifeEvent(accountEvent);

            EventAdded?.Invoke(this, accountEvent);
        }

        #endregion

        #region Helper Method

        private LifeEnum EventSelectedToLifeEnum(string eventSelected)
        {
            if (eventSelected.Equals("One-Time"))
                return LifeEnum.OneTime;
            else if (eventSelected.Equals("Monthly"))
                return LifeEnum.MonthlyContribute;

            return LifeEnum.MonthlyContribute;
        }

        #endregion
    }
}

