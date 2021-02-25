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
    public class AccountEventDataService : GenericDataService<AccountEvent>
    {

        #region Fields

        private static string _tableName = "AccountEvent";

        #endregion

        #region Constructors

        public AccountEventDataService() 
            : base(_tableName)
        {
        }

        #endregion

        #region Methods

        public async Task<List<IAccountEvent>> InsertAccountEvents(int accountId,List<IAccountEvent> eventList)
        {
            var outputList = new List<IAccountEvent>();
            foreach (var item in eventList)
            {
                var result = new AccountEvent(item);
                result.AccountId = accountId;
                result = base.Insert(result).Result;
                outputList.Add(result);
            }

            return outputList;
        }

        public async Task<List<IAccountEvent>> LoadFromAccountID(int referenceAccountID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = await cnn.QueryAsync<AccountEvent>($"SELECT * FROM {_tableName} WHERE AccountId = @AccountId", new { AccountId = referenceAccountID });
                return output.AsList<IAccountEvent>();
            }
        }

        public async Task DeleteByAccountID(int refAccountId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                await cnn.ExecuteAsync($"DELETE FROM {_tableName} WHERE AccountId=@AccountId", new { AccountId = refAccountId });
            }
        }

        public async Task SaveEventList(List<IAccountEvent> accountEvents)
        {
            foreach (var item in accountEvents)
            {
                base.Save(item.Id, new AccountEvent(item));
            }
        }

        #endregion


    }
}
