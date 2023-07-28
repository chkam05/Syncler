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
    public class SyncManager : INotifyPropertyChanged, IDisposable
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private static SyncManager _instance;
        private static object _instanceLock = new object();
        private DispatcherHandler _dispatcherHandler;
        private ObservableCollection<SyncThread> _syncThreads;

        public ConfigManager ConfigManager { get; private set; }


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

        public DispatcherHandler DispatcherHandler
        {
            get => _dispatcherHandler;
            private set
            {
                UpdateDispatcherHandler(value);
                OnPropertyChanged(nameof(DispatcherHandler));
            }
        }

        public ObservableCollection<SyncThread> SyncThreads
        {
            get => _syncThreads;
            set
            {
                _syncThreads = value;
                _syncThreads.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(SyncThreads)); };
                OnPropertyChanged(nameof(SyncThreads));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Private SyncManager instance class constructor. </summary>
        private SyncManager()
        {
            //  Initialize modules.
            ConfigManager = ConfigManager.Instance;
            ConfigManager.PropertyChanged += OnConfigUpdate;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Dispose SyncManager and threads. </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            //  Remove threads.
            if (SyncThreads?.Any() ?? false)
            {
                try
                {
                    foreach (var syncThread in SyncThreads)
                    {
                        syncThread.Dispose();
                        SyncThreads.Remove(syncThread);
                    }
                }
                catch (InvalidOperationException)
                {
                    //  Just ignore that.
                }
            }
        }

        #endregion CLASS METHODS

        #region DISPATCHER MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update dispatcher handler. </summary>
        /// <param name="dispatcherHandler"> New dispatcher handler. </param>
        public void UpdateDispatcherHandler(DispatcherHandler dispatcherHandler)
        {
            _dispatcherHandler = dispatcherHandler;

            foreach (var syncThread in SyncThreads)
                syncThread.UpdateDispatcherHandler(dispatcherHandler);
        }

        #endregion DISPATCHER MANAGEMENT METHODS

        #region LOAD & SAVE DATA

        //  --------------------------------------------------------------------------------
        /// <summary> Create thread from sync groups. </summary>
        private void LoadData()
        {
            SyncThreads = new ObservableCollection<SyncThread>();

            foreach (var syncGroup in ConfigManager.SyncGroups)
                SyncThreads.Add(new SyncThread(syncGroup));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update threads after changing sync groups data. </summary>
        private void UpdateData()
        {
            List<string> syncGroupIds = new List<string>();

            //  Add new thread or update existing one.
            foreach (var syncGroup in ConfigManager.SyncGroups)
            {
                var syncThread = SyncThreads.FirstOrDefault(s => s.SyncGroup.Id == syncGroup.Id);

                if (syncThread == null)
                    SyncThreads.Add(new SyncThread(syncGroup));
                else
                    syncThread.UpdateSyncGroup(syncGroup);

                syncGroupIds.Add(syncGroup.Id);
            }

            var oldSyncThreads = SyncThreads.Where(s => !syncGroupIds.Contains(s.SyncGroup.Id));

            //  Remove old threads.
            if (oldSyncThreads.Any())
            {
                foreach (var oldSyncThread in oldSyncThreads)
                {
                    oldSyncThread.Dispose();
                    SyncThreads.Remove(oldSyncThread);
                }
            }
        }

        #endregion LOAD & SAVE DATA

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after updating config in config manager. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Property Changed Event Arguments. </param>
        protected void OnConfigUpdate(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConfigManager.SyncGroups))
                UpdateData();
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

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }
}
