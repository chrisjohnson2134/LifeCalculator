using LifeCalculator.Framework.BaseVM;
using Microsoft.VisualStudio.PlatformUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeCalculator.Control.ViewModels
{
    public class TransactionListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<TransactionItemViewModel> _transactionViewModels;

        public IEnumerable<TransactionItemViewModel> TransactionItemViewModels => _transactionViewModels;

        private TransactionItemViewModel _incomingTransactionItemViewModel;
        public TransactionItemViewModel IncomingTransactionViewModel
        {
            get
            {
                return _incomingTransactionItemViewModel;
            }
            set
            {
                _incomingTransactionItemViewModel = value;
                OnPropertyChanged(nameof(IncomingTransactionViewModel));
            }
        }

        private TransactionItemViewModel _removedTransactionItemViewModel;
        public TransactionItemViewModel RemovedTransactionItemViewModel
        {
            get
            {
                return _removedTransactionItemViewModel;
            }
            set
            {
                _removedTransactionItemViewModel = value;
                OnPropertyChanged(nameof(RemovedTransactionItemViewModel));
            }
        }

        private TransactionItemViewModel _insertedTransactionItemViewModel;
        public TransactionItemViewModel InsertedTransactionItemViewModel
        {
            get
            {
                return _insertedTransactionItemViewModel;
            }
            set
            {
                _insertedTransactionItemViewModel = value;
                OnPropertyChanged(nameof(InsertedTransactionItemViewModel));
            }
        }

        private TransactionItemViewModel _targetTransactionItemViewModel;
        public TransactionItemViewModel TargetTransactionItemViewModel
        {
            get
            {
                return _targetTransactionItemViewModel;
            }
            set
            {
                _targetTransactionItemViewModel = value;
                OnPropertyChanged(nameof(TargetTransactionItemViewModel));
            }
        }

        public ICommand TransactionItemReceivedCommand { get; }
        public ICommand TransactionItemRemovedCommand { get; }
        public ICommand TransactionItemInsertedCommand { get; }

        public TransactionListViewModel()
        {
            _transactionViewModels = new ObservableCollection<TransactionItemViewModel>();

            TransactionItemReceivedCommand = new DelegateCommand(TransactionItemReceivedCommandHandler);
            TransactionItemRemovedCommand = new DelegateCommand(TransactionItemRemovedCommandHandler);
            TransactionItemInsertedCommand = new DelegateCommand(TransactionItemInsertedCommandHandler);
        }

        public  void ClearItems()
        {
            _transactionViewModels.Clear();
        }

        public void AddTransactionItem(TransactionItemViewModel item)
        {
            if (!_transactionViewModels.Contains(item))
            {
                _transactionViewModels.Add(item);
            }
        }

        public void InsertTransactionItem(TransactionItemViewModel insertedTransactionItem, TransactionItemViewModel targetTransactionItem)
        {
            if (insertedTransactionItem == targetTransactionItem)
            {
                return;
            }

            int oldIndex = _transactionViewModels.IndexOf(insertedTransactionItem);
            int nextIndex = _transactionViewModels.IndexOf(targetTransactionItem);

            if (oldIndex != -1 && nextIndex != -1)
            {
                _transactionViewModels.Move(oldIndex, nextIndex);
            }
        }

        #region Command Handler

        public void RemoveTransactionItem(TransactionItemViewModel item)
        {
            _transactionViewModels.Remove(item);
        }

        private void TransactionItemInsertedCommandHandler(object obj)
        {
            InsertTransactionItem(InsertedTransactionItemViewModel, TargetTransactionItemViewModel);
        }

        private void TransactionItemRemovedCommandHandler(object obj)
        {
            RemoveTransactionItem(RemovedTransactionItemViewModel);
        }

        private void TransactionItemReceivedCommandHandler(object obj)
        {
            AddTransactionItem(IncomingTransactionViewModel);
        }

        #endregion

    }
}
