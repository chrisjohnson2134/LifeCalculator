using System;
using System.Globalization;
using System.Windows.Data;

namespace LifeCalculator.Tools.Common.Converters
{
    public class EqualValueToParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return value.ToString() == parameter.ToString();
        }

        /// <summary>
        /// Lets a group of RadioButtons drive an enum property two-way: the one being checked
        /// reports its parameter as the new value, and the ones being unchecked report nothing.
        /// Without the DoNothing the group would write a value on every unchecked button too,
        /// and the last one to fire would win.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool isChecked) || !isChecked || parameter == null)
                return Binding.DoNothing;

            Type enumType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (enumType.IsEnum)
                return Enum.Parse(enumType, parameter.ToString());

            return parameter;
        }
    }
}
