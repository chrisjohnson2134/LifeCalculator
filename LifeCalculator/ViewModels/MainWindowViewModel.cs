//keep track of project https://kanbanflow.com/board/eFu1Zwn

using LifeCalculator.Commands;
using LifeCalculator.Framework.Authenticator;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Navigation;
using LifeCalculator.ViewModels.Factory;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace LifeCalculator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private readonly INavigator _navigator;
        private readonly IAuthenticator _authenticator;
        private readonly IAccountStore _accountStore;

        #endregion

        #region Constructors

        public MainWindowViewModel(INavigator navigator, ViewModelFactory viewModelFactory,
            IAuthenticator authenticator, IAccountStore accountStore)
        {
            _navigator = navigator;
            _authenticator = authenticator;
            _accountStore = accountStore;
            _navigator.StateChanged += Navigator_OnStateChanged;
            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, viewModelFactory);
            LogoutCommand = new RelayCommand<object>(LogoutCommand_Execute, LogoutCommand_CanExecute);
            UpdateCurrentViewModelCommand.Execute(ViewType.Welcome);

        }

        #endregion

        #region Properties

        public ViewModelBase CurrentViewModel => _navigator.CurrentViewModel;

        public ViewType CurrentViewType => _navigator.CurrentViewType;

        public ICommand UpdateCurrentViewModelCommand { get; private set; }

        public ICommand LogoutCommand { get; private set; }

        public bool IsLoggedIn => _authenticator.IsLoggedIn;

        #endregion

        #region Command Methods

        private bool LogoutCommand_CanExecute(object obj)
        {
            return _accountStore.CurrentAccount != null;
        }

        private void LogoutCommand_Execute(object obj)
        {
            _accountStore.CurrentAccount = null;
            UpdateCurrentViewModelCommand.Execute(ViewType.Welcome);
        }
        
        #endregion

        #region Event Handlers

        private void Navigator_OnStateChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
            OnPropertyChanged(nameof(CurrentViewType));
            OnPropertyChanged(nameof(IsLoggedIn));
        }

        #endregion
    }
}
