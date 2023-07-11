using Syncler.Data.Synchronisation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Syncler.Converters.Synchronisation
{
    public class SyncFileModeNameConverter : IValueConverter
    {

        //  VAIRABLES

        private static readonly Dictionary<SyncFileMode, string> _dict = new Dictionary<SyncFileMode, string>()
        {
            { SyncFileMode.NONE, "Do nothing" },
            { SyncFileMode.COPY, "Copy" },
            { SyncFileMode.RENAME, "Rename and copy" },
            { SyncFileMode.REMOVE, "Delete" }
        };


        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var syncFileMode = (SyncFileMode)value;

            if (_dict.ContainsKey(syncFileMode))
                return _dict[syncFileMode];

            return _dict[SyncFileMode.NONE];
        }

        //  --------------------------------------------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = (string)value;

            if (_dict.ContainsValue(stringValue))
                return _dict.FirstOrDefault(kvp => kvp.Value == stringValue).Key;

            return SyncFileMode.NONE;
        }

    }
}
