using LifeCalculator.Framework.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Framework.Budget
{
    public class BudgetManager: IBudgetManager
    {
        #region Events

        public event EventHandler BudgetsSorted;

        #endregion

        #region Fields

        #endregion

        #region Constructors

        public BudgetManager()
        {
            BudgetItems = new List<BudgetItemModel>();
            Transactions = new List<TransactionItem>();
        }

        #endregion

        #region Properties

        public List<BudgetItemModel> BudgetItems { get; set; }
        public List<TransactionItem> Transactions { get; set; }

        public bool AutoSort { get; set; }

        #endregion

        public void AddTransaction(TransactionItem transactionItem)
        {
            Transactions.Add(transactionItem);
            AddTransactionToBudgetItem(transactionItem);
        }

        public void AddTransactions(List<TransactionItem> transactionItems)
        {
            Transactions.AddRange(transactionItems);
            foreach (TransactionItem transactionItem in transactionItems)
            {
                AddTransactionToBudgetItem(transactionItem);
            }
        }

        public void RemoveTransactionById(string id)
        {
            var transaction = GetTransactionById(id);
            Transactions.Remove(transaction);
            BudgetItems.FirstOrDefault(t => t.Name.Equals(transaction.BudgetCategory)).Transactions.Remove(transaction);
        }

        public TransactionItem GetTransctionByName(string transactioName)
        {
            return transactioName == null ? null : Transactions.FirstOrDefault(t => t.Name.Equals(transactioName));
        }

        public TransactionItem GetTransactionById(string ID)
        {
            return ID == null ? null : Transactions.FirstOrDefault(t => t.Id.Equals(ID));
        }

        public void SortByBudgetCategory()
        {
            foreach (var transactionItems in Transactions.GroupBy(t => t.BudgetCategory))
            {
                var budgetCategory = BudgetItems.FirstOrDefault(t => t.Name.Equals(transactionItems.Key));
                if (budgetCategory == null)
                    BudgetItems.Add(new BudgetItemModel(transactionItems));
                else
                    budgetCategory.Transactions = transactionItems.ToList();

            }

            BudgetsSorted?.Invoke(this, new EventArgs());
        }

        #region Private Methods

        private void AddTransactionToBudgetItem(TransactionItem transactionItem)
        {
            if (!AutoSort)
                transactionItem.BudgetCategory = "Uncategorized";

            BudgetItemModel budgetItem = BudgetItems.FirstOrDefault(t => t.Name.Equals(transactionItem.BudgetCategory));
            if (budgetItem != null)
            {
                budgetItem.Transactions.Add(transactionItem);
            }
            else
            {
                budgetItem = new BudgetItemModel() { Name = transactionItem.BudgetCategory };
                budgetItem.Transactions.Add(transactionItem);
                BudgetItems.Add(budgetItem);
            }
        }

        public void AddBudgetItem(string itemName)
        {
            BudgetItems.Add(new BudgetItemModel() { Name=itemName });
        }

        #endregion

    }
}
