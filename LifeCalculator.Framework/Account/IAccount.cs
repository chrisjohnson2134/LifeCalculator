using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Account
{
    public interface IAccount
    {
        string Name { get; set; }
        List<ILifeEvent> AccountLifeEvents { get; set; }

        event EventHandler<ILifeEvent> LifeEventAdded;

        List<MonthlyColumn> Calculation();

        void AddLifeEvent(ILifeEvent lifeEvent);
    }
}