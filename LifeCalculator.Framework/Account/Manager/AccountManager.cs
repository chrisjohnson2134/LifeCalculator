using LifeCalculator.Framework.Services.AccDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Account.Manager
{
    public class AccountManager
    {

        public event EventHandler<IAccount> AccountAdded;
        public event EventHandler<IAccount> AccountChanged;
        public event EventHandler<IAccount> AccountDeleted;

        public AccountManager()
        {
            Accounts = new List<IAccount>();
        }

        public List<IAccount> Accounts { get; set; }

        public void AddAccount(IAccount account)
        {
            Accounts.Add(account);
            account.ValueChanged += Account_ValueChanged;

            AccountAdded?.Invoke(this, account);
        }

        private void Account_ValueChanged(object sender, IAccount e)
        {
            AccountChanged?.Invoke(this, e);
        }

        public IAccount GetAccount(int id)
        {
            return Accounts.FirstOrDefault(t => t.Id == id);
        }

        public IAccount GetAccount(string name)
        {
            return Accounts.FirstOrDefault(t => t.Name.Equals(name));
        }

        public void DeleteAccount(IAccount account)
        {
            Accounts.RemoveAll(t => t.Id == account.Id);

            AccountDeleted?.Invoke(this, account);
        }

    }
}
