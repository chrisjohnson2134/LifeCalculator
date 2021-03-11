using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Database;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Account
{
    public class LoanAccount : IAccount, IDatabaseable
    {
        private LoanAccount loanAccount;

        public int Id { get; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public double MonthlyPayment { get; set; }
        public double LoanAmount { get; set; }
        public double DownPayment { get; set; }
        public double InterestRate { get; set; }
        public double InterestPaid { get; set; }
        public double PrincipalPaid { get; set; }
        public int LoanLengthMonths { get; set; }
        [IgnoreDatabase]
        public List<IAccountEvent> AccountLifeEvents { get; set; }

        public event EventHandler<IAccountEvent> LifeEventAdded;

        public LoanAccount()
        {

        }

        public LoanAccount(string name, DateTime date, int loanLengthMonths, double interestRate, double loanAmount, double downPayment)
        {
            Name = name;
            InterestRate = interestRate / 100;
            LoanAmount = loanAmount - downPayment;
            DownPayment = downPayment;
            LoanLengthMonths = loanLengthMonths;

            MonthlyPayment = Math.Floor((loanAmount - downPayment) * (Math.Pow((1 + InterestRate / 12), 360) * InterestRate)
                / (12 * (Math.Pow((1 + InterestRate / 12), loanLengthMonths) - 1)));

            AccountLifeEvents = new List<IAccountEvent>();

            AddLifeEvent(new AccountEvent() { Name = "Start - " + Name, StartDate = date, LifeEventType = LifeEnum.StartLifeEvent,AccountType = AccountTypes.LoanAccount });
            AddLifeEvent(new AccountEvent() { Name = "Stop - " + Name, StartDate = date.AddMonths(loanLengthMonths), LifeEventType = LifeEnum.EndLifeEvent, AccountType = AccountTypes.LoanAccount });
        }

        public LoanAccount(LoanAccount loanAccount)
        {
            this.loanAccount = loanAccount;
        }

        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
            lifeEvent.AccountType = AccountTypes.LoanAccount;
            AccountLifeEvents.Add(lifeEvent);
            LifeEventAdded?.Invoke(this, lifeEvent);
        }

        public List<MonthlyColumn> Calculation()
        {
            double currValue = LoanAmount;
            double interestPay;
            double principalPay;
            InterestPaid = 0;
            PrincipalPaid = 0;
            List<MonthlyColumn> monthlies = new List<MonthlyColumn>();

            monthlies.Add(new MonthlyColumn());
            int monthDiff = 0;

            AccountLifeEvents.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            IAccountEvent startLifeEvent = AccountLifeEvents.Find(i => i.LifeEventType == LifeEnum.StartLifeEvent);
            IAccountEvent stopLifeEvent = AccountLifeEvents.Find(i => i.LifeEventType == LifeEnum.EndLifeEvent);
            monthDiff = Math.Abs(startLifeEvent.StartDate.Year * 12 + (startLifeEvent.StartDate.Month - 1)
                    - (stopLifeEvent.StartDate.Year * 12 + (stopLifeEvent.StartDate.Month - 1)));


            for (int j = 0; j < monthDiff; j++)
            {
                interestPay = currValue * InterestRate / 12;

                if (MonthlyPayment < currValue)
                    principalPay = MonthlyPayment - interestPay + additionalPriPaymentCalculation(startLifeEvent.StartDate.AddMonths(1 + j));
                else if (currValue > 0)
                    principalPay = currValue;
                else
                    principalPay = 0;

                InterestPaid += interestPay;
                PrincipalPaid += principalPay;
                currValue = currValue - principalPay;
                monthlies.Add(new MonthlyColumn()
                {
                    Name = startLifeEvent.Name,
                    Gain = LoanAmount - PrincipalPaid,
                    Date = startLifeEvent.StartDate.AddMonths(1 + j)
                });
            }

            monthlies[monthlies.Count - 1].Gain = monthlies[monthlies.Count - 1].Gain + currValue;
            return monthlies;
        }

        private double additionalPriPaymentCalculation(DateTime dateTime)
        {
            double additonalAmount = 0;

            AccountLifeEvents.FindAll(i => i.StartDate < dateTime && dateTime < i.EndDate && i.LifeEventType == LifeEnum.MonthlyContribute)
                .ForEach(i => additonalAmount += i.Amount);

            AccountLifeEvents.FindAll(i => i.StartDate.Year == dateTime.Year && dateTime.Month == i.StartDate.Month && i.LifeEventType == LifeEnum.OneTime)
                .ForEach(i => additonalAmount += i.Amount);

            if(additonalAmount > 0)
                Console.WriteLine(additonalAmount);

            return additonalAmount;
        }

        public override bool Equals(object obj)
        {
            var entity = obj as LoanAccount;

            if (obj == null)
                return false;

            if (entity.Id == Id &&
                entity.UserId == UserId &&
                entity.Name.Equals(Name) &&
                entity.MonthlyPayment == MonthlyPayment &&
                entity.LoanAmount == LoanAmount &&
                entity.DownPayment == DownPayment &&
                entity.InterestRate == InterestRate &&
                entity.InterestPaid == InterestPaid &&
                entity.PrincipalPaid == PrincipalPaid &&
                entity.LoanLengthMonths == LoanLengthMonths)
            {
                foreach (var item in AccountLifeEvents)
                {
                    var accEvent = entity.AccountLifeEvents.Find(t => t.Id == item.Id);
                    if (!accEvent.Equals(item))
                        return false;
                }
                return true;
            }

            return false;
        }
    }
}
