using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Control.ViewModels
{
    public class ModifyCompoundViewModel : ViewModelBase, IAccount
    {
        #region Fields

        public event EventHandler ValueChanged;
        public event EventHandler<IAccountEvent> LifeEventAdded;

        private CompoundAccount _account;

        #endregion

        #region Constructors

        public ModifyCompoundViewModel()
        {

        }

        public ModifyCompoundViewModel(CompoundAccount account)
        {
            _account = account;
        }

        #endregion

        #region Properties

        public int Id => _account.Id;

        public int UserId
        {
            get
            {
                return _account.UserId;
            }
            set
            {
                _account.UserId = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("UserId");
            }
        }

        public string Name
        {
            get
            {
                return _account.Name;
            }
            set
            {
                _account.Name = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("Name");
            }
        }

        public double InitialAmount
        {
            get
            {
                return _account.InitialAmount;
            }
            set
            {
                _account.InitialAmount = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("InitialAmount");
            }
        }

        public double InterestRate
        {
            get
            {
                return _account.InterestRate;
            }
            set
            {
                _account.InterestRate = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("InterestRate");
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _account.StartDate;
            }
            set
            {
                _account.StartDate = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("StartDate");
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _account.EndDate;
            }
            set
            {
                _account.EndDate = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("EndDate");
            }
        }

        public List<IAccountEvent> AccountLifeEvents { get; set; }

        
        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
            throw new NotImplementedException();
        }

        public List<MonthlyColumn> Calculation()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
