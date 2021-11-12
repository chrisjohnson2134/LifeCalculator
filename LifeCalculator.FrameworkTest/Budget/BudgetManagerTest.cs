using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Budget;
using NUnit.Framework;

namespace LifeCalculator.FrameworkTest.Budget
{
    [TestFixture]
    public class BudgetManagerTest : BudgetManagerTestContext
    {

        #region Tests

        [Test]
        public void AutoSortBudgetCategoryAutoSortDisabled()
        {
            IBudgetManager budgetManager = CreateBudgetManagerWithData(false);

            var uncategorizedCategory = budgetManager.BudgetItems.Find(t => t.Name.Equals("Uncategorized"));
            Assert.AreEqual(4, uncategorizedCategory.Transactions.Count);
        }

        [Test]
        public void SortByBudgetCategoryAutoSortAndRemoveItem()
        {
            IBudgetManager budgetManager = CreateBudgetManagerWithData();

            TransactionItem newTransactionItem = new TransactionItem() { Id = "5", Name = "New item", BudgetCategory = "Food" };

            budgetManager.AddTransaction(newTransactionItem);

            var entertainmentCategory = budgetManager.BudgetItems.Find(t => t.Name.Equals("Entertainment"));
            Assert.AreEqual(2, entertainmentCategory.Transactions.Count);
            Assert.AreEqual(entertainmentCategory.Transactions.Find(t => t.Id.Equals("1")), transactionItem1);
            Assert.AreEqual(entertainmentCategory.Transactions.Find(t => t.Id.Equals("4")), transactionItem4);

            var foodCategory = budgetManager.BudgetItems.Find(t => t.Name.Equals("Food"));
            Assert.AreEqual(2, foodCategory.Transactions.Count);
            Assert.AreEqual(foodCategory.Transactions.Find(t => t.Id.Equals("2")), transactionItem2);
            Assert.AreEqual(foodCategory.Transactions.Find(t => t.Id.Equals("5")), newTransactionItem);

            var HousingCategory = budgetManager.BudgetItems.Find(t => t.Name.Equals("Housing"));
            Assert.AreEqual(1, HousingCategory.Transactions.Count);
            Assert.AreEqual(HousingCategory.Transactions.Find(t => t.Id.Equals("3")), transactionItem3);

            budgetManager.RemoveTransactionById("5");
            Assert.AreEqual(1, foodCategory.Transactions.Count);
            Assert.IsNull(foodCategory.Transactions.Find(t => t.Id.Equals("5")));

        }

        [Test]
        public void AddAndGetByIdAndName()
        {
            IBudgetManager budgetManager = CreateBudgetManager();
            string testID = transactionItem1.Id;
            string testName = transactionItem1.Name;

            budgetManager.AddTransaction(transactionItem1);

            Assert.AreEqual(1, budgetManager.Transactions.Count);

            Assert.AreEqual(budgetManager.GetTransactionById(testID), transactionItem1);
            Assert.AreEqual(budgetManager.GetTransactionById("null"), null);

            Assert.AreEqual(budgetManager.GetTransctionByName(testName), transactionItem1);
            Assert.AreEqual(budgetManager.GetTransctionByName("null"), null);

        }

        [Test]
        public void AddMultipleTransactions()
        {
            var budgetManager = CreateBudgetManager();

            budgetManager.AddTransactions(transactionItems);

            Assert.AreEqual(transactionItems, budgetManager.Transactions);

        }

        #endregion
    }
}
