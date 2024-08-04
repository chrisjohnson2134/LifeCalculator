using LifeCalculator.Framework.Services.DataService;
using System;

namespace LifeCalculator.Framework.Accounts
{
    public class TransactionItem
    {
        public TransactionItem()
        {
            Id = -1;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TransactionId { get; set; }
        public string AccountId { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string BudgetCategory { get; set; }
        public string BudgetCategoryPlaidDefault { get; set; }
        public string FormattedAmount
        {
            get
            {
                if (Amount < 0)
                    return "-$" + $"{Math.Abs(Amount):F2}";
                else
                    return "$" + $"{Amount:F2}";
            }
        }

        public override string ToString()
        {
            return Date.ToShortDateString() + " " + Name + " " + FormattedAmount;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TransactionItem))
                return false;

            TransactionItem other = (TransactionItem)obj;
            if(other.Id == Id &&
                other.Name == Name &&
                other.TransactionId.Equals(TransactionId) &&
                other.AccountId.Equals(AccountId) &&
                other.Date.Equals(Date) &&
                other.Amount == Amount &&
                other.BudgetCategory.Equals(BudgetCategory) &&
                other.BudgetCategoryPlaidDefault.Equals(BudgetCategoryPlaidDefault))
                return true;

            return false;
        }
    }
}
