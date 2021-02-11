using LifeCalculator.Commands;
using LifeCalculator.Framework.Authenticator;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Navigation;
using LifeCalculator.ViewModels.Factory;
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
            LoginCommand = new LoginCommand(this, loginNavigator, authenticator);
            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(loginNavigator, viewModelFactory);
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

        public ICommand LoginCommand { get; private set; }

        public ICommand UpdateCurrentViewModelCommand { get; private set; }

        #endregion
    }
}
