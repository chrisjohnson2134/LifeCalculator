using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.BaseVM;
using System.Windows.Input;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.CurrentAccountStorage;
using System.Collections.Generic;
using Microsoft.VisualStudio.PlatformUI;

namespace LifeCalculator.ViewModels
{
    public class BudgetViewModel : ViewModelBase
    {
        #region Fields

        private IBudgetManager _budgetManager;

        #endregion

        #region Constructors

        public BudgetViewModel(IAccountStore accountStore)
        {
            _budgetManager = accountStore.CurrentAccount.BudgetManager;

            List<TransactionItem> transactionItemsMocked = new List<TransactionItem>()
            {
                new TransactionItem{Name = "Hamburger" ,BudgetCategory="Food"},
                new TransactionItem{Name = "HotDog" ,BudgetCategory="Food"},
                new TransactionItem{Name = "Blue Cheese" ,BudgetCategory="Food"},

                new TransactionItem{Name = "Gas" ,BudgetCategory="Car"},
                new TransactionItem{Name = "Maintenance" ,BudgetCategory="Car"},
                new TransactionItem{Name = "Wiper Blades" ,BudgetCategory="Car"},

                new TransactionItem{Name = "Paint" ,BudgetCategory="House"},
                new TransactionItem{Name = "Plants" ,BudgetCategory="House"},
                new TransactionItem{Name = "Chairs" ,BudgetCategory="House"},
            };

            _budgetManager.AutoSort = true;
            _budgetManager.AddTransactions(transactionItemsMocked);

            AddBudgetItemCommand = new DelegateCommand(AddBudgetItemcommandHandler);

            TransactionSorterControl = new TransactionSorterViewModel(_budgetManager);

        }

        private void AddBudgetItemcommandHandler(object obj)
        {
            if (string.IsNullOrEmpty(AddBudgetITemName))
                return;

            _budgetManager.AddBudgetItem(AddBudgetITemName);

            AddBudgetITemName = string.Empty;
        }

        #endregion

        #region Properties

        #region Budget Item Collections

        public TransactionSorterViewModel TransactionSorterControl { get; set; }


        #endregion

        public string AddBudgetITemName { get; set; }

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

        #region Initialize Methods

        #endregion
    }
}
