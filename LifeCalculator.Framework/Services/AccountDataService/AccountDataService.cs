using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Services.AccountDataServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.AccDataService
{
    public class AccountDataService
    {
        public AccountDataService() 
        {
        }

        public async Task<ISimulatedAccount> Insert(ISimulatedAccount entity)
        {
            ISimulatedAccount outputAccount = null;
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

        public async Task<ISimulatedAccount> Load(ISimulatedAccount entity)
        {
            if (entity == null)
                return null;

            ISimulatedAccount outputAccount = null;
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

        public async Task Save(ISimulatedAccount entity)
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

        public async Task Delete(ISimulatedAccount entity)
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

        public async Task<List<ISimulatedAccount>> InsertAccountsList(List<ISimulatedAccount> accounts)
        {
            var outputList = new List<ISimulatedAccount>();
            foreach (var item in accounts)
            {
                outputList.Add(await Insert(item));
            }

            return outputList;
        }

        public async Task DeleteAccountList(List<ISimulatedAccount> accounts)
        {
            foreach (var item in accounts)
            {
                await Delete(item);
            }
        }

        public async Task UpdateAccountList(List<ISimulatedAccount> accounts)
        {
            foreach (var item in accounts)
            {
                await Save(item);
            }
        }

        public async Task<List<ISimulatedAccount>> LoadAccountsByUserId(int userId)
        {
            var loanAccountDataSevice = new LoanAccountDataService();
            var compoundAccountDataService = new CompoundAccountDataService();

            var accountList = new List<ISimulatedAccount>();

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
