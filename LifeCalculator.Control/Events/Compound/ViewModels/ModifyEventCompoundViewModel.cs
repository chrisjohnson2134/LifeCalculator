﻿using LifeCalculator.Framework.LifeEvents;
using Prism.Mvvm;
using System;

namespace LifeCalculator.Control.ViewModels
{
    public class ModifyEventCompoundViewModel : BindableBase , ILifeEvent
    {
        #region Fields

        private ILifeEvent _lifeEvent;

        #endregion

        #region Constructors

        public ModifyEventCompoundViewModel()
        {
        }

        public ModifyEventCompoundViewModel(ILifeEvent e)
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
                RaisePropertyChanged("Name");
            }
        }

        public LifeEnum LifeEventType { get; set; }

        public DateTime Date
        {
            get => _lifeEvent.Date;
            set
            {
                _lifeEvent.Date = value;
                ValueChanged?.Invoke(this, new EventArgs());
                RaisePropertyChanged("Date");
            }
        }

        public double Amount
        {
            get => _lifeEvent.Amount;
            set
            {
                _lifeEvent.Amount = value;
                ValueChanged?.Invoke(this, new EventArgs());
                RaisePropertyChanged("Amount");
            }
        }

        public double CurrentValue
        {
            get => _lifeEvent.CurrentValue;
            set
            {
                _lifeEvent.CurrentValue = value;
                ValueChanged?.Invoke(this, new EventArgs());
                RaisePropertyChanged("CurrentValue");
            }
        }

        public double InterestRate
        {
            get => _lifeEvent.InterestRate;
            set
            {
                _lifeEvent.InterestRate = value;
                ValueChanged?.Invoke(this, new EventArgs());
                RaisePropertyChanged("InterestRate");
            }
        }

        public bool FinalEvent
        {
            get => _lifeEvent.FinalEvent;
            set
            {
                _lifeEvent.FinalEvent = value;
                ValueChanged?.Invoke(this, new EventArgs());
                RaisePropertyChanged("FinalEvent");
            }
        }

        public event EventHandler ValueChanged;

        #endregion

    }
    
}
