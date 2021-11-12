using LifeCalculator.Framework.Accounts;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Budget
{
    public interface IBudgetManager
    {
        bool AutoSort { get; set; }

        List<BudgetItemModel> BudgetItems { get; set; }
        List<TransactionItem> Transactions { get; }

        TransactionItem GetTransctionByName(string transactioName);
        TransactionItem GetTransactionById(string ID);
        void AddTransaction(TransactionItem transactionItem);
        void AddTransactions(List<TransactionItem> transactionItems);
        void RemoveTransactionById(string v);

        void SortByBudgetCategory();

    }
}
