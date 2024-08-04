using Dapper;
using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.PlaidAccInfoDataService
{
    public class TransactionDataService : GenericDataService<TransactionItem>
    {
        string _tableName;
        public TransactionDataService(string tableName = nameof(TransactionItem)) 
            : base(tableName)
        {
            _tableName = tableName;
        }

        public async Task<List<TransactionItem>> LoadAllByAccountId(int referenceAccountID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var idList = await cnn.QueryAsync<TransactionItem>($"SELECT * FROM {_tableName} WHERE AccountId = @accountId", new { accountId = referenceAccountID });
                var outputList = new List<TransactionItem>();

                foreach (var item in idList)
                {
                    outputList.Add(await Load(item.Id));
                }

                return outputList;
            }
        }
    }
}
