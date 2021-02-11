using System;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Authenticator
{
    public interface IAuthenticator
    {
        #region Events

        event Action StateChanged;

        #endregion

        #region Properties

        FinancialAccount.FinancialAccount CurrentAccount { get; }

        bool IsLoggedIn { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Get's an account for a user's credentials.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="password">the user's password.</param>
        /// <returns>The account for the user.</returns>
        Task Login(string username, string password);

        void Logout();

        #endregion
    }
}
