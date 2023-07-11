using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Synchronisation
{
    public enum SyncFileMode
    {
        NONE = 0,
        COPY = 1,
        RENAME = 2,
        REMOVE = 3,
    }
}
