using LifeCalculator.Framework.ColumnDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Framework.Simulation
{
    /// <summary>
    /// Aligns every debt's post-rollover balance timeline and every growth account's own
    /// Calculation() timeline onto a shared monthly timeline, producing total assets, total
    /// debt, and net worth per month.
    /// </summary>
    public static class NetWorthAggregator
    {
        /// <summary>
        /// These are monthly projections, so everything is bucketed to the first of its month.
        /// Without this, two accounts created minutes apart carry different times-of-day and
        /// never line up: at one series' timestamps the other looks like it hasn't started,
        /// and its balance silently reads as zero.
        /// </summary>
        private static DateTime ToMonthBucket(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static List<NetWorthColumn> Aggregate(
            DebtPayoffResult debtPayoffResult,
            Dictionary<string, List<MonthlyColumn>> assetTimelines)
        {
            debtPayoffResult = debtPayoffResult ?? new DebtPayoffResult();
            assetTimelines = assetTimelines ?? new Dictionary<string, List<MonthlyColumn>>();

            var sortedDebtSeries = debtPayoffResult.BalancesByDebtName.Values
                .Select(BuildMonthlySeries)
                .ToList();

            var sortedAssetSeries = assetTimelines.Values
                .Select(BuildMonthlySeries)
                .ToList();

            var allDates = new SortedSet<DateTime>();
            foreach (var series in sortedDebtSeries)
                foreach (var entry in series)
                    allDates.Add(entry.Key);
            foreach (var series in sortedAssetSeries)
                foreach (var entry in series)
                    allDates.Add(entry.Key);

            var result = new List<NetWorthColumn>();

            foreach (var date in allDates)
            {
                double totalDebt = sortedDebtSeries.Sum(series => LatestValueAsOf(series, date));
                double totalAssets = sortedAssetSeries.Sum(series => LatestValueAsOf(series, date));

                result.Add(new NetWorthColumn
                {
                    Date = date,
                    TotalDebt = Math.Round(totalDebt, 2),
                    TotalAssets = Math.Round(totalAssets, 2),
                    NetWorth = Math.Round(totalAssets - totalDebt, 2)
                });
            }

            return result;
        }

        /// <summary>
        /// Collapses a raw calculation series into one value per month, dropping the
        /// placeholder row each Calculation() prepends (see MonthlyColumn.IsPlaceholder).
        /// When a month has several entries the last one wins — it's the month-end balance.
        /// </summary>
        private static List<KeyValuePair<DateTime, double>> BuildMonthlySeries(List<MonthlyColumn> series)
        {
            var byMonth = new SortedDictionary<DateTime, double>();

            foreach (var column in series.Where(c => !c.IsPlaceholder).OrderBy(c => c.Date))
                byMonth[ToMonthBucket(column.Date)] = column.Gain;

            return byMonth.ToList();
        }

        /// <summary>Holds the last known value at or before <paramref name="date"/> (0 if the series hasn't started yet).</summary>
        private static double LatestValueAsOf(List<KeyValuePair<DateTime, double>> sortedSeries, DateTime date)
        {
            double? last = null;

            foreach (var entry in sortedSeries)
            {
                if (entry.Key > date)
                    break;

                last = entry.Value;
            }

            return last ?? 0;
        }
    }
}
