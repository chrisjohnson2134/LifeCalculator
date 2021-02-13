using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Database;
using LifeCalculator.Framework.Database.Queries;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LifeCalculator.Framework.Account
{
    public class CompoundAccount : GenericDataService<CompoundAccount> , IAccount
    {
        #region Events

        public event EventHandler<IAccountEvent> LifeEventAdded;

        #endregion

        #region Constructors

        public CompoundAccount()
            :base("CompoundAccounts")
        {
            AccountLifeEvents = new List<IAccountEvent>();
        }

        public CompoundAccount(string AccountName)
            :base("CompoundAccounts")
        {
            Name = AccountName;
            AccountLifeEvents = new List<IAccountEvent>();
        }

        #endregion

        #region Properties

        public string Name { get; set; }
        public int Id { get; }
        public double InitialAmount { get; set; }
        public double FinalAmount { get; set; }
        [IgnoreDatabase]
        public List<IAccountEvent> AccountLifeEvents { get; set; }

        #endregion

        #region Methods

        public void SetupBasicCalculation(DateTime startDate, DateTime endDate, double interestRate,
            double initialAmount, double additionalAmount)
        {

            InitialAmount = initialAmount;

            InvestmentAccountEvent lifeEventStart = new InvestmentAccountEvent()
            {
                StartDate = startDate,
                InterestRate = interestRate,
                Amount = additionalAmount,
                Name = this.Name,
                CurrentValue = initialAmount
            };

            InvestmentAccountEvent lifeEventEnd = new InvestmentAccountEvent()
            {
                StartDate = endDate,
                Name = this.Name,
                CurrentValue = FinalAmount
            };

            AddLifeEvent(lifeEventStart);
            AddLifeEvent(lifeEventEnd);

            Calculation();
            CompoundQueries.Save(this);
        }

        public List<MonthlyColumn> Calculation()
        {
            double currValue = InitialAmount;
            List<MonthlyColumn> monthlies = new List<MonthlyColumn>();
            int monthDiff = 0;
            FinalAmount = 0;

            AccountLifeEvents.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            monthlies.Add(new MonthlyColumn());

            for (int i = 0; i < AccountLifeEvents.Count - 1; i++)
            {
                monthDiff = Math.Abs((AccountLifeEvents[i].StartDate.Year * 12 + (AccountLifeEvents[i].StartDate.Month - 1))
                    - (AccountLifeEvents[(i + 1)].StartDate.Year * 12 + (AccountLifeEvents[(i + 1)].StartDate.Month - 1)));
                AccountLifeEvents[(i + 1)].CurrentValue = 0;

                for (int j = 0; j < monthDiff; j++)
                {
                    currValue = (currValue + AccountLifeEvents[i].Amount) * (1 + (AccountLifeEvents[i].InterestRate / 100) / 12);
                    monthlies.Add(new MonthlyColumn() { Name = AccountLifeEvents[i].Name, Gain = currValue, Date = AccountLifeEvents[i].StartDate.AddMonths(j) });
                }

                AccountLifeEvents[(i + 1)].CurrentValue = currValue;
            }

            if (monthDiff != 0)
                FinalAmount = monthlies[monthlies.Count - 1].Gain;

            return monthlies;
        }

        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
            AccountLifeEvents.Add(lifeEvent);
            LifeEventAdded?.Invoke(this, lifeEvent);
        }

        #endregion
    }
}
