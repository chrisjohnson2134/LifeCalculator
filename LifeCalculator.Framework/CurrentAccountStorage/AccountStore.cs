using System;

namespace LifeCalculator.Framework.CurrentAccountStorage
{
    public class AccountStore : IAccountStore
    {
        #region Events

        public event Action StateChanged;

        #endregion

        #region Fields

        private FinancialAccount.FinancialAccount _currentFinancialAccount;

        #endregion

        #region Properties

        public FinancialAccount.FinancialAccount CurrentAccount
        {
            get
            {
                return _currentFinancialAccount;
            }
            set
            {
                _currentFinancialAccount = value;
                StateChanged.Invoke();
            }
        }

        #endregion
    }
}
