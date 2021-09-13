using LifeCalculator.Framework.SimulatedAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Control.Accounts
{
    public interface IControlAccount
    {
        event EventHandler<ISimulatedAccount> AccountAdded;
        event EventHandler<ISimulatedAccount> AccountModified;
    }
}
