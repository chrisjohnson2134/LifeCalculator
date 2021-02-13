using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using System.Collections.ObjectModel;

namespace LifeCalculator.ViewModels
{
    public class LoanProfileViewModel : ViewModelBase
    {
        #region Fields

        private IAccountStore _accountStore;

        #endregion


        #region Constructors

        public LoanProfileViewModel(IAccountStore accountStore)
        {
            _accountStore = accountStore;
        }

        #endregion

        #region Properties

        public ObservableCollection<LoanAccount> Loans { get; set; }

        #endregion


    }
}
