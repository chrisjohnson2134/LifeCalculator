using System;

namespace LifeCalculator.Framework.CustomExceptions
{
    public class PasswordsDoNotMatchException : Exception
    {
        #region Constructors

        public PasswordsDoNotMatchException()
        {
        }

        public PasswordsDoNotMatchException(string message) : base(message)
        {

        }

        public PasswordsDoNotMatchException(string message, Exception innerException) : base(message, innerException)
        {

        }

        #endregion
    }
}
