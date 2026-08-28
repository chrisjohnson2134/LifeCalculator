using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Control.ViewModels
{
    public class ExpenseRowViewModel : ValidatableViewModelBase
    {
        private readonly ExpenseItem _expense;
        private readonly IExpenseManager _expenseManager;

        public ExpenseRowViewModel(ExpenseItem expense, IExpenseManager expenseManager)
        {
            _expense = expense;
            _expenseManager = expenseManager;

            DeleteExpenseCommand = new RelayCommand(DeleteExpense);

            ValidateAll();
        }

        public int Id => _expense.Id;

        public List<BudgetItemSection> Categories { get; } =
            Enum.GetValues(typeof(BudgetItemSection)).Cast<BudgetItemSection>().ToList();

        public string Name
        {
            get => _expense.Name;
            set
            {
                Validate(nameof(Name), () => !string.IsNullOrWhiteSpace(value), "Name is required.");

                if (!string.IsNullOrWhiteSpace(value))
                {
                    _expense.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public double MonthlyAmount
        {
            get => _expense.MonthlyAmount;
            set
            {
                Validate(nameof(MonthlyAmount), () => value >= 0, "Amount cannot be negative.");

                if (value >= 0)
                {
                    _expense.MonthlyAmount = value;
                    OnPropertyChanged(nameof(MonthlyAmount));
                }
            }
        }

        public BudgetItemSection Category
        {
            get => _expense.Category;
            set
            {
                _expense.Category = value;
                OnPropertyChanged(nameof(Category));
            }
        }

        public IRelayCommand DeleteExpenseCommand { get; }

        private void ValidateAll()
        {
            Validate(nameof(Name), () => !string.IsNullOrWhiteSpace(_expense.Name), "Name is required.");
            Validate(nameof(MonthlyAmount), () => _expense.MonthlyAmount >= 0, "Amount cannot be negative.");
        }

        private void DeleteExpense()
        {
            _expenseManager.DeleteExpense(_expense);
        }
    }
}
