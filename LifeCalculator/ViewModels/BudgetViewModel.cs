using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Income;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Simulation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LifeCalculator.ViewModels
{
    /// <summary>
    /// The Budget screen owns the user's recurring monthly expenses. These same expenses drive
    /// the Life Calculator's monthly-surplus projection, so the two screens can't disagree.
    /// </summary>
    public class BudgetViewModel : ValidatableViewModelBase
    {
        private readonly IAccountStore _accountStore;
        private readonly IExpenseManager _expenseManager;

        public BudgetViewModel(IAccountStore accountStore)
        {
            _accountStore = accountStore;
            _expenseManager = accountStore.CurrentAccount.ExpenseManager;

            Expenses = new ObservableCollection<ExpenseRowViewModel>();

            _expenseManager.ExpenseAdded += ExpenseManager_Changed;
            _expenseManager.ExpenseChanged += ExpenseManager_Changed;
            _expenseManager.ExpenseDeleted += ExpenseManager_Deleted;

            AddExpenseCommand = new RelayCommand(AddExpense, () => !HasErrors);
            LinkCommandToValidation(AddExpenseCommand);
            AddStarterExpensesCommand = new RelayCommand(AddStarterExpenses);

            NewExpenseCategory = BudgetItemSection.Housing;

            foreach (var expense in _expenseManager.GetAllExpenses())
                Expenses.Add(new ExpenseRowViewModel(expense, _expenseManager));

            ValidateAll();
            RefreshTotals();
        }

        #region Properties

        public ObservableCollection<ExpenseRowViewModel> Expenses { get; }

        public List<BudgetItemSection> Categories { get; } =
            Enum.GetValues(typeof(BudgetItemSection)).Cast<BudgetItemSection>().ToList();

        private string _newExpenseName;
        public string NewExpenseName
        {
            get => _newExpenseName;
            set
            {
                _newExpenseName = value;
                Validate(nameof(NewExpenseName), () => !string.IsNullOrWhiteSpace(_newExpenseName), "Name is required.");
                OnPropertyChanged(nameof(NewExpenseName));
            }
        }

        private double _newExpenseAmount;
        public double NewExpenseAmount
        {
            get => _newExpenseAmount;
            set
            {
                _newExpenseAmount = value;
                Validate(nameof(NewExpenseAmount), () => _newExpenseAmount > 0, "Amount must be greater than 0.");
                OnPropertyChanged(nameof(NewExpenseAmount));
            }
        }

        public BudgetItemSection NewExpenseCategory { get; set; }

        public IRelayCommand AddExpenseCommand { get; }
        public IRelayCommand AddStarterExpensesCommand { get; }

        /// <summary>
        /// The usual monthly bills, seeded at $0 so you only have to fill in amounts.
        /// Deliberately excludes loan payments (car, student, credit card): those belong in
        /// the Life Calculator as debts with interest and payoff dates, and the surplus
        /// calculation already subtracts their monthly payments — listing them here too would
        /// double-count them.
        /// </summary>
        private static readonly (string Name, BudgetItemSection Category)[] StarterExpenses =
        {
            ("Rent", BudgetItemSection.Housing),
            ("Electricity", BudgetItemSection.Housing),
            ("Internet", BudgetItemSection.Housing),
            ("Phone", BudgetItemSection.Housing),
            ("Renters insurance", BudgetItemSection.Insurance),
            ("Car insurance", BudgetItemSection.Insurance),
            ("Gas", BudgetItemSection.Transportation),
            ("Groceries", BudgetItemSection.Food),
            ("Eating out", BudgetItemSection.Food),
            ("Entertainment", BudgetItemSection.Personal),
            ("Subscriptions", BudgetItemSection.Personal),
            ("Shopping", BudgetItemSection.Personal),
            ("Clothes", BudgetItemSection.Personal),
            ("Haircuts", BudgetItemSection.Personal),
            ("Miscellaneous", BudgetItemSection.Personal)
        };

        private double _totalMonthlyExpenses;
        public double TotalMonthlyExpenses
        {
            get => _totalMonthlyExpenses;
            private set { _totalMonthlyExpenses = value; OnPropertyChanged(nameof(TotalMonthlyExpenses)); }
        }

        private double _totalMonthlyIncome;
        public double TotalMonthlyIncome
        {
            get => _totalMonthlyIncome;
            private set { _totalMonthlyIncome = value; OnPropertyChanged(nameof(TotalMonthlyIncome)); }
        }

        private double _totalMonthlyDebtPayments;
        public double TotalMonthlyDebtPayments
        {
            get => _totalMonthlyDebtPayments;
            private set { _totalMonthlyDebtPayments = value; OnPropertyChanged(nameof(TotalMonthlyDebtPayments)); }
        }

        private double _totalMonthlyContributions;
        public double TotalMonthlyContributions
        {
            get => _totalMonthlyContributions;
            private set { _totalMonthlyContributions = value; OnPropertyChanged(nameof(TotalMonthlyContributions)); }
        }

        private double _leftOver;
        public double LeftOver
        {
            get => _leftOver;
            private set
            {
                _leftOver = value;
                OnPropertyChanged(nameof(LeftOver));
                OnPropertyChanged(nameof(IsOverspending));
            }
        }

        public bool IsOverspending => LeftOver < 0;

        #endregion

        #region Methods

        private void ValidateAll()
        {
            Validate(nameof(NewExpenseName), () => !string.IsNullOrWhiteSpace(_newExpenseName), "Name is required.");
            Validate(nameof(NewExpenseAmount), () => _newExpenseAmount > 0, "Amount must be greater than 0.");
        }

        private void AddExpense()
        {
            var expense = new ExpenseItem
            {
                Name = NewExpenseName,
                MonthlyAmount = NewExpenseAmount,
                Category = NewExpenseCategory,
                UserId = _accountStore.CurrentAccount.Id
            };

            _expenseManager.AddExpense(expense);

            NewExpenseName = string.Empty;
            NewExpenseAmount = 0;
        }

        /// <summary>
        /// Adds any starter expense the user doesn't already have, so it's safe to press more
        /// than once and won't clobber amounts they've already entered.
        /// </summary>
        private void AddStarterExpenses()
        {
            var existingNames = new HashSet<string>(
                _expenseManager.GetAllExpenses().Select(e => e.Name ?? string.Empty),
                StringComparer.OrdinalIgnoreCase);

            foreach (var starter in StarterExpenses)
            {
                if (existingNames.Contains(starter.Name))
                    continue;

                _expenseManager.AddExpense(new ExpenseItem
                {
                    Name = starter.Name,
                    MonthlyAmount = 0,
                    Category = starter.Category,
                    UserId = _accountStore.CurrentAccount.Id
                });
            }
        }

        private void ExpenseManager_Changed(object sender, ExpenseItem e)
        {
            if (Expenses.All(row => row.Id != e.Id))
                Expenses.Add(new ExpenseRowViewModel(e, _expenseManager));

            RefreshTotals();
        }

        private void ExpenseManager_Deleted(object sender, ExpenseItem e)
        {
            var row = Expenses.FirstOrDefault(r => r.Id == e.Id);
            if (row != null)
                Expenses.Remove(row);

            RefreshTotals();
        }

        /// <summary>
        /// Runs the same CashFlowSimulator the Life Calculator uses, so "left over" here and
        /// "monthly surplus" there are the same number by construction. Subtracting only
        /// expenses would overstate what's actually free, since debt payments and investment
        /// contributions are already committed.
        /// </summary>
        private void RefreshTotals()
        {
            var account = _accountStore.CurrentAccount;
            TotalMonthlyExpenses = _expenseManager.GetTotalMonthlyExpenses();

            var accounts = account.SimulatedAccountManager.GetAllAccounts();

            // Accounts loaded from the database need their events manager wired before their
            // projections can be read.
            foreach (var simulated in accounts)
            {
                switch (simulated)
                {
                    case LoanAccount loan: loan.SetEventsManager(account.AccountsEventsManager); break;
                    case CompoundAccount compound: compound.SetEventsManager(account.AccountsEventsManager); break;
                    case RetirementAccount retirement: retirement.SetEventsManager(account.AccountsEventsManager); break;
                }
            }

            var debts = accounts.OfType<LoanAccount>().ToList();
            var growthAccounts = accounts.Where(a => a is CompoundAccount || a is RetirementAccount)
                .Cast<ISimulatedAccount>()
                .ToList();

            var debtResult = DebtPayoffSimulator.Simulate(debts, account.PayoffStrategy);

            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var cashFlow = CashFlowSimulator.Calculate(account, currentMonth, currentMonth, debtResult, growthAccounts);

            if (cashFlow.Count > 0)
            {
                var month = cashFlow[0];
                TotalMonthlyIncome = month.TotalIncome;
                TotalMonthlyDebtPayments = month.TotalDebtPayments;
                TotalMonthlyContributions = month.TotalContributions;
                LeftOver = month.Surplus;
            }
            else
            {
                TotalMonthlyIncome = 0;
                TotalMonthlyDebtPayments = 0;
                TotalMonthlyContributions = 0;
                LeftOver = -TotalMonthlyExpenses;
            }
        }

        #endregion
    }
}
