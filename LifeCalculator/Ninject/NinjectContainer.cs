﻿using LifeCalculator.Framework.Authenticator;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Managers.Interfaces;
using LifeCalculator.Framework.Services.AuthenticationService;
using LifeCalculator.Framework.Services.FinancialAccountService;
using LifeCalculator.Navigation;
using LifeCalculator.ViewModels;
using LifeCalculator.ViewModels.Factory;
using Microsoft.AspNetCore.Identity;
using Ninject;
using Ninject.Modules;

namespace LifeCalculator.Ninject
{
    public class NinjectContainer : NinjectModule
    {
        public override void Load()
        {
            IKernel kernel = Kernel;
            Bind<MainWindowViewModel>().ToSelf().InSingletonScope();
            Bind<HomeViewModel>().ToSelf().InSingletonScope();
            Bind<LoanProfileViewModel>().ToSelf().InSingletonScope();
            Bind<FinancialProfileViewModel>().ToSelf().InSingletonScope();
            Bind<LoginViewModel>().ToSelf().InSingletonScope();
            Bind<INavigator>().To<Navigator>().InSingletonScope();
            Bind<IAccountManager>().To<AccountManager>().InSingletonScope();
            Bind<IViewModelFactory>().To<ViewModelFactory>().InSingletonScope();
            Bind<IAccountStore>().To<AccountStore>().InSingletonScope();
            Bind<IAuthenticator>().To<Authenticator>().InSingletonScope();
            Bind<IFinancialAccountService>().To<FinancialAccountService>().InSingletonScope();
            Bind<IAuthenticationService>().To<AuthenticationService>().InSingletonScope();
            Bind<CreateViewModel<HomeViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<HomeViewModel>();
            });
            Bind<CreateViewModel<FinancialProfileViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<FinancialProfileViewModel>();
            });
            Bind<CreateViewModel<LoanProfileViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<LoanProfileViewModel>();
            });
            Bind<CreateViewModel<LoginViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<LoginViewModel>();
            });

        }
    }
}
