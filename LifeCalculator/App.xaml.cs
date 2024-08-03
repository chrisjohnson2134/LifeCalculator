using LifeCalculator.Framework.Settings;
using LifeCalculator.Ninject;
using LifeCalculator.ViewModels;
using LifeCalculator.Views;
using Ninject;
using System.Windows;

namespace LifeCalculator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IKernel kernel = new StandardKernel();
            NinjectContainer container = new NinjectContainer();
            kernel.Load(container);
            //AppSettings.LoadCredentials();
            Window window = new MainWindow();
            window.DataContext = kernel.Get<MainWindowViewModel>();

            window.Show();
            base.OnStartup(e);
        }
    }
}
