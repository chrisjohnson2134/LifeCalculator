using LifeCalculator.Control.Settings.Interfaces;
using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Services.PlaidService;
using LifeCalculator.Framework.Settings;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.ObjectModel;
using LifeCalculator.Framework.Services.PlaidAccInfoDataService;

namespace LifeCalculator.Control.ViewModels
{
    public class PlaidDevSettingsViewModel : ViewModelBase, ISettingsViewModel
    {

        #region Fields

        BudgetManager _budgetManager;
        InstitutionDataService institutionDataService;

        #endregion

        #region Constructors

        public PlaidDevSettingsViewModel(IAccountStore accountStore)
        {
            _budgetManager = accountStore.CurrentAccount.BudgetManager;
            //_budgetManager.AutoSort = true;
            Name = "Plaid Dev Settings";

            if (Framework.Enums.Environment.Development == AppSettings.Instance.PlaidApiSettings.Environment)
            {
                Institutions = new ObservableCollection<Institution>();
                foreach (var item in AppSettings.Instance.DevelopmentInstitutions)
                {
                    Institutions.Add(item);
                }
            }
            else
                Institutions = new ObservableCollection<Institution>();

            Transactions = new ObservableCollection<TransactionItem>();

            AddNewAccountCommand = new DelegateCommand(AddNewAccountCommandHandler);
            AddAccountCommand = new DelegateCommand(AddAccountCommandHandler);
            SaveSettingsCommand = new DelegateCommand(SaveSettingsCommandHandler);
            LoadTransactionsCommand = new DelegateCommand(LoadTransactionsCommandHandler);

            institutionDataService = new InstitutionDataService();


            StartDate = DateTime.Now.AddMonths(-1);
            EndDate = DateTime.Now;
        }

        #endregion

        #region Properties

        public string Name { get; set; }


        private string _institutionID;
        public string InstitutionID
        {
            get
            {
                return _institutionID;
            }
            set
            {
                _institutionID = value;
                OnPropertyChanged("InstitutionID");
            }
        }

        private string _publicToken;
        public string PublicToken
        {
            get
            {
                return _publicToken;
            }
            set
            {
                _publicToken = value;
                OnPropertyChanged("PublicToken");
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        public ObservableCollection<Institution> Institutions { get; set; }
        public ObservableCollection<TransactionItem> Transactions { get; set; }


        public DelegateCommand AddNewAccountCommand { get; set; }
        public DelegateCommand AddAccountCommand { get; set; }
        public DelegateCommand SaveSettingsCommand { get; set; }
        public DelegateCommand LoadTransactionsCommand { get; set; }

        #endregion

        #region Command Handlers

        private void AddNewAccountCommandHandler()
        {
            PlaidService.StartPlaidLink();
        }

        private async void AddAccountCommandHandler()
        {
            Institution bank = PlaidService.GetInstitutionById(InstitutionID);
            bank.Credentials = PlaidService.AuthorizeInstitution(bank, PublicToken);

            var accounts = PlaidService.GetInstitutionAccounts(AppSettings.Instance.DevelopmentInstitutions[0]);
            bank.Accounts = accounts;

            if (AppSettings.Instance.PlaidApiSettings.Environment == Framework.Enums.Environment.Development)
            {
                AppSettings.Instance.DevelopmentInstitutions.Add(bank);
                var insertedBank = await institutionDataService.Insert(bank);
                Institutions.Add(insertedBank);
            }
            else
            {
                AppSettings.Instance.SandboxInstitutions.Add(bank);
                Institutions.Add(bank);
            }
        }

        public void SaveSettingsCommandHandler()
        {
            //AppSettings.SaveSettings();
        }

        private void LoadTransactionsCommandHandler(object obj)
        {
            var transactions = PlaidService.GetTransactions(AppSettings.Instance.DevelopmentInstitutions[0],DateTime.Now.AddMonths(-2),DateTime.Now);
            Transactions.Clear();
            if (Framework.Enums.Environment.Development == AppSettings.Instance.PlaidApiSettings.Environment && AppSettings.Instance.DevelopmentInstitutions[0] != null)
                foreach (var item in PlaidService.GetTransactions(AppSettings.Instance.DevelopmentInstitutions[0], StartDate, EndDate))
                {
                    Transactions.Add(item);
                    _budgetManager.AddTransaction(item);
                }
            else if (Framework.Enums.Environment.Sandbox == AppSettings.Instance.PlaidApiSettings.Environment && AppSettings.Instance.SandboxInstitutions[0] != null)
                foreach (var item in PlaidService.GetTransactions(AppSettings.Instance.SandboxInstitutions[0], StartDate, EndDate))
                {
                    Transactions.Add(item);
                }
        }

        #endregion

        #region HelperMethod

        #endregion
    }
}
