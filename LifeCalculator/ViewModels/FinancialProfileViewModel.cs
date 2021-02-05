using LifeCalculator.Framework.AccountManager;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.FinancialProfile;
using Microsoft.VisualStudio.PlatformUI;
using System.Windows.Input;

namespace LifeCalculator.ViewModels
{
    public class FinancialProfileViewModel : ViewModelBase
    {
        #region Fields

        private readonly FinancialProfile _financialProfile;
        private bool _editViewVisible;
        private bool _summaryViewVisible;

        #endregion

        #region Constructors

        public FinancialProfileViewModel(IAccountManager accountManager)
        {
            _financialProfile = new FinancialProfile(accountManager);
            _financialProfile.Salary = 2000;
            SummaryViewVisible = true;
            EditViewCommand = new DelegateCommand(EditViewCommand_Execute, EditViewCommand_CanExecute);
            SummaryViewCommand = new DelegateCommand(SummaryViewCommand_Execute, SummaryViewCommand_CanExecute);
        }

        #endregion

        #region Properties

        public double Salary
        {
            get => _financialProfile.Salary;
            set
            {
                _financialProfile.Salary = value;
                OnPropertyChanged(nameof(Salary));
            }
        }

        public double NetMonthlyIncome
        {
            get => _financialProfile.NetMonthlyIncome;
            set
            {
                _financialProfile.NetMonthlyIncome = value;
                OnPropertyChanged(nameof(NetMonthlyIncome));
            }
        }

        public double Rent
        {
            get => _financialProfile.Rent;
            set
            {
                _financialProfile.Rent = value;
                OnPropertyChanged(nameof(Rent));
            }
        }

        public double WaterBill
        {
            get => _financialProfile.WaterBill;
            set
            {
                _financialProfile.WaterBill = value;
                OnPropertyChanged(nameof(WaterBill));
            }
        }

        public double ElectricBill
        {
            get => _financialProfile.ElectricBill;
            set
            {
                _financialProfile.ElectricBill = value;
                OnPropertyChanged(nameof(ElectricBill));
            }
        }

        public double InternetBill
        {
            get => _financialProfile.InternetBill;
            set
            {
                _financialProfile.InternetBill = value;
                OnPropertyChanged(nameof(InternetBill));
            }
        }

        public double CableBill
        {
            get => _financialProfile.CableBill;
            set
            {
                _financialProfile.CableBill = value;
                OnPropertyChanged(nameof(CableBill));
            }
        }

        public double Subscriptions
        {
            get => _financialProfile.Subscriptions;
            set
            {
                _financialProfile.Subscriptions = value;
                OnPropertyChanged(nameof(Subscriptions));
            }
        }

        public double Groceries
        {
            get => _financialProfile.Groceries;
            set
            {
                _financialProfile.Groceries = value;
                OnPropertyChanged(nameof(Groceries));
            }
        }

        public double EmergencyFundContributions
        {
            get => _financialProfile.EmergencyFundContributions;
            set
            { 
                _financialProfile.EmergencyFundContributions = value;
                OnPropertyChanged(nameof(EmergencyFundContributions));
            }
        }

        public double Gas
        {
            get => _financialProfile.Gas;
            set
            {
                _financialProfile.Gas = value;
                OnPropertyChanged(nameof(Gas));
            }
        }

        public double CarInsurance
        {
            get => _financialProfile.CarInsurance;
            set
            {
                _financialProfile.CarInsurance = value;
                OnPropertyChanged(nameof(CarInsurance));
            }
        }

        public double HomeInsurance
        {
            get => _financialProfile.HomeInsurance;
            set
            {
                _financialProfile.HomeInsurance = value;
                OnPropertyChanged(nameof(HomeInsurance));
            }
        }

        public double CarPayments
        {
            get => _financialProfile.CarPayments;
            set
            {
                _financialProfile.CarPayments = value;
                OnPropertyChanged(nameof(CarPayments));
            }
        }

        public double OtherDebts
        {
            get => _financialProfile.OtherDebts;
            set
            {
                _financialProfile.OtherDebts = value;
                OnPropertyChanged(nameof(OtherDebts));
            }
        }

        public double MiscellaneousPayments
        {
            get => _financialProfile.MiscellaneousPayments;
            set
            {
                _financialProfile.MiscellaneousPayments = value;
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
