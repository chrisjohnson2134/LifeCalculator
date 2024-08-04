using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Budget;
using System.Collections.ObjectModel;

namespace LifeCalculator.Control.ViewModels
{
    public class TransactionSorterViewModel : ViewModelBase
    {
        public BudgetManager _budgetManager;

        public TransactionSorterViewModel(BudgetManager budgetManager)
        {
            _budgetManager = budgetManager;

            primaryBudgetItemsControlViewModel = new BudgetItemsControlViewModel(_budgetManager);
        }

        public BudgetItemsControlViewModel primaryBudgetItemsControlViewModel { get; }
        public BudgetManager budgetManager => _budgetManager;

    }
}
