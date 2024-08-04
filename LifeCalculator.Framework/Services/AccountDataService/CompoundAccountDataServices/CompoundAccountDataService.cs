using Dapper;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.DataService;
using LifeCalculator.Framework.Services.EventsDataService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.AccountDataServices
{
    public class CompoundAccountDataService : GenericDataService<CompoundAccount> , ICompoundAccountDataService
    {
        #region Fields

        private string _tableName;

        #endregion

        #region Constructors

        public CompoundAccountDataService()
            : base("CompoundAccount")
        {
            _tableName = "CompoundAccount";
        }

        public async Task<List<CompoundAccount>> LoadAccountsByUserId(int referenceAccountID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var idList = await cnn.QueryAsync<CompoundAccount>($"SELECT * FROM {_tableName} WHERE UserId = @userId", new { userId = referenceAccountID });
                var outputList = new List<CompoundAccount>();

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
