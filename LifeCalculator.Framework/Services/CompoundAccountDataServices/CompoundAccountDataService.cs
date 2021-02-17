using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.AccountDataServices
{
    public class CompoundAccountDataService : GenericDataService<CompoundAccount> , ICompoundAccountDataService
    {
        #region Fields

        private string _tableName;

        #endregion

        #region Constructors

        public CompoundAccountDataService(string tableName)
            : base(tableName)
        {
            _tableName = tableName;
        }

        #endregion


    }
}
