using LifeCalculator.Framework.ColumnDefinitions;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;

namespace LifeCalculator.Framework.SimulatedAccount
{
    public class LoanAccount : ISimulatedAccount
    {
        public event EventHandler<IAccountEvent> LifeEventAdded;
        public event EventHandler<IAccount> ValueChanged;

        IAccountsEventsManager _accountsEventsManager;

        private int _id = -1;
        public int Id
        {
            get => _id;
            set
            {
                if (_id == -1)
                {
                    _id = value;
                }
            }
        }


        private int _userId;
        public int UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        /// <summary>
        /// True once the user types a payment directly, pinning it. Persisted so a reload
        /// doesn't revert to the amortized payment — that was why edits appeared not to save.
        /// </summary>
        private bool _hasCustomMonthlyPayment;
        public bool HasCustomMonthlyPayment
        {
            get => _hasCustomMonthlyPayment;
            set => _hasCustomMonthlyPayment = value;
        }

        private double _monthlyPayment;
        public double MonthlyPayment
        {
            get
            {
                return _monthlyPayment;
            }
            set
            {
                setMonthlyPaymentAndSolveForLength(value);
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _loanAmount;
        public double LoanAmount
        {
            get
            {
                return _loanAmount;
            }
            set
            {
                _loanAmount = value;
                updateMonthlyPayment();
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _downPayment;
        public double DownPayment
        {
            get
            {
                return _downPayment;
            }
            set
            {
                _downPayment = value;
                updateMonthlyPayment();
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _interestRate;
        public double InterestRate
        {
            get
            {
                return _interestRate;
            }
            set
            {
                _interestRate = value;
                updateMonthlyPayment();
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _interestPaid;
        public double InterestPaid
        {
            get
            {
                return _interestPaid;
            }
            private set
            {
                _interestPaid = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private double _principalPaid;
        public double PrincipalPaid
        {
            get
            {
                return _principalPaid;
            }
            private set
            {
                _principalPaid = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        private int _loanLengthMonths;
        public int LoanLengthMonths
        {
            get
            {
                return _loanLengthMonths;
            }
            set
            {
                _loanLengthMonths = value;
                updateMonthlyPayment();
                ValueChanged?.Invoke(this, this);
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                ValueChanged?.Invoke(this, this);
            }
        }

        [IgnoreDatabase]
        public List<IAccountEvent> AccountLifeEvents => _accountsEventsManager.GetAllAccountEventsByAccountId(Id,AccountTypes.LoanAccount);

        public LoanAccount()
        {
        }

        public LoanAccount(IAccountsEventsManager accountsEventsManager)
        {
            SetEventsManager(accountsEventsManager);
        }

        public LoanAccount(IAccountsEventsManager _eventsManager, string name, DateTime date, int loanLengthMonths, double interestRate, double loanAmount, double downPayment)
        {
            SetEventsManager(_eventsManager);

            _name = name;
            _interestRate = interestRate / 100;
            _loanAmount = loanAmount;
            _downPayment = downPayment;
            _loanLengthMonths = loanLengthMonths;
            _startDate = date;

            updateMonthlyPayment();

        }

        public void AddLifeEvent(IAccountEvent lifeEvent)
        {
            lifeEvent.ValueChanged += LifeEvent_ValueChanged;
            _accountsEventsManager.AddAccountEvent(lifeEvent);
            ValueChanged?.Invoke(this, this);
        }

        private void LifeEvent_ValueChanged(object sender, IAccountEvent e)
        {
            ValueChanged?.Invoke(this, this);
        }

        /// <summary>
        /// Recomputes whichever value the user hasn't pinned.
        ///
        /// Normally the payment is derived from the term. But once someone types a payment
        /// directly, that payment is what they've committed to — so changing the balance or
        /// rate afterwards should move the PAYOFF DATE, not silently rewrite their payment.
        /// This also matters on load: Dapper hydrates through these same public setters, so
        /// without the guard a saved custom payment gets overwritten by the amortized figure
        /// the moment the row is read back.
        /// </summary>
        private void updateMonthlyPayment()
        {
            if (_hasCustomMonthlyPayment)
            {
                if (_monthlyPayment > 0)
                    solveForLength(_monthlyPayment);

                return;
            }

            if (_loanLengthMonths <= 0)
            {
                _monthlyPayment = 0;
                return;
            }

            // The standard amortization formula is 0/0 (NaN) at exactly 0% interest;
            // a 0% loan's payment is simply principal spread evenly over its term.
            if (Math.Abs(_interestRate) < 1e-9)
            {
                _monthlyPayment = Math.Round((_loanAmount - _downPayment) / _loanLengthMonths, 2);
                return;
            }

            _monthlyPayment = (_loanAmount - _downPayment) * (Math.Pow((1 + (_interestRate / 12)), _loanLengthMonths) * _interestRate)
                / (12 * (Math.Pow((1 + (_interestRate / 12)), _loanLengthMonths) - 1));

            _monthlyPayment = Math.Round(_monthlyPayment,2);
        }

        /// <summary>
        /// The inverse of updateMonthlyPayment(): given a payment the user wants to make,
        /// solves for how many months it takes to pay off the loan, so editing the payment
        /// directly (rather than the term) is a supported, equally valid way to plan a loan.
        /// </summary>
        private void setMonthlyPaymentAndSolveForLength(double payment)
        {
            _hasCustomMonthlyPayment = true;
            _monthlyPayment = Math.Round(payment, 2);
            solveForLength(_monthlyPayment);
        }

        /// <summary>Derives the payoff term implied by a given monthly payment.</summary>
        private void solveForLength(double payment)
        {
            double principal = _loanAmount - _downPayment;

            if (payment <= 0 || principal <= 0)
                return;

            double monthlyRate = _interestRate / 12;

            if (Math.Abs(_interestRate) < 1e-9)
            {
                _loanLengthMonths = (int)Math.Ceiling(principal / payment);
                return;
            }

            double interestOnlyPayment = principal * monthlyRate;

            if (payment <= interestOnlyPayment)
            {
                // This payment would never cover even the interest - the loan would never
                // amortize. Keep the previous term rather than producing an infinite
                // or negative one.
                return;
            }

            double months = -Math.Log(1 - (principal * monthlyRate) / payment) / Math.Log(1 + monthlyRate);

            _loanLengthMonths = (int)Math.Ceiling(months);
        }

        public List<MonthlyColumn> Calculation()
        {
            double currValue = _loanAmount - _downPayment;
            double interestPay;
            double principalPay;
            _interestPaid = 0;
            _principalPaid = 0;
            List<MonthlyColumn> monthlies = new List<MonthlyColumn>();

            monthlies.Add(new MonthlyColumn());
            int monthDiff = 0;

            AccountLifeEvents.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            DateTime stopDate = _startDate.AddMonths(LoanLengthMonths);

            monthDiff = Math.Abs(_startDate.Year * 12 + (_startDate.Month - 1)
                    - (stopDate.Year * 12 + (stopDate.Month - 1)));


            for (int j = 0; j < monthDiff; j++)
            {
                interestPay = currValue * _interestRate / 12;

                if (_monthlyPayment < currValue)
                    principalPay = _monthlyPayment - interestPay + additionalPriPaymentCalculation(StartDate.AddMonths(1 + j));
                else if (currValue > 0)
                    principalPay = currValue;
                else
                    principalPay = 0;

                _interestPaid += interestPay;
                _principalPaid += principalPay;
                currValue = currValue - principalPay;

                

                monthlies.Add(new MonthlyColumn()
                {
                    Name = _name,
                    Gain = Math.Round((_loanAmount - _downPayment) - _principalPaid,2),
                    Date = _startDate.AddMonths(1 + j)
                });

                if (currValue < 0)
                {
                    currValue = 0;
                    monthlies[monthlies.Count-1].Gain = 0;
                    break;
                }

            }

            monthlies[monthlies.Count - 1].Gain = Math.Round(monthlies[monthlies.Count - 1].Gain + currValue,2);
            return monthlies;
        }

        private double additionalPriPaymentCalculation(DateTime dateTime)
        {
            return AccountEventResolver.ResolveAdditionalAmount(AccountLifeEvents, dateTime);
        }

        public void SetEventsManager(IAccountsEventsManager accountsEventsManager)
        {
            _accountsEventsManager = accountsEventsManager;
        }

        public override bool Equals(object obj)
        {
            var other = obj as LoanAccount;

            return obj == null ? false :
                Id.Equals(other.Id) &&
                DownPayment.Equals(other.DownPayment) &&
                InterestPaid.Equals(other.InterestPaid) &&
                InterestRate.Equals(other.InterestRate) &&
                LoanAmount.Equals(other.LoanAmount) &&
                LoanLengthMonths.Equals(other.LoanLengthMonths) &&
                PrincipalPaid.Equals(other.PrincipalPaid) &&
                UserId.Equals(other.UserId) && 
                Name.Equals(other.Name) &&
                MonthlyPayment.Equals(other.MonthlyPayment) &&
                StartDate.Equals(other.StartDate);
        }
    }
}
