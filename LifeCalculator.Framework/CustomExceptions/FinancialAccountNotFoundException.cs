using System;

namespace LifeCalculator.Framework.CustomExceptions
{
    public class FinancialAccountNotFoundException : Exception
    {
        #region Constructors

        public FinancialAccountNotFoundException()
        {
        }

        public FinancialAccountNotFoundException(string message) : base(message)
        {

        }

        public FinancialAccountNotFoundException(string message, Exception innerException) : base(message, innerException)
        {

        }

        #endregion
    }
}
