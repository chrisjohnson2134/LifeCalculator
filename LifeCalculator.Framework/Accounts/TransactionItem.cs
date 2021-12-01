using System;

namespace LifeCalculator.Framework.Accounts
{
    public class TransactionItem
    {
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
    }
}
