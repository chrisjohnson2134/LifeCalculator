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

        public double MonthlyAmount
        {
            get => _incomeStream.MonthlyAmount;
            set
            {
                Validate(nameof(MonthlyAmount), () => value > 0, "Monthly amount must be greater than 0.");

                if (value > 0)
                {
                    _incomeStream.MonthlyAmount = value;
                    OnPropertyChanged(nameof(MonthlyAmount));
                }
            }
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
                OnPropertyChanged(nameof(AmountLabel));
            }
        }

        public string AmountLabel => IsGross ? "MONTHLY (GROSS)" : "MONTHLY (TAKE-HOME)";

        /// <summary>
        /// Optional, and only relevant when a 401(k) is linked to this stream — employer match
        /// caps are a percentage of gross pay.
        /// </summary>
        public double GrossAnnualSalary
        {
            get => _incomeStream.GrossAnnualSalary;
            set
            {
                Validate(nameof(GrossAnnualSalary), () => value >= 0, "Salary cannot be negative.");

                if (value >= 0)
                {
                    _incomeStream.GrossAnnualSalary = value;
                    OnPropertyChanged(nameof(GrossAnnualSalary));
                }
            }
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
            Validate(nameof(MonthlyAmount), () => _incomeStream.MonthlyAmount > 0, "Monthly amount must be greater than 0.");
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
