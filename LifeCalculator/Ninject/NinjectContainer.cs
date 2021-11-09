using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.Authenticator;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Services.AuthenticationService;
using LifeCalculator.Framework.Services.FinancialAccountService;
using LifeCalculator.Framework.Services.UserService;
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

            Bind<MainWindowViewModel>().ToSelf().InSingletonScope();
            Bind<HomeViewModel>().ToSelf().InSingletonScope();
            Bind<LoanProfileViewModel>().ToSelf().InSingletonScope();
            Bind<FinancialProfileViewModel>().ToSelf().InSingletonScope();
            Bind<LoginViewModel>().ToSelf().InSingletonScope();
            Bind<RegisterViewModel>().ToSelf().InSingletonScope();
            Bind<WelcomePageViewModel>().ToSelf().InSingletonScope();
            Bind<BudgetViewModel>().ToSelf().InSingletonScope();
            Bind<CalculatorViewModel>().ToSelf().InSingletonScope();
            Bind<SettingsViewModel>().ToSelf().InSingletonScope();
            Bind<PlaidDevSettingsViewModel>().ToSelf().InSingletonScope();
            Bind<TransactionSorterViewModel>().ToSelf().InSingletonScope();
            Bind<BudgetItemsControlViewModel>().ToSelf().InSingletonScope();
            Bind<BudgetItemTileViewModel>().ToSelf().InSingletonScope();
            Bind<TransactionListViewModel>().ToSelf().InSingletonScope();
            Bind<TransactionItemViewModel>().ToSelf().InSingletonScope();

            Bind<INavigator>().To<Navigator>().InSingletonScope();
            Bind<IViewModelFactory>().To<ViewModelFactory>().InSingletonScope();
            Bind<IAccountStore>().To<AccountStore>().InSingletonScope();
            Bind<IAuthenticator>().To<Authenticator>().InSingletonScope();

            Bind<IFinancialAccountDataService>().To<FinancialAccountDataService>().InTransientScope().WithConstructorArgument("FinancialAccounts");
            Bind<IUserDataService>().To<UserDataService>().InTransientScope().WithConstructorArgument("Users");
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
            Bind<CreateViewModel<BudgetViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<BudgetViewModel>();
            });
            Bind<CreateViewModel<LoginViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<LoginViewModel>();
            });
            Bind<CreateViewModel<RegisterViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<RegisterViewModel>();
            });
            Bind<CreateViewModel<WelcomePageViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<WelcomePageViewModel>();
            });
            Bind<CreateViewModel<CalculatorViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<CalculatorViewModel>();
            });
            Bind<CreateViewModel<SettingsViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<SettingsViewModel>();
            });
            Bind<CreateViewModel<PlaidDevSettingsViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<PlaidDevSettingsViewModel>();
            });

        }
    }
}
