using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.EventsDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Managers
{
    public  class AccountsEventsManager : IAccountsEventsManager
    {
        public event EventHandler<IAccountEvent> AccountEventAdded;
        public event EventHandler<IAccountEvent> AccountEventChanged;
        public event EventHandler<IAccountEvent> AccountEventDeleted;

        AccountEventDataService _accountEventDataService;
        

        public AccountsEventsManager()
        {
            _accountEventDataService = new AccountEventDataService();
        }

        private List<IAccountEvent> AccountEvents = new List<IAccountEvent>();

        public IAccountEvent GetAccountEvent(int id)
        {
            return AccountEvents.FirstOrDefault(t => t.Id == id);
        }

        public IAccountEvent GetAccountEvent(string name)
        {
            return AccountEvents.FirstOrDefault(t => t.Name.Equals(name));
        }

        public List<IAccountEvent> GetAllAccountEvents()
        {
            return AccountEvents;
        }

        public List<IAccountEvent> GetAllAccountEventsByAccountId(int accountId)
        {
            return AccountEvents.Where(t => t.AccountId == accountId).ToList();
        }

        public void DeleteAccountEvent(IAccountEvent AccountEvent)
        {
            deleteAccountEventAsync(AccountEvent);
        }

        #region Event Handlers

        public void AddAccountEvent(IAccountEvent AccountEvent)
        {
            addAccountAccountEventAsync(AccountEvent);
        }

        public void AddAccountEvents(List<IAccountEvent> accountEvents)
        {
            foreach (IAccountEvent accountEvent in accountEvents) 
                addAccountAccountEventAsync(accountEvent);
        }

        private void AccountEvent_ValueChanged(object sender, IAccountEvent e)
        {
            saveAccountEventAsync(e);
        }

        #endregion

        #region Private

        private async void addAccountAccountEventAsync(IAccountEvent AccountEvent)
        {
            if (AccountEvent.Id == -1)
            {
                IAccountEvent acc = await _accountEventDataService.Insert(AccountEvent as AccountEvent);
                AccountEvent.Id = acc.Id;
            }

            AccountEvents.Add(AccountEvent);
            AccountEvent.ValueChanged += AccountEvent_ValueChanged;

            AccountEventAdded?.Invoke(this, AccountEvent);
        }

        private async void saveAccountEventAsync(IAccountEvent AccountEvent)
        {
            await _accountEventDataService.Save(AccountEvent.Id,AccountEvent as AccountEvent);
            AccountEventChanged?.Invoke(this, AccountEvent);
        }

        private async void deleteAccountEventAsync(IAccountEvent AccountEvent)
        {
            await _accountEventDataService.Delete(AccountEvent.Id);

            AccountEvents.RemoveAll(t => t.Id == AccountEvent.Id);
            AccountEventDeleted?.Invoke(this, AccountEvent);
        }

        public async Task LoadFromDb()
        {
            var listLoaded = await _accountEventDataService.LoadAll();
            foreach (var accountEvent in listLoaded)
                addAccountAccountEventAsync(accountEvent);
        }

        #endregion
    }
}
