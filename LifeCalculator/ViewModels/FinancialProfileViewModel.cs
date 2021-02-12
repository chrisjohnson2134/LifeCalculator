using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.FinancialAccount;
using Microsoft.VisualStudio.PlatformUI;
using System.Windows.Input;
using LifeCalculator.Framework.Managers.Interfaces;

namespace LifeCalculator.ViewModels
{
    public class FinancialProfileViewModel : ViewModelBase
    {
        #region Fields

        private readonly FinancialAccount _financialAccount;
        private bool _editViewVisible;
        private bool _summaryViewVisible;

        #endregion

        #region Constructors

        public FinancialProfileViewModel()
        {
            _financialAccount = new FinancialAccount();
            _financialAccount.Salary = 2000;
            SummaryViewVisible = true;
            EditViewCommand = new DelegateCommand(EditViewCommand_Execute, EditViewCommand_CanExecute);
            SummaryViewCommand = new DelegateCommand(SummaryViewCommand_Execute, SummaryViewCommand_CanExecute);
        }

        #endregion

        #region Properties

        public double Salary
        {
            get => _financialAccount.Salary;
            set
            {
                _financialAccount.Salary = value;
                OnPropertyChanged(nameof(Salary));
            }
        }

        public double NetMonthlyIncome
        {
            get => _financialAccount.NetMonthlyIncome;
            set
            {
                _financialAccount.NetMonthlyIncome = value;
                OnPropertyChanged(nameof(NetMonthlyIncome));
            }
        }

        public double Rent
        {
            get => _financialAccount.Rent;
            set
            {
                _financialAccount.Rent = value;
                OnPropertyChanged(nameof(Rent));
            }
        }

        public double WaterBill
        {
            get => _financialAccount.WaterBill;
            set
            {
                _financialAccount.WaterBill = value;
                OnPropertyChanged(nameof(WaterBill));
            }
        }

        public double ElectricBill
        {
            get => _financialAccount.ElectricBill;
            set
            {
                _financialAccount.ElectricBill = value;
                OnPropertyChanged(nameof(ElectricBill));
            }
        }

        public double InternetBill
        {
            get => _financialAccount.InternetBill;
            set
            {
                _financialAccount.InternetBill = value;
                OnPropertyChanged(nameof(InternetBill));
            }
        }

        public double CableBill
        {
            get => _financialAccount.CableBill;
            set
            {
                _financialAccount.CableBill = value;
                OnPropertyChanged(nameof(CableBill));
            }
        }

        public double Subscriptions
        {
            get => _financialAccount.Subscriptions;
            set
            {
                _financialAccount.Subscriptions = value;
                OnPropertyChanged(nameof(Subscriptions));
            }
        }

        public double Groceries
        {
            get => _financialAccount.Groceries;
            set
            {
                _financialAccount.Groceries = value;
                OnPropertyChanged(nameof(Groceries));
            }
        }

        public double EmergencyFundContributions
        {
            get => _financialAccount.EmergencyFundContributions;
            set
            { 
                _financialAccount.EmergencyFundContributions = value;
                OnPropertyChanged(nameof(EmergencyFundContributions));
            }
        }

        public double Gas
        {
            get => _financialAccount.Gas;
            set
            {
                _financialAccount.Gas = value;
                OnPropertyChanged(nameof(Gas));
            }
        }

        public double CarInsurance
        {
            get => _financialAccount.CarInsurance;
            set
            {
                _financialAccount.CarInsurance = value;
                OnPropertyChanged(nameof(CarInsurance));
            }
        }

        public double HomeInsurance
        {
            get => _financialAccount.HomeInsurance;
            set
            {
                _financialAccount.HomeInsurance = value;
                OnPropertyChanged(nameof(HomeInsurance));
            }
        }

        public double CarPayments
        {
            get => _financialAccount.CarPayments;
            set
            {
                _financialAccount.CarPayments = value;
                OnPropertyChanged(nameof(CarPayments));
            }
        }

        public double OtherDebts
        {
            get => _financialAccount.OtherDebts;
            set
            {
                _financialAccount.OtherDebts = value;
                OnPropertyChanged(nameof(OtherDebts));
            }
        }

        public double MiscellaneousPayments
        {
            get => _financialAccount.MiscellaneousPayments;
            set
            {
                _financialAccount.MiscellaneousPayments = value;
                OnPropertyChanged(nameof(MiscellaneousPayments));
            }
        }

        public bool EditViewVisible
        {
            get
            {
                return _editViewVisible;
            }
            set
            {
                _editViewVisible = value;
                OnPropertyChanged(nameof(EditViewVisible));
            }
        }

        public bool SummaryViewVisible
        {
            get
            {
                return _summaryViewVisible;
            }
            set
            {
                _summaryViewVisible = value;
                OnPropertyChanged(nameof(SummaryViewVisible));
            }
        }

        #region Commands

        public ICommand EditViewCommand { get; private set; }

        public ICommand SummaryViewCommand { get; private set; }

        #endregion

        #endregion

        #region Command Methods

        public void EditViewCommand_Execute()
        {
            EditViewVisible = true;
            SummaryViewVisible = false;
        }

        public bool EditViewCommand_CanExecute()
        {
            return SummaryViewVisible == true;
        }

        public void SummaryViewCommand_Execute()
        {
            SummaryViewVisible = true;
            EditViewVisible = false;
        }

        public bool SummaryViewCommand_CanExecute()
        {
           return EditViewVisible == true;
        }

        #endregion
    }
}
