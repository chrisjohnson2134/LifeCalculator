using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Services.PlaidAccInfoDataService;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.FrameworkTest.Managers
{
    [TestFixture]
    public class TransactionManagerTest
    {
        #region Setup

        TransactionDataService _transactionDataService = new TransactionDataService();

        public TransactionManagerTest()
        {

        }

        public TransactionsManager CreateTransactionManager()
        {
            return new TransactionsManager();
        }

        #endregion

        #region Test

        [Test]
        //1.)Insert/Delete Transaction @ database 2.)Add/Delete Item event should fire
        //3.)Transaction should be in/out list
        public async Task AddAndDeleteFuncationalityWorksWithDatabse()
        {
            var transactionManager = CreateTransactionManager();

            TransactionItem addedItem = new TransactionItem();
            TransactionItem deletedItem = new TransactionItem();

            transactionManager.AccountTransactionAdded += (obj,e) => { addedItem = e; };
            transactionManager.AccountTransactionDeleted += (obj,e) => { deletedItem = e; };

            var transaction = new TransactionItem()
            {
                AccountId = "301k",
                TransactionId = "12342321",
                Amount = 23.00,
                Date = DateTime.Parse("12/22/2021"),
                Name = "AMC Theaters",
                BudgetCategoryPlaidDefault = "[entertainment,fun]",
                BudgetCategory = "entertainment",
            };

            transactionManager.AddAccountTransaction(transaction);
            Assert.AreEqual(transaction, addedItem);
            Assert.AreEqual(transaction,transactionManager.GetAccountTransaction(transaction.Id));
            var transactionLoaded = await _transactionDataService.Load(transaction.Id);
            Assert.AreEqual(transaction, transactionLoaded);

            transactionManager.DeleteAccountTransaction(transaction);
            Assert.AreEqual(transaction, deletedItem);
            Assert.AreEqual(null, transactionManager.GetAccountTransaction(transaction.Id));

            try
            {
                transactionLoaded = await _transactionDataService.Load(transaction.Id);
                Assert.Fail("Item not deleted.");
            }catch(Exception ex)
            {
                string msg = $"TransactionItem with id [{transaction.Id}] could not be found.";
                string error = ex.Message;
                Assert.AreEqual(error, msg);
            }

        }

        [Test,Ignore("Not Implemented")]
        public void CorrectUserTransactionsLoaded()
        {

        }

        #endregion

    }
}
