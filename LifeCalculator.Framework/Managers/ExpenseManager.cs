using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.Services.ExpenseDataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Managers
{
    public class ExpenseManager : IExpenseManager
    {
        public event EventHandler<ExpenseItem> ExpenseAdded;
        public event EventHandler<ExpenseItem> ExpenseChanged;
        public event EventHandler<ExpenseItem> ExpenseDeleted;

        private readonly ExpenseItemDataService _dataService;
        private readonly List<ExpenseItem> _expenses = new List<ExpenseItem>();

        public ExpenseManager()
        {
            _dataService = new ExpenseItemDataService();
        }

        public List<ExpenseItem> GetAllExpenses()
        {
            return _expenses;
        }

        public double GetTotalMonthlyExpenses()
        {
            return _expenses.Sum(e => e.MonthlyAmount);
        }

        public void AddExpense(ExpenseItem expense)
        {
            addExpenseAsync(expense);
        }

        public void DeleteExpense(ExpenseItem expense)
        {
            deleteExpenseAsync(expense);
        }

        public async Task LoadFromDb(int userId)
        {
            var loaded = await _dataService.LoadByUserId(userId);
            foreach (var expense in loaded)
                addExpenseAsync(expense);
        }

        private void Expense_ValueChanged(object sender, ExpenseItem e)
        {
            saveExpenseAsync(e);
        }

        private async void addExpenseAsync(ExpenseItem expense)
        {
            if (expense.Id == -1)
            {
                var inserted = await _dataService.Insert(expense);
                expense.Id = inserted.Id;
            }

            _expenses.Add(expense);
            expense.ValueChanged += Expense_ValueChanged;

            ExpenseAdded?.Invoke(this, expense);
        }

        private async void saveExpenseAsync(ExpenseItem expense)
        {
            await _dataService.Save(expense.Id, expense);
            ExpenseChanged?.Invoke(this, expense);
        }

        private async void deleteExpenseAsync(ExpenseItem expense)
        {
            await _dataService.Delete(expense.Id);
            _expenses.RemoveAll(t => t.Id == expense.Id);
            ExpenseDeleted?.Invoke(this, expense);
        }
    }
}
