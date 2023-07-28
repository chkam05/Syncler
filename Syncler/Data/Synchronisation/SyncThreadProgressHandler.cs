using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Synchronisation
{
    public class SyncThreadProgressHandler
    {

        //  VARIABLES

        public string Message { get; set; }
        public object Parameter { get; set; }
        public long? Progress { get; set; }
        public long? ProgressMax { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncThreadProgressHandler class constructor. </summary>
        public SyncThreadProgressHandler()
        {
            //
        }

        #endregion CLASS METHODS

    }
}
