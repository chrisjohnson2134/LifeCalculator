﻿using LifeCalculator.Control.Views;
using LifeCalculator.Framework.AccountManager;
using Prism.Ioc;
using Prism.Modularity;
using Unity;

namespace LifeCalculator.Control
{
    public class ControlModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<IAccountManager>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Accounts Views
            containerRegistry.RegisterForNavigation<AddCompound>("AddCompound");
            containerRegistry.RegisterForNavigation<AddLoan>("AddLoan");

            //Events Views
            containerRegistry.RegisterForNavigation<AddEventCompound>("AddEventCompound");
        }
    }
}