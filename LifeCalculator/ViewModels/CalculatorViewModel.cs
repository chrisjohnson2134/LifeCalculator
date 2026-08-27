using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.Services.AccDataService;
using LifeCalculator.Control.Accounts;
using LifeCalculator.Control.Events;
using LifeCalculator.Framework.Services.EventsDataService;
using LifeCalculator.Framework.Managers;

namespace LifeCalculator.ViewModels
{
    public class CalculatorViewModel : ViewModelBase
    {
        #region Fields

        private IAccountStore _accountStore;
        private IAccountsEventsManager _accountsEventsManager;

        private string _accountType;
        private IModifyAccount _accountSelected;

        #endregion

        #region Constructors

        public CalculatorViewModel(IAccountStore accountStore)
        {
            _accountStore = accountStore;
            _accountStore.CurrentAccount.SimulatedAccountManager.AccountAdded += AccountManager_AccountAdded;
            _accountStore.CurrentAccount.SimulatedAccountManager.AccountChanged += AccountManager_AccountChanged;
            _accountStore.CurrentAccount.SimulatedAccountManager.AccountDeleted += AccountManager_AccountDeleted;

            _accountsEventsManager = _accountStore.CurrentAccount.AccountsEventsManager;
            _accountsEventsManager.AccountEventChanged += AccountsEventsManager_EventChanged;

            ValueCollection = new SeriesCollection();

            AccountsList = new ObservableCollection<IModifyAccount>();

            foreach (var account in _accountStore.CurrentAccount.SimulatedAccountManager.GetAllAccounts())
            {
                addAccountToList(account);
            }

            ReChart(new object(), EventArgs.Empty);
        }

        
        #endregion

        #region Properties

        //live charts
        public SeriesCollection ValueCollection { get; set; }

        // Real calendar years used as the (categorical) X axis labels.
        private string[] _labels = new string[0];
        public string[] Labels
        {
            get => _labels;
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }

        // Currency formatter for the Y axis.
        public Func<double, string> YFormatter { get; set; } = value => value.ToString("C0");

        // Running counter so each account keeps a stable palette color.
        private int _colorCounter = 0;


        //Add Account
        public string AccountType
        {
            get => _accountType;
            set
            {
                _accountType = value;
                NavigateAddAccount(_accountType);
            }
        }

        public ObservableCollection<string> AccountTypesList
        {
            get
            {
                return new ObservableCollection<string>() { "Add Compound", "Add Loan" };
            }
        }

        //Add Event
        public IModifyAccount AccountSelected
        {
            get => _accountSelected;
            set
            {
                _accountSelected = value;
                NavigateAddEvent(_accountSelected);
            }
        }

        private IControlAccount _currentViewModel;
        public IControlAccount CurrentViewModel
        { 
            get{ return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged("CurrentViewModel");
            }
        }

        private IControlEvent _currentEventViewModel;
        public IControlEvent CurrentEventViewModel
        {
            get { return _currentEventViewModel; }
            set
            {
                _currentEventViewModel = value;
                OnPropertyChanged("CurrentEventViewModel");
            }
        }

        //Everything Else
        public ObservableCollection<IModifyAccount> AccountsList { get; set; }

        #endregion

        #region Event Handlers

        private async void AccountManager_AccountAdded(object sender, IAccount e)
        {
            addAccountToList(e);

            ReChart(this,EventArgs.Empty);
        }

        private void AccountManager_AccountChanged(object sender, IAccount e)
        {
            ReChart(new object(), EventArgs.Empty);
        }

        private void AccountsEventsManager_EventChanged(object sender, IAccountEvent e)
        {
            ReChart(new object(), EventArgs.Empty);
        }

        private void AccountManager_AccountDeleted(object sender, IAccount e)
        {
            var seriesToRemove = ValueCollection.FirstOrDefault(s => s.Title.Equals(e.Name));
            if (seriesToRemove != null)
                ValueCollection.Remove(seriesToRemove);

            var itemVm = AccountsList.FirstOrDefault(a => a.Name.Equals(e.Name));
            if (itemVm != null)
                AccountsList.Remove(itemVm);

            ReChart(this, EventArgs.Empty);
        }

        #endregion

        #region Private Methods

        #region UI Command Handlers

        private void NavigateAddAccount(string account)
        {
            if (account.Equals("Add Compound"))
            {
                CurrentViewModel = new AddCompoundViewModel(_accountStore);
            }
            else if (account.Equals("Add Loan"))
            {
                CurrentViewModel = new AddLoanViewModel(_accountStore);
            }
        }


        private void NavigateAddEvent(IModifyAccount accountSelected)
        {
            if (accountSelected is ModifyLoanViewModel loanAccount)
            {
                CurrentEventViewModel = new AddEventViewModel(loanAccount.Account);
            }
            else if (accountSelected is ModifyCompoundViewModel compoundAccount)
            {
                CurrentEventViewModel = new AddEventViewModel(compoundAccount.Account);
            }
        }

        #endregion

        private void addAccountToList(IAccount account)
        {
            var brush = AccountColorPalette.BrushAt(_colorCounter++);

            IModifyAccount vm = null;
            if (account is LoanAccount loanAccount)
            {
                loanAccount.SetEventsManager(_accountsEventsManager);
                vm = new ModifyLoanViewModel(loanAccount, _accountStore.CurrentAccount.SimulatedAccountManager);
            }
            else if (account is CompoundAccount compoundAccount)
            {
                compoundAccount.SetEventsManager(_accountsEventsManager);
                vm = new ModifyCompoundViewModel(compoundAccount, _accountStore.CurrentAccount.SimulatedAccountManager);
            }

            if (vm != null)
            {
                vm.SeriesColor = brush;
                AccountsList.Add(vm);
            }

            AddChartSeries(account.Name, brush);
        }


        // Add a grouped column series for one account, colored to match its list entry.
        private void AddChartSeries(string seriesName, Brush brush)
        {
            try
            {
                var series = new ColumnSeries
                {
                    Title = seriesName,
                    Values = new ChartValues<double>(),
                    Fill = brush,
                    Stroke = brush,
                    StrokeThickness = 0,
                    MaxColumnWidth = 28,
                    LabelPoint = point => string.Format("{0:C0}", point.Y)
                };

                ValueCollection.Add(series);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Recomputes the chart.
        /// </summary>
        /// <remarks>
        /// All accounts share one categorical X axis of calendar years, so columns are
        /// grouped side-by-side (no overlap) and labels show the real year. Years an
        /// account doesn't span are filled with NaN so nothing is drawn for them.
        /// </remarks>
        private void ReChart(object sender, EventArgs e)
        {
            var accounts = _accountStore.CurrentAccount.SimulatedAccountManager.GetAllAccounts().ToList();

            // Build the shared timeline (union of every account's years) and a
            // year -> value lookup per account (last month of the year wins = year-end).
            var years = new SortedSet<int>();
            var valuesByAccount = new Dictionary<string, Dictionary<int, double>>();

            foreach (var acc in accounts)
            {
                var monthlyCalculation = (acc as ISimulatedAccount).Calculation();
                var yearly = new Dictionary<int, double>();

                foreach (var month in monthlyCalculation)
                {
                    yearly[month.Date.Year] = month.Gain;
                    years.Add(month.Date.Year);
                }

                valuesByAccount[acc.Name] = yearly;
            }

            var yearList = years.ToList();
            Labels = yearList.Select(y => y.ToString()).ToArray();

            foreach (var series in ValueCollection)
            {
                Dictionary<int, double> yearly;
                valuesByAccount.TryGetValue(series.Title, out yearly);

                var values = new ChartValues<double>();
                foreach (var year in yearList)
                    values.Add(yearly != null && yearly.ContainsKey(year) ? yearly[year] : double.NaN);

                series.Values = values;
            }
        }

        #endregion
    }
}
