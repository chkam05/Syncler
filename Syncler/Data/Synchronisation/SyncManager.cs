using Newtonsoft.Json;
using Syncler.Attributes;
using Syncler.Data.Configuration;
using Syncler.Data.Events;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Syncler.Delegates;

namespace Syncler.Data.Synchronisation
{
    public class SyncManager : INotifyPropertyChanged
    {

        //  CONST

        private const string DATA_FILE_NAME = "data.json";


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;
        public event ErrorRelayEventHandler ErrorRelay;


        //  VARIABLES

        private static SyncManager _instance;
        private static object _instanceLock = new object();

        private bool _checkBeforeSave = false;
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
                            _instance.LoadData();
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

                foreach (var groupConfig in _syncConfig.Groups)
                    groupConfig.ItemsCollectionChanged += OnGroupItemsCollectionChanged;

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
        /// <summary> Load data from json file. </summary>
        public void LoadData()
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
        /// <summary> Check data before save. </summary>
        /// <param name="errorMessage"> Output error message. </param>
        /// <returns> True - data can be save; False - otherwise. </returns>
        private bool SaveDataCheck(out string errorMessage)
        {
            errorMessage = "";

            if (_checkBeforeSave)
            {
                bool error = false;
                StringBuilder errorMessageSb = new StringBuilder();

                foreach (var groupConfig in SyncConfig.Groups)
                {
                    if (groupConfig.Items.Count < 2)
                    {
                        errorMessageSb.AppendLine($"Group \"{groupConfig.Name}\" contains less then 2.");
                        error = true;
                    }
                }

                _checkBeforeSave = error;

                if (error)
                {
                    errorMessage = errorMessageSb.ToString();
                    return false;
                }
            }

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save data to json file. </summary>
        /// <returns> True - Data has been saved successfully; False - otherwise. </returns>
        public bool SaveData()
        {
            if (SyncConfig != null)
            {
                if (!SaveDataCheck(out string errorMessage))
                {
                    ErrorRelay?.Invoke(this, new ErrorRelayEventArgs(errorMessage, true));
                    return false;
                }

                string configDirPath = Path.Combine(
                    Environment.GetEnvironmentVariable("APPDATA"),
                    ApplicationHelper.GetApplicationName());

                if (!Directory.Exists(configDirPath))
                    Directory.CreateDirectory(configDirPath);

                string configFilePath = Path.Combine(configDirPath, DATA_FILE_NAME);
                string configFileContent = JsonConvert.SerializeObject(SyncConfig, Formatting.Indented);

                File.WriteAllText(configFilePath, configFileContent);
                return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Trigger auto save. </summary>
        /// <param name="autosave"> Autosave. </param>
        private void TriggerAutoSave(bool autosave)
        {
            if (autosave)
                SaveData();
        }

        #endregion LOAD & SAVE METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after modifying group items collection. </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnGroupItemsCollectionChanged(object sender, ExtNotifyCollectionChangedEventArgs e)
        {
            if (e.CurrentItems.Count < 2)
                _checkBeforeSave = true;

            TriggerAutoSave(e.AutoSave);
        }

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

        #region CONFIG MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add group config. </summary>
        /// <param name="groupConfig"> Group config to add. </param>
        /// <param name="autosave"> Save after add. </param>
        public void AddGroupConfig(GroupConfig groupConfig, bool autosave = false)
        {
            groupConfig.ItemsCollectionChanged += OnGroupItemsCollectionChanged;
            SyncConfig.Groups.Add(groupConfig);
            TriggerAutoSave(autosave);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of sync groups names. </summary>
        /// <returns> Sync groups names. </returns>
        public List<string> GetSyncGroupNames()
        {
            return SyncConfig.Groups.Select(g => g.Name).ToList();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if group config exists in configuration. </summary>
        /// <param name="groupConfig"> Group config. </param>
        /// <returns> True - group config exists in configuration; False - otherwise. </returns>
        public bool HasGroupConfig(GroupConfig groupConfig)
        {
            return SyncConfig.Groups.Contains(groupConfig);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove group config. </summary>
        /// <param name="groupConfig"> Group config to remove. </param>
        /// <param name="autosave"> Save after remove. </param>
        public void RemoveGroupConfig(GroupConfig groupConfig, bool autosave = false)
        {
            SyncConfig.Groups.Remove(groupConfig);
            TriggerAutoSave(autosave);
        }

        #endregion CONFIG MANAGEMENT METHODS

    }
}
