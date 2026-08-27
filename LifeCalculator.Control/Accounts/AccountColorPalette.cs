using System.Windows.Media;

namespace LifeCalculator.Control.Accounts
{
    /// <summary>
    /// Shared, stable color palette so an account is drawn with the same color
    /// in the chart and in the accounts list.
    /// </summary>
    public static class AccountColorPalette
    {
        private static readonly Color[] _colors = new[]
        {
            (Color)ColorConverter.ConvertFromString("#1982C4"), // blue
            (Color)ColorConverter.ConvertFromString("#FF595E"), // red
            (Color)ColorConverter.ConvertFromString("#8AC926"), // green
            (Color)ColorConverter.ConvertFromString("#FFCA3A"), // yellow
            (Color)ColorConverter.ConvertFromString("#6A4C93"), // purple
            (Color)ColorConverter.ConvertFromString("#FF924C"), // orange
            (Color)ColorConverter.ConvertFromString("#4267AC"), // indigo
            (Color)ColorConverter.ConvertFromString("#52A675"), // teal
        };

        public static Color ColorAt(int index) => _colors[index % _colors.Length];

        public static SolidColorBrush BrushAt(int index)
        {
            var brush = new SolidColorBrush(ColorAt(index));
            brush.Freeze();
            return brush;
        }
    }
}
