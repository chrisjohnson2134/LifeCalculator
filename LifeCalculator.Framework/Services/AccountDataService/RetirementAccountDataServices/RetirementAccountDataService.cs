using Dapper;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Services.DataService;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.AccountDataServices
{
    public class RetirementAccountDataService : GenericDataService<RetirementAccount>, IRetirementAccountDataService
    {
        #region Fields

        private string _tableName;

        #endregion

        #region Constructors

        public RetirementAccountDataService()
            : base("RetirementAccount")
        {
            _tableName = "RetirementAccount";
        }

        public async Task<List<RetirementAccount>> LoadAccountsByUserId(int referenceAccountID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var idList = await cnn.QueryAsync<RetirementAccount>($"SELECT * FROM {_tableName} WHERE UserId = @userId", new { userId = referenceAccountID });
                var outputList = new List<RetirementAccount>();

                foreach (var item in idList)
                {
                    outputList.Add(await Load(item.Id));
                }

                return outputList;
            }
        }

        #endregion
    }
}
