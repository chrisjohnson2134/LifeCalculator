using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Framework.Tax
{
    public class StateTaxRate
    {
        public string Code { get; set; }
        public string Name { get; set; }

        /// <summary>Approximate effective state income tax rate, as a percent.</summary>
        public double RatePercent { get; set; }

        public StateTaxRate(string code, string name, double ratePercent)
        {
            Code = code;
            Name = name;
            RatePercent = ratePercent;
        }

        /// <summary>What the dropdown shows: "California — 6.0%".</summary>
        public string Display => RatePercent > 0
            ? $"{Name} — {RatePercent:0.##}%"
            : $"{Name} — no income tax";

        /// <summary>
        /// The ComboBox's closed-state presenter falls back to ToString() in some templated
        /// setups, which would otherwise print the type name.
        /// </summary>
        public override string ToString() => Display;
    }

    /// <summary>
    /// Approximate state income tax rates, so users can pick their state instead of having to
    /// know their own effective rate.
    ///
    /// These are ESTIMATES. States with flat taxes use their actual rate; progressive states
    /// use a typical middle-income effective rate, which will be off for very high or very low
    /// earners. Local/city income taxes (NYC, many Ohio and Pennsylvania municipalities) are
    /// not included. The rate stays editable so anyone who knows their real number can use it.
    /// </summary>
    public static class StateTaxRates
    {
        public const string CustomCode = "CUSTOM";

        public static readonly StateTaxRate Custom = new StateTaxRate(CustomCode, "Custom rate", 0);

        private static readonly List<StateTaxRate> _states = new List<StateTaxRate>
        {
            new StateTaxRate("AL", "Alabama", 5.0),
            new StateTaxRate("AK", "Alaska", 0),
            new StateTaxRate("AZ", "Arizona", 2.5),
            new StateTaxRate("AR", "Arkansas", 3.9),
            new StateTaxRate("CA", "California", 6.0),
            new StateTaxRate("CO", "Colorado", 4.4),
            new StateTaxRate("CT", "Connecticut", 5.0),
            new StateTaxRate("DE", "Delaware", 5.5),
            new StateTaxRate("DC", "District of Columbia", 6.5),
            new StateTaxRate("FL", "Florida", 0),
            new StateTaxRate("GA", "Georgia", 5.39),
            new StateTaxRate("HI", "Hawaii", 7.5),
            new StateTaxRate("ID", "Idaho", 5.695),
            new StateTaxRate("IL", "Illinois", 4.95),
            new StateTaxRate("IN", "Indiana", 3.05),
            new StateTaxRate("IA", "Iowa", 3.8),
            new StateTaxRate("KS", "Kansas", 5.25),
            new StateTaxRate("KY", "Kentucky", 4.0),
            new StateTaxRate("LA", "Louisiana", 3.0),
            new StateTaxRate("ME", "Maine", 6.75),
            new StateTaxRate("MD", "Maryland", 4.75),
            new StateTaxRate("MA", "Massachusetts", 5.0),
            new StateTaxRate("MI", "Michigan", 4.25),
            new StateTaxRate("MN", "Minnesota", 6.8),
            new StateTaxRate("MS", "Mississippi", 4.7),
            new StateTaxRate("MO", "Missouri", 4.7),
            new StateTaxRate("MT", "Montana", 5.9),
            new StateTaxRate("NE", "Nebraska", 5.2),
            new StateTaxRate("NV", "Nevada", 0),
            new StateTaxRate("NH", "New Hampshire", 0),
            new StateTaxRate("NJ", "New Jersey", 5.0),
            new StateTaxRate("NM", "New Mexico", 4.9),
            new StateTaxRate("NY", "New York", 5.5),
            new StateTaxRate("NC", "North Carolina", 4.5),
            new StateTaxRate("ND", "North Dakota", 1.95),
            new StateTaxRate("OH", "Ohio", 3.0),
            new StateTaxRate("OK", "Oklahoma", 4.75),
            new StateTaxRate("OR", "Oregon", 8.75),
            new StateTaxRate("PA", "Pennsylvania", 3.07),
            new StateTaxRate("RI", "Rhode Island", 4.75),
            new StateTaxRate("SC", "South Carolina", 6.2),
            new StateTaxRate("SD", "South Dakota", 0),
            new StateTaxRate("TN", "Tennessee", 0),
            new StateTaxRate("TX", "Texas", 0),
            new StateTaxRate("UT", "Utah", 4.65),
            new StateTaxRate("VT", "Vermont", 6.6),
            new StateTaxRate("VA", "Virginia", 5.0),
            new StateTaxRate("WA", "Washington", 0),
            new StateTaxRate("WV", "West Virginia", 4.82),
            new StateTaxRate("WI", "Wisconsin", 5.3),
            new StateTaxRate("WY", "Wyoming", 0),
            Custom
        };

        public static IReadOnlyList<StateTaxRate> All => _states;

        public static StateTaxRate FromCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return _states.First(s => s.Code == "TX");

            return _states.FirstOrDefault(s => s.Code == code) ?? Custom;
        }
    }
}
