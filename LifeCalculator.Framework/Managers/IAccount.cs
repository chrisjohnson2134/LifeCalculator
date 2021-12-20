using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.SimulatedAccount
{
    public interface IAccount
    {
        event EventHandler<IAccount> ValueChanged;

        string Name { get; set; }
        int Id { get; set; }
        int UserId { get; set; }

        List<MonthlyColumn> Calculation();//REMOVE
    }
}