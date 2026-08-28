using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Income;
using LifeCalculator.Framework.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Control.ViewModels
{
    public class ModifyIncomeStreamViewModel : ValidatableViewModelBase
    {
        private readonly IncomeStream _incomeStream;
        private readonly IIncomeStreamManager _incomeStreamManager;

        public ModifyIncomeStreamViewModel(IncomeStream incomeStream, IIncomeStreamManager incomeStreamManager)
        {
            _incomeStream = incomeStream;
            _incomeStreamManager = incomeStreamManager;

            DeleteIncomeStreamCommand = new RelayCommand(DeleteIncomeStream);

            ValidateAll();
        }

        public int Id => _incomeStream.Id;

        public List<IncomeStreamType> StreamTypes { get; } = Enum.GetValues(typeof(IncomeStreamType)).Cast<IncomeStreamType>().ToList();

        public string Name
        {
            get => _incomeStream.Name;
            set
            {
                Validate(nameof(Name), () => !string.IsNullOrWhiteSpace(value), "Name is required.");

                if (!string.IsNullOrWhiteSpace(value))
                {
                    _incomeStream.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public List<PayFrequency> PayFrequencies { get; } = Enum.GetValues(typeof(PayFrequency)).Cast<PayFrequency>().ToList();

        public double PayRate
        {
            get => _incomeStream.PayRate;
            set
            {
                Validate(nameof(PayRate), () => value > 0, "Pay must be greater than 0.");

                if (value > 0)
                {
                    _incomeStream.PayRate = value;
                    RaiseDerivedChanged();
                }
            }
        }

        public PayFrequency PayFrequency
        {
            get => _incomeStream.PayFrequency;
            set
            {
                _incomeStream.PayFrequency = value;
                OnPropertyChanged(nameof(IsHourly));
                OnPropertyChanged(nameof(PayRateLabel));
                RaiseDerivedChanged();
            }
        }

        public bool IsHourly => _incomeStream.PayFrequency == PayFrequency.Hourly;

        public double HoursPerWeek
        {
            get => _incomeStream.HoursPerWeek;
            set
            {
                Validate(nameof(HoursPerWeek), () => !IsHourly || (value > 0 && value <= 168),
                    "Hours per week must be between 0 and 168.");

                if (!IsHourly || (value > 0 && value <= 168))
                {
                    _incomeStream.HoursPerWeek = value;
                    RaiseDerivedChanged();
                }
            }
        }

        public string PayRateLabel
        {
            get
            {
                switch (_incomeStream.PayFrequency)
                {
                    case PayFrequency.Hourly: return "HOURLY RATE";
                    case PayFrequency.Annual: return IsGross ? "ANNUAL SALARY" : "ANNUAL TAKE-HOME";
                    default: return IsGross ? "GROSS PER CHEQUE" : "TAKE-HOME PER CHEQUE";
                }
            }
        }

        public double MonthlyAmount => _incomeStream.MonthlyAmount;

        public double AnnualAmount => _incomeStream.AnnualAmount;

        private void RaiseDerivedChanged()
        {
            OnPropertyChanged(nameof(PayRate));
            OnPropertyChanged(nameof(PayFrequency));
            OnPropertyChanged(nameof(HoursPerWeek));
            OnPropertyChanged(nameof(MonthlyAmount));
            OnPropertyChanged(nameof(AnnualAmount));
        }

        public IncomeStreamType StreamType
        {
            get => _incomeStream.StreamType;
            set
            {
                _incomeStream.StreamType = value;
                OnPropertyChanged(nameof(StreamType));
            }
        }

        public List<IncomeTaxTreatment> TaxTreatments { get; } =
            Enum.GetValues(typeof(IncomeTaxTreatment)).Cast<IncomeTaxTreatment>().ToList();

        public IncomeTaxTreatment TaxTreatment
        {
            get => _incomeStream.TaxTreatment;
            set
            {
                _incomeStream.TaxTreatment = value;
                OnPropertyChanged(nameof(TaxTreatment));
            }
        }

        public bool IsGross
        {
            get => _incomeStream.IsGross;
            set
            {
                _incomeStream.IsGross = value;
                OnPropertyChanged(nameof(IsGross));
                OnPropertyChanged(nameof(IsAlreadyNet));
                OnPropertyChanged(nameof(PayRateLabel));
            }
        }

        /// <summary>Inverse of <see cref="IsGross"/>; the checkbox is phrased as the exception.</summary>
        public bool IsAlreadyNet
        {
            get => !IsGross;
            set => IsGross = !value;
        }

        public DateTime StartDate
        {
            get => _incomeStream.StartDate;
            set
            {
                _incomeStream.StartDate = value;
                ValidateDateRange();
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime? EndDate
        {
            get => _incomeStream.EndDate;
            set
            {
                _incomeStream.EndDate = value;
                ValidateDateRange();
                OnPropertyChanged(nameof(EndDate));
            }
        }

        /// <summary>
        /// Mirrors the add form: unchecking clears the end date, making the stream ongoing.
        /// Must raise PropertyChanged — the End Date picker's IsEnabled binds to it.
        /// </summary>
        public bool HasEndDate
        {
            get => _incomeStream.EndDate != null;
            set
            {
                if (value && _incomeStream.EndDate == null)
                    EndDate = _incomeStream.StartDate.AddYears(1);
                else if (!value)
                    EndDate = null;

                OnPropertyChanged(nameof(HasEndDate));
            }
        }

        public IRelayCommand DeleteIncomeStreamCommand { get; set; }

        private void ValidateAll()
        {
            Validate(nameof(Name), () => !string.IsNullOrWhiteSpace(_incomeStream.Name), "Name is required.");
            Validate(nameof(PayRate), () => _incomeStream.PayRate > 0, "Pay must be greater than 0.");
            ValidateDateRange();
        }

        private void ValidateDateRange()
        {
            Validate(nameof(EndDate), () => _incomeStream.EndDate == null || _incomeStream.EndDate > _incomeStream.StartDate, "End date must be after the start date.");
        }

        private void DeleteIncomeStream()
        {
            _incomeStreamManager.DeleteIncomeStream(_incomeStream);
        }
    }
}
