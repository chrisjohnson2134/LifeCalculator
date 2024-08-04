using LifeCalculator.Control.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LifeCalculator.Control.Views
{
    /// <summary>
    /// Interaction logic for BudgetItemTile.xaml
    /// </summary>
    public partial class BudgetItemTile : UserControl
    {

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransactionInsertedCommandProperty =
            DependencyProperty.Register("TransactionInsertedCommand", typeof(ICommand), typeof(BudgetItemTile),
                new PropertyMetadata(null));

        public ICommand TransactionInsertedCommand
        {
            get { return (ICommand)GetValue(TransactionInsertedCommandProperty); }
            set { SetValue(TransactionInsertedCommandProperty, value); }
        }

        public object TransactionInserted
        {
            get { return (TransactionItemViewModel)GetValue(TransactionInsertedProperty); }
            set { SetValue(TransactionInsertedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransactionInsertedProperty =
            DependencyProperty.Register("TransactionInserted", typeof(TransactionItemViewModel), typeof(BudgetItemTile),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public BudgetItemTile()
        {
            InitializeComponent();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            TransactionInserted = (TransactionItemViewModel)e.Data.GetData(DataFormats.Serializable);
            TransactionInsertedCommand.Execute(TransactionInserted);
        }
    }
}
