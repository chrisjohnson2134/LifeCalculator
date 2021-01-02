using LifeCalculator.Framework.Account;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.AccountManager
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
