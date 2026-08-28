using LifeCalculator.Framework.Enums;
using System;

namespace LifeCalculator.Framework.Budget
{
    /// <summary>
    /// A recurring monthly expense (rent, groceries, insurance...). This is the single source
    /// of truth for planned spending: the Budget screen edits these, and the Life Calculator's
    /// cash-flow projection subtracts them, so the two screens always agree.
    /// </summary>
    public class ExpenseItem
    {
        public event EventHandler<ExpenseItem> ValueChanged;

        public int Id { get; set; } = -1;

        private int _userId;
        public int UserId
        {
            get => _userId;
            set { _userId = value; ValueChanged?.Invoke(this, this); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; ValueChanged?.Invoke(this, this); }
        }

        private double _monthlyAmount;
        public double MonthlyAmount
        {
            get => _monthlyAmount;
            set { _monthlyAmount = value; ValueChanged?.Invoke(this, this); }
        }

        private BudgetItemSection _category;
        public BudgetItemSection Category
        {
            get => _category;
            set { _category = value; ValueChanged?.Invoke(this, this); }
        }
    }
}
