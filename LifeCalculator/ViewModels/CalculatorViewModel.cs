using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using LifeCalculator.Framework.Chart;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.Services.AccDataService;
using LifeCalculator.Control.Accounts;
using LifeCalculator.Control.Events;
using LifeCalculator.Framework.Services.EventsDataService;


namespace LifeCalculator.ViewModels
{
    public class CalculatorViewModel : ViewModelBase
    {
        #region Fields

        private IAccountStore _accountStore;

        private string _accountType;
        private IModifyAccount _accountSelected;
        AccountDataService accountService;
        AccountEventDataService eventDataService;

        #endregion

        #region Properties

        //live charts
        public Func<double, string> Formatter { get; set; }
        public SeriesCollection ValueCollection { get; set; }


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

        #region Constructors

        public CalculatorViewModel(IAccountStore accountStore)
        {
            _accountStore = accountStore;

            ValueCollection = new SeriesCollection();

            AccountsList = new ObservableCollection<IModifyAccount>();

            eventDataService = new AccountEventDataService();
            accountService = new AccountDataService();

            _accountStore.CurrentAccount.SimulatedAccountManager.AccountAdded += AccountManager_AccountAdded;
            _accountStore.CurrentAccount.SimulatedAccountManager.AccountChanged += AccountManager_AccountChanged;
            _accountStore.CurrentAccount.SimulatedAccountManager.AccountDeleted += AccountManager_AccountDeleted;

            foreach (var account in _accountStore.CurrentAccount.SimulatedAccountManager.Accounts)
            {
                 addAccountToList(account);
            }

            ReChart(new object(),EventArgs.Empty);
        }

        public CalculatorViewModel()
        {

        }

        #endregion

        #region Event Handlers

        private async void AccountManager_AccountAdded(object sender, IAccount e)
        {
            try
            {
                IAccount acc = await accountService.Insert(e);
                e.Id = acc.Id;

                addAccountToList(e);
            }
            catch
            {
                addAccountToList(e);
            }

            ReChart(this,EventArgs.Empty);
        }

        private async void AccountManager_AccountChanged(object sender, IAccount e)
        {
            try
            {
                await accountService.Save(e);
            }
            catch { }
            ReChart(new object(), EventArgs.Empty);
        }

        private async void AccountManager_AccountDeleted(object sender, IAccount e)
        {
            try
            {
                await accountService.Delete(e);

                foreach (var item in ValueCollection)
                    if(item.Title.Equals(e.Name))
                        ValueCollection.Remove(item);

                foreach (var item in AccountsList)
                    if (item.Name.Equals(e.Name))
                        AccountsList.Remove(item);
            }
            catch
            {
            }

            ReChart(this, EventArgs.Empty);
        }

        public async void _currentEventViewModel_EventAddedEvent(object sendere, IAccountEvent e)
        {
            var eventInserted = await eventDataService.Insert(new AccountEvent(e));
            e.Id = e.Id;
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
                CurrentEventViewModel = new AddEventLoanViewModel(loanAccount.Account);
                CurrentEventViewModel.EventAdded += _currentEventViewModel_EventAddedEvent;
            }
            else if (accountSelected is ModifyCompoundViewModel compoundAccount)
            {
                CurrentEventViewModel = new AddEventCompoundViewModel(compoundAccount.Account);
                CurrentEventViewModel.EventAdded += _currentEventViewModel_EventAddedEvent;
            }
        }

        #endregion

        private void addAccountToList(IAccount account)
        {
            if (account is LoanAccount loanAccount)
            {
                var vm = new ModifyLoanViewModel(loanAccount,_accountStore.CurrentAccount.SimulatedAccountManager);
                AccountsList.Add(vm);
            }

            else if (account is CompoundAccount compoundAccount)
            {
                var vm = new ModifyCompoundViewModel(compoundAccount, _accountStore.CurrentAccount.SimulatedAccountManager);
                AccountsList.Add(vm);
            }

            AddChartSeries(account.Name);
        }


        // Add Chart series
        private void AddChartSeries(string seriesName)
        {
            try
            {
                var dayConfig = Mappers.Xy<BarChartColumn>()
                .X(dayModel => dayModel.Date.Ticks / (TimeSpan.FromDays(1).Ticks * 365.2425))
                .Y(dayModel => dayModel.CurrentValue);

                var series = new ColumnSeries(dayConfig);
                series.Title = seriesName;
                series.Values = new ChartValues<BarChartColumn>();
                ValueCollection.Add(series);
                Formatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks * 365.2425)).ToString("yyyy");//MM/yyyy
            }
            catch (Exception)
            {
            }

        }

        ///// <summary>
        /// Method Recomputes the Chart
        /// </summary>
        /// <remarks>
        /// Use Method to Re-Display the chart whenever modifications are made or Events
        /// are added
        /// </remarks>
        private void ReChart(object sender, EventArgs e)
        {
            foreach (var acc in _accountStore.CurrentAccount.SimulatedAccountManager.Accounts)
                foreach (var collection in ValueCollection)
                {
                    if (collection.Title.Equals(acc.Name))
                    {
                        collection.Values.Clear();

                        var monthlyCalculation = acc.Calculation();

                        for (int i = 0; i < monthlyCalculation.Count; i++)
                        {
                            if (i % 12 == 0 && i != 0)
                            {
                                collection.Values.Add(new BarChartColumn()
                                {
                                    Name = acc.Name,
                                    CurrentValue = monthlyCalculation[i].Gain,
                                    Date = monthlyCalculation[i].Date
                                });
                            }
                        }
                    }
                }
            
        }

        #endregion
    }
}
