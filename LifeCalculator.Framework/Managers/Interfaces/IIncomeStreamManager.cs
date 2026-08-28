using LifeCalculator.Framework.Income;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Managers
{
    public interface IIncomeStreamManager
    {
        event EventHandler<IncomeStream> IncomeStreamAdded;
        event EventHandler<IncomeStream> IncomeStreamChanged;
        event EventHandler<IncomeStream> IncomeStreamDeleted;

        void AddIncomeStream(IncomeStream incomeStream);
        void DeleteIncomeStream(IncomeStream incomeStream);
        List<IncomeStream> GetAllIncomeStreams();
        Task LoadFromDb(int userId);
    }
}
