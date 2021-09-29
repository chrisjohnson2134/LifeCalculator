using LifeCalculator.Framework.CustomExceptions;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Services.FinancialAccountService;
using LifeCalculator.Framework.Services.UserService;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
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
            Users.User storedUser = await _userDataService.LoadByUsername(username);
                       
            PasswordVerificationResult passwordResult =
                _passwordHasher.VerifyHashedPassword(username, storedUser.PasswordHashed, password);

            if (passwordResult != PasswordVerificationResult.Success)
            {
                throw new InvalidPasswordException("Username or password is incorrect.");
            }

            FinancialAccount.FinancialAccount storedAccount = await _financialAccountService.LoadByUsername(username);         

            return storedAccount;
            
        }

        public async Task<RegistrationResult> Register(string email, string username, string password, string confirmPassword)
        {
            RegistrationResult result = RegistrationResult.Success;

            if (password == confirmPassword)
            {              
                bool existingAccount = await _userDataService.DoesUserExistByEmail(email);

                if (existingAccount == false)
                {
                    bool usernameAccount = await _userDataService.DoesUserExistByUsername(username);

                    if (usernameAccount == false)
                    {
                        string hashedPassword = _passwordHasher.HashPassword(username, password);

                        Users.User user = new Users.User
                        {
                            Email = email,
                            Username = username,
                            PasswordHashed = hashedPassword,
                            DateRegistered = DateTime.Now.ToString()
                        };

                        FinancialAccount.FinancialAccount financialAccount = new FinancialAccount.FinancialAccount
                        {
                            AccountHolder = user.Username
                        };

                        await _userDataService.Insert(user);
                        await _financialAccountService.Insert(financialAccount);
                    }
                    else
                    {
                        result = RegistrationResult.UsernameAlreadyExists;
                    }
                }
                else
                {
                    result = RegistrationResult.EmailAlreadyExists;
                }
            }
            else
            {
                result = RegistrationResult.PasswordsDoNotMatch;
            }

            
            
            return result;
        }
    }
}
