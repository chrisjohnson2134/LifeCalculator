using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Services.DataService;
using System;

namespace LifeCalculator.Framework.Income
{
    /// <summary>
    /// A source of monthly income (salary, freelance, rental, etc.) that feeds the Life
    /// Calculator's cash-flow surplus calculation alongside FinancialAccount's bill fields.
    /// </summary>
    public class IncomeStream
    {
        public event EventHandler<IncomeStream> ValueChanged;

        public int Id { get; set; } = -1;

        private int _userId;
        public int UserId
        {
            get => _userId;
            set { _userId = value; ValueChanged?.Invoke(this, this); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; ValueChanged?.Invoke(this, this); }
        }

        private double _monthlyAmount;
        public double MonthlyAmount
        {
            get => _monthlyAmount;
            set { _monthlyAmount = value; ValueChanged?.Invoke(this, this); }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>Null means ongoing/no known end date.</summary>
        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set { _endDate = value; ValueChanged?.Invoke(this, this); }
        }

        private IncomeStreamType _streamType;
        public IncomeStreamType StreamType
        {
            get => _streamType;
            set { _streamType = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>
        /// Optional gross annual salary for this stream. Not used for income at all —
        /// <see cref="MonthlyAmount"/> (take-home) is the real number. This exists solely
        /// because employer 401(k) match caps are defined against gross pay ("up to 6% of
        /// salary"), so a retirement account linked to this stream needs it. Zero when unknown.
        /// </summary>
        private double _grossAnnualSalary;
        public double GrossAnnualSalary
        {
            get => _grossAnnualSalary;
            set { _grossAnnualSalary = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>
        /// True when MonthlyAmount is gross rather than take-home. Retained so the optional
        /// "estimate my take-home from gross" helper can still work; the default is take-home,
        /// which is what people read off a payslip.
        /// </summary>
        private bool _isGross;
        public bool IsGross
        {
            get => _isGross;
            set { _isGross = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>Payroll-tax treatment. Only meaningful when <see cref="IsGross"/> is true.</summary>
        private IncomeTaxTreatment _taxTreatment;
        public IncomeTaxTreatment TaxTreatment
        {
            get => _taxTreatment;
            set { _taxTreatment = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>
        /// Shown when picking which job a retirement account belongs to.
        /// [IgnoreDatabase] is essential: GenericDataService persists every public property by
        /// reflection, so a computed one would be written as a column that doesn't exist.
        /// </summary>
        [IgnoreDatabase]
        public string Display => string.IsNullOrWhiteSpace(Name)
            ? "(unnamed)"
            : $"{Name} — {MonthlyAmount:C0}/mo";

        public override string ToString() => Display;

        /// <summary>
        /// Compares whole months, not exact days. Projections are monthly and ask about the
        /// first of the month, so a stream starting on the 27th must still count as income for
        /// that month — a day-level comparison silently zeroes it out.
        /// </summary>
        public bool IsActiveDuring(DateTime date)
        {
            DateTime month = MonthOf(date);

            if (MonthOf(StartDate) > month)
                return false;

            return EndDate == null || month <= MonthOf(EndDate.Value);
        }

        private static DateTime MonthOf(DateTime date) => new DateTime(date.Year, date.Month, 1);
    }
}
