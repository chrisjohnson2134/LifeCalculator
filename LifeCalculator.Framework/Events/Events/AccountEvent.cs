using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.LifeEvents
{
    public class AccountEvent : IAccountEvent
    {
        public event EventHandler ValueChanged;

        public string Name { get; set; }
        public int Id { get; set; }
        public int AccountId { get; set; }
        public LifeEnum LifeEventType { get; set; }
        public AccountTypes AccountType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double CurrentValue { get; set; }
        public double Amount { get; set; }
        public double InterestRate { get; set; }

        public AccountEvent()
        {
        }

        public AccountEvent(IAccountEvent entity)
        {
            Name = entity.Name;
            Id = entity.Id;
            AccountId = entity.AccountId;
            LifeEventType = entity.LifeEventType;
            AccountType = entity.AccountType;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            CurrentValue = entity.CurrentValue;
            Amount = entity.Amount;
            InterestRate = entity.InterestRate;
        }

        public override bool Equals(object obj)
        {
            AccountEvent accountEvent = obj as AccountEvent;
            if (accountEvent == null)
                return false;

            if (Id == accountEvent.Id &&
                AccountId == accountEvent.AccountId &&
                Name.Equals(accountEvent.Name) &&
                LifeEventType == accountEvent.LifeEventType &&
                AccountType == accountEvent.AccountType &&
                StartDate.Equals(accountEvent.StartDate) &&
                EndDate.Equals(accountEvent.EndDate) &&
                CurrentValue == accountEvent.CurrentValue &&
                Amount == accountEvent.Amount &&
                InterestRate == accountEvent.InterestRate)
                    return true;

            return false;
        }
    }

    
}
