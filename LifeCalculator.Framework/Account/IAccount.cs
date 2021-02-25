using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Account
{
    public interface IAccount
    {
        string Name { get; set; }
        int Id { get; }
        int UserId { get; set; }
        List<IAccountEvent> AccountLifeEvents { get; set; }

        event EventHandler<IAccountEvent> LifeEventAdded;

        List<MonthlyColumn> Calculation();

        void AddLifeEvent(IAccountEvent lifeEvent);
    }
}