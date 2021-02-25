using Dapper;
using LifeCalculator.Framework.Account;
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

namespace LifeCalculator.Framework.Services.AccDataService
{
    public class AccountDataService : GenericDataService<ConcreteAccount>
    {
        private string _tableName = "ConcreteAccount";

        public AccountDataService() :
            base("ConcreteAccount")
        {
            _tableName = "ConcreteAccount";
        }

        public async Task<IAccount> Insert(IAccount entity)
        {
            AccountEventDataService dataService = new AccountEventDataService();

            ConcreteAccount outputObj = base.Insert(new ConcreteAccount(entity)).Result;
            outputObj.AccountLifeEvents = dataService.InsertAccountEvents(outputObj.Id,entity.AccountLifeEvents).Result;

            return outputObj;
        }

        public async Task<IAccount> Load(int entityId)
        {
            AccountEventDataService dataService = new AccountEventDataService();

            ConcreteAccount outputObj = new ConcreteAccount();
            try
            {
                outputObj = base.Load(entityId).Result;
                outputObj.AccountLifeEvents = dataService.LoadFromAccountID(outputObj.Id).Result;
            }
            catch 
            {
                return null;
            }
            

            return outputObj;
        }

        public async Task Save(IAccount entity)
        {
            AccountEventDataService dataService = new AccountEventDataService();

            await base.Save(entity.Id, new ConcreteAccount(entity));
            dataService.SaveEventList(entity.AccountLifeEvents);

        }

        public async Task Delete(IAccount entity)
        {
            AccountEventDataService dataService = new AccountEventDataService();

            await base.Delete(entity.Id);
            dataService.DeleteByAccountID(entity.Id);

        }

        public async Task<List<IAccount>> InsertAccountsList(List<IAccount> accounts)
        {
            var outputList = new List<IAccount>();
            foreach (var item in accounts)
            {
                outputList.Add(Insert(new ConcreteAccount(item)).Result);
            }

            return outputList;
        }

        public async Task DeleteAccountList(List<IAccount> accounts)
        {
            foreach (var item in accounts)
            {
                Delete(item.Id);
            }
        }

        public async Task UpdateAccountList(List<IAccount> accounts)
        {
            foreach (var item in accounts)
            {
                Save(item.Id,new ConcreteAccount(item));
            }
        }

        public async Task<List<IAccount>> LoadAccountsByUserId(int userId)
        {
            AccountEventDataService dataService = new AccountEventDataService();

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var enumObject = await cnn.QueryAsync<ConcreteAccount>($"SELECT * FROM {_tableName} WHERE UserId = @UserId", new { UserId = userId });
                List<IAccount> outputObject = new List<IAccount>();
                foreach(var item in enumObject)
                {
                    outputObject.Add(new ConcreteAccount(item));
                }

                foreach(var item in outputObject)
                {
                    item.AccountLifeEvents = dataService.LoadFromAccountID(item.Id).Result;
                }

                return outputObject;
            }

            return new List<IAccount>();
        }

    }
}
