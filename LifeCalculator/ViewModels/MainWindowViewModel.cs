using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.AccountManager;
using LifeCalculator.Framework.LifeEvents;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveCharts;
using Prism.Regions;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.Chart;

//keep track of project https://kanbanflow.com/board/eFu1Zwn

namespace LifeCalculator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private IRegionManager _regionManager;

        #region Properties

        //live charts
        public Func<double, string> Formatter { get; set; }
        public SeriesCollection ValueCollection { get; set; }


        //Add Account
        private string accountType;
        public string AccountType
        {
            get
            {
                return accountType;
            }
            set
            {
                accountType = value;
                NavigateAddAccount(accountType);
            }
        }

        public List<string> AccountTypesList
        {
            get
            {
                return new List<string>() { "AddCompound", "AddLoan" };
            }
        }

        private void NavigateAddAccount(string viewName)
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("accountManager", _accountManager);

            _regionManager.RequestNavigate("AddAccountRegion", viewName, navigationParams);
        }


        //Add Event
        private string _accountSelected;
        public string AccountSelected
        {
            get => _accountSelected;
            set
            {
                _accountSelected = value;
                NavigateAddEvent("AddEventCompound");
            }
        }

        private void NavigateAddEvent(string viewName)
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("account", _accountManager.Accounts.Find(i => i.Name.Contains(AccountSelected)));

            _regionManager.RequestNavigate("AddEventRegion", viewName, navigationParams);
        }


        //Everything Else
        public ObservableCollection<IAccount> AccountsList { get; set; }
        public ObservableCollection<ILifeEvent> LifeEvents { get; set; }

        public List<ILifeEvent> _lifeEvents { get; set; }
        public IAccountManager _accountManager { get; set; }

        #endregion

        #region Constructors

        public MainWindowViewModel(IAccountManager accountManager, IRegionManager regionManager)
        {
            _accountManager = accountManager;
            _accountManager.AccountAdded += AccountAddedHandler;

            _regionManager = regionManager;

            LifeEvents = new ObservableCollection<ILifeEvent>();
            AccountsList = new ObservableCollection<IAccount>();
            _lifeEvents = new List<ILifeEvent>();

            ValueCollection = new SeriesCollection();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Life Event Added to an Account 
        /// </summary>
        private void LifeEventAddedHandler(object sender, ILifeEvent e)
        {
            addEventToList(e);
            //e.ValueChanged += ReChart;
            //if(typeof(InvestmentLifeEvent).Equals(e.GetType()))
            //    _lifeEvents.Add(new ModifyEventCompoundViewModel(e));

            //LifeEvents.Clear();

            //foreach (var item in _lifeEvents.OrderBy(i => i.Date))
            //{
            //    item.ValueChanged += ReChart;
            //    LifeEvents.Add(item);
            //}

            //ReChart(new object(), new EventArgs());
        }

        /// <summary>
        /// User Added account
        /// </summary>
        private void AccountAddedHandler(object sender, IAccount e)
        {
            AccountsList.Add(e);
            e.LifeEventAdded += LifeEventAddedHandler;
            //e.AccountLifeEvents.ForEach(i => _lifeEvents.Add(i));
            e.AccountLifeEvents.ForEach(i => addEventToList(i));

            //foreach (var item in _lifeEvents.OrderBy(i => i.Date))
            //{
            //    item.ValueChanged += ReChart;
            //    LifeEvents.Add(item);
            //}

            try
            {
                var dayConfig = Mappers.Xy<BarChartColumn>()
                .X(dayModel => dayModel.Date.Ticks / (TimeSpan.FromDays(1).Ticks * 365.2425))
                .Y(dayModel => dayModel.CurrentValue);

                //Years
                //365.2425 is the days in a solar year
                //var dayConfig = Mappers.Xy<DateModel>()
                //  .X(dateModel => dateModel.DateTime.Ticks / (TimeSpan.FromDays(1).Ticks * 365.2425))
                //  .Y(dateModel => dateModel.Value);
                ////and the formatter
                //Formatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks * 365.2425)).ToString("yyyy");

                var series = new ColumnSeries(dayConfig);
                series.Title = e.Name;
                series.Values = new ChartValues<BarChartColumn>();
                ValueCollection.Add(series);
                Formatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks * 365.2425)).ToString("yyyy");//MM/yyyy
            }
            catch (Exception)
            {
            }

            ReChart(new object(), new EventArgs());
        }

        #endregion

        #region Private Methods

        private void addEventToList(ILifeEvent lifeEvent)
        {
            lifeEvent.ValueChanged += ReChart;
            if (typeof(InvestmentLifeEvent).Equals(lifeEvent.GetType()))
                _lifeEvents.Add(new ModifyEventCompoundViewModel(lifeEvent));
            else if (typeof(MortgageLifeEvent).Equals(lifeEvent.GetType()))
                _lifeEvents.Add(new ModifyEventCompoundViewModel(lifeEvent));

            LifeEvents.Clear();

            foreach (var item in _lifeEvents.OrderBy(i => i.Date))
            {
                item.ValueChanged += ReChart;
                LifeEvents.Add(item);
            }

            ReChart(new object(), new EventArgs());
        }

        #endregion

        #region Helper Methods

        private void ReChart(object sender, EventArgs e)
        {
            foreach (var acc in _accountManager.Accounts)
                foreach (var a in ValueCollection)
                {
                    if (a.Title.Equals(acc.Name))
                    {
                        a.Values.Clear();

                        var monthlyCalculation = acc.Calculation();

                        for (int i = 0; i < monthlyCalculation.Count;i++)
                        {
                            if(i % 11 == 0)
                                a.Values.Add(new BarChartColumn() { Name = acc.Name, CurrentValue = monthlyCalculation[i].Amount, 
                                Date = monthlyCalculation[i].Date });
                        }
                        //acc.Calculation().ForEach(i => a.Values.Add(new BarChartColumn() { Name = i.Name, CurrentValue = i.Amount, Date = i.Date }));
                    }
                }
        }

        #endregion

    }
}
