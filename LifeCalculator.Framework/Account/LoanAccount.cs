using LifeCalculator.Framework.ColumnDefinitions;
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
        public List<ILifeEvent> AccountLifeEvents { get; set; }

        public event EventHandler<ILifeEvent> LifeEventAdded;

        public LoanAccount()
        {
            
        }

        public LoanAccount(string name, DateTime date, double interestRate, double loanAmount, double downPayment)
        {
            Name = name;
            InterestRate = interestRate/100;
            LoanAmount = loanAmount - downPayment;
            DownPayment = downPayment;

            MonthlyPayment = Math.Floor((loanAmount - downPayment) * (Math.Pow((1 + InterestRate / 12), 360) * InterestRate) 
                / (12 * (Math.Pow((1 + InterestRate / 12), 360) - 1)));

            AccountLifeEvents = new List<ILifeEvent>();

            AddLifeEvent(new MortgageLifeEvent() {Name="Start - " + Name, Date = date });
            AddLifeEvent(new MortgageLifeEvent() {Name="Stop - " + Name, Date = date.AddYears(30) });
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

            for (int i = 0; i < AccountLifeEvents.Count - 1; i++)
            {
                monthDiff = Math.Abs((AccountLifeEvents[i].Date.Year * 12 + (AccountLifeEvents[i].Date.Month - 1))
                    - (AccountLifeEvents[(i + 1)].Date.Year * 12 + (AccountLifeEvents[(i + 1)].Date.Month - 1)));

                for (int j = 0; j < monthDiff; j++)
                {
                    interestPay = currValue * InterestRate / 12;
                    principalPay = MonthlyPayment - interestPay;
                    InterestPaid += interestPay;
                    PrincipalPaid += principalPay;
                    currValue = currValue - principalPay;
                    (AccountLifeEvents[i] as MortgageLifeEvent).InterestPaid += interestPay;
                    (AccountLifeEvents[i] as MortgageLifeEvent).PrincipalPaid += principalPay;
                    monthlies.Add(new MonthlyColumn() { Name = AccountLifeEvents[i].Name, Gain = PrincipalPaid,
                        Date = AccountLifeEvents[i].Date.AddMonths(1+j) });
                }
            }

            monthlies[monthlies.Count - 1].Gain = monthlies[monthlies.Count - 1].Gain + currValue;

            return monthlies;
        }

    }
}
