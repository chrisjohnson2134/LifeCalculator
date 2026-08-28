using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Income;
using LifeCalculator.Framework.Tax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Control.ViewModels
{
    public class AddIncomeStreamViewModel : ValidatableViewModelBase
    {
        #region Events

        public event EventHandler<IncomeStream> IncomeStreamAdded;

        #endregion

        #region Fields

        private readonly IAccountStore _accountStore;

        #endregion

        #region Constructors

        public AddIncomeStreamViewModel(IAccountStore accountStore)
        {
            _accountStore = accountStore;

            AddIncomeStreamCommand = new RelayCommand(AddIncomeStreamCommandHandler, () => !HasErrors);
            LinkCommandToValidation(AddIncomeStreamCommand);

            StartDate = DateTime.Now;
            StreamType = IncomeStreamType.Salary;
            PayFrequency = PayFrequency.Annual;

            ValidateAll();
        }

        #endregion

        #region Properties

        public List<IncomeStreamType> StreamTypes { get; } = Enum.GetValues(typeof(IncomeStreamType)).Cast<IncomeStreamType>().ToList();

        public List<PayFrequency> PayFrequencies { get; } = Enum.GetValues(typeof(PayFrequency)).Cast<PayFrequency>().ToList();

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Validate(nameof(Name), () => !string.IsNullOrWhiteSpace(_name), "Name is required.");
                OnPropertyChanged(nameof(Name));
            }
        }

        private double _payRate;
        public double PayRate
        {
            get => _payRate;
            set
            {
                _payRate = value;
                Validate(nameof(PayRate), () => _payRate > 0, "Pay must be greater than 0.");
                OnPropertyChanged(nameof(PayRate));
                RaiseEstimateChanged();
            }
        }

        private PayFrequency _payFrequency;
        public PayFrequency PayFrequency
        {
            get => _payFrequency;
            set
            {
                _payFrequency = value;
                OnPropertyChanged(nameof(PayFrequency));
                OnPropertyChanged(nameof(PayRateLabel));
                OnPropertyChanged(nameof(IsHourly));
                RaiseEstimateChanged();
            }
        }

        public bool IsHourly => PayFrequency == PayFrequency.Hourly;

        private double _hoursPerWeek = 40;
        public double HoursPerWeek
        {
            get => _hoursPerWeek;
            set
            {
                _hoursPerWeek = value;
                Validate(nameof(HoursPerWeek), () => !IsHourly || (_hoursPerWeek > 0 && _hoursPerWeek <= 168),
                    "Hours per week must be between 0 and 168.");
                OnPropertyChanged(nameof(HoursPerWeek));
                RaiseEstimateChanged();
            }
        }

        public string PayRateLabel
        {
            get
            {
                switch (PayFrequency)
                {
                    case PayFrequency.Hourly: return "HOURLY RATE";
                    case PayFrequency.Annual: return IsGross ? "ANNUAL SALARY" : "ANNUAL TAKE-HOME";
                    default: return IsGross ? "GROSS PER CHEQUE" : "TAKE-HOME PER CHEQUE";
                }
            }
        }

        private IncomeStreamType _streamType;
        public IncomeStreamType StreamType
        {
            get => _streamType;
            set
            {
                _streamType = value;

                // Sensible default treatment for the type picked; still overridable.
                TaxTreatment = _streamType == IncomeStreamType.Freelance
                    ? IncomeTaxTreatment.SelfEmployment
                    : _streamType == IncomeStreamType.Rental
                        ? IncomeTaxTreatment.NoPayrollTax
                        : IncomeTaxTreatment.W2Wages;

                OnPropertyChanged(nameof(StreamType));
            }
        }

        public List<IncomeTaxTreatment> TaxTreatments { get; } =
            Enum.GetValues(typeof(IncomeTaxTreatment)).Cast<IncomeTaxTreatment>().ToList();

        private IncomeTaxTreatment _taxTreatment;
        public IncomeTaxTreatment TaxTreatment
        {
            get => _taxTreatment;
            set { _taxTreatment = value; OnPropertyChanged(nameof(TaxTreatment)); RaiseEstimateChanged(); }
        }

        /// <summary>
        /// Gross by default — that's what an offer letter states, and it's the only basis that
        /// lets us estimate tax rather than ask the user to. Unticking it means "this money is
        /// never withheld against", which is true of gifts and Roth withdrawals.
        /// </summary>
        private bool _isGross = true;
        public bool IsGross
        {
            get => _isGross;
            set
            {
                _isGross = value;
                OnPropertyChanged(nameof(IsGross));
                OnPropertyChanged(nameof(IsAlreadyNet));
                OnPropertyChanged(nameof(PayRateLabel));
                RaiseEstimateChanged();
            }
        }

        /// <summary>The checkbox is phrased as the exception ("already after tax"), so it binds
        /// to the inverse. A property rather than a converter keeps it visible to validation.</summary>
        public bool IsAlreadyNet
        {
            get => !IsGross;
            set => IsGross = !value;
        }

        public double AnnualGross => IncomeStream.AnnualiseRate(PayRate, PayFrequency, HoursPerWeek);

        public double MonthlyGross => AnnualGross / 12;

        /// <summary>
        /// The marginal effect of adding this stream: household take-home with it, minus
        /// household take-home without it. Tax is progressive over total income, so a second
        /// job's take-home depends on what the first already earns — estimating this stream in
        /// isolation would flatter it by starting again from the 10% bracket.
        /// </summary>
        public double EstimatedMonthlyTakeHome
        {
            get
            {
                if (!IsGross)
                    return MonthlyGross;

                var account = _accountStore?.CurrentAccount;
                if (account?.IncomeStreamManager == null || AnnualGross <= 0)
                    return 0;

                var existing = account.IncomeStreamManager.GetAllIncomeStreams() ?? new List<IncomeStream>();

                var candidate = new IncomeStream
                {
                    Name = Name,
                    PayFrequency = PayFrequency,
                    HoursPerWeek = HoursPerWeek,
                    PayRate = PayRate,
                    IsGross = true,
                    TaxTreatment = TaxTreatment,
                    StartDate = StartDate
                };

                double withoutIt = NetMonthly(existing);
                double withIt = NetMonthly(existing.Concat(new[] { candidate }));

                return Math.Max(0, withIt - withoutIt);
            }
        }

        public double EstimatedMonthlyTax => Math.Max(0, MonthlyGross - EstimatedMonthlyTakeHome);

        public double EffectiveTaxRate => MonthlyGross <= 0 ? 0 : EstimatedMonthlyTax / MonthlyGross;

        /// <summary>Reads the household-level tax settings the user set on their profile.</summary>
        private double NetMonthly(IEnumerable<IncomeStream> streams)
        {
            var account = _accountStore.CurrentAccount;

            var estimate = HouseholdTaxEstimator.Estimate(
                streams,
                account.FilingStatus,
                account.PreTaxDeductionsAnnual,
                account.StateTaxRatePercent);

            return (estimate.NetFromGrossAnnual + estimate.AlreadyNetAnnual) / 12;
        }

        private void RaiseEstimateChanged()
        {
            OnPropertyChanged(nameof(AnnualGross));
            OnPropertyChanged(nameof(MonthlyGross));
            OnPropertyChanged(nameof(EstimatedMonthlyTakeHome));
            OnPropertyChanged(nameof(EstimatedMonthlyTax));
            OnPropertyChanged(nameof(EffectiveTaxRate));
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                ValidateDateRange();
                OnPropertyChanged(nameof(StartDate));
            }
        }

        // Must raise PropertyChanged: the End Date picker's IsEnabled binds to this, so an
        // auto-property here leaves the picker permanently disabled.
        private bool _hasEndDate;
        public bool HasEndDate
        {
            get => _hasEndDate;
            set
            {
                _hasEndDate = value;

                if (_hasEndDate && _endDate == null)
                    EndDate = _startDate.AddYears(1);

                ValidateDateRange();
                OnPropertyChanged(nameof(HasEndDate));
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                ValidateDateRange();
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public IRelayCommand AddIncomeStreamCommand { get; set; }

        #endregion

        #region Validation

        private void ValidateAll()
        {
            Validate(nameof(Name), () => !string.IsNullOrWhiteSpace(_name), "Name is required.");
            Validate(nameof(PayRate), () => _payRate > 0, "Pay must be greater than 0.");
            Validate(nameof(HoursPerWeek), () => !IsHourly || (_hoursPerWeek > 0 && _hoursPerWeek <= 168),
                "Hours per week must be between 0 and 168.");
            ValidateDateRange();
        }

        private void ValidateDateRange()
        {
            Validate(nameof(EndDate), () => !HasEndDate || _endDate == null || _endDate > _startDate, "End date must be after the start date.");
        }

        #endregion

        #region Command Handlers

        private void AddIncomeStreamCommandHandler()
        {
            // PayFrequency and HoursPerWeek before PayRate: the rate setter derives MonthlyAmount
            // from all three, so it has to run last to see the final frequency.
            var incomeStream = new IncomeStream
            {
                Name = Name,
                PayFrequency = PayFrequency,
                HoursPerWeek = HoursPerWeek,
                PayRate = PayRate,
                StartDate = StartDate,
                EndDate = HasEndDate ? EndDate : null,
                StreamType = StreamType,
                IsGross = IsGross,
                TaxTreatment = TaxTreatment,
                UserId = _accountStore.CurrentAccount.Id
            };

            _accountStore.CurrentAccount.IncomeStreamManager.AddIncomeStream(incomeStream);

            IncomeStreamAdded?.Invoke(this, incomeStream);
        }

        #endregion
    }
}
