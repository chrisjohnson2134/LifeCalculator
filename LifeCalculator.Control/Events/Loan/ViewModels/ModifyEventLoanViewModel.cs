using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using System;

namespace LifeCalculator.Control.Events.Loan.ViewModels
{
    public class ModifyEventLoanViewModel : IAccountEvent
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

        public string Name { get; set; }
        public LifeEnum LifeEventType { get; set; }
        public DateTime StartDate { get; set; }
        public bool FinalEvent { get; set; }
        public double CurrentValue { get; set; }
        public double Amount { get; set; }
        public double InterestRate { get; set; }
        public DateTime EndDate { get; set; }

        public int Id { get; }

        public int AccountId { get; set; }

        public event EventHandler ValueChanged;

        #endregion


    }
}
