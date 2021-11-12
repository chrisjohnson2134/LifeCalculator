using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Budget;
using Microsoft.VisualStudio.PlatformUI;
using System.Collections.Generic;
using System.Windows.Input;

namespace LifeCalculator.Control.ViewModels
{
    public class BudgetItemTileViewModel : ViewModelBase
    {
        BudgetItemModel _budgetItem;

        public BudgetItemTileViewModel(BudgetItemModel budgetItem)
        {
            _budgetItem = budgetItem;
            TransactionAddCommand = new DelegateCommand(TransactionAddCommandHandler);

            TransactionListViewModel = new TransactionListViewModel();

            if(budgetItem.Transactions != null)
            foreach (var item in budgetItem.Transactions)
            {
                TransactionListViewModel.AddTransactionItem(new TransactionItemViewModel(item));
            }

        }

        public string Name
        {
            get { return _budgetItem.Name; }
            set
            {
                _budgetItem.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Description
        {
            get { return _budgetItem.Description; }
            set
            {
                _budgetItem.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public double SpentAmount
        {
            get { return _budgetItem.SpentAmount; }
            set
            {
                _budgetItem.SpentAmount = value;
                OnPropertyChanged(nameof(SpentAmount));
            }
        }

        public double PlannedAmount
        {
            get { return _budgetItem.PlannedAmount; }
            set
            {
                _budgetItem.PlannedAmount = value;
                OnPropertyChanged(nameof(PlannedAmount));
            }
        }

        public List<TransactionItem> Transactions => _budgetItem.Transactions;

        public ICommand TransactionAddCommand { get; }

        public TransactionListViewModel TransactionListViewModel { get; set; }

        public TransactionItem AddTransactionItem { get; set; }

        #region Command Handler

        private void TransactionAddCommandHandler(object obj)
        {
        }

        #endregion

    }
}
