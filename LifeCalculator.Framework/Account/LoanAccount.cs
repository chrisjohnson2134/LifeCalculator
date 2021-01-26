using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Account
{
    public class LoanAccount : IAccount
    {
        public string Name { get; set; }
        public double MonthlyPayment { get; set; }
        public double LoanAmount { get; set; }
        public double DownPayment { get; set; }
        public double InterestRate { get; set; }
        public double InterestPaid { get; set; }
        public double PrincipalPaid { get; set; }
        public int LoanLengthMonths { get; set; }
        public List<ILifeEvent> AccountLifeEvents { get; set; }

        public event EventHandler<ILifeEvent> LifeEventAdded;

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

            AccountLifeEvents = new List<ILifeEvent>();

            AddLifeEvent(new LoanLifeEvent() { Name = "Start - " + Name, Date = date, LifeEventType = LifeEnum.StartLifeEvent });
            AddLifeEvent(new LoanLifeEvent() { Name = "Stop - " + Name, Date = date.AddMonths(loanLengthMonths), LifeEventType = LifeEnum.EndLifeEvent });
        }

        public void AddLifeEvent(ILifeEvent lifeEvent)
        {
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

            AccountLifeEvents.Sort((x, y) => x.Date.CompareTo(y.Date));

            ILifeEvent startLifeEvent = AccountLifeEvents.Find(i => i.LifeEventType == LifeEnum.StartLifeEvent);
            ILifeEvent stopLifeEvent = AccountLifeEvents.Find(i => i.LifeEventType == LifeEnum.EndLifeEvent);
            monthDiff = Math.Abs(startLifeEvent.Date.Year * 12 + (startLifeEvent.Date.Month - 1)
                    - (stopLifeEvent.Date.Year * 12 + (stopLifeEvent.Date.Month - 1)));


            for (int j = 0; j < monthDiff; j++)
            {
                interestPay = currValue * InterestRate / 12;

                if (MonthlyPayment < currValue)
                    principalPay = MonthlyPayment - interestPay + additionalPriPaymentCalculation(startLifeEvent.Date.AddMonths(1 + j));
                else if (currValue > 0)
                    principalPay = currValue;
                else
                    principalPay = 0;

                InterestPaid += interestPay;
                PrincipalPaid += principalPay;
                currValue = currValue - principalPay;
                (startLifeEvent as LoanLifeEvent).InterestPaid += interestPay;
                (startLifeEvent as LoanLifeEvent).PrincipalPaid += principalPay;
                monthlies.Add(new MonthlyColumn()
                {
                    Name = startLifeEvent.Name,
                    Gain = PrincipalPaid,
                    Date = startLifeEvent.Date.AddMonths(1 + j)
                });
            }

            monthlies[monthlies.Count - 1].Gain = monthlies[monthlies.Count - 1].Gain + currValue;

            return monthlies;
        }

        private double additionalPriPaymentCalculation(DateTime dateTime)
        {
            double additonalAmount = 0;

            AccountLifeEvents.FindAll(i => i.Date < dateTime && dateTime < i.EndDate)
                .ForEach(i => additonalAmount += i.Amount); 


            return additonalAmount;
        }
    }
}
