using LifeCalculator.Commands;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Navigation;
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

        public LoginViewModel(INavigator loginNavigator)
        {
            LoginCommand = new LoginCommand(this, loginNavigator);
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

        #endregion
    }
}
