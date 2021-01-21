using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Managers.Interfaces;
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
using LifeCalculator.Control.Events.Loan.ViewModels;

//keep track of project https://kanbanflow.com/board/eFu1Zwn

namespace LifeCalculator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Fields

        private IRegionManager _regionManager;
        private IAccountManager _accountManager;

        private string _accountType;
        private string _accountSelected;
        private List<ILifeEvent> _lifeEvents;
        
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

        public List<string> AccountTypesList
        {
            get
            {
                return new List<string>() { "Add Compound", "Add Loan" };
            }
        }

        //Add Event
        public string AccountSelected
        {
            get => _accountSelected;
            set
            {
                _accountSelected = value;
                NavigateAddEvent("AddEventCompound");
            }
        }

        //Everything Else
        public ObservableCollection<IAccount> AccountsList { get; set; }
        public ObservableCollection<ILifeEvent> LifeEvents { get; set; }

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
        }

        /// <summary>
        /// User Added account
        /// </summary>
        /// <remarks>
        /// Also sets up a chart for the chart.
        /// </remarks>
        private void AccountAddedHandler(object sender, IAccount e)
        {
            AccountsList.Add(e);
            e.LifeEventAdded += LifeEventAddedHandler;
            e.AccountLifeEvents.ForEach(i => addEventToList(i));

            try
            {
                var dayConfig = Mappers.Xy<BarChartColumn>()
                .X(dayModel => dayModel.Date.Ticks / (TimeSpan.FromDays(1).Ticks * 365.2425))
                .Y(dayModel => dayModel.CurrentValue);

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

        /// <summary>
        /// Add LifeEvent to the LifeEvents List
        /// </summary>
        /// <remarks>
        /// Uses the _lifeEvents List<> to sort the events and then add it 
        /// to the Observable collection.
        /// </remarks>
        /// <param name="lifeEvent"></param>
        private void addEventToList(ILifeEvent lifeEvent)
        {
            lifeEvent.ValueChanged += ReChart;
            if (typeof(InvestmentLifeEvent).Equals(lifeEvent.GetType()))
                _lifeEvents.Add(new ModifyEventCompoundViewModel(lifeEvent));
            else if (typeof(MortgageLifeEvent).Equals(lifeEvent.GetType()))
                _lifeEvents.Add(new ModifyEventLoanViewModel(lifeEvent));

            LifeEvents.Clear();

            foreach (var item in _lifeEvents.OrderBy(i => i.Date))
            {
                item.ValueChanged += ReChart;
                LifeEvents.Add(item);
            }

            ReChart(new object(), new EventArgs());
        }

        /// <summary>
        /// Navigate the Account Region to specified region
        /// </summary>
        /// <param name="viewName"></param>
        private void NavigateAddAccount(string viewName)
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("accountManager", _accountManager);

            _regionManager.RequestNavigate("AddAccountRegion", viewName.Replace(" ", ""), navigationParams);
        }

        /// <summary>
        /// Navigate the Event Region to specified region
        /// </summary>
        /// <param name="viewName"></param>
        private void NavigateAddEvent(string viewName)
        {
            var navigationParams = new NavigationParameters();

            var accountSelected = _accountManager.Accounts.Find(i => i.Name.Contains(AccountSelected));

            navigationParams.Add("account", accountSelected);

            if (typeof(LoanAccount).Equals(accountSelected.GetType()))
                _regionManager.RequestNavigate("AddEventRegion", "AddEventLoan", navigationParams);
            else if (typeof(CompoundAccount).Equals(accountSelected.GetType()))
                _regionManager.RequestNavigate("AddEventRegion", "AddEventCompound", navigationParams);

        }
        
        /// <summary>
        /// Method Recomputes the Chart
        /// </summary>
        /// <remarks>
        /// Use Method to Re-Display the chart whenever modifications are made or Events
        /// are added
        /// </remarks>
        private void ReChart(object sender, EventArgs e)
        {
            foreach (var acc in _accountManager.Accounts)
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
