using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.LifeEvents;
using Microsoft.VisualStudio.PlatformUI;
using System;

namespace LifeCalculator.Control.ViewModels
{
    public class AddEventCompoundViewModel : ViewModelBase
    {
        #region Fields

        private IAccount _account;

        #endregion

        #region Constructors

        public AddEventCompoundViewModel()
        {
            AddEventCommand = new DelegateCommand(AddLifeEventCommandHandler);
            EventDate = DateTime.Now;
        }

        #endregion

        #region Properties

        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public bool StartEvent { get; set; }
        public bool StopEvent { get; set; }
        public double AmountToContribute { get; set; }
        public double InterestValue { get; set; }
        public DelegateCommand AddEventCommand { get; set; }

        #endregion

        #region Command Handlers

        private void AddLifeEventCommandHandler()
        {

            _account.AddLifeEvent(new InvestmentAccountEvent()
            {
                Name = EventName,
                Amount = AmountToContribute,
                StartDate = EventDate,
                InterestRate = InterestValue * .01
            });

            _account.Calculation();
        }

        #endregion
    }
}

