using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Budget;
using System.Collections.ObjectModel;

namespace LifeCalculator.Control.ViewModels
{
    public class TransactionSorterViewModel : ViewModelBase
    {
        public IBudgetManager _budgetManager;

        public TransactionSorterViewModel(IBudgetManager budgetManager)
        {
            _budgetManager = budgetManager;

            primaryBudgetItemsControlViewModel = new BudgetItemsControlViewModel(_budgetManager);
        }

        public BudgetItemsControlViewModel primaryBudgetItemsControlViewModel { get; }
        public IBudgetManager budgetManager => _budgetManager;

    }
}
