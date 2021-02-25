using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.EventsDataService
{
    public class EventConcrete : IAccountEvent
    {
        public string Name { get; set; }
        public int Id { get; }
        public int AccountId { get; set; }
        public LifeEnum LifeEventType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double CurrentValue { get; set; }
        public double Amount { get; set; }
        public double InterestRate { get; set; }

        public event EventHandler ValueChanged;

        public EventConcrete()
        {

        }

        public EventConcrete(IAccountEvent entity)
        {
            Name = entity.Name;
            AccountId = entity.Id;
            LifeEventType = entity.LifeEventType;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            CurrentValue = entity.CurrentValue;
            Amount = entity.Amount;
            InterestRate = entity.InterestRate;
        }
    }
}
