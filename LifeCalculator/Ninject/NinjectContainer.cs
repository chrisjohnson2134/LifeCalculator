﻿using LifeCalculator.Framework.AccountManager;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Navigation;
using LifeCalculator.ViewModels;
using LifeCalculator.ViewModels.Factory;
using Ninject;
using Ninject.Modules;

namespace LifeCalculator.Ninject
{
    public class NinjectContainer : NinjectModule
    {
        public override void Load()
        {
            IKernel kernel = Kernel;
            Bind<INavigator>().To<Navigator>().InSingletonScope();
            Bind<IAccountManager>().To<AccountManager>().InSingletonScope();
            Bind<IViewModelFactory>().To<ViewModelFactory>().InSingletonScope();
            Bind<MainWindowViewModel>().ToSelf().InSingletonScope();
            Bind<HomeViewModel>().ToSelf().InSingletonScope();
            Bind<CreateViewModel<HomeViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<HomeViewModel>();
            });

        }
    }
}
