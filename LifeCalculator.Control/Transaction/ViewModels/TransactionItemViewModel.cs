using LifeCalculator.Framework.Accounts;

namespace LifeCalculator.Control.ViewModels
{
    public class TransactionItemViewModel
    {
        public TransactionItemViewModel(TransactionItem transactionItem)
        {
            Transaction = transactionItem; ;
        }

        public TransactionItem Transaction { get; set; }
    }
}
