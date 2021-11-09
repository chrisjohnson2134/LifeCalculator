using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.BudgetItems;
using Microsoft.VisualStudio.PlatformUI;
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

        public ICommand TransactionAddCommand { get; }

        public TransactionItem AddTransactionItem { get; set; }

        #region Command Handler

        private void TransactionAddCommandHandler(object obj)
        {
        }

        #endregion

    }
}
