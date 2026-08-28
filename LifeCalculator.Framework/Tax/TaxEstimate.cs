namespace LifeCalculator.Framework.Tax
{
    /// <summary>
    /// A breakdown of estimated annual tax, so the UI can show where the money goes rather
    /// than just a single take-home number.
    /// </summary>
    public class TaxEstimate
    {
        public double GrossAnnual { get; set; }
        public double PreTaxDeductions { get; set; }

        public double FederalTax { get; set; }
        public double SocialSecurityTax { get; set; }
        public double MedicareTax { get; set; }
        public double StateTax { get; set; }

        public double TotalTax => FederalTax + SocialSecurityTax + MedicareTax + StateTax;

        /// <summary>Take-home pay: gross minus tax minus pre-tax deductions.</summary>
        public double NetAnnual => GrossAnnual - TotalTax - PreTaxDeductions;

        public double NetMonthly => NetAnnual / 12;

        /// <summary>Total tax as a share of gross — the "all-in" rate people actually feel.</summary>
        public double EffectiveTaxRate => GrossAnnual > 0 ? TotalTax / GrossAnnual : 0;
    }
}
