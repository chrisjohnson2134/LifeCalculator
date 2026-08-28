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
        public static double ResolveAdditionalAmount(List<IAccountEvent> events, DateTime date)
        {
            double additionalAmount = 0;

            events.FindAll(i => i.StartDate <= date && date <= i.EndDate && i.LifeEventType == LifeEnum.MonthlyContribute)
                .ForEach(i => additionalAmount += i.Amount);

            events.FindAll(i => i.StartDate.Year == date.Year && date.Month == i.StartDate.Month && i.LifeEventType == LifeEnum.OneTime)
                .ForEach(i => additionalAmount += i.Amount);

            return additionalAmount;
        }
    }
}
