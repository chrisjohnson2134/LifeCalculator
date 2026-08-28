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
        [System.ComponentModel.Description("Net worth")]
        NetWorth,
        [System.ComponentModel.Description("Account growth")]
        AccountGrowth,
        [System.ComponentModel.Description("Debt payoff")]
        DebtPayoff,
        [System.ComponentModel.Description("Emergency fund")]
        EmergencyFund
    }

    /// <summary>
    /// How the time-series views are drawn. A stacked variant was tried and dropped: stacking
    /// assets and debt made the total harder to read, not easier, because the thing you actually
    /// want off this chart is one number's path over time.
    /// </summary>
    public enum ChartStyle
    {
        [System.ComponentModel.Description("Line")]
        Line,
        [System.ComponentModel.Description("Area")]
        Area
    }

    /// <summary>
    /// How far out to plot. A 40-year projection squashes the next two years into a few pixels,
    /// which is exactly the part someone is usually trying to read.
    /// </summary>
    public enum ChartRange
    {
        [System.ComponentModel.Description("1 year")]
        OneYear = 1,
        [System.ComponentModel.Description("5 years")]
        FiveYears = 5,
        [System.ComponentModel.Description("10 years")]
        TenYears = 10,
        [System.ComponentModel.Description("30 years")]
        ThirtyYears = 30,
        [System.ComponentModel.Description("All")]
        All = 0
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
            DebtTimelineSeries = new ObservableCollection<ISeries>();
            EmergencyGaugeSeries = new ObservableCollection<ISeries>();
            SeriesToggles = new ObservableCollection<ChartSeriesToggle>();

            _selectedChartStyle = ChartStyle.Line;
            _selectedChartRange = ChartRange.TenYears;

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
                OnPropertyChanged(nameof(IsEmergencyChartVisible));
                OnPropertyChanged(nameof(SupportsChartStyle));
                OnPropertyChanged(nameof(SupportsLegend));
                OnPropertyChanged(nameof(ChartCaption));
                Recalculate();
            }
        }

        public bool IsDebtChartVisible => SelectedChartView == ChartView.DebtPayoff;
        public bool IsGrowthChartVisible => SelectedChartView == ChartView.AccountGrowth;
        public bool IsNetWorthChartVisible => SelectedChartView == ChartView.NetWorth;
        public bool IsEmergencyChartVisible => SelectedChartView == ChartView.EmergencyFund;

        /// <summary>Line/Area/Stacked only mean something for the two time-series views.</summary>
        public bool SupportsChartStyle => IsNetWorthChartVisible || IsGrowthChartVisible;

        public bool SupportsLegend => IsNetWorthChartVisible || IsGrowthChartVisible || IsDebtChartVisible;

        /// <summary>One line saying what the current view actually shows.</summary>
        public string ChartCaption
        {
            get
            {
                switch (SelectedChartView)
                {
                    case ChartView.NetWorth:
                        return "Everything you own minus everything you owe, projected forward.";
                    case ChartView.AccountGrowth:
                        return "Each account's balance over time.";
                    case ChartView.DebtPayoff:
                        return "How long until each debt is cleared, in payoff order. Bars end on the month the balance hits zero.";
                    default:
                        return "Progress toward your emergency fund goal.";
                }
            }
        }

        public List<ChartView> ChartViews { get; } = Enum.GetValues(typeof(ChartView)).Cast<ChartView>().ToList();

        private ChartStyle _selectedChartStyle;
        public ChartStyle SelectedChartStyle
        {
            get => _selectedChartStyle;
            set
            {
                _selectedChartStyle = value;
                OnPropertyChanged(nameof(SelectedChartStyle));
                OnPropertyChanged(nameof(ChartCaption));
                Recalculate();
            }
        }

        public List<ChartStyle> ChartStyles { get; } = Enum.GetValues(typeof(ChartStyle)).Cast<ChartStyle>().ToList();

        private ChartRange _selectedChartRange;
        public ChartRange SelectedChartRange
        {
            get => _selectedChartRange;
            set
            {
                _selectedChartRange = value;
                OnPropertyChanged(nameof(SelectedChartRange));
                Recalculate();
            }
        }

        public List<ChartRange> ChartRanges { get; } = Enum.GetValues(typeof(ChartRange)).Cast<ChartRange>().ToList();

        /// <summary>Clickable legend; hiding a series rebuilds the chart without it.</summary>
        public ObservableCollection<ChartSeriesToggle> SeriesToggles { get; }

        // Debt payoff timeline: horizontal bars, one per debt, length = months until cleared.
        public ObservableCollection<ISeries> DebtTimelineSeries { get; }
        public Axis[] DebtTimelineXAxes { get; } = { new Axis { Labeler = value => FormatMonths(value), MinStep = 6 } };
        public Axis[] DebtTimelineYAxes { get; } = { new Axis { Labels = new List<string>() } };

        // Emergency fund gauge: a donut of funded vs remaining.
        public ObservableCollection<ISeries> EmergencyGaugeSeries { get; }

        private string _emergencyGaugeCaption;
        public string EmergencyGaugeCaption
        {
            get => _emergencyGaugeCaption;
            private set { _emergencyGaugeCaption = value; OnPropertyChanged(nameof(EmergencyGaugeCaption)); }
        }

        private string _emergencyGaugeHeadline;
        public string EmergencyGaugeHeadline
        {
            get => _emergencyGaugeHeadline;
            private set { _emergencyGaugeHeadline = value; OnPropertyChanged(nameof(EmergencyGaugeHeadline)); }
        }

        private static string FormatMonths(double months)
        {
            if (months < 0) return string.Empty;
            if (months < 12) return $"{months:0}mo";
            return $"{months / 12:0.#}yr";
        }

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
        private void Recalculate() => Recalculate(rebuildToggles: true);

        /// <summary>
        /// <paramref name="rebuildToggles"/> is false when the user has just clicked a legend
        /// entry: the toggles are already correct, and rebuilding them mid-click would replace
        /// the object that raised the event.
        /// </summary>
        private void Recalculate(bool rebuildToggles)
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

            var netWorthTimeline = NetWorthAggregator.Aggregate(debtResult, assetTimelines);

            // Toggles first: every rebuild below filters on them.
            if (rebuildToggles)
                RebuildSeriesToggles(assetTimelines);

            RebuildDebtChart(debtResult);
            RebuildDebtTimeline(debtResult);
            RebuildGrowthChart(assetTimelines, assetColors);
            RebuildNetWorthChart(netWorthTimeline);
            RebuildEmergencyGauge();

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

                if (!IsSeriesVisible(debt.Name))
                    continue;

                DebtSeriesCollection.Add(BuildLineSeries(debt.Name, series, debt.SeriesColor));
            }
        }

        /// <summary>
        /// Horizontal bars, one per debt, ordered by when they clear. Overlapping balance lines
        /// make it genuinely hard to see which debt finishes first — and with rollover that
        /// ordering is the whole point of picking avalanche over snowball.
        /// </summary>
        private void RebuildDebtTimeline(DebtPayoffResult debtResult)
        {
            DebtTimelineSeries.Clear();

            DateTime now = DateTime.Now;

            var rows = Debts
                .Where(d => IsSeriesVisible(d.Name))
                .Select(d => new
                {
                    d.Name,
                    d.SeriesColor,
                    PayoffDate = debtResult.PayoffDateByDebtName.TryGetValue(d.Name, out var date) ? date : null
                })
                // Debts that never clear sort last; they'd otherwise lead with a zero-length bar.
                .OrderBy(r => r.PayoffDate == null)
                .ThenBy(r => r.PayoffDate)
                .ToList();

            if (rows.Count == 0)
                return;

            DebtTimelineYAxes[0].Labels = rows.Select(r => r.Name).ToList();

            var monthsPerRow = rows
                .Select(r => r.PayoffDate == null
                    ? 0
                    : Math.Max(0, ((r.PayoffDate.Value.Year - now.Year) * 12) + (r.PayoffDate.Value.Month - now.Month)))
                .ToList();

            // The date sits past the end of its bar, so the axis needs room for it. Without the
            // headroom the longest bar reaches the edge of the plot and its label is clipped —
            // and the longest bar is exactly the one whose date you most want to read.
            double longestBar = monthsPerRow.Count == 0 ? 0 : monthsPerRow.Max();
            DebtTimelineXAxes[0].MinLimit = 0;
            DebtTimelineXAxes[0].MaxLimit = Math.Max(12, longestBar * 1.35);

            // One RowSeries per debt so each keeps its own colour; the null padding places each
            // bar on its own row rather than stacking them all at the bottom.
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];

                var values = new double?[rows.Count];
                values[i] = monthsPerRow[i];

                DebtTimelineSeries.Add(new RowSeries<double?>
                {
                    Name = row.PayoffDate == null
                        ? $"{row.Name} (not on track)"
                        : $"{row.Name} — {row.PayoffDate.Value:MMM yyyy}",
                    Values = values,
                    Fill = new SolidColorPaint(ToSkColor(row.SeriesColor)),
                    Stroke = null,
                    MaxBarWidth = 26,
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(160)),
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Right,
                    DataLabelsFormatter = point => row.PayoffDate == null
                        ? "not on track"
                        : row.PayoffDate.Value.ToString("MMM yyyy")
                });
            }
        }

        private void RebuildGrowthChart(Dictionary<string, List<MonthlyColumn>> assetTimelines, Dictionary<string, Brush> assetColors)
        {
            GrowthSeriesCollection.Clear();

            foreach (var kvp in assetTimelines)
            {
                if (!IsSeriesVisible(kvp.Key))
                    continue;

                var brush = assetColors.TryGetValue(kvp.Key, out var b) ? b : Brushes.Gray;

                GrowthSeriesCollection.Add(
                    BuildLineSeries(kvp.Key, kvp.Value, brush, SelectedChartStyle == ChartStyle.Area));
            }
        }

        private void RebuildNetWorthChart(List<NetWorthColumn> netWorthTimeline)
        {
            NetWorthSeriesCollection.Clear();

            var window = RangeCutoff();

            var points = netWorthTimeline
                .Where(c => InRange(c.Date, window))
                .Select(c => new DateTimePoint(c.Date, c.NetWorth))
                .ToArray();

            NetWorthSeriesCollection.Add(new LineSeries<DateTimePoint>
            {
                Name = "Net Worth",
                Values = points,
                Stroke = new SolidColorPaint(SKColors.MediumSeaGreen, 3),
                Fill = SelectedChartStyle == ChartStyle.Area
                    ? new SolidColorPaint(SKColors.MediumSeaGreen.WithAlpha(60))
                    : null,
                GeometrySize = 0
            });
        }

        /// <summary>
        /// Rebuilds the legend to match the accounts that exist, preserving whatever the user has
        /// already hidden. Rebuilt rather than reconciled in place because accounts come and go.
        /// </summary>
        private void RebuildSeriesToggles(Dictionary<string, List<MonthlyColumn>> assetTimelines)
        {
            var previous = SeriesToggles.ToDictionary(t => t.Name, t => t.IsVisible);

            SeriesToggles.Clear();

            foreach (var debt in Debts)
                AddToggle(debt.Name, debt.SeriesColor, previous);

            foreach (var investment in Investments)
                AddToggle(investment.Name, investment.SeriesColor, previous);

            foreach (var retirement in RetirementAccounts)
                AddToggle(retirement.Name, retirement.SeriesColor, previous);

            var fund = EmergencyFund?.Fund;
            if (fund != null)
                AddToggle(fund.Name, AccountColorPalette.BrushAt(_colorCounter), previous);

        }

        private void AddToggle(string name, Brush color, Dictionary<string, bool> previous)
        {
            if (string.IsNullOrWhiteSpace(name) || SeriesToggles.Any(t => t.Name == name))
                return;

            bool visible = !previous.TryGetValue(name, out var wasVisible) || wasVisible;

            SeriesToggles.Add(new ChartSeriesToggle(name, color, visible, OnSeriesToggled));
        }

        private void OnSeriesToggled()
        {
            // Rebuild the plotted series only — re-running RebuildSeriesToggles here would
            // recreate the very toggle the user just clicked.
            Recalculate(rebuildToggles: false);
        }

        private bool IsSeriesVisible(string name)
        {
            var toggle = SeriesToggles.FirstOrDefault(t => t.Name == name);
            return toggle == null || toggle.IsVisible;
        }

        /// <summary>Latest date to plot for the selected range, or null for "All".</summary>
        private DateTime? RangeCutoff()
        {
            if (SelectedChartRange == ChartRange.All)
                return null;

            return DateTime.Now.AddYears((int)SelectedChartRange);
        }

        private static bool InRange(DateTime date, DateTime? cutoff) => cutoff == null || date <= cutoff.Value;

        private void RebuildEmergencyGauge()
        {
            EmergencyGaugeSeries.Clear();

            var fund = EmergencyFund?.Fund;

            if (fund == null || fund.GoalAmount <= 0)
            {
                EmergencyGaugeHeadline = "No goal set";
                EmergencyGaugeCaption = fund == null
                    ? "Start an emergency fund on the Emergency Fund tab."
                    : "Set a goal on the Emergency Fund tab to track progress here.";
                return;
            }

            double funded = Math.Min(fund.InitialAmount, fund.GoalAmount);
            double remaining = Math.Max(0, fund.GoalAmount - funded);

            EmergencyGaugeSeries.Add(new PieSeries<double>
            {
                Name = "Saved",
                Values = new[] { funded },
                InnerRadius = 90,
                Fill = new SolidColorPaint(SKColors.SeaGreen),
                DataLabelsPaint = null
            });

            EmergencyGaugeSeries.Add(new PieSeries<double>
            {
                Name = "Still to save",
                Values = new[] { remaining },
                InnerRadius = 90,
                Fill = new SolidColorPaint(SKColors.LightGray.WithAlpha(120)),
                DataLabelsPaint = null
            });

            EmergencyGaugeHeadline = $"{fund.ProgressFraction * 100:0}% funded";
            EmergencyGaugeCaption = EmergencyFund.ProjectionText;
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

        private ISeries BuildLineSeries(string name, List<MonthlyColumn> monthly, Brush brush, bool filled = false)
        {
            var color = ToSkColor(brush);

            return new LineSeries<DateTimePoint>
            {
                Name = name,
                Values = ToPoints(monthly),
                Stroke = new SolidColorPaint(color, 3),
                Fill = filled ? new SolidColorPaint(color.WithAlpha(60)) : null,
                GeometrySize = 0
            };
        }

        private DateTimePoint[] ToPoints(List<MonthlyColumn> monthly)
        {
            DateTime? cutoff = RangeCutoff();

            // Skip the placeholder row each Calculation() prepends (see MonthlyColumn.IsPlaceholder) —
            // plotting its year-1 date would stretch the axis back two thousand years.
            return monthly
                .Where(m => !m.IsPlaceholder)
                .Where(m => InRange(m.Date, cutoff))
                .OrderBy(m => m.Date)
                .Select(m => new DateTimePoint(m.Date, m.Gain))
                .ToArray();
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
