using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Synchronisation
{
    public class SyncValidationConfig
    {

        //  VARIABLES

        public bool ByName { get; set; }
        public bool BySize { get; set; }
        public bool ByChecksum { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncValidationConfig class constructor. </summary>
        public SyncValidationConfig()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        public static SyncValidationConfig Default => new SyncValidationConfig()
        {
            ByName = true,
            BySize = true,
            ByChecksum = true
        };

        #endregion CLASS METHODS

    }
}
