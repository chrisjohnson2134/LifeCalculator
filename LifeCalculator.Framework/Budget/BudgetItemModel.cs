using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Enums;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Framework.Budget
{
    public class BudgetItemModel
    {
        #region

        public BudgetItemModel()
        {
            Transactions = new List<TransactionItem>();
        }

        public BudgetItemModel(IGrouping<string, TransactionItem> transactionItems)
        {
            Name = transactionItems.Key;
            Transactions = transactionItems.ToList();
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public double PlannedAmount { get; set; }

        public double SpentAmount { get; set; }

        public double RemainingAmount { get; set; }

        public BudgetItemSection Type { get; set; }

        public List<TransactionItem> Transactions { get; set; }

        #endregion
    }
}
