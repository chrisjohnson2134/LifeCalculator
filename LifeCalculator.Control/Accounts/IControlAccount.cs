using LifeCalculator.Framework.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Control.Accounts
{
    public interface IControlAccount
    {
        event EventHandler<IAccount> AccountAdded;
        event EventHandler<IAccount> AccountModified;
    }
}
