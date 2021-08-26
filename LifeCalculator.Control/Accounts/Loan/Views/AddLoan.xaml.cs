using System.Windows.Controls;

namespace LifeCalculator.Control.Views
{
    /// <summary>
    /// Interaction logic for AddLoan
    /// </summary>
    public partial class AddLoan : UserControl
    {
        public AddLoan()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
