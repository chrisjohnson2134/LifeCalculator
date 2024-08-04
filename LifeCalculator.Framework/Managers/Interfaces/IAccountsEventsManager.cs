using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Managers
{
    public interface IAccountsEventsManager
    {
        event EventHandler<IAccountEvent> AccountEventAdded;
        event EventHandler<IAccountEvent> AccountEventChanged;
        event EventHandler<IAccountEvent> AccountEventDeleted;

        void AddAccountEvent(IAccountEvent AccountEvent);
        IAccountEvent GetAccountEvent(int id);
        IAccountEvent GetAccountEvent(string name);
        List<IAccountEvent> GetAllAccountEvents();
        List<IAccountEvent> GetAllAccountEventsByAccountId(int accountId, AccountTypes accType);

        void DeleteAccountEvent(IAccountEvent AccountEvent);
        void AddAccountEvents(List<IAccountEvent> accountEvents);
        Task LoadFromDb();
    }
}
