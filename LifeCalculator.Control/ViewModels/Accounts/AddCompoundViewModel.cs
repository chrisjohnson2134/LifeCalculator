using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.AccountManager;
using LifeCalculator.Tools.Common.Converters;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeCalculator.Control.ViewModels
{
    //Compound Interest View Model
    public class AddCompoundViewModel : BindableBase, INavigationAware
    {
        public string AccountName { get; set; }
        public string DescriptionText { get; set; }
        public string InitialValue { get; set; }
        public DelegateCommand AddAccountCommand { get; set; }

        private IAccountManager _accountManager;

        public AddCompoundViewModel()
        {
            AddAccountCommand = new DelegateCommand(AddAccountCommandHandler);
        }

        private void AddAccountCommandHandler()
        {

            var investmentAccount = new InvestmentAccount(AccountName)
            {
                InitialAmount = ConvertString.ToDouble(InitialValue)
            };

            //investmentAccount.LifeEventAdded += LifeEventAddedHandler;
            _accountManager.AddAccount(investmentAccount);

            //try
            //{
            //    var dayConfig = Mappers.Xy<ILifeEvent>()
            //    .X(dayModel => dayModel.Date.Ticks / (TimeSpan.FromDays(1).Ticks * 30.44))
            //    .Y(dayModel => dayModel.CurrentValue);
            //    var series = new ColumnSeries(dayConfig);
            //    series.Title = investmentAccount.Name;
            //    series.Values = new ChartValues<ILifeEvent>();
            //    ValueCollection.Add(series);
            //    Formatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks * 30.44)).ToString("MM/yyyy");
            //}
            //catch (Exception e)
            //{

            //}
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

