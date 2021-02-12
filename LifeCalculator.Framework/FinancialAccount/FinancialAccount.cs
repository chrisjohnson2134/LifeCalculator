using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Managers.Interfaces;
using System.Collections.Generic;
using LifeCalculator.Framework.Users;

namespace LifeCalculator.Framework.FinancialAccount
{
    public class FinancialAccount
    {
        #region Fields

        private readonly IAccountManager _accountManager;

        #endregion

        #region Constructors

        public FinancialAccount()
        {
        }

        #endregion

        #region Properties
        public int id { get; set; }
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

        public List<IAccount> Accounts { get; set; }

        #endregion
    }
}
