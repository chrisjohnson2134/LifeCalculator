using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.PlatformUI;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Control.Events;

namespace LifeCalculator.Control.ViewModels
{
    public class AddEventLoanViewModel : ViewModelBase, IControlEvent
    {

        public event EventHandler<IAccountEvent> EventAdded;

        private ISimulatedAccount _loanAccount;

        public AddEventLoanViewModel(LoanAccount loanAccount)
        {
            _loanAccount = loanAccount;

            EventTypes = new List<string> { "One-Time", "Monthly" };

            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddYears(1);

            AddEventCommand = new DelegateCommand(AddEventCommandHandler);

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
                if (_eventSelected.Equals("One-Time"))
                    NeedsEndDate = false;
                else
                    NeedsEndDate = true;

                OnPropertyChanged("NeedsEndDate");
            }
        }
        public string EventName { get; set; }
        public DateTime StartDate { get; set; }
        public bool NeedsEndDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Contribute { get; set; }

        public DelegateCommand AddEventCommand { get; set; }


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
                AccountId = _loanAccount.Id,
                AccountType = AccountTypes.LoanAccount
            };

            _loanAccount.AddLifeEvent(accountEvent);

            EventAdded?.Invoke(this,accountEvent);
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
