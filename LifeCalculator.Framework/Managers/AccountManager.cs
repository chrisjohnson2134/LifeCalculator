using LifeCalculator.Framework.Account;
using System;
using System.Collections.Generic;
using LifeCalculator.Framework.Managers.Interfaces;
using LifeCalculator.Framework.Queries;

namespace LifeCalculator.Framework.Managers
{
    public class AccountManager : IAccountManager
    {

        public List<IAccount> Accounts { get; set; }
        public event EventHandler<IAccount> AccountAdded;

        public AccountManager()
        {
            //Accounts = LoanQueries.LoadLoanAccounts("user");
            if (Accounts == null)
                Accounts = new List<IAccount>();

        }

        public void AddAccount(IAccount account)
        {
            Accounts.Add(account);
            if (account.GetType().Equals(typeof(LoanAccount)))
                LoanQueries.Save((LoanAccount)account);
            AccountAdded?.Invoke(this, account);
        }
    }
}
