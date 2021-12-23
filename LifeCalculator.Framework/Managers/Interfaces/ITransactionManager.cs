using LifeCalculator.Framework.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Managers.Interfaces
{
    public interface ITransactionManager
    {
        event EventHandler<TransactionItem> AccountTransactionAdded;
        event EventHandler<TransactionItem> AccountTransactionChanged;
        event EventHandler<TransactionItem> AccountTransactionDeleted;


        TransactionItem GetAccountTransaction(int id);
        TransactionItem GetAccountTransaction(string name);
        List<TransactionItem> GetAllAccountTransactions();
        List<TransactionItem> GetAllAccountTransactionsByAccountId(string accountId);
        List<TransactionItem> GetAllAccountTransactionsThisMonth(DateTime date);
        void DeleteAccountTransaction(TransactionItem AccountTransaction);
        void AddAccountTransaction(TransactionItem AccountTransaction);
        void AddAccountTransactions(List<TransactionItem> accountTransactions);
        Task LoadFromDb();
    }
}
