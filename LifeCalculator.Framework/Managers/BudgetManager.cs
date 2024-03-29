﻿using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Framework.Budget
{
    public class BudgetManager
    {
        private ITransactionManager _transactionManager;
        #region Events

        public event EventHandler BudgetsSorted;

        #endregion

        #region Fields

        #endregion

        #region Constructors

        public BudgetManager()
        {
            BudgetItems = new List<BudgetItemModel>();
        }

        public BudgetManager(ITransactionManager transactionManager)
        {
            BudgetItems = new List<BudgetItemModel>();
            _transactionManager = transactionManager;

            Transactions = _transactionManager.GetAllAccountTransactionsThisMonth(DateTime.Now);
        }

        #endregion

        #region Properties

        public List<BudgetItemModel> BudgetItems { get; set; }
        public List<TransactionItem> Transactions { get; set; }//needs to be maybe more specific??//=> _transactionManager.GetAllAccountTransactions();

        public bool AutoSort { get; set; }

        #endregion

        #region Public Methods

        public void AddTransaction(TransactionItem transactionItem)
        {
            Transactions.Add(transactionItem);
            AddTransactionToBudgetItem(transactionItem);
        }

        public void AddTransactions(List<TransactionItem> transactionItems)
        {
            foreach (TransactionItem transactionItem in transactionItems)
            {
                AddTransactionToBudgetItem(transactionItem);
            }
            Transactions.AddRange(transactionItems);
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
            return ID == null ? null : Transactions.FirstOrDefault(t => t.TransactionId.Equals(ID));
        }

        public void ChangeTransactionCategory(string transactionName, string newCategoryName, bool changeForAll = false)
        {
            if (changeForAll)
            {
                var transactionList = Transactions.Where(t => t.Name.Equals(transactionName));
                foreach (var transaction in transactionList)
                {
                    transaction.BudgetCategory = newCategoryName;
                }
            }
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

        public void AddBudgetItem(string itemName, double budgetAmount)
        {
            BudgetItems.Add(new BudgetItemModel() { Name = itemName, PlannedAmount = budgetAmount });
            SortByBudgetCategory();
        }

        public void AddBudgetItem(BudgetItemModel budgetItem)
        {
            budgetItem.SpentAmount = 0;
            budgetItem.PlannedAmount = 100;
            BudgetItems.Add(budgetItem);
            SortByBudgetCategory();
        }

        #endregion

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
                AddBudgetItem(budgetItem);
            }
        }

        #endregion

    }
}
