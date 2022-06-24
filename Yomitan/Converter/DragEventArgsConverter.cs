using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Yomitan.Converter
{
    internal class DragEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DragEventArgs))
                return null;

            var args = (DragEventArgs)value;
            var data = args.Data as DataObject;

            var list = new List<string>();
            foreach (var item in data.GetFileDropList())
                list.Add(item);

            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
