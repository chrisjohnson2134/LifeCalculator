using LifeCalculator.Framework.BaseVM;

namespace LifeCalculator.Control.ViewModels
{
    public class TransactionSorterViewModel : ViewModelBase
    {
        public TransactionListViewModel InProgressTransactionItemListingViewModel { get; }
        public TransactionListViewModel CompletedTransactionItemListingViewModel { get; }
        public BudgetItemsControlViewModel primaryBudgetItemsControlViewModel { get; }

        public TransactionSorterViewModel(TransactionListViewModel inProgressTransactionItemListingViewModel, TransactionListViewModel completedTransactionItemListingViewModel, BudgetItemsControlViewModel budgetItemsControlViewModel)
        {
            InProgressTransactionItemListingViewModel = inProgressTransactionItemListingViewModel;
            CompletedTransactionItemListingViewModel = completedTransactionItemListingViewModel;
            primaryBudgetItemsControlViewModel = budgetItemsControlViewModel;
        }
    }
}
