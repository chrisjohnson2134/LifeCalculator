﻿using LifeCalculator.Framework.Account;
using System.Collections.Generic;
using LifeCalculator.Framework.Users;
using LifeCalculator.Framework.Services.DataService;
using LifeCalculator.Framework.Account.Manager;

namespace LifeCalculator.Framework.FinancialAccount
{
    public class FinancialAccount
    {

        #region Constructors

        public FinancialAccount()
        {
            AccountManager = new AccountManager();
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
        public AccountManager AccountManager { get; set; }

        #endregion
    }
}
