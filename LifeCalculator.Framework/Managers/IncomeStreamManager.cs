using LifeCalculator.Framework.Income;
using LifeCalculator.Framework.Services.IncomeStreamDataServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Managers
{
    public class IncomeStreamManager : IIncomeStreamManager
    {
        public event EventHandler<IncomeStream> IncomeStreamAdded;
        public event EventHandler<IncomeStream> IncomeStreamChanged;
        public event EventHandler<IncomeStream> IncomeStreamDeleted;

        private readonly IncomeStreamDataService _dataService;
        private readonly List<IncomeStream> _incomeStreams = new List<IncomeStream>();

        public IncomeStreamManager()
        {
            _dataService = new IncomeStreamDataService();
        }

        public List<IncomeStream> GetAllIncomeStreams()
        {
            return _incomeStreams;
        }

        public void AddIncomeStream(IncomeStream incomeStream)
        {
            addIncomeStreamAsync(incomeStream);
        }

        public void DeleteIncomeStream(IncomeStream incomeStream)
        {
            deleteIncomeStreamAsync(incomeStream);
        }

        public async Task LoadFromDb(int userId)
        {
            var loaded = await _dataService.LoadByUserId(userId);
            foreach (var incomeStream in loaded)
                addIncomeStreamAsync(incomeStream);
        }

        private void IncomeStream_ValueChanged(object sender, IncomeStream e)
        {
            saveIncomeStreamAsync(e);
        }

        private async void addIncomeStreamAsync(IncomeStream incomeStream)
        {
            if (incomeStream.Id == -1)
            {
                var inserted = await _dataService.Insert(incomeStream);
                incomeStream.Id = inserted.Id;
            }

            _incomeStreams.Add(incomeStream);
            incomeStream.ValueChanged += IncomeStream_ValueChanged;

            IncomeStreamAdded?.Invoke(this, incomeStream);
        }

        private async void saveIncomeStreamAsync(IncomeStream incomeStream)
        {
            await _dataService.Save(incomeStream.Id, incomeStream);
            IncomeStreamChanged?.Invoke(this, incomeStream);
        }

        private async void deleteIncomeStreamAsync(IncomeStream incomeStream)
        {
            await _dataService.Delete(incomeStream.Id);
            _incomeStreams.RemoveAll(t => t.Id == incomeStream.Id);
            IncomeStreamDeleted?.Invoke(this, incomeStream);
        }
    }
}
