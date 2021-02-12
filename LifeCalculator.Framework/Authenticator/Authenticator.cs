using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Services.AuthenticationService;
using System;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Authenticator
{
    public class Authenticator : IAuthenticator
    {

        #region Events

        public event Action StateChanged;

        #endregion

        #region Fields

        private readonly IAccountStore _accountStore;
        private readonly IAuthenticationService _authenticationService;

        #endregion

        #region Constructors

        public Authenticator(IAccountStore accountStore, IAuthenticationService authenticationService)
        {
            _accountStore = accountStore;
            _authenticationService = authenticationService;
        }

        #endregion

        #region Properties

        public FinancialAccount.FinancialAccount CurrentAccount
        {
            get
            {
               return _accountStore.CurrentAccount;
            }
            private set
            {
                _accountStore.CurrentAccount = value;
                StateChanged?.Invoke();
            }
        }

        public bool IsLoggedIn => CurrentAccount != null;

        #endregion

        #region Methods

        public async Task Login(string username, string password)
        {
            CurrentAccount = await _authenticationService.Login(username, password);
        }

        public void Logout()
        {
            CurrentAccount = null;
        }

        #endregion



    }
}
