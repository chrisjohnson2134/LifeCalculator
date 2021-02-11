using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.CustomExceptions
{
    public class UserNotFoundException : Exception
    {
        #region Constructors

        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string message) : base(message)
        {

        }

        public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {

        }

        #endregion
    }
}
