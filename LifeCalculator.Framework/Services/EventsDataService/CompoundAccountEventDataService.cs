using Dapper;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.EventsDataService
{
    public class CompoundAccountEventDataService : GenericDataService<AccountEvent>
    {

        #region Fields

        private static string _tableName = "AccountEvent";

        #endregion

        #region Constructors

        public CompoundAccountEventDataService() : base(_tableName)
        {
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<AccountEvent>> LoadFromAccountID(int referenceAccountID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = await cnn.QueryAsync<AccountEvent>($"SELECT * FROM {_tableName} WHERE AccountId = @AccountId", new { AccountId = referenceAccountID });
                return output.AsList<AccountEvent>();
            }
        }

        public virtual async Task DeleteByAccountID(int refAccountId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                await cnn.ExecuteAsync($"DELETE FROM {_tableName} WHERE AccountId=@AccountId", new { AccountId = refAccountId });
            }
        }

        #endregion


    }
}
