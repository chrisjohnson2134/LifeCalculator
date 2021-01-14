using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Tools.Common.Converters;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace LifeCalculator.Control.ViewModels
{
    public class AddEventCompoundViewModel : BindableBase, INavigationAware
    {
        private IAccount _account;

        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public bool StartEvent { get; set; }
        public bool StopEvent { get; set; }
        public string AmountToContribute { get; set; }
        public string InterestValue { get; set; }
        public DelegateCommand AddEventCommand { get; set; }


        public AddEventCompoundViewModel()
        {
            AddEventCommand = new DelegateCommand(AddLifeEventCommandHandler);
            EventDate = DateTime.Now;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _account = navigationContext.Parameters["account"] as IAccount;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }



        private void AddLifeEventCommandHandler()
        {

            _account.AddLifeEvent(new InvestmentLifeEvent()
            {
                Name = EventName,
                Amount = ConvertString.ToDouble(AmountToContribute),
                Date = EventDate,
                InterestRate = ConvertString.ToDouble(InterestValue) * .01
            });

            _account.Calculation();

            //ReChart(new object(), new EventArgs());
        }
    }
    }

