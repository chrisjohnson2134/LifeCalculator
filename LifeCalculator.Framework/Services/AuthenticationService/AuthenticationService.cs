using LifeCalculator.Framework.CustomExceptions;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Services.DataService;
using LifeCalculator.Framework.Services.FinancialAccountService;
using LifeCalculator.Framework.Services.UserService;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {

        #region Fields

        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly IFinancialAccountDataService _financialAccountService;
        private readonly IUserDataService _userDataService;

        #endregion

        #region Constructors

        public AuthenticationService(IFinancialAccountDataService financialAccountService, IUserDataService userDataService)
        {
            _passwordHasher = new PasswordHasher<string>();
            _financialAccountService = financialAccountService;
            _userDataService = userDataService;
        }

        #endregion

        public async Task<FinancialAccount.FinancialAccount> Login(string username, string password)
        {
            Users.User storedUser =  await _userDataService.LoadByUsername(username);

            if (storedUser == null)
                throw new UserNotFoundException("Username does not exist.");

            else
            {
                PasswordVerificationResult passwordResult =
                    _passwordHasher.VerifyHashedPassword(username, storedUser.PasswordHashed, password);

                if (passwordResult != PasswordVerificationResult.Success)
                {
                    throw new InvalidPasswordException(username, password);
                }

                FinancialAccount.FinancialAccount storedAccount = _financialAccountService.LoadByUsername(username).Result;

                if (storedAccount == null)
                    throw new FinancialAccountNotFoundException("This financial account does not exist.");

                return storedAccount;
            }
        }

        public async Task<RegistrationResult> Register(string email, string username, string password, string confirmPassword)
        {
            throw new System.NotImplementedException();
        }
    }
}
