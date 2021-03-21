using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Database;
using LifeCalculator.Framework.Database.Queries;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LifeCalculator.Framework.Account
{
    public class CompoundAccount : ViewModelBase, IAccount
    {

        #region Events

        public event EventHandler<IAccountEvent> LifeEventAdded;
        public event EventHandler ValueChanged;

        #endregion

        #region Constructors

        public CompoundAccount()
        {
            AccountLifeEvents = new List<IAccountEvent>();
        }

        public CompoundAccount(string AccountName)
        {
            Name = AccountName;
            AccountLifeEvents = new List<IAccountEvent>();
        }

        public CompoundAccount(CompoundAccount compoundAccount)
        {
            Name = compoundAccount.Name;
            AccountLifeEvents = new List<IAccountEvent>();
        }

        #endregion

        #region Properties

        public int Id { get; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public double InitialAmount { get; set; }
        public double MonthlyContribute { get; set; }
        public double InterestRate { get; set; }
        public double FinalAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [IgnoreDatabase]
        public List<IAccountEvent> AccountLifeEvents { get; set; }
        
        #endregion

        #region Methods

        public void SetupBasicCalculation(DateTime startDate, DateTime endDate, double interestRate,
            double initialAmount, double additionalAmount)
        {

            InitialAmount = initialAmount;
            InterestRate = interestRate;
            MonthlyContribute = additionalAmount;
            StartDate = startDate;
            EndDate = endDate;

            Calculation();
        }

        public List<MonthlyColumn> Calculation()
        {
            double currValue = InitialAmount;
            List<MonthlyColumn> monthlies = new List<MonthlyColumn>();
            int monthDiff = 0;
            FinalAmount = 0;

            AccountLifeEvents.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            monthlies.Add(new MonthlyColumn());

            //for (int i = 0; i < AccountLifeEvents.Count - 1; i++)
            //{
            monthDiff = Math.Abs((StartDate.Year * 12 + (StartDate.Month - 1))
                - (EndDate.Year * 12 + (EndDate.Month - 1)));
            

            for (int j = 0; j < monthDiff; j++)
                {
                currValue = (currValue + MonthlyContribute) * (1 + (InterestRate / 100) / 12);
                monthlies.Add(new MonthlyColumn() { Name = Name, Gain = currValue, Date = StartDate.AddMonths(j) });
                //currValue = (currValue + AccountLifeEvents[i].Amount) * (1 + (AccountLifeEvents[i].InterestRate / 100) / 12);
                //monthlies.Add(new MonthlyColumn() { Name = AccountLifeEvents[i].Name, Gain = currValue, Date = AccountLifeEvents[i].StartDate.AddMonths(j) });
            }

                //CurrentValue = currValue;
            //}

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

        #region Overrident Methods

        public override bool Equals(object obj)
        {
            var temp = obj as CompoundAccount;

            if (temp == null)
                return false;
            else if (temp.FinalAmount == this.FinalAmount &&
               temp.InitialAmount == InitialAmount &&
                temp.Id == Id &&
                temp.Name.Equals(Name))
            {
                foreach (var item in AccountLifeEvents)
                {
                    var accEvent = temp.AccountLifeEvents.Find(t => t.Id == item.Id);
                    if (!accEvent.Equals(item))
                        return false;
                }
                return true;
            }
            else
                return false;

        }

        #endregion
    }
}
