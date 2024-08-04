using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.PlaidAccInfoDataService
{
    public class InstitutionDataService : GenericDataService<Institution>
    {
        AccountDataService accountDataService;

        public InstitutionDataService(string tableName = nameof(Institution))
            : base(tableName)
        {
            accountDataService = new AccountDataService();
        }

        public override async Task<Institution> Insert(Institution entity)
        {
            var institution = await base.Insert(entity);

            //foreach (var item in entity.Accounts)
            //{
            //    accountDataService.Insert(item);
            //}

            return institution;
        }

    }
}
