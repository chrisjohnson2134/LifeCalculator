using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LifeCalculator.Behaviors
{
    /// <summary>
    /// Select-all-on-focus for text fields, so typing into a field (especially a numeric one
    /// like an amount or rate) immediately replaces its contents instead of requiring the user
    /// to manually select/clear it first. Applied app-wide via the default TextBox style.
    /// </summary>
    public static class TextBoxBehaviors
    {
        public static readonly DependencyProperty SelectAllOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllOnFocus",
                typeof(bool),
                typeof(TextBoxBehaviors),
                new PropertyMetadata(false, OnSelectAllOnFocusChanged));

        public static void SetSelectAllOnFocus(DependencyObject element, bool value) => element.SetValue(SelectAllOnFocusProperty, value);
        public static bool GetSelectAllOnFocus(DependencyObject element) => (bool)element.GetValue(SelectAllOnFocusProperty);

        private static void OnSelectAllOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBox textBox))
                return;

            if ((bool)e.NewValue)
            {
                textBox.GotFocus += TextBox_GotFocus;
                textBox.PreviewMouseLeftButtonDown += TextBox_PreviewMouseLeftButtonDown;
            }
            else
            {
                textBox.GotFocus -= TextBox_GotFocus;
                textBox.PreviewMouseLeftButtonDown -= TextBox_PreviewMouseLeftButtonDown;
            }
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }

        private static void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }
    }
}
