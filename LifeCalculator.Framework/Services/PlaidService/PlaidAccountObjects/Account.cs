using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.DataService;
using LifeCalculator.Framework.SimulatedAccount;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Accounts
{
    public class Account : IAccount
    {
        public Account()
        {
            RecentTransactions = new List<TransactionItem>();
        }

        public event EventHandler<IAccount> ValueChanged;

        public string Name { get; set; }
        public string PlaidID { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public int InstitutionId { get; set; }
        public Enums.PlaidAccountType Type { get; set; }
        public double AvailableBalance { get; set; }
        [IgnoreDatabase]
        public List<TransactionItem> RecentTransactions { get; set; }
        [IgnoreDatabase]
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

        public List<MonthlyColumn> Calculation()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name + " (" + FormattedBalance + ")";
        }
    }
}
