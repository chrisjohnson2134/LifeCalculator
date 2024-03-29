﻿using Dapper;
using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.CustomExceptions;
using LifeCalculator.Framework.Services.AccDataService;
using LifeCalculator.Framework.Services.DataService;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using LifeCalculator.Framework.Services.EventsDataService;

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
            _tableName = tableName;
        }

        public async Task<FinancialAccount.FinancialAccount> LoadByUsername(string username)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var accountsDataService = new AccountDataService();
                AccountEventDataService accountEventDataService = new AccountEventDataService();

                var result = await cnn.QuerySingleOrDefaultAsync<FinancialAccount.FinancialAccount>($"SELECT * FROM {_tableName} WHERE AccountHolder=@AccountHolder", new { AccountHolder = username });

                foreach (IAccount account in await accountsDataService.LoadAccountsByUserId(result.Id))
                {
                    result.SimulatedAccountManager.AddAccount(account, true);
                }

                await result.AccountsEventsManager.LoadFromDb();
                await result.TransactionManager.LoadFromDb();

                //foreach(IAccount acc in result.SimulatedAccountManager.GetAllAccounts())
                //{
                //    result.AccountsEventsManager.AddAccountEvents(await accountEventDataService.LoadFromAccountID(acc.Id));
                //}

                if (result == null)
                    throw new FinancialAccountNotFoundException($"{_tableName} with account holder [{username}] could not be found.");

                return result;
            }
        }

        #endregion
    }
}
