using LifeCalculator.Framework.Authenticator;
using LifeCalculator.Framework.CustomExceptions;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Navigation;
using LifeCalculator.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LifeCalculator.Commands
{
    public class RegisterCommand : ICommand
    {

        #region Events
        
        public event EventHandler CanExecuteChanged;

        #endregion

        #region Fields

        private IAuthenticator _authenticator;
        private RegisterViewModel _viewModel;

        #endregion

        #region Constructors

        public RegisterCommand(RegisterViewModel registerViewModel, IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            _viewModel = registerViewModel;
        }

        #endregion

        #region Methods

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
             var registerResult = await _authenticator.Register(_viewModel.Email, _viewModel.Username, _viewModel.Password, _viewModel.ConfirmPassword);
             if (registerResult == RegistrationResult.Success)
                 _viewModel.UpdateCurrentViewModelCommand.Execute(ViewType.Login);
            if (registerResult == RegistrationResult.EmailAlreadyExists)
                _viewModel.ErrorMessage = "This email address is already in use.";
            if (registerResult == RegistrationResult.PasswordsDoNotMatch)
                _viewModel.ErrorMessage = "Passwords do not match.";
            if (registerResult == RegistrationResult.UsernameAlreadyExists)
                _viewModel.ErrorMessage = "Username is already taken.";
        }

        #endregion


    }
}
