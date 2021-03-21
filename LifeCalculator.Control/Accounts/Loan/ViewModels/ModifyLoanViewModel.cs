using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Control.ViewModels
{
    public class ModifyLoanViewModel : ViewModelBase, IAccount
    {
        #region Fields

        private LoanAccount _account;
        public event EventHandler ValueChanged;
        public event EventHandler<IAccountEvent> LifeEventAdded;

        #endregion

        #region Constructors

        public ModifyLoanViewModel()
        {
        }

        public ModifyLoanViewModel(LoanAccount account)
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

        public double MonthlyPayment
        {
            get
            {
                return _account.MonthlyPayment;
            }
            set
            {
                _account.MonthlyPayment = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("MonthlyPayment");
            }
        }

        public double LoanAmount
        {
            get
            {
                return _account.LoanAmount;
            }
            set
            {
                _account.LoanAmount = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("LoanAmount");
            }
        }

        public double DownPayment
        {
            get
            {
                return _account.DownPayment;
            }
            set
            {
                _account.DownPayment = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("DownPayment");
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

        public double InterestPaid
        {
            get
            {
                return _account.InterestPaid;
            }
            set
            {
                _account.InterestPaid = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("InterestPaid");
            }
        }

        public double PrincipalPaid
        {
            get
            {
                return _account.PrincipalPaid;
            }
            set
            {
                _account.PrincipalPaid = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("PrincipalPaid");
            }
        }

        public int LoanLengthMonths
        {
            get
            {
                return _account.LoanLengthMonths;
            }
            set
            {
                _account.LoanLengthMonths = value;
                ValueChanged?.Invoke(this, new EventArgs());
                OnPropertyChanged("LoanLengthMonths");
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

        public List<IAccountEvent> AccountLifeEvents { get; set; }

        #endregion

        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
            //throw new NotImplementedException();
        }

        public List<MonthlyColumn> Calculation()
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
