using LifeCalculator.Commands;
using LifeCalculator.Framework.Authenticator;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Navigation;
using LifeCalculator.Tools.Common.Messages;
using LifeCalculator.ViewModels.Factory;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows.Input;

namespace LifeCalculator.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields

        private string _username;
        private string _password;

        #endregion

        #region Constructors

        public LoginViewModel(INavigator loginNavigator, ViewModelFactory viewModelFactory, IAuthenticator authenticator)
        {
            LoginCommand = new LoginCommand(this, authenticator);
            LoginErrorMessage = new ViewModelMessage();
            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(loginNavigator, viewModelFactory);
            ViewRegisterCommand = new DelegateCommand(ViewRegisterCommand_Execute, ViewRegisterCommand_CanExecute);

        }

        #endregion

        #region Properties

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ErrorMessage
        {
            set => LoginErrorMessage.Message = value;
        }

        public ViewModelMessage LoginErrorMessage { get; }

        public ICommand LoginCommand { get; private set; }

        public ICommand ViewRegisterCommand { get; set; }

        public ICommand UpdateCurrentViewModelCommand { get; private set; }

        #endregion

        #region Command Methods

        private void ViewRegisterCommand_Execute(object obj)
        {
            UpdateCurrentViewModelCommand.Execute(ViewType.Register);
        }

        private bool ViewRegisterCommand_CanExecute(object obj)
        {
            return true;
        }

        #endregion
    }
}
