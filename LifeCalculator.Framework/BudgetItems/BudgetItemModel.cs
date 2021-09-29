using LifeCalculator.Framework.Enums;

namespace LifeCalculator.Framework.BudgetItems
{
    public class BudgetItemModel
    {
        #region

        public BudgetItemModel()
        {

        }

        #endregion


        #region Properties

        public string Name { get; set; }

        public double PlannedAmount { get; set; }

        public double SpentAmount { get; set; }

        public double RemainingAmount { get; set; }

        public BudgetItemSection Type { get; set; }

        #endregion
    }
}
