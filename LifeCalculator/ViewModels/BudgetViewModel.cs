﻿using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.BaseVM;
using System.Windows.Input;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.CurrentAccountStorage;
using System.Collections.Generic;
using Microsoft.VisualStudio.PlatformUI;
using System;

namespace LifeCalculator.ViewModels
{
    public class BudgetViewModel : ViewModelBase
    {
        #region Fields

        private BudgetManager _budgetManager;

        #endregion

        #region Constructors

        public BudgetViewModel(IAccountStore accountStore)
        {
            _budgetManager = accountStore.CurrentAccount.BudgetManager;

            List<TransactionItem> transactionItemsMocked = new List<TransactionItem>()
            {
                new TransactionItem{Name = "Hamburger" ,BudgetCategory="Uncatogorized",Amount=50},
                new TransactionItem{Name = "HotDog" ,BudgetCategory="Uncatogorized",Amount=50},
                new TransactionItem{Name = "Blue Cheese" ,BudgetCategory="Uncatogorized",Amount=50},

                new TransactionItem{Name = "Gas" ,BudgetCategory="Uncatogorized",Amount=50},
                new TransactionItem{Name = "Maintenance" ,BudgetCategory="Uncatogorized",Amount=50},
                new TransactionItem{Name = "Wiper Blades" ,BudgetCategory="Uncatogorized",Amount=50},

                new TransactionItem{Name = "Paint" ,BudgetCategory="Uncatogorized",Amount=50},
                new TransactionItem{Name = "Plants" ,BudgetCategory="Uncatogorized",Amount=50},
                new TransactionItem{Name = "Chairs" ,BudgetCategory="Uncatogorized",Amount=50},
            };

            _budgetManager.AutoSort = true;
            _budgetManager.AddTransactions(transactionItemsMocked);

            AddBudgetItemCommand = new DelegateCommand(AddBudgetItemcommandHandler);

            TransactionSorterControl = new TransactionSorterViewModel(_budgetManager);
            _budgetManager.SortByBudgetCategory();

        }

        

        #endregion

        #region Properties

        #region Budget Item Collections

        public TransactionSorterViewModel TransactionSorterControl { get; set; }


        #endregion

        public string AddBudgetITemName { get; set; }
        public double AddBudgetITemPlannedAmount { get; set; }

        public ICommand AddBudgetItemCommand { get; set; }

        public double MonthlyIncome
        {
            get
            {
                var monthlyIncome = 0.0;

                return monthlyIncome;
            }
           
        }


        #endregion

        #region Command Handlers

        private void AddBudgetItemcommandHandler(object obj)
        {
            if (string.IsNullOrEmpty(AddBudgetITemName))
                return;

            _budgetManager.AddBudgetItem(AddBudgetITemName, AddBudgetITemPlannedAmount);

            AddBudgetITemName = string.Empty;
        }

        #endregion

    }
}
