using LifeCalculator.Framework.Enums;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        #region Methods

        Task<RegistrationResult> Register(string email, string username, string password, string confirmPassword);

        /// <summary>
        /// Get's an account for a user's credentials.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="password">the user's password.</param>
        /// <returns>The account for the user.</returns>
        Task<FinancialAccount.FinancialAccount> Login(string username, string password);

        #endregion
    }
}
