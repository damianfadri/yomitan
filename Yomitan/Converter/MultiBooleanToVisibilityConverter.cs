using System;
using System.Windows.Data;

namespace Yomitan.Converter
{
    public class MultiBooleanToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visible = false;
            foreach (object value in values)
                if (value is bool)
                    visible = visible || (bool)value;

            if (visible)
                // return System.Windows.Visibility.Visible;
                return true;
            else
                // return System.Windows.Visibility.Hidden;
                return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
