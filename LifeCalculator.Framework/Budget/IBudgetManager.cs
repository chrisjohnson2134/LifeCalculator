using LifeCalculator.Framework.Accounts;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Budget
{
    public interface IBudgetManager
    {
        event EventHandler BudgetsSorted;

        bool AutoSort { get; set; }

        List<BudgetItemModel> BudgetItems { get; set; }
        List<TransactionItem> Transactions { get; }

        TransactionItem GetTransctionByName(string transactioName);
        TransactionItem GetTransactionById(string ID);
        void AddTransaction(TransactionItem transactionItem);
        void AddTransactions(List<TransactionItem> transactionItems);
        void RemoveTransactionById(string v);

        void AddBudgetItem(string itemName);

        void SortByBudgetCategory();

    }
}
