using System;

namespace LifeCalculator.Framework.CurrentAccountStorage
{
    public interface IAccountStore
    {
        #region Properties

        FinancialAccount.FinancialAccount CurrentAccount { get; set; }

        #endregion

        #region Events

        event Action StateChanged;

        #endregion


    }
}
