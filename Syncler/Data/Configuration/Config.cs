using Newtonsoft.Json;
using Syncler.Data.Synchronisation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public List<SyncFileDiffrence> SyncMethods { get; set; }
        public Point WindowPosition { get; set; }
        public Size WindowSize { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Config class constructor. </summary>
        /// <param name="appearanceConfig"> Appearance configuration. </param>
        [JsonConstructor]
        public Config(
            AppearanceConfig appearanceConfig = null,
            List<SyncGroup> syncGroups = null,
            List<SyncFileDiffrence> syncMethods = null,
            Point? windowPosition = null,
            Size? windowSize = null)
        {
            AppearanceConfig = appearanceConfig ?? AppearanceConfig.Default;
            SyncGroups = syncGroups ?? new List<SyncGroup>();
            SyncMethods = syncMethods ?? new List<SyncFileDiffrence>() { SyncFileDiffrence.Name, SyncFileDiffrence.Size };
            WindowPosition = windowPosition.HasValue ? windowPosition.Value : new Point(200, 200);
            WindowSize = windowSize.HasValue ? windowSize.Value : new Size(800, 450);
        }

        #endregion CLASS METHODS

    }
}
