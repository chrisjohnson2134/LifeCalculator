using Dapper;
using LifeCalculator.Framework.Budget;
using LifeCalculator.Framework.Services.DataService;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.ExpenseDataServices
{
    public class ExpenseItemDataService : GenericDataService<ExpenseItem>
    {
        private static string _tableName = "ExpenseItem";

        public ExpenseItemDataService()
            : base(_tableName)
        {
        }

        public async Task<List<ExpenseItem>> LoadByUserId(int userId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var idList = await cnn.QueryAsync<ExpenseItem>($"SELECT * FROM {_tableName} WHERE UserId = @UserId", new { UserId = userId });
                var outputList = new List<ExpenseItem>();

                foreach (var item in idList)
                {
                    outputList.Add(await Load(item.Id));
                }

                return outputList;
            }
        }
    }
}
