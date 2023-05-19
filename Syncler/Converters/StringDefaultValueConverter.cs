using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Syncler.Converters
{
    public class StringDefaultValueConverter : IValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = (string)value;

            if (string.IsNullOrWhiteSpace(stringValue))
                return (string)parameter;

            return stringValue;
        }

        //  --------------------------------------------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = (string)value;

            if (string.IsNullOrWhiteSpace(stringValue))
                return (string)parameter;

            return stringValue;
        }

    }
}
