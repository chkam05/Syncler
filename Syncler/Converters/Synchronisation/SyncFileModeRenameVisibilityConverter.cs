using Syncler.Data.Synchronisation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Syncler.Converters.Synchronisation
{
    public class SyncFileModeRenameVisibilityConverter : IValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var syncFileMode = (SyncFileMode)value;
            return syncFileMode == SyncFileMode.RENAME ? Visibility.Visible : Visibility.Collapsed;
        }

        //  --------------------------------------------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
