using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Data;

namespace LifeCalculator.Tools.Common.Converters
{
    /// <summary>
    /// Displays an enum value using its [Description] attribute, so the UI can show real labels
    /// ("401(k)", "Roth IRA") for identifiers that C# won't allow verbatim (an identifier can't
    /// start with a digit, hence FourOhOneK). Falls back to splitting PascalCase into words, so
    /// an enum without descriptions still reads as "Debt Payoff" rather than "DebtPayoff".
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var type = value.GetType();
            if (!type.IsEnum)
                return value.ToString();

            string name = value.ToString();
            FieldInfo field = type.GetField(name);

            if (field != null)
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                    return attribute.Description;
            }

            return SplitPascalCase(name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(EnumDescriptionConverter)} is display-only.");
        }

        private static string SplitPascalCase(string name)
        {
            var builder = new StringBuilder(name.Length + 4);

            for (int i = 0; i < name.Length; i++)
            {
                if (i > 0 && char.IsUpper(name[i]) && !char.IsUpper(name[i - 1]))
                    builder.Append(' ');

                builder.Append(name[i]);
            }

            return builder.ToString();
        }
    }
}
