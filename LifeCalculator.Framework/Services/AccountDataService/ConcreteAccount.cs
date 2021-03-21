using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.AccDataService
{
    public class ConcreteAccount : IAccount
    {
        public event EventHandler ValueChanged;

        public string Name { get; set; }
        public int Id { get; }
        public int UserId { get; set; }
        public double InitialAmount { get; set; }
        public double InterestRate { get; set; }

        [IgnoreDatabase]
        public List<IAccountEvent> AccountLifeEvents { get; set; }
        
        public ConcreteAccount()
        {

        }

        public ConcreteAccount(IAccount entity)
        {
            Name = entity.Name;
            Id = entity.Id;
            UserId = entity.UserId;
            AccountLifeEvents = entity.AccountLifeEvents;
        }

        public event EventHandler<IAccountEvent> LifeEventAdded;
        public event EventHandler valueChanged;

        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
        }

        public List<MonthlyColumn> Calculation()
        {
            return new List<MonthlyColumn>();
        }
    }

}

