using System.Collections.Generic;

namespace LifeCalculator.Framework.Tax
{
    public class HouseholdTaxEstimate
    {
        /// <summary>Combined gross from every stream entered as gross.</summary>
        public double GrossAnnual { get; set; }

        /// <summary>Streams the user entered as already-take-home; untouched by the estimate.</summary>
        public double AlreadyNetAnnual { get; set; }

        public double PreTaxDeductions { get; set; }

        public double FederalTax { get; set; }
        public double SocialSecurityTax { get; set; }
        public double MedicareTax { get; set; }
        public double SelfEmploymentTax { get; set; }
        public double StateTax { get; set; }

        public double TotalTax => FederalTax + SocialSecurityTax + MedicareTax + SelfEmploymentTax + StateTax;

        /// <summary>Take-home from the gross streams only.</summary>
        public double NetFromGrossAnnual => GrossAnnual - TotalTax - PreTaxDeductions;

        /// <summary>Everything that actually lands in the account, across all streams.</summary>
        public double TotalNetAnnual => NetFromGrossAnnual + AlreadyNetAnnual;

        public double TotalNetMonthly => TotalNetAnnual / 12;

        public double EffectiveTaxRate => GrossAnnual > 0 ? TotalTax / GrossAnnual : 0;

        /// <summary>Per-stream take-home, keyed by stream id, so each row can show its own number.</summary>
        public Dictionary<int, double> NetMonthlyByStreamId { get; } = new Dictionary<int, double>();
    }
}
