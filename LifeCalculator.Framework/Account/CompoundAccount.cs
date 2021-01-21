using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Account
{
    public class CompoundAccount : IAccount
    {
        public string Name { get; set; }
        public double InitialAmount { get; set; }
        public double FinalAmount { get; set; }
        public List<ILifeEvent> AccountLifeEvents { get; set; }

        public event EventHandler<ILifeEvent> LifeEventAdded;

        #region Constructors

        public CompoundAccount()
        {
            AccountLifeEvents = new List<ILifeEvent>();
        }

        public CompoundAccount(string AccountName)
        {
            Name = AccountName;
            AccountLifeEvents = new List<ILifeEvent>();
        }

        #endregion

        #region Methods

        public void SetupBasicCalculation(DateTime startDate, DateTime endDate, double interestRate,
            double initialAmount, double additionalAmount)
        {

            InitialAmount = initialAmount;

            InvestmentLifeEvent lifeEventStart = new InvestmentLifeEvent()
            {
                Date = startDate,
                InterestRate = interestRate,
                Amount = additionalAmount,
                Name = this.Name,
                CurrentValue = initialAmount
            };

            InvestmentLifeEvent lifeEventEnd = new InvestmentLifeEvent()
            {
                Date = endDate,
                Name = this.Name,
                CurrentValue = FinalAmount
            };

            AddLifeEvent(lifeEventStart);
            AddLifeEvent(lifeEventEnd);

            Calculation();
        }

        public List<MonthlyColumn> Calculation()
        {
            double currValue = InitialAmount;
            List<MonthlyColumn> monthlies = new List<MonthlyColumn>();
            int monthDiff = 0;
            FinalAmount = 0;

            AccountLifeEvents.Sort((x, y) => x.Date.CompareTo(y.Date));

            monthlies.Add(new MonthlyColumn());

            for (int i = 0; i < AccountLifeEvents.Count - 1; i++)
            {
                monthDiff = Math.Abs((AccountLifeEvents[i].Date.Year * 12 + (AccountLifeEvents[i].Date.Month - 1))
                    - (AccountLifeEvents[(i + 1)].Date.Year * 12 + (AccountLifeEvents[(i + 1)].Date.Month - 1)));
                AccountLifeEvents[(i + 1)].CurrentValue = 0;

                for (int j = 0; j < monthDiff; j++)
                {
                    currValue = (currValue + AccountLifeEvents[i].Amount) * (1 + (AccountLifeEvents[i].InterestRate / 100) / 12);
                    monthlies.Add(new MonthlyColumn() { Name = AccountLifeEvents[i].Name, Gain = currValue, Date = AccountLifeEvents[i].Date.AddMonths(j) });
                }

                AccountLifeEvents[(i + 1)].CurrentValue = currValue;
            }

            if (monthDiff != 0)
                FinalAmount = monthlies[monthlies.Count - 1].Gain;

            return monthlies;
        }

        public void AddLifeEvent(ILifeEvent lifeEvent)
        {
            AccountLifeEvents.Add(lifeEvent);
            LifeEventAdded?.Invoke(this, lifeEvent);
        }

        #endregion


    }
}
