using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.FinancialAccount;
using Microsoft.VisualStudio.PlatformUI;
using System.Windows.Input;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Services.FinancialAccountService;

namespace LifeCalculator.ViewModels
{
    public class FinancialProfileViewModel : ViewModelBase
    {
        #region Fields

        private FinancialAccount _currentAccount;
        private IFinancialAccountDataService _financialAccountService;
        private bool _editViewVisible;
        private bool _summaryViewVisible;

        #endregion

        #region Constructors

        public FinancialProfileViewModel(IAccountStore accountStore, IFinancialAccountDataService financialAccountService)
        {
            _currentAccount = accountStore.CurrentAccount;
            _financialAccountService = financialAccountService;
            SummaryViewVisible = true;
            EditViewCommand = new DelegateCommand(EditViewCommand_Execute, EditViewCommand_CanExecute);
            SummaryViewCommand = new DelegateCommand(SummaryViewCommand_Execute, SummaryViewCommand_CanExecute);
        }

        #endregion

        #region Properties

        public double Salary
        {
            get => _currentAccount.Salary;
            set
            {
                _currentAccount.Salary = value;
                OnPropertyChanged(nameof(Salary));
            }
        }

        public double NetMonthlyIncome
        {
            get => _currentAccount.NetMonthlyIncome;
            set
            {
                _currentAccount.NetMonthlyIncome = value;
                OnPropertyChanged(nameof(NetMonthlyIncome));
            }
        }

        public double Rent
        {
            get => _currentAccount.Rent;
            set
            {
                _currentAccount.Rent = value;
                OnPropertyChanged(nameof(Rent));
            }
        }

        public double WaterBill
        {
            get => _currentAccount.WaterBill;
            set
            {
                _currentAccount.WaterBill = value;
                OnPropertyChanged(nameof(WaterBill));
            }
        }

        public double ElectricBill
        {
            get => _currentAccount.ElectricBill;
            set
            {
                _currentAccount.ElectricBill = value;
                OnPropertyChanged(nameof(ElectricBill));
            }
        }

        public double InternetBill
        {
            get => _currentAccount.InternetBill;
            set
            {
                _currentAccount.InternetBill = value;
                OnPropertyChanged(nameof(InternetBill));
            }
        }

        public double CableBill
        {
            get => _currentAccount.CableBill;
            set
            {
                _currentAccount.CableBill = value;
                OnPropertyChanged(nameof(CableBill));
            }
        }

        public double Subscriptions
        {
            get => _currentAccount.Subscriptions;
            set
            {
                _currentAccount.Subscriptions = value;
                OnPropertyChanged(nameof(Subscriptions));
            }
        }

        public double Groceries
        {
            get => _currentAccount.Groceries;
            set
            {
                _currentAccount.Groceries = value;
                OnPropertyChanged(nameof(Groceries));
            }
        }

        public double EmergencyFundContributions
        {
            get => _currentAccount.EmergencyFundContributions;
            set
            { 
                _currentAccount.EmergencyFundContributions = value;
                OnPropertyChanged(nameof(EmergencyFundContributions));
            }
        }

        public double Gas
        {
            get => _currentAccount.Gas;
            set
            {
                _currentAccount.Gas = value;
                OnPropertyChanged(nameof(Gas));
            }
        }

        public double CarInsurance
        {
            get => _currentAccount.CarInsurance;
            set
            {
                _currentAccount.CarInsurance = value;
                OnPropertyChanged(nameof(CarInsurance));
            }
        }

        public double HomeInsurance
        {
            get => _currentAccount.HomeInsurance;
            set
            {
                _currentAccount.HomeInsurance = value;
                OnPropertyChanged(nameof(HomeInsurance));
            }
        }

        public double CarPayments
        {
            get => _currentAccount.CarPayments;
            set
            {
                _currentAccount.CarPayments = value;
                OnPropertyChanged(nameof(CarPayments));
            }
        }

        public double OtherDebts
        {
            get => _currentAccount.OtherDebts;
            set
            {
                _currentAccount.OtherDebts = value;
                OnPropertyChanged(nameof(OtherDebts));
            }
        }

        public double MiscellaneousPayments
        {
            get => _currentAccount.MiscellaneousPayments;
            set
            {
                _currentAccount.MiscellaneousPayments = value;
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

        /// <summary>
        /// Saves the entered data into the users financial account in the database
        /// and swaps back to the edit view.
        /// </summary>
        public void SummaryViewCommand_Execute()
        {
            SummaryViewVisible = true;
            EditViewVisible = false;
            _financialAccountService.Save(_currentAccount.Id ,_currentAccount);
        }

        public bool SummaryViewCommand_CanExecute()
        {
           return EditViewVisible == true;
        }

        #endregion
    }
}
