using Dapper;
using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.AccountDataServices;
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
    public class AccountDataService
    {
        public AccountDataService() 
        {
        }

        public async Task<IAccount> Insert(IAccount entity)
        {
            IAccount outputAccount = null;
            if (entity is LoanAccount)
            {
                var dataService = new LoanAccountDataService();
                outputAccount = await dataService.Insert( entity as LoanAccount );
            }
            else if(entity is CompoundAccount)
            {
                var dataService = new CompoundAccountDataService();
                outputAccount = await dataService.Insert(entity as CompoundAccount);
            }

            return outputAccount;
        }

        public async Task<IAccount> Load(IAccount entity)
        {
            if (entity == null)
                return null;

            IAccount outputAccount = null;
            if (entity is LoanAccount)
            {
                var dataService = new LoanAccountDataService();
                outputAccount = await dataService.Load(entity.Id);
            }
            else if (entity is CompoundAccount)
            {
                var dataService = new CompoundAccountDataService();
                outputAccount = await dataService.Load(entity.Id);
            }

            return outputAccount;
        }

        public async Task Save(IAccount entity)
        {
            if (entity is LoanAccount)
            {
                var dataService = new LoanAccountDataService();
                 await dataService.Save(entity.Id,entity as LoanAccount);
            }
            else if (entity is CompoundAccount)
            {
                var dataService = new CompoundAccountDataService();
                await dataService.Save(entity.Id,entity as CompoundAccount);
            }
        }

        public async Task Delete(IAccount entity)
        {
            if (entity == null)
                return;

            if (entity is LoanAccount)
            {
                var dataService = new LoanAccountDataService();
                await dataService.Delete(entity.Id);
            }
            else if (entity is CompoundAccount)
            {
                var dataService = new CompoundAccountDataService();
                await dataService.Delete(entity.Id);
            }

        }

        public async Task<List<IAccount>> InsertAccountsList(List<IAccount> accounts)
        {
            var outputList = new List<IAccount>();
            foreach (var item in accounts)
            {
                outputList.Add(await Insert(item));
            }

            return outputList;
        }

        public async Task DeleteAccountList(List<IAccount> accounts)
        {
            foreach (var item in accounts)
            {
                Delete(item);
            }
        }

        public async Task UpdateAccountList(List<IAccount> accounts)
        {
            foreach (var item in accounts)
            {
                Save(item);
            }
        }

        public async Task<List<IAccount>> LoadAccountsByUserId(int userId)
        {
            var loanAccountDataSevice = new LoanAccountDataService();
            var compoundAccountDataService = new CompoundAccountDataService();

            var accountList = new List<IAccount>();

            foreach (var item in await compoundAccountDataService.LoadAccountsByUserId(userId))
            {
                accountList.Add(item);
            }

            foreach (var item in await loanAccountDataSevice.LoadAccountsByUserId(userId))
            {
                accountList.Add(item);
            }

            return accountList;
        }

    }
}
