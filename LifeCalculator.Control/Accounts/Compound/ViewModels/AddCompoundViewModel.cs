using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.AccountManager;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using LifeCalculator.Framework.LifeEvents;

namespace LifeCalculator.Control.ViewModels
{
    //Compound Interest View Model
    public class AddCompoundViewModel : BindableBase, INavigationAware
    {
        public string AccountName { get; set; }
        public string DescriptionText { get; set; }
        public double InitialValue { get; set; }
        public double Interest { get; set; }
        public double Contribute { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StopDate { get; set; }
        public DelegateCommand AddAccountCommand { get; set; }

        private IAccountManager _accountManager;


        public AddCompoundViewModel()
        {
            AddAccountCommand = new DelegateCommand(AddAccountCommandHandler);

            StartDate = DateTime.Now;
            StopDate = DateTime.Now.AddYears(1);
        }

        private void AddAccountCommandHandler()
        {

            var investmentAccount = new CompoundAccount(AccountName)
            {
                InitialAmount = InitialValue
            };

            InvestmentLifeEvent startEvent = new InvestmentLifeEvent()
            {
                Name = "Start - " + AccountName,
                Date = StartDate,
                InterestRate = Interest,
                Amount = Contribute
            };

            InvestmentLifeEvent stopEvent = new InvestmentLifeEvent()
            {
                Name = "Stop - " + AccountName,
                Date = StopDate,
                InterestRate = 0,
                Amount = 0
            };

            investmentAccount.AddLifeEvent(startEvent);
            investmentAccount.AddLifeEvent(stopEvent);

            _accountManager.AddAccount(investmentAccount);
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _accountManager = navigationContext.Parameters["accountManager"] as IAccountManager;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
            //throw new System.NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new System.NotImplementedException();
        }
    }
}

