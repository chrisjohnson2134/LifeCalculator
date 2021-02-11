using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.BaseVM;
using System.Collections.ObjectModel;

namespace LifeCalculator.ViewModels
{
    public class LoanProfileViewModel : ViewModelBase
    {
        #region Fields

        #endregion


        #region Constructors

        public LoanProfileViewModel()
        {

        }

        #endregion

        #region Properties

        public ObservableCollection<LoanAccount> Loans { get; set; }

        #endregion


    }
}
