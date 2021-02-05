using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Managers.Interfaces;
using System.Collections.Generic;

namespace LifeCalculator.Framework.FinancialProfile
{
    public class FinancialProfile
    {
        #region Fields

        private readonly IAccountManager _accountManager;

        #endregion

        #region Constructors

        public FinancialProfile(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        #endregion

        #region Properties

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

        public List<IAccount> AccountTypes { get; set; }

        #endregion
    }
}
