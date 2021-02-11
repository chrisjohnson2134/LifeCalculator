using Dapper;
using LifeCalculator.Framework.Services.DataService;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.FinancialAccountService
{
    public class FinancialAccountDataService : GenericDataService<FinancialAccount.FinancialAccount>, IFinancialAccountDataService
    {
        #region Fields

        private string _tableName;

        #endregion

        #region Constructors

        public FinancialAccountDataService(string tableName) : base(tableName)
        {
        }

        public async Task<FinancialAccount.FinancialAccount> LoadByUsername(string username)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var result = await cnn.QuerySingleOrDefaultAsync<FinancialAccount.FinancialAccount>($"SELECT * FROM {_tableName} WHERE AccountHolder=@AccountHolder", new { AccountHolder = username });
                if (result == null)
                    throw new KeyNotFoundException($"{_tableName} with account [{username}] could not be found.");

                return result;
            }
        }

        #endregion
    }
}
