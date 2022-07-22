using LifeCalculator.Framework.Services.AccDataService;
using LifeCalculator.Framework.SimulatedAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Managers
{
    public class AccountManager
    {

        public event EventHandler<IAccount> AccountAdded;
        public event EventHandler<IAccount> AccountChanged;
        public event EventHandler<IAccount> AccountDeleted;

        AccountDataService accountService;


        public AccountManager()
        {
            Accounts = new List<IAccount>();
            accountService = new AccountDataService();
        }

        private List<IAccount> Accounts { get; set; }

        public void AddAccount(IAccount account,bool addedFromDB = false)
        {
            addAccountAsync(account, addedFromDB);
        }

        private void Account_ValueChanged(object sender, IAccount e)
        {
            saveAccountAsync(e);
        }

        public IAccount GetAccount(int id)
        {
            return Accounts.FirstOrDefault(t => t.Id == id);
        }

        public IAccount GetAccount(string name)
        {
            return Accounts.FirstOrDefault(t => t.Name.Equals(name));
        }

        public List<IAccount> GetAllAccounts()
        {
            return Accounts;
        }

        public void DeleteAccount(IAccount account)
        {
            deleteAccountAsync(account);
        }

        #region Private

        private async void addAccountAsync(IAccount account,bool addedFromDB)
        {
            if(!addedFromDB)
            {
                IAccount acc = await accountService.Insert(account);
                account.Id = acc.Id;
            }
            
            Accounts.Add(account);
            account.ValueChanged += Account_ValueChanged;

            AccountAdded?.Invoke(this, account);
        }

        private async void saveAccountAsync(IAccount account)
        {
            await accountService.Save(account);
            AccountChanged?.Invoke(this, account);
        }

        private async void deleteAccountAsync(IAccount account)
        {
            await accountService.Delete(account);

            Accounts.RemoveAll(t => t.Id == account.Id);
            AccountDeleted?.Invoke(this, account);
        }

        #endregion



    }
}
