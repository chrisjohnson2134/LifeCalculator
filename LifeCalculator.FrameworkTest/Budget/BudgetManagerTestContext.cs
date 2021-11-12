using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Budget;
using System.Collections.Generic;

namespace LifeCalculator.FrameworkTest.Budget
{
    public class BudgetManagerTestContext
    {
        #region Setup


        public List<TransactionItem> transactionItems = new List<TransactionItem>();

        public TransactionItem transactionItem1 = new TransactionItem() { Id = "1", Name = "Item1", BudgetCategory = "Entertainment" };
        public TransactionItem transactionItem2 = new TransactionItem() { Id = "2", Name = "Item2", BudgetCategory = "Food" };
        public TransactionItem transactionItem3 = new TransactionItem() { Id = "3", Name = "Item3", BudgetCategory = "Housing" };
        public TransactionItem transactionItem4 = new TransactionItem() { Id = "4", Name = "Item4", BudgetCategory = "Entertainment" };

        public BudgetManagerTestContext()
        {
            transactionItems = new List<TransactionItem>()
            {
                transactionItem1, transactionItem2, transactionItem3, transactionItem4
            };
        }

        public BudgetManager CreateBudgetManager()
        {
            return new BudgetManager();
        }

        public BudgetManager CreateBudgetManagerWithData(bool autoSort = true)
        {
            BudgetManager budgetManager = new BudgetManager();
            budgetManager.AutoSort = autoSort;
            budgetManager.AddTransactions(transactionItems);
            return budgetManager;
        }

        #endregion
    }
}
