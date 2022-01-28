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

namespace LifeCalculator.Framework.Services
{
    public class LoanAccountDataService : GenericDataService<LoanAccount>
    {

        #region Fields

        private string _tableName;

        #endregion

        #region Constructors

        public LoanAccountDataService()
            : base("LoanAccount")
        {
            _tableName = "LoanAccount";
        }

        #endregion

        #region CRUD Methods

        public async Task<List<LoanAccount>> LoadAccountsByUserId(int referenceAccountID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var idList = await cnn.QueryAsync<LoanAccount>($"SELECT * FROM {_tableName} WHERE UserId = @userId", new { userId = referenceAccountID });
                var outputList = new List<LoanAccount>();
                
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
