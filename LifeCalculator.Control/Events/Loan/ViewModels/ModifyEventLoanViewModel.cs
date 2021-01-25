using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using System;

namespace LifeCalculator.Control.Events.Loan.ViewModels
{
    public class ModifyEventLoanViewModel : ILifeEvent
    {
        #region Fields

        private ILifeEvent lifeEvent;

        #endregion


        #region Constructors

        public ModifyEventLoanViewModel(ILifeEvent lifeEvent)
        {
            this.lifeEvent = lifeEvent;
        }

        #endregion


        #region Properties

        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public LifeEnum LifeEventType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime Date { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool FinalEvent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double CurrentValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Amount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double InterestRate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler ValueChanged;

        #endregion


    }
}
