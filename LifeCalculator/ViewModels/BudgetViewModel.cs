using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.BaseVM;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LifeCalculator.Commands;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.CurrentAccountStorage;
using System.Collections.Generic;

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

            TransactionSorterControl = new TransactionSorterViewModel(_budgetManager);

        }

        #endregion

        #region Properties

        #region Budget Item Collections

        public TransactionSorterViewModel TransactionSorterControl { get; set; }


        #endregion

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
