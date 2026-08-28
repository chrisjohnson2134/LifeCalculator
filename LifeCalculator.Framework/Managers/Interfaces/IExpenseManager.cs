using LifeCalculator.Framework.Budget;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Managers
{
    public interface IExpenseManager
    {
        event EventHandler<ExpenseItem> ExpenseAdded;
        event EventHandler<ExpenseItem> ExpenseChanged;
        event EventHandler<ExpenseItem> ExpenseDeleted;

        void AddExpense(ExpenseItem expense);
        void DeleteExpense(ExpenseItem expense);
        List<ExpenseItem> GetAllExpenses();
        double GetTotalMonthlyExpenses();
        Task LoadFromDb(int userId);
    }
}
