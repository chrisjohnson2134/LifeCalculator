using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.BudgetItems;
using System.Collections.ObjectModel;

namespace LifeCalculator.Control.ViewModels
{
    public class BudgetItemsControlViewModel : ViewModelBase
    {
        public ObservableCollection<BudgetItemTileViewModel> BudgetItemsList { get; set; }

        public BudgetItemsControlViewModel()
        {
            BudgetItemsList = new ObservableCollection<BudgetItemTileViewModel>();
            BudgetItemsList.Add(new BudgetItemTileViewModel(new BudgetItemModel { Name = "Income", PlannedAmount = 100 }));
            BudgetItemsList.Add(new BudgetItemTileViewModel(new BudgetItemModel { Name = "Food", PlannedAmount = 100 }));
            BudgetItemsList.Add(new BudgetItemTileViewModel(new BudgetItemModel { Name = "Housing", PlannedAmount = 100 }));
            BudgetItemsList.Add(new BudgetItemTileViewModel(new BudgetItemModel { Name = "Utitlies", PlannedAmount = 100 }));
            BudgetItemsList.Add(new BudgetItemTileViewModel(new BudgetItemModel { Name = "Fun", PlannedAmount = 50 }));
            BudgetItemsList.Add(new BudgetItemTileViewModel(new BudgetItemModel { Name = "Food", PlannedAmount = 100 }));
            BudgetItemsList.Add(new BudgetItemTileViewModel(new BudgetItemModel { Name = "Housing", PlannedAmount = 100 }));
            BudgetItemsList.Add(new BudgetItemTileViewModel(new BudgetItemModel { Name = "Utitlies", PlannedAmount = 100 }));
            //    BudgetItemsList.Add(new BudgetItem { Name = "Fun", BudgetAmount = 50 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Food", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Housing", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Utitlies", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Fun", BudgetAmount = 50 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Food", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Housing", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Utitlies", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Fun", BudgetAmount = 50 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Food", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Housing", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Utitlies", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Fun", BudgetAmount = 50 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Food", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Housing", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Utitlies", BudgetAmount = 100 });
            //    BudgetItemsList.Add(new BudgetItem { Name = "Fun", BudgetAmount = 50 });
        }
    }
}
