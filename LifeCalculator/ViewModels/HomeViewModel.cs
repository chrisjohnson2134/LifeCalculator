using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using LifeCalculator.Framework.Chart;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using System.Collections.Specialized;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.Services.AccDataService;
using LifeCalculator.Control.Accounts;
using System.Linq;
using LifeCalculator.Control.Events;
using LifeCalculator.Control.Events.Loan.ViewModels;
using LifeCalculator.Framework.Services.EventsDataService;

namespace LifeCalculator.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        #region Fields

        private IAccountStore _accountStore;

        private string _accountType;
        private IAccount _accountSelected;
        private List<IAccountEvent> _accountEvents;
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
        public IAccount AccountSelected
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
        public ObservableCollection<IAccount> AccountsList { get; set; }
        public ObservableCollection<IAccountEvent> LifeEvents { get; set; }

        #endregion

        #region Constructors

        public HomeViewModel(IAccountStore accountStore)
        {
            _accountStore = accountStore;

            ValueCollection = new SeriesCollection();

            _accountEvents = new List<IAccountEvent>();
            LifeEvents = new ObservableCollection<IAccountEvent>();
            AccountsList = new ObservableCollection<IAccount>();
            AccountsList.CollectionChanged += AccountsList_CollectionChanged;

            eventDataService = new AccountEventDataService();
            accountService = new AccountDataService();

            CurrentViewModel = new AddCompoundViewModel(accountStore);
            CurrentViewModel.AccountAdded += CurrentViewModel_AccountAdded;
            //_accountEvents = new List<IAccountEvent>();

            foreach (var account in _accountStore.CurrentAccount.Accounts)
            {
                 addAccountToList(account);
            }

            ReChart(new object(),EventArgs.Empty);
        }

        #endregion

        #region Event Handlers

        private async void CurrentViewModel_AccountAdded(object sender, IAccount e)
        {
            IAccount acc = await accountService.Insert(e);

            addAccountToList(acc);

            ReChart(this,EventArgs.Empty);
        }

        public void _currentEventViewModel_EventAddedEvent(object sendere, IAccountEvent e)
        {
            var eventInserted = eventDataService.Insert(new AccountEvent(e));
            addEventToList(e);
        }

        private void AccountsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ReChart(new object(), new EventArgs());
        }

        #endregion

        #region Private Methods


        private void NavigateAddAccount(string account)
        {
            if(account.Equals("Add Compound"))
            {
                CurrentViewModel = new AddCompoundViewModel(_accountStore);
            }
            else if(account.Equals("Add Loan"))
            {
                CurrentViewModel = new AddLoanViewModel(_accountStore);
            }
            CurrentViewModel.AccountAdded += CurrentViewModel_AccountAdded;
        }


        private void NavigateAddEvent(IAccount accountSelected)
        {
            if (accountSelected is LoanAccount loanAccount)
            {
                CurrentEventViewModel = new AddEventLoanViewModel(loanAccount);
            }

            CurrentEventViewModel.EventAdded += _currentEventViewModel_EventAddedEvent;
        }

        /// <summary>
        /// Add LifeEvent to the LifeEvents List
        /// </summary>
        /// <remarks>
        /// Uses the _lifeEvents List<> to sort the events and then add it 
        /// to the Observable collection.
        /// </remarks>
        /// <param name = "lifeEvent" ></ param >
        private void addEventToList(IAccountEvent lifeEvent)
        {
            lifeEvent.ValueChanged += ReChart;
            if (typeof(AccountEvent).Equals(lifeEvent.GetType()))
                _accountEvents.Add(new ModifyEventCompoundViewModel(lifeEvent));

            LifeEvents.Clear();

            foreach (var item in _accountEvents.OrderBy(i => i.StartDate))
            {
                item.ValueChanged += ReChart;
                LifeEvents.Add(item);
            }

            ReChart(new object(), new EventArgs());
        }

        private void addAccountToList(IAccount account)
        {
            if (account is LoanAccount loanAccount)
            {
                var vm = new ModifyLoanViewModel(loanAccount);
                vm.ValueChanged += ReChart;
                AccountsList.Add(vm);
            }

            if (account is CompoundAccount compoundAccount)
            {
                var vm = new ModifyCompoundViewModel(compoundAccount);
                vm.ValueChanged += ReChart;
                AccountsList.Add(vm);
            }


            AddChartSeries(account.Name);
            account.ValueChanged += ReChart;
            foreach (var item in account.AccountLifeEvents)
            {
                //LifeEvents.Add(item);
                addEventToList(item);
            }
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
            foreach (var acc in _accountStore.CurrentAccount.Accounts)
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
