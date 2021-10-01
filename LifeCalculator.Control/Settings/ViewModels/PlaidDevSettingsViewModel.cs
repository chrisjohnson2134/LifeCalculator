using LifeCalculator.Control.Settings.Interfaces;
using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Services.PlaidService;
using LifeCalculator.Framework.Settings;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Control.ViewModels
{
    public class PlaidDevSettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        #region Constructors

        public PlaidDevSettingsViewModel()
        {
            Name = "Plaid Dev Settings";

            if (Framework.Enums.Environment.Development == AppSettings.Instance.PlaidSettings.Environment)
                Institutions = new ObservableCollection<Institution>(AppSettings.Instance.DevelopmentInstitutions);
            else
                Institutions = new ObservableCollection<Institution>(AppSettings.Instance.SandboxInstitutions);

            Transactions = new ObservableCollection<Transaction>();

            AddNewAccountCommand = new DelegateCommand(AddNewAccountCommandHandler);
            AddAccountCommand = new DelegateCommand(AddAccountCommandHandler);
            SaveSettingsCommand = new DelegateCommand(SaveSettingsCommandHandler);
            LoadTransactionsCommand = new DelegateCommand(LoadTransactionsCommandHandler);

            StartDate = DateTime.Now.AddMonths(-1);
            EndDate = DateTime.Now;
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public string CLientId
        {
            get { return AppSettings.Instance.PlaidSettings.Client_Id; }
            set { AppSettings.Instance.PlaidSettings.Client_Id = value; }
        }

        public string PublicKey
        {
            get { return AppSettings.Instance.PlaidSettings.Public_Key; }
            set { AppSettings.Instance.PlaidSettings.Public_Key = value; }
        }

        public string SandboxSecret
        {
            get { return AppSettings.Instance.PlaidSettings.Sandbox_Secret; }
            set { AppSettings.Instance.PlaidSettings.Sandbox_Secret = value; }
        }

        public string DevelopmentSecret
        {
            get { return AppSettings.Instance.PlaidSettings.Development_Secret; }
            set { AppSettings.Instance.PlaidSettings.Development_Secret = value; }
        }

        public string SelectedEnvironment
        {
            get { return AppSettings.Instance.PlaidSettings.Environment.ToString(); }
            set
            {
                AppSettings.Instance.PlaidSettings.Environment = (Framework.Enums.Environment)Enum.Parse(typeof(Framework.Enums.Environment), value);
                EnvironmentChanged();
            }
        }



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
        public ObservableCollection<Transaction> Transactions { get; set; }
        public List<string> EnvironmentOptions => new List<string> { "Sandbox", "Development" };


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

        private void AddAccountCommandHandler()
        {
            Institution bank = PlaidService.GetInstitutionById(InstitutionID);
            bank.Credentials = PlaidService.AuthorizeInstitution(bank, PublicToken);

            if (AppSettings.Instance.PlaidSettings.Environment == Framework.Enums.Environment.Development)
            {
                AppSettings.Instance.DevelopmentInstitutions.Add(bank);
                Institutions.Add(bank);
            }
            else
            {
                AppSettings.Instance.SandboxInstitutions.Add(bank);
                Institutions.Add(bank);
            }

            AppSettings.SaveSettings();

            //InstitutionID = string.Empty;
            //PublicToken = string.Empty;
        }

        public void SaveSettingsCommandHandler()
        {
            AppSettings.SaveSettings();
        }

        private void LoadTransactionsCommandHandler(object obj)
        {
            Transactions.Clear();
            if (Framework.Enums.Environment.Development == AppSettings.Instance.PlaidSettings.Environment && AppSettings.Instance.DevelopmentInstitutions[0] != null)
                foreach (var item in PlaidService.GetTransactions(AppSettings.Instance.DevelopmentInstitutions[0], StartDate, EndDate))
                {
                    Transactions.Add(item);
                }
            else if (Framework.Enums.Environment.Sandbox == AppSettings.Instance.PlaidSettings.Environment && AppSettings.Instance.SandboxInstitutions[0] != null)
                foreach (var item in PlaidService.GetTransactions(AppSettings.Instance.SandboxInstitutions[0], StartDate, EndDate))
                {
                    Transactions.Add(item);
                }
        }

        #endregion

        #region HelperMethod

        private void EnvironmentChanged()
        {
            Institutions.Clear();

            if (Framework.Enums.Environment.Development == AppSettings.Instance.PlaidSettings.Environment)
            {
                foreach (var bank in AppSettings.Instance.DevelopmentInstitutions)
                {
                    Institutions.Add(bank);
                }
            }
            else
            {
                foreach (var bank in AppSettings.Instance.SandboxInstitutions)
                {
                    Institutions.Add(bank);
                }
            }

            AppSettings.SaveSettings();


        }

        #endregion
    }
}
