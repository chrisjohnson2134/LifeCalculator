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

        /// <summary>
        /// Monthly pay on whatever basis <see cref="IsGross"/> says. Derived from
        /// <see cref="PayRate"/> and <see cref="PayFrequency"/> rather than typed in, but kept
        /// as a stored settable property for two reasons: the reflection-based data service
        /// hydrates through public setters, and rows written before pay frequency existed still
        /// carry a real value here.
        /// </summary>
        private double _monthlyAmount;
        public double MonthlyAmount
        {
            get => _monthlyAmount;
            set { _monthlyAmount = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>
        /// Pay per period in the unit named by <see cref="PayFrequency"/> — an hourly wage when
        /// hourly, otherwise the amount on one cheque (or the annual salary when Annual).
        /// </summary>
        private double _payRate;
        public double PayRate
        {
            get => _payRate;
            set { _payRate = value; RecalculateMonthlyAmount(); ValueChanged?.Invoke(this, this); }
        }

        private PayFrequency _payFrequency = PayFrequency.Annual;
        public PayFrequency PayFrequency
        {
            get => _payFrequency;
            set { _payFrequency = value; RecalculateMonthlyAmount(); ValueChanged?.Invoke(this, this); }
        }

        /// <summary>Only consulted when <see cref="PayFrequency"/> is Hourly.</summary>
        private double _hoursPerWeek = 40;
        public double HoursPerWeek
        {
            get => _hoursPerWeek;
            set { _hoursPerWeek = value; RecalculateMonthlyAmount(); ValueChanged?.Invoke(this, this); }
        }

        /// <summary>Annualised pay on this stream's own basis (gross unless IsGross is false).</summary>
        [IgnoreDatabase]
        public double AnnualAmount => AnnualiseRate(PayRate, PayFrequency, HoursPerWeek);

        /// <summary>
        /// Converts a rate into an annual figure. Weeks-per-year is 52 rather than 52.1775 to
        /// match how employers actually quote and schedule pay.
        /// </summary>
        public static double AnnualiseRate(double rate, PayFrequency frequency, double hoursPerWeek)
        {
            switch (frequency)
            {
                case PayFrequency.Hourly: return rate * hoursPerWeek * 52;
                case PayFrequency.Weekly: return rate * 52;
                case PayFrequency.BiWeekly: return rate * 26;
                case PayFrequency.SemiMonthly: return rate * 24;
                case PayFrequency.Monthly: return rate * 12;
                default: return rate;
            }
        }

        /// <summary>
        /// Guarded on PayRate > 0 so hydrating a legacy row — where the pay-rate columns default
        /// to 0 — can't wipe out the MonthlyAmount that was loaded from the database. Property
        /// hydration order is not guaranteed, so an unguarded recalc would zero those rows out
        /// depending on which setter the data service happened to call last.
        /// </summary>
        private void RecalculateMonthlyAmount()
        {
            if (_payRate <= 0)
                return;

            _monthlyAmount = AnnualiseRate(_payRate, _payFrequency, _hoursPerWeek) / 12;
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
        /// Gross annual pay, used to size employer 401(k) match caps ("up to 6% of salary").
        /// Now derived from the pay rate whenever this stream is entered gross, so nobody has
        /// to state their salary twice. The stored value is only used for streams entered as
        /// take-home, where gross genuinely isn't known.
        /// </summary>
        private double _grossAnnualSalary;
        public double GrossAnnualSalary
        {
            get => IsGross && AnnualAmount > 0 ? AnnualAmount : _grossAnnualSalary;
            set { _grossAnnualSalary = value; ValueChanged?.Invoke(this, this); }
        }

        /// <summary>
        /// True when pay is entered gross and we estimate the tax. This is the default: it's
        /// what an offer letter and a payslip's top line state, and it's the only basis that
        /// lets us model tax, 401(k) match, and pre-tax deductions rather than guess at them.
        /// Cleared for income that is never withheld against — a gift, a Roth withdrawal.
        /// </summary>
        private bool _isGross = true;
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
            : IsGross && GrossAnnualSalary > 0
                ? $"{Name} — {GrossAnnualSalary:C0}/yr gross"
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
