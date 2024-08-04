using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Managers.Interfaces;
using LifeCalculator.Framework.Services.PlaidAccInfoDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Managers
{
    public class TransactionsManager : ITransactionManager
    {
        public event EventHandler<TransactionItem> AccountTransactionAdded;
        public event EventHandler<TransactionItem> AccountTransactionChanged;
        public event EventHandler<TransactionItem> AccountTransactionDeleted;

        TransactionDataService _accountTransactionDataService;

        public TransactionsManager()
        {
            _accountTransactionDataService = new TransactionDataService();
        }

        private List<TransactionItem> AccountTransactions = new List<TransactionItem>();

        public TransactionItem GetAccountTransaction(int id)
        {
            return AccountTransactions.FirstOrDefault(t => t.Id == id);
        }

        public TransactionItem GetAccountTransaction(string name)
        {
            return AccountTransactions.FirstOrDefault(t => t.Name.Equals(name));
        }

        public List<TransactionItem> GetAllAccountTransactions()
        {
            return AccountTransactions;
        }

        public List<TransactionItem> GetAllAccountTransactionsByAccountId(string accountId)
        {
            return AccountTransactions.Where(t => t.AccountId == accountId).ToList();
        }

        public List<TransactionItem> GetAllAccountTransactionsThisMonth(DateTime date)
        {
            return AccountTransactions.Where(t => t.Date.Month == date.Month && t.Date.Year == date.Year).ToList();
        }

        public void DeleteAccountTransaction(TransactionItem AccountTransaction)
        {
            deleteAccountTransactionAsync(AccountTransaction);
        }

        #region Transaction Handlers

        public void AddAccountTransaction(TransactionItem AccountTransaction)
        {
            addAccountAccountTransactionAsync(AccountTransaction);
        }

        public void AddAccountTransactions(List<TransactionItem> accountTransactions)
        {
            foreach (TransactionItem accountTransaction in accountTransactions)
                addAccountAccountTransactionAsync(accountTransaction);
        }

        private void AccountTransaction_ValueChanged(object sender, TransactionItem e)
        {
            saveAccountTransactionAsync(e);
        }

        #endregion

        #region Private

        private async void addAccountAccountTransactionAsync(TransactionItem AccountTransaction)
        {
            if (AccountTransaction.Id == -1)
            {
                TransactionItem acc = await _accountTransactionDataService.Insert(AccountTransaction);
                AccountTransaction.Id = acc.Id;
            }

            AccountTransactions.Add(AccountTransaction);

            AccountTransactionAdded?.Invoke(this, AccountTransaction);
        }

        private async void saveAccountTransactionAsync(TransactionItem AccountTransaction)
        {
            await _accountTransactionDataService.Save(AccountTransaction.Id, AccountTransaction);
            AccountTransactionChanged?.Invoke(this, AccountTransaction);
        }

        private async void deleteAccountTransactionAsync(TransactionItem AccountTransaction)
        {
            await _accountTransactionDataService.Delete(AccountTransaction.Id);

            AccountTransactions.RemoveAll(t => t.Id == AccountTransaction.Id);
            AccountTransactionDeleted?.Invoke(this, AccountTransaction);
        }

        public async Task LoadFromDb()
        {
            var listLoaded = await _accountTransactionDataService.LoadAll();
            foreach (var accountTransaction in listLoaded)
                addAccountAccountTransactionAsync(accountTransaction);
        }

        #endregion
    }
}

