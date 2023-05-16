using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Syncler.Data.Configuration
{
    public class Config
    {

        //  VARIABLES

        public AppearanceConfig AppearanceConfig { get; set; }
        public List<SyncGroup> SyncGroups { get; set; }
        public Point WindowPosition { get; set; }
        public Size WindowSize { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Config class constructor. </summary>
        /// <param name="appearanceConfig"> Appearance configuration. </param>
        [JsonConstructor]
        public Config(AppearanceConfig appearanceConfig = null, List<SyncGroup> syncGroups = null)
        {
            AppearanceConfig = appearanceConfig ?? AppearanceConfig.Default;
            SyncGroups = syncGroups ?? new List<SyncGroup>();
        }

        #endregion CLASS METHODS

    }
}
