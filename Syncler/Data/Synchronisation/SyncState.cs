using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Synchronisation
{
    public enum SyncState
    {
        NONE = 0,
        SCANNING = 1,
        SYNCING = 2,
        STOPPED_SCANNING = 3,
        STOPPED_SYNCING = 4,
    }
}
