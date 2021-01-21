using LifeCalculator.Control;
using LifeCalculator.Control.Views;
using LifeCalculator.Framework.Managers.Interfaces;
using LifeCalculator.Framework.Managers;
using LifeCalculator.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace LifeCalculator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IAccountManager, AccountManager>();
            containerRegistry.RegisterForNavigation<AddCompound>("CompoundInterest");
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<ControlModule>();
        }
    }
}
