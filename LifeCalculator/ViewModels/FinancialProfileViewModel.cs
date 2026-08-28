using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.FinancialAccount;
using LifeCalculator.Framework.Income;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Services.FinancialAccountService;
using LifeCalculator.Framework.Tax;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LifeCalculator.ViewModels
{
    /// <summary>
    /// The "money in" screen: salary (which the 401(k) employer-match cap is computed against)
    /// and the income streams that drive the Life Calculator's monthly-surplus projection.
    /// Expenses live on the Budget screen.
    /// </summary>
    public class FinancialProfileViewModel : ValidatableViewModelBase
    {
        #region Fields

        private readonly FinancialAccount _currentAccount;
        private readonly IFinancialAccountDataService _financialAccountService;
        private readonly IIncomeStreamManager _incomeStreamManager;
        private readonly IAccountStore _accountStore;

        #endregion

        #region Constructors

        public FinancialProfileViewModel(IAccountStore accountStore, IFinancialAccountDataService financialAccountService)
        {
            _accountStore = accountStore;
            _currentAccount = accountStore.CurrentAccount;
            _financialAccountService = financialAccountService;
            _incomeStreamManager = accountStore.CurrentAccount.IncomeStreamManager;

            IncomeStreams = new ObservableCollection<ModifyIncomeStreamViewModel>();

            _incomeStreamManager.IncomeStreamAdded += IncomeStreamManager_Changed;
            _incomeStreamManager.IncomeStreamChanged += IncomeStreamManager_Changed;
            _incomeStreamManager.IncomeStreamDeleted += IncomeStreamManager_Deleted;

            ToggleAddIncomeCommand = new RelayCommand(ToggleAddIncome);

            foreach (var stream in _incomeStreamManager.GetAllIncomeStreams())
                AddIncomeRow(stream);

            RefreshTotals();
            RefreshTaxEstimate();
        }

        #endregion

        #region Properties

        public double Salary
        {
            get => _currentAccount.Salary;
            set
            {
                Validate(nameof(Salary), () => value >= 0, "Salary cannot be negative.");

                if (value < 0)
                    return;

                _currentAccount.Salary = value;
                SaveAndRefreshTax();
                OnPropertyChanged(nameof(Salary));
            }
        }

        /// <summary>Take-home for the year — always populated, unlike a gross-only figure.</summary>
        public double AnnualTakeHome => TotalMonthlyIncome * 12;

        public List<FilingStatus> FilingStatuses { get; } =
            Enum.GetValues(typeof(FilingStatus)).Cast<FilingStatus>().ToList();

        public FilingStatus FilingStatus
        {
            get => _currentAccount.FilingStatus;
            set
            {
                _currentAccount.FilingStatus = value;
                SaveAndRefreshTax();
                OnPropertyChanged(nameof(FilingStatus));
            }
        }

        public IReadOnlyList<StateTaxRate> States { get; } = StateTaxRates.All;

        /// <summary>
        /// Picking a state fills in its rate, so nobody has to know their own effective rate.
        /// Editing the rate afterwards switches to "Custom" rather than silently disagreeing
        /// with the state label.
        /// </summary>
        public StateTaxRate SelectedState
        {
            get => StateTaxRates.FromCode(_currentAccount.StateCode);
            set
            {
                if (value == null)
                    return;

                _currentAccount.StateCode = value.Code;

                if (value.Code != StateTaxRates.CustomCode)
                    _currentAccount.StateTaxRatePercent = value.RatePercent;

                SaveAndRefreshTax();
                OnPropertyChanged(nameof(SelectedState));
                OnPropertyChanged(nameof(StateTaxRatePercent));
                OnPropertyChanged(nameof(IsCustomStateRate));
            }
        }

        public bool IsCustomStateRate => _currentAccount.StateCode == StateTaxRates.CustomCode;

        public double StateTaxRatePercent
        {
            get => _currentAccount.StateTaxRatePercent;
            set
            {
                Validate(nameof(StateTaxRatePercent), () => value >= 0 && value <= 20, "State rate must be between 0 and 20%.");

                if (value < 0 || value > 20)
                    return;

                _currentAccount.StateTaxRatePercent = value;

                // A hand-edited rate no longer matches the named state.
                if (Math.Abs(SelectedState.RatePercent - value) > 0.001)
                {
                    _currentAccount.StateCode = StateTaxRates.CustomCode;
                    OnPropertyChanged(nameof(SelectedState));
                    OnPropertyChanged(nameof(IsCustomStateRate));
                }

                SaveAndRefreshTax();
                OnPropertyChanged(nameof(StateTaxRatePercent));
            }
        }

        public double PreTaxDeductionsAnnual
        {
            get => _currentAccount.PreTaxDeductionsAnnual;
            set
            {
                Validate(nameof(PreTaxDeductionsAnnual), () => value >= 0, "Deductions cannot be negative.");
                Validate(nameof(PreTaxDeductionsAnnual), () => value <= _currentAccount.Salary || _currentAccount.Salary == 0,
                    "Deductions can't exceed your salary.");

                if (value < 0)
                    return;

                _currentAccount.PreTaxDeductionsAnnual = value;
                SaveAndRefreshTax();
                OnPropertyChanged(nameof(PreTaxDeductionsAnnual));
            }
        }

        private HouseholdTaxEstimate _taxEstimate = new HouseholdTaxEstimate();
        public HouseholdTaxEstimate TaxEstimate
        {
            get => _taxEstimate;
            private set
            {
                _taxEstimate = value;
                OnPropertyChanged(nameof(TaxEstimate));
                OnPropertyChanged(nameof(EffectiveTaxRateText));
                OnPropertyChanged(nameof(HasGrossStreams));
            }
        }

        public string EffectiveTaxRateText => $"{TaxEstimate.EffectiveTaxRate:P1} of gross";

        public string TaxYearText => $"Based on {TaxEstimator.TaxYear} federal brackets";

        /// <summary>Drives whether the tax breakdown is worth showing at all.</summary>
        public bool HasGrossStreams => TaxEstimate.GrossAnnual > 0;

        public ObservableCollection<ModifyIncomeStreamViewModel> IncomeStreams { get; }

        private double _totalMonthlyIncome;
        public double TotalMonthlyIncome
        {
            get => _totalMonthlyIncome;
            private set
            {
                _totalMonthlyIncome = value;
                OnPropertyChanged(nameof(TotalMonthlyIncome));
                OnPropertyChanged(nameof(AnnualTakeHome));
            }
        }

        private bool _isAddIncomeOpen;
        public bool IsAddIncomeOpen
        {
            get => _isAddIncomeOpen;
            private set { _isAddIncomeOpen = value; OnPropertyChanged(nameof(IsAddIncomeOpen)); }
        }

        private AddIncomeStreamViewModel _addIncomeStreamViewModel;
        public AddIncomeStreamViewModel AddIncomeStreamViewModel
        {
            get => _addIncomeStreamViewModel;
            private set { _addIncomeStreamViewModel = value; OnPropertyChanged(nameof(AddIncomeStreamViewModel)); }
        }

        public IRelayCommand ToggleAddIncomeCommand { get; }

        #endregion

        #region Methods

        private void ToggleAddIncome()
        {
            IsAddIncomeOpen = !IsAddIncomeOpen;

            if (!IsAddIncomeOpen)
                return;

            var vm = new AddIncomeStreamViewModel(_accountStore);
            vm.IncomeStreamAdded += (s, e) => IsAddIncomeOpen = false;
            AddIncomeStreamViewModel = vm;
        }

        private void IncomeStreamManager_Changed(object sender, IncomeStream e)
        {
            if (IncomeStreams.All(row => row.Id != e.Id))
                AddIncomeRow(e);

            RefreshTaxEstimate();
        }

        /// <summary>
        /// Recomputes the moment a row is edited, rather than waiting for the manager's async
        /// database save to round-trip — otherwise the summary cards lag behind what's on screen.
        /// </summary>
        private void AddIncomeRow(IncomeStream stream)
        {
            var row = new ModifyIncomeStreamViewModel(stream, _incomeStreamManager);
            row.PropertyChanged += (s, e) => RefreshTaxEstimate();
            IncomeStreams.Add(row);
        }

        private void IncomeStreamManager_Deleted(object sender, IncomeStream e)
        {
            var row = IncomeStreams.FirstOrDefault(r => r.Id == e.Id);
            if (row != null)
                IncomeStreams.Remove(row);

            RefreshTaxEstimate();
        }

        /// <summary>
        /// Shows take-home: gross streams contribute their post-tax share, streams already
        /// entered as take-home contribute as-is.
        /// </summary>
        private void RefreshTotals()
        {
            DateTime today = DateTime.Now;

            TotalMonthlyIncome = _incomeStreamManager.GetAllIncomeStreams()
                .Where(s => s.IsActiveDuring(today))
                .Sum(s => _taxEstimate.NetMonthlyByStreamId.TryGetValue(s.Id, out var net)
                    ? net
                    : s.MonthlyAmount);
        }

        private void SaveAndRefreshTax()
        {
            _financialAccountService.Save(_currentAccount.Id, _currentAccount);
            RefreshTaxEstimate();
        }

        /// <summary>
        /// Taxes every gross stream together rather than one at a time — income tax is
        /// progressive over total income, so a second stream is taxed at the marginal rate the
        /// first one already pushed you into.
        /// </summary>
        private void RefreshTaxEstimate()
        {
            TaxEstimate = HouseholdTaxEstimator.Estimate(
                _incomeStreamManager.GetAllIncomeStreams(),
                _currentAccount.FilingStatus,
                _currentAccount.PreTaxDeductionsAnnual,
                _currentAccount.StateTaxRatePercent);

            RefreshTotals();
        }


        #endregion
    }
}
