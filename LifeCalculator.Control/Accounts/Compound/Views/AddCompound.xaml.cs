using System.Windows.Controls;

namespace LifeCalculator.Control.Views
{
    /// <summary>
    /// Interaction logic for Compound
    /// </summary>
    public partial class AddCompound : UserControl
    {
        public AddCompound()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
