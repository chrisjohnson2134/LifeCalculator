using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Services.TransactionDataService;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace LifeCalculator.FrameworkTest.Services.TransactionService
{
    [TestFixture]
    public class TransactionDataServiceTest
    {
        #region Setup
        #endregion

        #region Test

        [Test]
        public async Task CRUDTest()
        {
            TransactionDataService transactionDataService = new TransactionDataService();

            TransactionItem transItem = new TransactionItem
            {
                Name = "Cookies",
                BudgetCategory = "Food",
                BudgetCategoryPlaidDefault = "Food",
                AccountId = "FoodACC",
                Amount = 50,
                Date = DateTime.Now,
                TransactionId = "id1"
            };

            var output = await transactionDataService.Insert(transItem);

            Assert.IsNotNull(output);

            Assert.AreEqual(output.TransactionId, output.TransactionId);

            output.Amount = 100;

            await transactionDataService.Save(output.Id, output);

            var updatedOutput = await transactionDataService.Load(output.Id);

            Assert.AreEqual(100.0, updatedOutput.Amount);

            await transactionDataService.Delete(output.Id);

            try
            {
                updatedOutput = await transactionDataService.Load(output.Id);
            }
            catch
            {
                updatedOutput = null;
            }

            Assert.IsNull(updatedOutput);

        }

        #endregion

    }
}
