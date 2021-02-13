using System;

namespace LifeCalculator.Framework.CustomExceptions
{
    public class UsernameAlreadyExistsException : Exception
    {
        #region Constructors

        public UsernameAlreadyExistsException()
        {
        }

        public UsernameAlreadyExistsException(string message) : base(message)
        {

        }

        public UsernameAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {

        }

        #endregion
    }
}
