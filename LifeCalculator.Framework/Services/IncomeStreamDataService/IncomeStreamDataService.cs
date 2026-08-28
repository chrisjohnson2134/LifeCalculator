using Dapper;
using LifeCalculator.Framework.Income;
using LifeCalculator.Framework.Services.DataService;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.IncomeStreamDataServices
{
    public class IncomeStreamDataService : GenericDataService<IncomeStream>
    {
        private static string _tableName = "IncomeStream";

        public IncomeStreamDataService()
            : base(_tableName)
        {
        }

        public async Task<List<IncomeStream>> LoadByUserId(int userId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var idList = await cnn.QueryAsync<IncomeStream>($"SELECT * FROM {_tableName} WHERE UserId = @UserId", new { UserId = userId });
                var outputList = new List<IncomeStream>();

                foreach (var item in idList)
                {
                    outputList.Add(await Load(item.Id));
                }

                return outputList;
            }
        }
    }
}
