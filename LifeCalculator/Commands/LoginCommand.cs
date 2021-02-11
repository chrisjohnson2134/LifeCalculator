﻿using LifeCalculator.Framework.Authenticator;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Navigation;
using LifeCalculator.ViewModels;
using System;
using System.Windows;
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
        private readonly IAuthenticator _authenticator;

        #endregion


        #region Constructors

        public LoginCommand(LoginViewModel loginViewModel, INavigator navigator, IAuthenticator authenticator)
        {
            _loginViewModel = loginViewModel;
            _navigator = navigator;
            _authenticator = authenticator;
        }

        #endregion


        #region Methods

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            try
            {
                await _authenticator.Login(_loginViewModel.Username, _loginViewModel.Password);
                _loginViewModel.UpdateCurrentViewModelCommand.Execute(ViewType.Home);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion
    }
}
