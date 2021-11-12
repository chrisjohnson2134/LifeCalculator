using LifeCalculator.Framework.Accounts;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Framework.Budget
{
    public class BudgetManager: IBudgetManager
    {
        #region Fields

        #endregion

        #region Constructors

        public BudgetManager()
        {
            BudgetItems = new List<BudgetItemModel>();
            //BudgetItems.Add(new BudgetItemModel
            //{
            //    Name = "Food",
            //    Transactions = new List<TransactionItem>
            //    {
            //        new TransactionItem{Name = "Hamburger" ,BudgetCategory="Food"},
            //        new TransactionItem{Name = "HotDog" ,BudgetCategory="Food"},
            //        new TransactionItem{Name = "Blue Cheese" ,BudgetCategory="Food"},
            //    }
            //});
            //BudgetItems.Add(new BudgetItemModel { Name = "Housing" });
            //BudgetItems.Add(new BudgetItemModel { Name = "Entertainment" });
            //BudgetItems.Add(new BudgetItemModel { Name = "Car" });
            Transactions = new List<TransactionItem>();
            //SortByBudgetCategory();
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

        #endregion

    }
}
