using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Budget;
using System.Collections.Generic;

namespace LifeCalculator.FrameworkTest.Budget
{
    public class BudgetManagerTestContext
    {
        #region Setup


        public readonly List<TransactionItem> transactionItems = new List<TransactionItem>();

        public readonly TransactionItem transactionItem1 = new TransactionItem() { TransactionId = "1", Name = "Item1", BudgetCategory = "Entertainment" };
        public readonly TransactionItem transactionItem2 = new TransactionItem() { TransactionId = "2", Name = "Item2", BudgetCategory = "Food" };
        public readonly TransactionItem transactionItem3 = new TransactionItem() { TransactionId = "3", Name = "Item3", BudgetCategory = "Housing" };
        public readonly TransactionItem transactionItem4 = new TransactionItem() { TransactionId = "4", Name = "Item4", BudgetCategory = "Entertainment" };

        public BudgetManagerTestContext()
        {
            transactionItems = new List<TransactionItem>()
            {
                transactionItem1, transactionItem2, transactionItem3, transactionItem4
            };
        }

        public BudgetManager CreateBudgetManager(bool autoSort = true)
        {
            BudgetManager budgetManager = new BudgetManager();
            budgetManager.AutoSort = autoSort;
            return budgetManager;
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
