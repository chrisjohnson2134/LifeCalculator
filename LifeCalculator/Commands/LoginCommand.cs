using LifeCalculator.Navigation;
using LifeCalculator.ViewModels;
using System;
using System.Windows.Input;

namespace LifeCalculator.Commands
{
    public class LoginCommand : ICommand
    {

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Fields

        private readonly LoginViewModel _loginViewModel;
        private readonly INavigator _navigator;

        #endregion


        #region Constructors

        public LoginCommand(LoginViewModel loginViewModel, INavigator navigator)
        {
            _loginViewModel = loginViewModel;
            _navigator = navigator;
        }

        #endregion


        #region Methods

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
