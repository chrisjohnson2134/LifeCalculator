using Dapper;
using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.PlaidAccInfoDataService
{
    public class AccountDataService : GenericDataService<Account>
    {
        string _tableName;
        TransactionDataService transactionDataService;
        public AccountDataService(string tableName = nameof(Account))
            : base(tableName)
        {
            _tableName = tableName;
            transactionDataService = new TransactionDataService();
        }

        public override async Task<Account> Insert(Account entity)
        {
            var account = await base.Insert(entity);
            foreach (var item in entity.RecentTransactions)
            {
                item.AccountId = account.PlaidID;
                transactionDataService.Insert(item);
            }

            return account;
        }

        public override async Task<Account> Load(int id)
        {
            var account = await base.Load(id);

            var transactions = await transactionDataService.LoadAllByAccountId(id);

            account.RecentTransactions = transactions;

            return account;
        }

        public async Task<List<Account>> LoadByInstitutionId(int referenceInstitutionID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var idList = await cnn.QueryAsync<Account>($"SELECT * FROM {_tableName} WHERE InstitutionId = @institutionId", new { institutionId = referenceInstitutionID });
                var outputList = new List<Account>();

                foreach (var item in idList)
                {
                    outputList.Add(await Load(item.Id));
                }

                return outputList;
            }
        }
    }
}
