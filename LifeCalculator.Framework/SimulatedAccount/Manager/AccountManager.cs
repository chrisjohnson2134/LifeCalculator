using LifeCalculator.Framework.Services.AccDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.SimulatedAccount.Manager
{
    public class AccountManager
    {

        public event EventHandler<ISimulatedAccount> AccountAdded;
        public event EventHandler<ISimulatedAccount> AccountChanged;
        public event EventHandler<ISimulatedAccount> AccountDeleted;

        public AccountManager()
        {
            Accounts = new List<ISimulatedAccount>();
        }

        public List<ISimulatedAccount> Accounts { get; set; }

        public void AddAccount(ISimulatedAccount account)
        {
            Accounts.Add(account);
            account.ValueChanged += Account_ValueChanged;

            AccountAdded?.Invoke(this, account);
        }

        private void Account_ValueChanged(object sender, ISimulatedAccount e)
        {
            AccountChanged?.Invoke(this, e);
        }

        public ISimulatedAccount GetAccount(int id)
        {
            return Accounts.FirstOrDefault(t => t.Id == id);
        }

        public ISimulatedAccount GetAccount(string name)
        {
            return Accounts.FirstOrDefault(t => t.Name.Equals(name));
        }

        public void DeleteAccount(ISimulatedAccount account)
        {
            Accounts.RemoveAll(t => t.Id == account.Id);

            AccountDeleted?.Invoke(this, account);
        }

    }
}
