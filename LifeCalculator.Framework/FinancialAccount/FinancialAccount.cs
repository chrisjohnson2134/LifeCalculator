using LifeCalculator.Framework.SimulatedAccount;
using System.Collections.Generic;
using LifeCalculator.Framework.Users;
using LifeCalculator.Framework.Services.DataService;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.Managers.Interfaces;

namespace LifeCalculator.Framework.FinancialAccount
{
    public class FinancialAccount
    {

        #region Constructors

        public FinancialAccount()
        {
            SimulatedAccountManager = new AccountManager();
            TransactionManager = new TransactionsManager();
            BudgetManager = new BudgetManager(TransactionManager);
            AccountsEventsManager = new AccountsEventsManager();
        }

        #endregion

        #region Properties
        public int Id { get; set; }

        //public int FinancialAccountId { get; set; }
        public string AccountHolder { get; set; }

        public double Salary { get; set; }

        public double NetMonthlyIncome { get; set; }

        public double Rent { get; set; }

        public double WaterBill { get; set; }

        public double ElectricBill { get; set; }

        public double InternetBill { get; set; }

        public double CableBill { get; set; }

        public double Subscriptions { get; set; }

        public double Groceries { get; set; }

        public double EmergencyFundContributions { get; set; }
        
        public double Gas { get; set; }

        public double CarInsurance { get; set; }

        public double HomeInsurance { get; set; }

        public double CarPayments { get; set; }

        public double OtherDebts { get; set; }

        public double MiscellaneousPayments { get; set; }

        [IgnoreDatabase]
        public AccountManager SimulatedAccountManager { get; set; }
        [IgnoreDatabase]
        public AccountManager UserAccountManager { get; set; }
        [IgnoreDatabase]
        public BudgetManager BudgetManager { get; set; }
        [IgnoreDatabase]
        public ITransactionManager TransactionManager { get; set; }
        [IgnoreDatabase]
        public IAccountsEventsManager AccountsEventsManager { get; set; }

        #endregion
    }
}
