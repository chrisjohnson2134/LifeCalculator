using LifeCalculator.Framework.Enums;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {

        #region Fields

        private readonly IPasswordHasher<string> _passwordHasher;

        #endregion

        #region Constructors

        public AuthenticationService()
        {
            _passwordHasher = new PasswordHasher<string>();
        }

        #endregion

        public async Task<FinancialAccount.FinancialAccount> Login(string username, string password)
        {
            FinancialAccount.FinancialAccount storedAccount = _financialAccountService.GetByUsername(username).Result;

            if (storedAccount == null)
            {
                //Make UserNotFoundException
                throw new System.ArgumentException();
            }
            else
            {
                PasswordVerificationResult passwordResult =
                    _passwordHasher.VerifyHashedPassword(username, storedAccount.AccountHolder.PasswordHashed, password);

                if (passwordResult != PasswordVerificationResult.Success)
                {
                    //Make InvalidPasswordException
                    throw new System.ArgumentException(username, password);
                }

                return storedAccount;
            }
        }

    public async Task<RegistrationResult> Register(string email, string username, string password, string confirmPassword)
        {
            throw new System.NotImplementedException();
        }
    }
}
