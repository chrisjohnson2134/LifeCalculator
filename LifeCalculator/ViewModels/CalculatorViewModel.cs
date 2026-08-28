using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Control.Accounts;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Income;
using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Simulation;
using LifeCalculator.Framework.Services.FinancialAccountService;

namespace LifeCalculator.ViewModels
{
    public enum ChartView
    {
        [System.ComponentModel.Description("Debt payoff")]
        DebtPayoff,
        [System.ComponentModel.Description("Account growth")]
        AccountGrowth,
        [System.ComponentModel.Description("Net worth")]
        NetWorth
    }

    public class CalculatorViewModel : ViewModelBase
    {
        #region Fields

        private readonly IAccountStore _accountStore;
        private readonly IAccountsEventsManager _accountsEventsManager;
        private readonly IFinancialAccountDataService _financialAccountService;

        private int _colorCounter = 0;

        #endregion

        #region Constructors

        public CalculatorViewModel(IAccountStore accountStore, IFinancialAccountDataService financialAccountService)
        {
            _accountStore = accountStore;
            _financialAccountService = financialAccountService;

            _accountStore.CurrentAccount.SimulatedAccountManager.AccountAdded += AccountManager_AccountAdded;
            _accountStore.CurrentAccount.SimulatedAccountManager.AccountChanged += AccountManager_AccountChanged;
            _accountStore.CurrentAccount.SimulatedAccountManager.AccountDeleted += AccountManager_AccountDeleted;

            _accountsEventsManager = _accountStore.CurrentAccount.AccountsEventsManager;
            _accountsEventsManager.AccountEventChanged += AccountsEventsManager_EventChanged;
            _accountsEventsManager.AccountEventDeleted += AccountsEventsManager_EventChanged;

            _accountStore.CurrentAccount.IncomeStreamManager.IncomeStreamAdded += IncomeStreamManager_Changed;
            _accountStore.CurrentAccount.IncomeStreamManager.IncomeStreamChanged += IncomeStreamManager_Changed;
            _accountStore.CurrentAccount.IncomeStreamManager.IncomeStreamDeleted += IncomeStreamManager_Changed;

            Debts = new ObservableCollection<ModifyLoanViewModel>();
            Investments = new ObservableCollection<ModifyCompoundViewModel>();
            RetirementAccounts = new ObservableCollection<ModifyRetirementViewModel>();

            DebtSeriesCollection = new ObservableCollection<ISeries>();
            GrowthSeriesCollection = new ObservableCollection<ISeries>();
            NetWorthSeriesCollection = new ObservableCollection<ISeries>();

            ToggleAddDebtCommand = new RelayCommand(ToggleAddDebt);
            ToggleAddInvestmentCommand = new RelayCommand(ToggleAddInvestment);
            ToggleAddRetirementCommand = new RelayCommand(ToggleAddRetirement);

            EmergencyFund = new EmergencyFundViewModel(
                _accountStore,
                _accountStore.CurrentAccount.SimulatedAccountManager,
                _accountsEventsManager,
                _accountStore.CurrentAccount.ExpenseManager);

            // The fund is an asset and its contribution competes with everything else for the
            // monthly surplus, so a change there has to re-run the whole projection.
            EmergencyFund.GoalChanged += (s, e) => Recalculate();

            // This view model is a singleton, so it is not rebuilt when the user navigates back
            // from the Budget screen. Without these the months-of-expenses figures and the
            // 3/6/12-month goal presets would keep showing the expense total from whenever the
            // page was first opened.
            var expenseManager = _accountStore.CurrentAccount.ExpenseManager;
            if (expenseManager != null)
            {
                expenseManager.ExpenseAdded += ExpenseManager_Changed;
                expenseManager.ExpenseChanged += ExpenseManager_Changed;
                expenseManager.ExpenseDeleted += ExpenseManager_Changed;
            }

            _payoffStrategy = _accountStore.CurrentAccount.PayoffStrategy;
            _selectedChartView = ChartView.NetWorth;

            foreach (var account in _accountStore.CurrentAccount.SimulatedAccountManager.GetAllAccounts())
            {
                addAccountToList(account);
            }

            Recalculate();
        }

        #endregion

        #region Properties

        public ObservableCollection<ModifyLoanViewModel> Debts { get; }
        public ObservableCollection<ModifyCompoundViewModel> Investments { get; }
        public ObservableCollection<ModifyRetirementViewModel> RetirementAccounts { get; }

        /// <summary>Single fund — see EmergencyFundViewModel for why it isn't a collection.</summary>
        public EmergencyFundViewModel EmergencyFund { get; }

        // Charts
        public ObservableCollection<ISeries> DebtSeriesCollection { get; }
        public ObservableCollection<ISeries> GrowthSeriesCollection { get; }
        public ObservableCollection<ISeries> NetWorthSeriesCollection { get; }

        // Each chart gets its OWN axis instances. LiveCharts axes are stateful and bound to a
        // single chart; sharing one instance across the three CartesianCharts corrupts their
        // rendering.
        public Axis[] DebtDateAxes { get; } = { BuildDateAxis() };
        public Axis[] DebtCurrencyAxes { get; } = { BuildCurrencyAxis() };

        public Axis[] GrowthDateAxes { get; } = { BuildDateAxis() };
        public Axis[] GrowthCurrencyAxes { get; } = { BuildCurrencyAxis() };

        public Axis[] NetWorthDateAxes { get; } = { BuildDateAxis() };
        public Axis[] NetWorthCurrencyAxes { get; } = { BuildCurrencyAxis() };

        private static Axis BuildDateAxis()
        {
            return new Axis
            {
                // Four-digit year: projections routinely run decades out, and "65" is
                // ambiguous between 1965 and 2065.
                Labeler = value => TicksToDate(value).ToString("MMM yyyy"),
                UnitWidth = TimeSpan.FromDays(30).Ticks,
                MinStep = TimeSpan.FromDays(30).Ticks,
                LabelsRotation = 15
            };
        }

        private static Axis BuildCurrencyAxis()
        {
            return new Axis { Labeler = value => value.ToString("C0") };
        }

        /// <summary>Axis separators can land outside DateTime's range while panning/zooming.</summary>
        private static DateTime TicksToDate(double value)
        {
            if (value < DateTime.MinValue.Ticks)
                return DateTime.MinValue;

            if (value > DateTime.MaxValue.Ticks)
                return DateTime.MaxValue;

            return new DateTime((long)value);
        }

        private ChartView _selectedChartView;
        public ChartView SelectedChartView
        {
            get => _selectedChartView;
            set
            {
                _selectedChartView = value;
                OnPropertyChanged(nameof(SelectedChartView));
                OnPropertyChanged(nameof(IsDebtChartVisible));
                OnPropertyChanged(nameof(IsGrowthChartVisible));
                OnPropertyChanged(nameof(IsNetWorthChartVisible));
            }
        }

        public bool IsDebtChartVisible => SelectedChartView == ChartView.DebtPayoff;
        public bool IsGrowthChartVisible => SelectedChartView == ChartView.AccountGrowth;
        public bool IsNetWorthChartVisible => SelectedChartView == ChartView.NetWorth;

        public List<ChartView> ChartViews { get; } = Enum.GetValues(typeof(ChartView)).Cast<ChartView>().ToList();

        private DebtPayoffStrategy _payoffStrategy;
        public DebtPayoffStrategy PayoffStrategy
        {
            get => _payoffStrategy;
            set
            {
                _payoffStrategy = value;
                _accountStore.CurrentAccount.PayoffStrategy = value;
                _financialAccountService.Save(_accountStore.CurrentAccount.Id, _accountStore.CurrentAccount);
                OnPropertyChanged(nameof(PayoffStrategy));
                Recalculate();
            }
        }

        public List<DebtPayoffStrategy> PayoffStrategies { get; } =
            Enum.GetValues(typeof(DebtPayoffStrategy)).Cast<DebtPayoffStrategy>().ToList();

        // Summary cards

        private double _totalDebt;
        public double TotalDebt
        {
            get => _totalDebt;
            private set { _totalDebt = value; OnPropertyChanged(nameof(TotalDebt)); }
        }

        private double _totalAssets;
        public double TotalAssets
        {
            get => _totalAssets;
            private set { _totalAssets = value; OnPropertyChanged(nameof(TotalAssets)); }
        }

        private double _netWorth;
        public double NetWorth
        {
            get => _netWorth;
            private set { _netWorth = value; OnPropertyChanged(nameof(NetWorth)); }
        }

        private string _debtFreeDateText = "No debts";
        public string DebtFreeDateText
        {
            get => _debtFreeDateText;
            private set { _debtFreeDateText = value; OnPropertyChanged(nameof(DebtFreeDateText)); }
        }

        private double _monthlySurplus;
        public double MonthlySurplus
        {
            get => _monthlySurplus;
            private set
            {
                _monthlySurplus = value;
                OnPropertyChanged(nameof(MonthlySurplus));
                OnPropertyChanged(nameof(IsSurplusNegative));
            }
        }

        public bool IsSurplusNegative => MonthlySurplus < 0;

        // Inline "add" panels — toggled open/closed instead of opening a separate dialog window.

        private bool _isAddDebtOpen;
        public bool IsAddDebtOpen
        {
            get => _isAddDebtOpen;
            private set { _isAddDebtOpen = value; OnPropertyChanged(nameof(IsAddDebtOpen)); }
        }

        private bool _isAddInvestmentOpen;
        public bool IsAddInvestmentOpen
        {
            get => _isAddInvestmentOpen;
            private set { _isAddInvestmentOpen = value; OnPropertyChanged(nameof(IsAddInvestmentOpen)); }
        }

        private bool _isAddRetirementOpen;
        public bool IsAddRetirementOpen
        {
            get => _isAddRetirementOpen;
            private set { _isAddRetirementOpen = value; OnPropertyChanged(nameof(IsAddRetirementOpen)); }
        }

        private AddLoanViewModel _addDebtViewModel;
        public AddLoanViewModel AddDebtViewModel
        {
            get => _addDebtViewModel;
            private set { _addDebtViewModel = value; OnPropertyChanged(nameof(AddDebtViewModel)); }
        }

        private AddCompoundViewModel _addInvestmentViewModel;
        public AddCompoundViewModel AddInvestmentViewModel
        {
            get => _addInvestmentViewModel;
            private set { _addInvestmentViewModel = value; OnPropertyChanged(nameof(AddInvestmentViewModel)); }
        }

        private AddRetirementViewModel _addRetirementViewModel;
        public AddRetirementViewModel AddRetirementViewModel
        {
            get => _addRetirementViewModel;
            private set { _addRetirementViewModel = value; OnPropertyChanged(nameof(AddRetirementViewModel)); }
        }

        public IRelayCommand ToggleAddDebtCommand { get; }
        public IRelayCommand ToggleAddInvestmentCommand { get; }
        public IRelayCommand ToggleAddRetirementCommand { get; }

        #endregion

        #region Command Handlers

        private void ToggleAddDebt()
        {
            IsAddDebtOpen = !IsAddDebtOpen;

            if (!IsAddDebtOpen)
                return;

            var vm = new AddLoanViewModel(_accountStore);
            vm.AccountAdded += (s, e) => IsAddDebtOpen = false;
            AddDebtViewModel = vm;
        }

        private void ToggleAddInvestment()
        {
            IsAddInvestmentOpen = !IsAddInvestmentOpen;

            if (!IsAddInvestmentOpen)
                return;

            var vm = new AddCompoundViewModel(_accountStore);
            vm.AccountAdded += (s, e) => IsAddInvestmentOpen = false;
            AddInvestmentViewModel = vm;
        }

        private void ToggleAddRetirement()
        {
            IsAddRetirementOpen = !IsAddRetirementOpen;

            if (!IsAddRetirementOpen)
                return;

            var vm = new AddRetirementViewModel(_accountStore);
            vm.AccountAdded += (s, e) => IsAddRetirementOpen = false;
            AddRetirementViewModel = vm;
        }

        #endregion

        #region Event Handlers

        private void AccountManager_AccountAdded(object sender, IAccount e)
        {
            addAccountToList(e);
            Recalculate();
        }

        private void AccountManager_AccountChanged(object sender, IAccount e)
        {
            Recalculate();
        }

        private void AccountsEventsManager_EventChanged(object sender, IAccountEvent e)
        {
            Recalculate();
        }

        /// <summary>
        /// Income streams are edited on the Financial Profile screen, but they feed this
        /// screen's monthly-surplus card — so recompute whenever they change.
        /// </summary>
        private void IncomeStreamManager_Changed(object sender, IncomeStream e)
        {
            Recalculate();
        }

        /// <summary>
        /// Editing the Budget moves both the monthly surplus and the emergency fund's
        /// months-of-expenses figures, so refresh the fund panel as well as the projection.
        /// </summary>
        private void ExpenseManager_Changed(object sender, LifeCalculator.Framework.Budget.ExpenseItem e)
        {
            EmergencyFund?.RefreshFromBudget();
            Recalculate();
        }

        private void AccountManager_AccountDeleted(object sender, IAccount e)
        {
            var debt = Debts.FirstOrDefault(a => a.Name.Equals(e.Name));
            if (debt != null) Debts.Remove(debt);

            var investment = Investments.FirstOrDefault(a => a.Name.Equals(e.Name));
            if (investment != null) Investments.Remove(investment);

            var retirement = RetirementAccounts.FirstOrDefault(a => a.Name.Equals(e.Name));
            if (retirement != null) RetirementAccounts.Remove(retirement);

            Recalculate();
        }

        #endregion

        #region Private Methods

        private void addAccountToList(IAccount account)
        {
            // The emergency fund has its own section rather than a row in one of the lists, so
            // it only needs its events manager wired. Handled before the colour is taken so it
            // doesn't consume a series colour the account lists are going to use.
            if (account is EmergencyFundAccount emergencyFund)
            {
                emergencyFund.SetEventsManager(_accountsEventsManager);
                return;
            }

            var brush = AccountColorPalette.BrushAt(_colorCounter++);

            if (account is LoanAccount loanAccount)
            {
                loanAccount.SetEventsManager(_accountsEventsManager);
                var vm = new ModifyLoanViewModel(loanAccount, _accountStore.CurrentAccount.SimulatedAccountManager, _accountsEventsManager) { SeriesColor = brush };
                Debts.Add(vm);
            }
            else if (account is CompoundAccount compoundAccount)
            {
                compoundAccount.SetEventsManager(_accountsEventsManager);
                var vm = new ModifyCompoundViewModel(compoundAccount, _accountStore.CurrentAccount.SimulatedAccountManager, _accountsEventsManager) { SeriesColor = brush };
                Investments.Add(vm);
            }
            else if (account is RetirementAccount retirementAccount)
            {
                retirementAccount.SetEventsManager(_accountsEventsManager);
                var vm = new ModifyRetirementViewModel(retirementAccount, _accountStore.CurrentAccount.SimulatedAccountManager, _accountStore.CurrentAccount.IncomeStreamManager, _accountsEventsManager) { SeriesColor = brush };
                RetirementAccounts.Add(vm);
            }
        }

        /// <summary>
        /// Recomputes every projection (debt payoff, account growth, net worth, cash-flow
        /// surplus) and refreshes the three charts plus the summary cards.
        /// </summary>
        private void Recalculate()
        {
            var debtAccounts = Debts.Select(vm => vm.Account).ToList();
            var debtResult = DebtPayoffSimulator.Simulate(debtAccounts, PayoffStrategy);


            var assetTimelines = new Dictionary<string, List<MonthlyColumn>>();
            var assetColors = new Dictionary<string, Brush>();

            foreach (var investment in Investments)
            {
                assetTimelines[investment.Name] = investment.Account.Calculation();
                assetColors[investment.Name] = investment.SeriesColor;
            }

            foreach (var retirement in RetirementAccounts)
            {
                assetTimelines[retirement.Name] = retirement.Account.Calculation(ResolveMonthlySalary(retirement.Account));
                assetColors[retirement.Name] = retirement.SeriesColor;
            }

            // Cash in an emergency fund is still net worth, so it belongs on the growth and
            // net-worth charts alongside every other asset.
            var emergencyFundAccount = EmergencyFund?.Fund;
            if (emergencyFundAccount != null)
            {
                assetTimelines[emergencyFundAccount.Name] = emergencyFundAccount.Calculation();
                assetColors[emergencyFundAccount.Name] = AccountColorPalette.BrushAt(_colorCounter);
            }

            RebuildDebtChart(debtResult);
            RebuildGrowthChart(assetTimelines, assetColors);

            var netWorthTimeline = NetWorthAggregator.Aggregate(debtResult, assetTimelines);
            RebuildNetWorthChart(netWorthTimeline);

            UpdateSummaryCards(debtResult, netWorthTimeline);

            var growthAccountsForCashFlow = Investments.Select(vm => (ISimulatedAccount)vm.Account)
                .Concat(RetirementAccounts.Select(vm => (ISimulatedAccount)vm.Account))
                .ToList();

            if (emergencyFundAccount != null)
                growthAccountsForCashFlow.Add(emergencyFundAccount);

            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var cashFlow = CashFlowSimulator.Calculate(_accountStore.CurrentAccount, currentMonth, currentMonth, debtResult, growthAccountsForCashFlow);
            MonthlySurplus = cashFlow.Count > 0 ? cashFlow[0].Surplus : 0;
        }

        /// <summary>
        /// Employer match caps are defined against GROSS pay ("up to 6% of salary"), so this
        /// uses the linked stream's optional gross salary. If that's blank we fall back to the
        /// stream's take-home, which understates the cap slightly but is far better than
        /// treating the match as zero.
        /// </summary>
        private double ResolveMonthlySalary(RetirementAccount account)
        {
            var linked = _accountStore.CurrentAccount.IncomeStreamManager
                .GetAllIncomeStreams()
                .FirstOrDefault(s => s.Id == account.LinkedIncomeStreamId);

            if (linked != null)
            {
                return linked.GrossAnnualSalary > 0
                    ? linked.GrossAnnualSalary / 12
                    : linked.MonthlyAmount;
            }

            return _accountStore.CurrentAccount.Salary / 12;
        }

        private void RebuildDebtChart(DebtPayoffResult debtResult)
        {
            DebtSeriesCollection.Clear();

            foreach (var debt in Debts)
            {
                if (!debtResult.BalancesByDebtName.TryGetValue(debt.Name, out var series))
                    continue;

                DebtSeriesCollection.Add(BuildLineSeries(debt.Name, series, debt.SeriesColor));
            }
        }

        private void RebuildGrowthChart(Dictionary<string, List<MonthlyColumn>> assetTimelines, Dictionary<string, Brush> assetColors)
        {
            GrowthSeriesCollection.Clear();

            foreach (var kvp in assetTimelines)
            {
                var brush = assetColors.TryGetValue(kvp.Key, out var b) ? b : Brushes.Gray;
                GrowthSeriesCollection.Add(BuildLineSeries(kvp.Key, kvp.Value, brush));
            }
        }

        private void RebuildNetWorthChart(List<NetWorthColumn> netWorthTimeline)
        {
            NetWorthSeriesCollection.Clear();

            NetWorthSeriesCollection.Add(new LineSeries<DateTimePoint>
            {
                Name = "Net Worth",
                Values = netWorthTimeline.Select(c => new DateTimePoint(c.Date, c.NetWorth)).ToArray(),
                Stroke = new SolidColorPaint(SKColors.MediumSeaGreen, 3),
                Fill = null,
                GeometrySize = 0
            });
        }

        private void UpdateSummaryCards(DebtPayoffResult debtResult, List<NetWorthColumn> netWorthTimeline)
        {
            // Match on the current month: the timeline is bucketed monthly, so comparing exact
            // timestamps would arbitrarily favour whichever row happens to sit nearest "now".
            DateTime currentMonthBucket = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var closest = netWorthTimeline.FirstOrDefault(c => c.Date == currentMonthBucket)
                ?? netWorthTimeline
                    .OrderBy(c => Math.Abs((c.Date - currentMonthBucket).Ticks))
                    .FirstOrDefault();

            TotalDebt = closest?.TotalDebt ?? 0;
            TotalAssets = closest?.TotalAssets ?? 0;
            NetWorth = closest?.NetWorth ?? 0;

            if (Debts.Count == 0)
            {
                DebtFreeDateText = "No debts";
            }
            else if (debtResult.PayoffDateByDebtName.Values.Any(d => d == null))
            {
                DebtFreeDateText = "Not on track to pay off";
            }
            else
            {
                DateTime debtFreeDate = debtResult.PayoffDateByDebtName.Values.Max().Value;
                DebtFreeDateText = debtFreeDate.ToString("MMMM yyyy");
            }
        }

        private static ISeries BuildLineSeries(string name, List<MonthlyColumn> monthly, Brush brush)
        {
            return new LineSeries<DateTimePoint>
            {
                Name = name,
                // Skip the placeholder row each Calculation() prepends (see MonthlyColumn.IsPlaceholder) —
                // plotting its year-1 date would stretch the axis back two thousand years.
                Values = monthly
                    .Where(m => !m.IsPlaceholder)
                    .OrderBy(m => m.Date)
                    .Select(m => new DateTimePoint(m.Date, m.Gain))
                    .ToArray(),
                Stroke = new SolidColorPaint(ToSkColor(brush), 3),
                Fill = null,
                GeometrySize = 0
            };
        }

        private static SKColor ToSkColor(Brush brush)
        {
            if (brush is SolidColorBrush solid)
            {
                var c = solid.Color;
                return new SKColor(c.R, c.G, c.B, c.A);
            }

            return SKColors.Gray;
        }

        #endregion
    }
}
