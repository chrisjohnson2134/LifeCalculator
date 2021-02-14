//keep track of project https://kanbanflow.com/board/eFu1Zwn

using LifeCalculator.Commands;
using LifeCalculator.Framework.Authenticator;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Navigation;
using LifeCalculator.ViewModels.Factory;
using System.Windows.Input;

namespace LifeCalculator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private readonly INavigator _navigator;
        private readonly IAuthenticator _authenticator;

        #endregion

        #region Constructors

        public MainWindowViewModel(INavigator navigator, ViewModelFactory viewModelFactory, IAuthenticator authenticator)
        {
            _navigator = navigator;
            _authenticator = authenticator;
            _navigator.StateChanged += Navigator_OnStateChanged;
            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, viewModelFactory);
            UpdateCurrentViewModelCommand.Execute(ViewType.Welcome);
        }

        #endregion

        #region Properties

        public ViewModelBase CurrentViewModel => _navigator.CurrentViewModel;

        public ICommand UpdateCurrentViewModelCommand { get; private set; }

        public bool IsLoggedIn => _authenticator.IsLoggedIn;

        #endregion

        #region Event Handlers

        private void Navigator_OnStateChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
            OnPropertyChanged(nameof(IsLoggedIn));
        }

        #endregion
    }
}
