using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LifeCalculator.Control.Views
{
    /// <summary>
    /// Interaction logic for TransactionItemListingView.xaml
    /// </summary>
    public partial class TransactionListView : UserControl
    {
        public static readonly DependencyProperty IncomingTransactionItemProperty =
            DependencyProperty.Register("IncomingTransactionItem", typeof(object), typeof(TransactionListView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object IncomingTransactionItem
        {
            get { return (object)GetValue(IncomingTransactionItemProperty); }
            set { SetValue(IncomingTransactionItemProperty, value); }
        }

        public static readonly DependencyProperty RemovedTransactionItemProperty =
            DependencyProperty.Register("RemovedTransactionItem", typeof(object), typeof(TransactionListView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object RemovedTransactionItem
        {
            get { return (object)GetValue(RemovedTransactionItemProperty); }
            set { SetValue(RemovedTransactionItemProperty, value); }
        }

        public static readonly DependencyProperty TransactionItemDropCommandProperty =
            DependencyProperty.Register("TransactionItemDropCommand", typeof(ICommand), typeof(TransactionListView),
                new PropertyMetadata(null));

        public ICommand TransactionItemDropCommand
        {
            get { return (ICommand)GetValue(TransactionItemDropCommandProperty); }
            set { SetValue(TransactionItemDropCommandProperty, value); }
        }

        public static readonly DependencyProperty TransactionItemRemovedCommandProperty =
            DependencyProperty.Register("TransactionItemRemovedCommand", typeof(ICommand), typeof(TransactionListView),
                new PropertyMetadata(null));

        public ICommand TransactionItemRemovedCommand
        {
            get { return (ICommand)GetValue(TransactionItemRemovedCommandProperty); }
            set { SetValue(TransactionItemRemovedCommandProperty, value); }
        }

        public static readonly DependencyProperty TransactionItemInsertedCommandProperty =
            DependencyProperty.Register("TransactionItemInsertedCommand", typeof(ICommand), typeof(TransactionListView),
                new PropertyMetadata(null));

        public ICommand TransactionItemInsertedCommand
        {
            get { return (ICommand)GetValue(TransactionItemInsertedCommandProperty); }
            set { SetValue(TransactionItemInsertedCommandProperty, value); }
        }

        public static readonly DependencyProperty InsertedTransactionItemProperty =
            DependencyProperty.Register("InsertedTransactionItem", typeof(object), typeof(TransactionListView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object InsertedTransactionItem
        {
            get { return (object)GetValue(InsertedTransactionItemProperty); }
            set { SetValue(InsertedTransactionItemProperty, value); }
        }

        public static readonly DependencyProperty TargetTransactionItemProperty =
            DependencyProperty.Register("TargetTransactionItem", typeof(object), typeof(TransactionListView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object TargetTransactionItem
        {
            get { return (object)GetValue(TargetTransactionItemProperty); }
            set { SetValue(TargetTransactionItemProperty, value); }
        }

        public TransactionListView()
        {
            InitializeComponent();
        }

        private void TransactionItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                sender is FrameworkElement frameworkElement)
            {
                object TransactionItem = frameworkElement.DataContext;

                DragDropEffects dragDropResult = DragDrop.DoDragDrop(frameworkElement,
                    new DataObject(DataFormats.Serializable, TransactionItem),
                    DragDropEffects.Move);

                if (dragDropResult == DragDropEffects.None)
                {
                    AddTransactionItem(TransactionItem);
                }
            }
        }

        private void TransactionItem_DragOver(object sender, DragEventArgs e)
        {
            if (TransactionItemInsertedCommand?.CanExecute(null) ?? false)
            {
                if (sender is FrameworkElement element)
                {
                    TargetTransactionItem = element.DataContext;
                    InsertedTransactionItem = e.Data.GetData(DataFormats.Serializable);

                    TransactionItemInsertedCommand?.Execute(null);
                }
            }
        }

        private void TransactionItemList_DragOver(object sender, DragEventArgs e)
        {
            object TransactionItem = e.Data.GetData(DataFormats.Serializable);
            AddTransactionItem(TransactionItem);
        }

        private void AddTransactionItem(object TransactionItem)
        {
            if (TransactionItemDropCommand?.CanExecute(null) ?? false)
            {
                IncomingTransactionItem = TransactionItem;
                TransactionItemDropCommand?.Execute(null);
            }
        }

        private void TransactionItemList_DragLeave(object sender, DragEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(lvItems, e.GetPosition(lvItems));

            if (result == null)
            {
                if (TransactionItemRemovedCommand?.CanExecute(null) ?? false)
                {
                    RemovedTransactionItem = e.Data.GetData(DataFormats.Serializable);
                    TransactionItemRemovedCommand?.Execute(null);
                }
            }
        }

    }
}
