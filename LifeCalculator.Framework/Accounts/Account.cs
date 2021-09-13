using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Accounts
{
    public class Account
    {
        public Account()
        {
            RecentTransactions = new List<Transaction>();
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public Enums.PlaidAccountType Type { get; set; }
        public double AvailableBalance { get; set; }
        public List<Transaction> RecentTransactions { get; set; }
        public string FormattedBalance
        {
            get
            {
                if (AvailableBalance < 0)
                    return "-$" + $"{Math.Abs(AvailableBalance):F2}";
                else
                    return "$" + $"{AvailableBalance:F2}";
            }
        }

        public override string ToString()
        {
            return Name + " (" + FormattedBalance + ")";
        }
    }
}
