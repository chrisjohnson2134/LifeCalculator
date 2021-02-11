using LifeCalculator.Framework.Services.DataService;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.FinancialAccountService
{
    public interface IFinancialAccountDataService : IDataService<FinancialAccount.FinancialAccount>
    {
        Task<FinancialAccount.FinancialAccount> LoadByUsername(string username);
    }
}
