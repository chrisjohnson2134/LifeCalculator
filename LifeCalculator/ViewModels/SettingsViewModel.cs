using LifeCalculator.Control.Settings.Interfaces;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.ViewModels.Factory;
using System.Collections.ObjectModel;
using LifeCalculator.Control.ViewModels;

namespace LifeCalculator.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IViewModelFactory _viewModelFactory;

        public SettingsViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;

            SettingsTabs = new ObservableCollection<ISettingsViewModel>();

            //SettingsTabs.Add(new PlaidDevSettingsViewModel());
            SettingsTabs.Add(viewModelFactory.CreateViewModel(Framework.Enums.ViewType.PlaidDevSettings) as ISettingsViewModel);
        }

        public ObservableCollection<ISettingsViewModel> SettingsTabs { get; set; }

    }
}
