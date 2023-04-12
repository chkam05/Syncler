using Newtonsoft.Json;
using Syncler.Attributes;
using Syncler.Data.Configuration;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Syncler.Data.Synchronisation
{
    public class SyncManager : INotifyPropertyChanged
    {

        //  CONST

        private const string DATA_FILE_NAME = "data.json";


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private static SyncManager _instance;
        private static object _instanceLock = new object();

        private SyncConfig _syncConfig;
        private bool _loaded = false;


        //  GETTERS & SETTERS

        public static SyncManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SyncManager();
                            _instance.LoadSettings();
                        }
                    }
                }

                return _instance;
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public SyncConfig SyncConfig
        {
            get => _syncConfig;
            private set
            {
                _syncConfig = value;
                _loaded = true;
                UpdateConfigurationProperties();
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Private SyncManager instance class constructor. </summary>
        private SyncManager()
        {
            //
        }

        #endregion CLASS METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load settings from json file. </summary>
        public void LoadSettings()
        {
            string configDirPath = Path.Combine(
                Environment.GetEnvironmentVariable("APPDATA"),
                ApplicationHelper.GetApplicationName());

            if (!Directory.Exists(configDirPath))
                Directory.CreateDirectory(configDirPath);

            string configFilePath = Path.Combine(configDirPath, DATA_FILE_NAME);

            if (!File.Exists(configFilePath))
                File.WriteAllText(configFilePath, string.Empty);

            string configFileContent = File.ReadAllText(configFilePath);

            if (!string.IsNullOrEmpty(configFileContent))
            {
                SyncConfig syncConfig = JsonConvert.DeserializeObject<SyncConfig>(configFileContent);

                if (syncConfig != null)
                {
                    SyncConfig = syncConfig;
                    return;
                }
            }

            SyncConfig = new SyncConfig();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save settings to json file. </summary>
        public void SaveSettings()
        {
            if (SyncConfig != null)
            {
                string configDirPath = Path.Combine(
                    Environment.GetEnvironmentVariable("APPDATA"),
                    ApplicationHelper.GetApplicationName());

                if (!Directory.Exists(configDirPath))
                    Directory.CreateDirectory(configDirPath);

                string configFilePath = Path.Combine(configDirPath, DATA_FILE_NAME);
                string configFileContent = JsonConvert.SerializeObject(SyncConfig, Formatting.Indented);

                File.WriteAllText(configFilePath, configFileContent);
            }
        }

        #endregion LOAD & SAVE METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler event. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update configuration properties after loading configuration from file. </summary>
        private void UpdateConfigurationProperties()
        {
            if (_loaded)
            {
                var thisType = this.GetType();
                var properties = ObjectHelper.GetObjectProperties(this).Where(p => p.CanWrite);

                foreach (var propInfo in properties)
                {
                    var property = thisType.GetProperty(propInfo.Name);

                    if (property != null)
                    {
                        if (ObjectHelper.HasAttribute(property, typeof(ConfigPropertyUpdateAttrib)))
                        {
                            var attribs = ObjectHelper.GetAttribute<ConfigPropertyUpdateAttrib>(property);
                            if (attribs != null && attribs.Any(a => a.AllowUpdate == false))
                                continue;
                        }

                        OnPropertyChanged(property.Name);
                    }
                }
            }
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }
}
