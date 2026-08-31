using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.SimulatedAccount
{
    /// <summary>
    /// Resolves the total extra amount (from one-time and monthly-recurring life events) that
    /// applies to an account for a given month. Shared by LoanAccount, CompoundAccount, and
    /// RetirementAccount, which previously each had their own copy of this logic.
    /// </summary>
    public static class AccountEventResolver
    {
        /// <summary>
        /// Compares whole months, not exact days.
        ///
        /// Everything downstream is monthly and asks about the first of the month, while an event
        /// carries the timestamp it happened to be created at. A contribution set up on the 27th
        /// would fail a day-level "has it started yet?" test against the 1st and silently count as
        /// zero for that month — which showed up as the monthly surplus refusing to move when a
        /// contribution was edited.
        /// </summary>
        public static double ResolveAdditionalAmount(List<IAccountEvent> events, DateTime date)
        {
            double additionalAmount = 0;

            if (events == null)
                return 0;

            DateTime month = MonthOf(date);

            events.FindAll(i => i.LifeEventType == LifeEnum.MonthlyContribute
                                && MonthOf(i.StartDate) <= month
                                && month <= MonthOf(i.EndDate))
                .ForEach(i => additionalAmount += i.Amount);

            events.FindAll(i => i.LifeEventType == LifeEnum.OneTime
                                && MonthOf(i.StartDate) == month)
                .ForEach(i => additionalAmount += i.Amount);

            return additionalAmount;
        }

        private static DateTime MonthOf(DateTime date) => new DateTime(date.Year, date.Month, 1);
    }
}
