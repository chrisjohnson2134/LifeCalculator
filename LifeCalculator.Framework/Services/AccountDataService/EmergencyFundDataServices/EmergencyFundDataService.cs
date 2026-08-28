using Dapper;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Services.DataService;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.AccountDataServices
{
    public class EmergencyFundDataService : GenericDataService<EmergencyFundAccount>, IEmergencyFundDataService
    {
        #region Fields

        private string _tableName;

        #endregion

        #region Constructors

        public EmergencyFundDataService()
            : base("EmergencyFund")
        {
            _tableName = "EmergencyFund";
        }

        public async Task<List<EmergencyFundAccount>> LoadAccountsByUserId(int referenceAccountID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var idList = await cnn.QueryAsync<EmergencyFundAccount>($"SELECT * FROM {_tableName} WHERE UserId = @userId", new { userId = referenceAccountID });
                var outputList = new List<EmergencyFundAccount>();

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
