using LifeCalculator.Framework.Account;
using System;
using System.Collections.Generic;
using LifeCalculator.Framework.Managers.Interfaces;

namespace LifeCalculator.Framework.Managers
{
    public class AccountManager : IAccountManager
    {

        public List<IAccount> Accounts { get; set; }
        public event EventHandler<IAccount> AccountAdded;

        public AccountManager()
        {
            Accounts = new List<IAccount>();
        }

        public void AddAccount(IAccount account)
        {
            Accounts.Add(account);
            AccountAdded?.Invoke(this, account);
        }
    }
}
