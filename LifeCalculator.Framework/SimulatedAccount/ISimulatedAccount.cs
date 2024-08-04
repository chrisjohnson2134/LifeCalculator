using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.SimulatedAccount
{
    public interface ISimulatedAccount : IAccount
    {
        List<MonthlyColumn> Calculation();
        void AddLifeEvent(IAccountEvent lifeEvent);

    }
}
