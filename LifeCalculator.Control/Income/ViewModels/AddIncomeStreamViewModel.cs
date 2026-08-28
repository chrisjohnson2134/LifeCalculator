using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Income;
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

            ValidateAll();
        }

        #endregion

        #region Properties

        public List<IncomeStreamType> StreamTypes { get; } = Enum.GetValues(typeof(IncomeStreamType)).Cast<IncomeStreamType>().ToList();

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

        private double _monthlyAmount;
        public double MonthlyAmount
        {
            get => _monthlyAmount;
            set
            {
                _monthlyAmount = value;
                Validate(nameof(MonthlyAmount), () => _monthlyAmount > 0, "Monthly amount must be greater than 0.");
                OnPropertyChanged(nameof(MonthlyAmount));
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
            set { _taxTreatment = value; OnPropertyChanged(nameof(TaxTreatment)); }
        }

        /// <summary>
        /// When true the amount entered is gross and tax is estimated across all gross streams
        /// together. The tax-treatment picker only matters in that case.
        /// </summary>
        /// <summary>Optional; only needed if a 401(k) will be linked to this stream.</summary>
        private double _grossAnnualSalary;
        public double GrossAnnualSalary
        {
            get => _grossAnnualSalary;
            set
            {
                _grossAnnualSalary = value;
                Validate(nameof(GrossAnnualSalary), () => _grossAnnualSalary >= 0, "Salary cannot be negative.");
                OnPropertyChanged(nameof(GrossAnnualSalary));
            }
        }

        private bool _isGross;
        public bool IsGross
        {
            get => _isGross;
            set
            {
                _isGross = value;
                OnPropertyChanged(nameof(IsGross));
                OnPropertyChanged(nameof(AmountLabel));
            }
        }

        public string AmountLabel => IsGross ? "MONTHLY AMOUNT (GROSS)" : "MONTHLY AMOUNT (TAKE-HOME)";

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
            Validate(nameof(MonthlyAmount), () => _monthlyAmount > 0, "Monthly amount must be greater than 0.");
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
            var incomeStream = new IncomeStream
            {
                Name = Name,
                MonthlyAmount = MonthlyAmount,
                StartDate = StartDate,
                EndDate = HasEndDate ? EndDate : null,
                StreamType = StreamType,
                IsGross = IsGross,
                TaxTreatment = TaxTreatment,
                GrossAnnualSalary = GrossAnnualSalary,
                UserId = _accountStore.CurrentAccount.Id
            };

            _accountStore.CurrentAccount.IncomeStreamManager.AddIncomeStream(incomeStream);

            IncomeStreamAdded?.Invoke(this, incomeStream);
        }

        #endregion
    }
}
