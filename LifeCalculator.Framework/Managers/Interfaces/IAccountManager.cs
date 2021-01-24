using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Managers.Interfaces
{
    public interface IAccountManager
    {
        List<IAccount> Accounts { get; set; }

        void AddAccount(IAccount account);
        event EventHandler<IAccount> AccountAdded;

    }
}