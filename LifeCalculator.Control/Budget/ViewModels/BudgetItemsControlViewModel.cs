using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Budget;
using System.Collections.ObjectModel;

namespace LifeCalculator.Control.ViewModels
{
    public class BudgetItemsControlViewModel : ViewModelBase
    {
        public ObservableCollection<BudgetItemTileViewModel> BudgetItemsList { get; set; }


        public BudgetItemsControlViewModel(IBudgetManager budgetManager)
        {
            BudgetItemsList = new ObservableCollection<BudgetItemTileViewModel>();
            foreach (var budgetItem in budgetManager.BudgetItems)
            {
                BudgetItemsList.Add(new BudgetItemTileViewModel(budgetItem,budgetManager));
            }

        }
    }
}
