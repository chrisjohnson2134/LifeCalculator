using CommunityToolkit.Mvvm.Input;
using LifeCalculator.Commands;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Simulation;
using LifeCalculator.Navigation;
using LifeCalculator.ViewModels.Factory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace LifeCalculator.ViewModels
{
    public class PayoffSummaryItem
    {
        public string Name { get; set; }
        public string PayoffDateText { get; set; }
    }

    public class HomeViewModel : ViewModelBase
    {
        private readonly IAccountStore _accountStore;

        public HomeViewModel(IAccountStore accountStore, INavigator navigator, ViewModelFactory viewModelFactory)
        {
            _accountStore = accountStore;
            NavigateCommand = new UpdateCurrentViewModelCommand(navigator, viewModelFactory);

            UpcomingPayoffs = new ObservableCollection<PayoffSummaryItem>();

            Recalculate();
        }

        /// <summary>Bind with CommandParameter="{x:Static nav:ViewType.Calculator}" etc.</summary>
        public ICommand NavigateCommand { get; }

        public double TotalDebt { get; private set; }
        public double TotalAssets { get; private set; }
        public double NetWorth { get; private set; }
        public string DebtFreeDateText { get; private set; } = "No debts";

        public ObservableCollection<PayoffSummaryItem> UpcomingPayoffs { get; }

        private void Recalculate()
        {
            var account = _accountStore.CurrentAccount;
            if (account == null)
                return;

            var accounts = account.SimulatedAccountManager.GetAllAccounts();

            // Accounts loaded from the database don't have their events manager wired up
            // until something sets it — CalculatorViewModel does this for its own use, and
            // Home needs the same accounts wired here since it may run first after login.
            foreach (var simulatedAccount in accounts)
            {
                switch (simulatedAccount)
                {
                    case LoanAccount loan:
                        loan.SetEventsManager(account.AccountsEventsManager);
                        break;
                    case CompoundAccount compound:
                        compound.SetEventsManager(account.AccountsEventsManager);
                        break;
                    case RetirementAccount retirementAccount:
                        retirementAccount.SetEventsManager(account.AccountsEventsManager);
                        break;
                }
            }

            var debts = accounts.OfType<LoanAccount>().ToList();
            var growthAccounts = accounts.Where(a => a is CompoundAccount || a is RetirementAccount).ToList();

            var debtResult = DebtPayoffSimulator.Simulate(debts, account.PayoffStrategy);

            var incomeStreams = account.IncomeStreamManager?.GetAllIncomeStreams()
                ?? new List<LifeCalculator.Framework.Income.IncomeStream>();

            var assetTimelines = new Dictionary<string, List<MonthlyColumn>>();

            foreach (var growthAccount in growthAccounts)
            {
                List<MonthlyColumn> calc;

                if (growthAccount is RetirementAccount retirement)
                {
                    // Match cap follows the linked job's salary; fall back to the profile
                    // figure for accounts created before streams could be linked.
                    var linked = incomeStreams.FirstOrDefault(s => s.Id == retirement.LinkedIncomeStreamId);
                    double monthlySalary = linked == null
                        ? account.Salary / 12
                        : (linked.GrossAnnualSalary > 0 ? linked.GrossAnnualSalary / 12 : linked.MonthlyAmount);
                    calc = retirement.Calculation(monthlySalary);
                }
                else
                {
                    calc = (growthAccount as ISimulatedAccount).Calculation();
                }

                assetTimelines[growthAccount.Name] = calc;
            }

            var netWorthTimeline = NetWorthAggregator.Aggregate(debtResult, assetTimelines);

            // The timeline is bucketed monthly — match this month, not the nearest timestamp.
            DateTime currentMonthBucket = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var closest = netWorthTimeline.FirstOrDefault(c => c.Date == currentMonthBucket)
                ?? netWorthTimeline
                    .OrderBy(c => Math.Abs((c.Date - currentMonthBucket).Ticks))
                    .FirstOrDefault();

            TotalDebt = closest?.TotalDebt ?? 0;
            TotalAssets = closest?.TotalAssets ?? 0;
            NetWorth = closest?.NetWorth ?? 0;

            UpcomingPayoffs.Clear();
            foreach (var kvp in debtResult.PayoffDateByDebtName.Where(p => p.Value != null).OrderBy(p => p.Value))
            {
                UpcomingPayoffs.Add(new PayoffSummaryItem { Name = kvp.Key, PayoffDateText = kvp.Value.Value.ToString("MMMM yyyy") });
            }

            if (debts.Count == 0)
                DebtFreeDateText = "No debts";
            else if (debtResult.PayoffDateByDebtName.Values.Any(d => d == null))
                DebtFreeDateText = "Not on track to pay off";
            else
                DebtFreeDateText = debtResult.PayoffDateByDebtName.Values.Max().Value.ToString("MMMM yyyy");

            OnPropertyChanged(string.Empty);
        }
    }
}
