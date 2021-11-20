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
        IBudgetManager _budgetManager;

        public BudgetItemTileViewModel(BudgetItemModel budgetItem,IBudgetManager budgetManager)
        {
            _budgetItem = budgetItem;

            _budgetManager = budgetManager;
            _budgetManager.BudgetsSorted += _budgetManager_BudgetsSorted;

            TransactionAddCommand = new DelegateCommand(TransactionAddCommandHandler);

            TransactionListViewModel = new TransactionListViewModel();

            if (budgetItem.Transactions != null)
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
            var transaction = (TransactionItemViewModel)obj;
            transaction.Transaction.BudgetCategory = Name;
            _budgetManager.ChangeTransactionCategory(transaction.Transaction.Name, Name, true);
            _budgetManager.SortByBudgetCategory();
        }

        #endregion

        private void _budgetManager_BudgetsSorted(object sender, System.EventArgs e)
        {
            TransactionListViewModel.ClearItems();
            SpentAmount = 0;
            if (_budgetItem.Transactions != null)
            foreach (var item in _budgetItem.Transactions)
            {
                    SpentAmount += item.Amount; 
                    TransactionListViewModel.AddTransactionItem(new TransactionItemViewModel(item));
            }
        }

    }
}
