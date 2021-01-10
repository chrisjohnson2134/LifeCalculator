using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.LifeEvents;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Account
{
    public class LoanAccount : IAccount
    {
        public string Name { get; set; }
        public double FinalAmount { get; set; }
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

        public LoanAccount(DateTime date,double interestRate,double loanAmount,double downPayment)
        {
            InterestRate = interestRate/100;
            LoanAmount = loanAmount - downPayment;
            DownPayment = downPayment;

            MonthlyPayment = Math.Floor((loanAmount - downPayment) * (Math.Pow((1 + InterestRate / 12), 360) * InterestRate) 
                / (12 * (Math.Pow((1 + InterestRate / 12), 360) - 1)));

            AccountLifeEvents = new List<ILifeEvent>();

            AddLifeEvent(new MortgageLifeEvent() {Name="Start", Date = DateTime.Now});
            AddLifeEvent(new MortgageLifeEvent() {Name="End", Date = DateTime.Now.AddYears(30) });
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
            List<MonthlyColumn> monthlies = new List<MonthlyColumn>();
            int monthDiff = 0;

            AccountLifeEvents.Sort((x, y) => x.Date.CompareTo(y.Date));

            for (int i = 0; i < AccountLifeEvents.Count - 1; i++)
            {
                monthDiff = Math.Abs((AccountLifeEvents[i].Date.Year * 12 + (AccountLifeEvents[i].Date.Month - 1))
                    - (AccountLifeEvents[(i + 1)].Date.Year * 12 + (AccountLifeEvents[(i + 1)].Date.Month - 1)));

                for (int j = 0; j < monthDiff; j++)
                {
                    interestPay = LoanAmount * InterestRate / 12;
                    principalPay = MonthlyPayment - interestPay;
                    InterestPaid += interestPay;
                    PrincipalPaid += principalPay;
                    LoanAmount = LoanAmount - principalPay;
                    (AccountLifeEvents[i] as MortgageLifeEvent).InterestPaid += interestPay;
                    (AccountLifeEvents[i] as MortgageLifeEvent).PrincipalPaid += principalPay;
                    monthlies.Add(new MonthlyColumn() { Name = AccountLifeEvents[i].Name, Amount = PrincipalPaid, Date = AccountLifeEvents[i].Date.AddMonths(j) });
                }
            }

            if (monthDiff != 0)
                FinalAmount = monthlies[monthlies.Count - 1].Amount;

            return monthlies;
        }

    }
}
