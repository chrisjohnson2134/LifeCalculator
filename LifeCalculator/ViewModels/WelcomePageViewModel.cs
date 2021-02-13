using LifeCalculator.Commands;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Navigation;
using LifeCalculator.ViewModels.Factory;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows.Input;

namespace LifeCalculator.ViewModels
{
    public class WelcomePageViewModel : ViewModelBase
    {
        #region Constructors

        public WelcomePageViewModel(INavigator navigator, IViewModelFactory viewModelFactory)
        {
            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, viewModelFactory);
            NavigateToLoginViewCommand = new DelegateCommand(NavigateToLoginView_Execute, NavigateToLoginView_CanExecute);
            NavigateToRegisterViewCommand = new DelegateCommand(NavigateToRegisterView_Execute, NavigateToRegisterView_CanExecute);
        }

        #endregion

        #region Properties

        public ICommand NavigateToLoginViewCommand { get; private set; }

        public ICommand NavigateToRegisterViewCommand { get; private set; }

        public ICommand UpdateCurrentViewModelCommand { get; private set; }

        #endregion

        #region Command Methods


        private bool NavigateToLoginView_CanExecute(object obj)
        {
            return true;
        }

        private void NavigateToLoginView_Execute(object obj)
        {
            UpdateCurrentViewModelCommand.Execute(ViewType.Login);
        }

        private bool NavigateToRegisterView_CanExecute(object obj)
        {
            return true;
        }

        private void NavigateToRegisterView_Execute(object obj)
        {
            UpdateCurrentViewModelCommand.Execute(ViewType.Register);
        }


        #endregion
    }
}
