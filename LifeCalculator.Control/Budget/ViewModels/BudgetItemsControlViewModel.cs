using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Budget;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace LifeCalculator.Control.ViewModels
{
    public class BudgetItemsControlViewModel : ViewModelBase
    {
        private BudgetManager _budgetManager;

        public BudgetItemsControlViewModel(BudgetManager budgetManager)
        {
            BudgetItemsList = new ObservableCollection<BudgetItemTileViewModel>();
            _budgetManager = budgetManager;

            foreach (var budgetItem in budgetManager.BudgetItems)
            {
                BudgetItemsList.Add(new BudgetItemTileViewModel(budgetItem,budgetManager));
            }

            _budgetManager.BudgetsSorted += _budgetManager_BudgetsSorted;

        }

        private void _budgetManager_BudgetsSorted(object sender, EventArgs e)
        {
            //BudgetItemsList.Clear();
            foreach (var budgetItem in _budgetManager.BudgetItems)
            {
                if (BudgetItemsList.FirstOrDefault(t => t.Name.Equals(budgetItem.Name)) ==  null)
                    BudgetItemsList.Add(new BudgetItemTileViewModel(budgetItem, _budgetManager));
            }
        }

        public ObservableCollection<BudgetItemTileViewModel> BudgetItemsList { get; set; }

    }
}
