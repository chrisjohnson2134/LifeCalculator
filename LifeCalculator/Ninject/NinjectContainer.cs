using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Managers.Interfaces;
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
            Bind<FinancialProfileViewModel>().ToSelf().InSingletonScope();
            Bind<CreateViewModel<HomeViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<HomeViewModel>();
            });
            Bind<CreateViewModel<FinancialProfileViewModel>>().ToMethod((Kernel) =>
            {
                return () => kernel.Get<FinancialProfileViewModel>();
            });

        }
    }
}
