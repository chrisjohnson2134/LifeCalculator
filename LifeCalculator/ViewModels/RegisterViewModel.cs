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
    public class RegisterViewModel : ViewModelBase
    {
        #region Fields

        private string _email;
        private string _username;
        private string _password;
        private string _confirmPassword;
        #endregion

        #region Constructors

        public RegisterViewModel(INavigator navigator, IAuthenticator authenticator, IViewModelFactory viewModelFactory)
        {
            RegisterCommand = new RegisterCommand(this, authenticator);
            RegisterErrorMessage = new ViewModelMessage();
            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, viewModelFactory);
            ViewLoginCommand = new DelegateCommand(ViewLoginCommand_Execute, ViewLoginCommand_CanExecute);

        }

        #endregion

        #region Properties

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public string ErrorMessage
        {
            set => RegisterErrorMessage.Message = value;
        }

        public ViewModelMessage RegisterErrorMessage { get; }

        public ICommand RegisterCommand { get; private set; }

        public ICommand UpdateCurrentViewModelCommand { get; private set; }

        public ICommand ViewLoginCommand { get; private set; }

        #endregion

        #region Command Methods

        private bool ViewLoginCommand_CanExecute(object obj)
        {
            return true;
        }

        private void ViewLoginCommand_Execute(object obj)
        {
            UpdateCurrentViewModelCommand.Execute(ViewType.Login);
            Email = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }

        #endregion
    }
}
