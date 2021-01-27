using LifeCalculator.Framework.Account;
using System;
using LifeCalculator.Framework.LifeEvents;
using Microsoft.VisualStudio.PlatformUI;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Managers.Interfaces;

namespace LifeCalculator.Control.ViewModels
{
    //Compound Interest View Model
    public class AddCompoundViewModel : ViewModelBase
    {

        #region Constructors

        public AddCompoundViewModel()
        {
            AddAccountCommand = new DelegateCommand(AddAccountCommandHandler);

            StartDate = DateTime.Now;
            StopDate = DateTime.Now.AddYears(1);
        }

        #endregion

        #region Properties

        public string AccountName { get; set; }
        public string DescriptionText { get; set; }
        public double InitialValue { get; set; }
        public double Interest { get; set; }
        public double Contribute { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StopDate { get; set; }
        public DelegateCommand AddAccountCommand { get; set; }

        private IAccountManager _accountManager;

        #endregion

        #region Command Handlers

        /// <summary>
        /// Creates an Account with a Start/Stop Event
        /// </summary>
        private void AddAccountCommandHandler()
        {

            var investmentAccount = new CompoundAccount(AccountName)
            {
                InitialAmount = InitialValue
            };

            InvestmentLifeEvent startEvent = new InvestmentLifeEvent()
            {
                Name = "Start - " + AccountName,
                StartDate = StartDate,
                InterestRate = Interest,
                Amount = Contribute
            };

            InvestmentLifeEvent stopEvent = new InvestmentLifeEvent()
            {
                Name = "Stop - " + AccountName,
                StartDate = StopDate,
                InterestRate = 0,
                Amount = 0
            };

            investmentAccount.AddLifeEvent(startEvent);
            investmentAccount.AddLifeEvent(stopEvent);

            _accountManager.AddAccount(investmentAccount);
        }

        #endregion

    }
}

