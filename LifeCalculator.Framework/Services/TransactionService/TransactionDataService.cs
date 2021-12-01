using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Services.DataService;

namespace LifeCalculator.Framework.Services.TransactionDataService
{
    public class TransactionDataService : GenericDataService<TransactionItem>
    {
        public TransactionDataService(string tableName = nameof(TransactionItem)) 
            : base(tableName)
        {
        }
    }
}
