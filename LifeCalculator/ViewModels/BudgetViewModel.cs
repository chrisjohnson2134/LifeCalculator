using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.BaseVM;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LifeCalculator.Commands;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.CurrentAccountStorage;

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
