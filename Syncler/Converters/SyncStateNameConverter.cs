using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using Syncler.Data.Synchronisation;

namespace Syncler.Converters
{
    public class SyncStateNameConverter : IValueConverter
    {

        //  CONST

        private static readonly Dictionary<SyncState, string> _dict = new Dictionary<SyncState, string>()
        {
            { SyncState.NONE, string.Empty },
            { SyncState.SCANNING, "Scanning" },
            { SyncState.STOPPED_SCANNING, "Scanning stopped" },
            { SyncState.SYNCING, "Synchronising" },
            { SyncState.STOPPED_SYNCING, "Synchronisation stopped" },
        };


        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var syncState = (SyncState)value;

            return _dict.TryGetValue(syncState, out string result)
                ? result : string.Empty;
        }

        //  --------------------------------------------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = (string)value;

            if (!string.IsNullOrEmpty(name) && _dict.Any(d => d.Value == name))
                return _dict.FirstOrDefault(d => d.Value == name).Key;

            return SyncState.NONE;
        }

    }
}
