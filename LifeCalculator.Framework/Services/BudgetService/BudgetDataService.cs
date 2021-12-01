using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.Services.DataService;

namespace LifeCalculator.Framework.Services.BudgetService
{
    internal class BudgetDataService : GenericDataService<BudgetItemModel>
    {
        public BudgetDataService(string tableName) 
            : base(tableName)
        {
        }
    }
}
