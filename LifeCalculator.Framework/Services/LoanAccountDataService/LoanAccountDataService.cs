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

        public override async Task<LoanAccount> Insert(LoanAccount entity)
        {
            AccountEventDataService dataService = new AccountEventDataService();

            LoanAccount outputObj = await base.Insert(entity);
            outputObj.AccountLifeEvents = new List<IAccountEvent>();

            for (int i = 0; i < entity.AccountLifeEvents.Count; i++)
            {
                entity.AccountLifeEvents[i].AccountId = outputObj.Id;
                outputObj.AddLifeEvent(await dataService.Insert((AccountEvent)entity.AccountLifeEvents[i]));
            }

            return outputObj;
        }

        public override async Task<LoanAccount> Load(int id)
        {
            AccountEventDataService dataService = new AccountEventDataService();

            LoanAccount outputObj = await base.Load(id);
            outputObj.AccountLifeEvents = new List<IAccountEvent>();

            var eventList = await dataService.LoadFromAccountID(id);

            foreach (var item in eventList)
            {
                if(item.AccountType == Enums.AccountTypes.LoanAccount)
                    outputObj.AddLifeEvent(item);
            }

            return outputObj;
        }

        public override async Task Save(int id, LoanAccount entity)
        {
            var dataService = new AccountEventDataService();

            foreach (var item in entity.AccountLifeEvents)
            {
                dataService.Save(item.Id, (AccountEvent)item);
            }

            base.Save(id, entity);
        }

        public virtual async Task Delete(int id)
        {
            var dataService = new AccountEventDataService();

            base.Delete(id);

            dataService.DeleteByAccountID(id);
        }

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
