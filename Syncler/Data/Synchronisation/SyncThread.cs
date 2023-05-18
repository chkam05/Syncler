using Syncler.Data.Configuration;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Synchronisation
{
    public class SyncThread : INotifyPropertyChanged, IDisposable
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private BackgroundWorker _bwScanner;
        private BackgroundWorker _bwSyncler;
        private DispatcherHandler _dispatherHandler;
        private SyncState _syncState = SyncState.NONE;
        private SyncGroup _syncGroup;


        //  GETTERS & SETTERS

        public SyncState SyncState
        {
            get => _syncState;
            set
            {
                _syncState = value;
                OnPropertyChanged(nameof(SyncState));
            }
        }

        public SyncGroup SyncGroup
        {
            get => _syncGroup;
            private set
            {
                _syncGroup = value;
                OnPropertyChanged(nameof(SyncGroup));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncThread class constructor. </summary>
        /// <param name="syncGroup"> Sync group. </param>
        public SyncThread(SyncGroup syncGroup)
        {
            SyncGroup = syncGroup;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
        public void Dispose()
        {
            //
        }

        #endregion CLASS METHODS

        #region DATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        public void UpdateDispatcherHandler(DispatcherHandler dispatcherHandler)
        {
            _dispatherHandler = dispatcherHandler;
        }

        //  --------------------------------------------------------------------------------
        public void UpdateSyncGroup(SyncGroup syncGroup)
        {
            if (syncGroup.Id == SyncGroup.Id)
            {
                //
            }
        }

        #endregion DATA MANAGEMENT METHODS

        #region FILES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        public void Scan()
        {
            Stop();

            _bwScanner = new BackgroundWorker();
            _bwScanner.WorkerReportsProgress = true;
            _bwScanner.WorkerSupportsCancellation = true;
            _bwScanner.DoWork += Scan;
            _bwScanner.ProgressChanged += OnScanProgress;
            _bwScanner.RunWorkerCompleted += OnScanComplete;

            SyncState = SyncState.SCANNING;
            _bwScanner.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        private void Scan(object sender, DoWorkEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnScanComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnScanProgress(object sender, ProgressChangedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnSacanComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            SyncState = e.Cancelled ? SyncState.STOPPED_SCANNING : SyncState.NONE;
        }

        //  --------------------------------------------------------------------------------
        public void Sync()
        {
            Stop();

            _bwSyncler = new BackgroundWorker();
            _bwSyncler.WorkerReportsProgress = true;
            _bwSyncler.WorkerSupportsCancellation = true;
            _bwSyncler.DoWork += Sync;
            _bwSyncler.ProgressChanged += OnSyncProgress;
            _bwSyncler.RunWorkerCompleted += OnSyncComplete;

            SyncState = SyncState.SYNCING;
            _bwSyncler.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        private void Sync(object sender, DoWorkEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnSyncComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            SyncState = e.Cancelled ? SyncState.STOPPED_SYNCING : SyncState.NONE;
        }

        //  --------------------------------------------------------------------------------
        private void OnSyncProgress(object sender, ProgressChangedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        public void Stop()
        {
            if (_bwScanner != null && _bwScanner.IsBusy)
                _bwScanner.CancelAsync();

            else if (_bwSyncler != null && _bwSyncler.IsBusy)
                _bwSyncler.CancelAsync();
        }

        #endregion FILES MANAGEMENT METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region STATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if thread is scanning. </summary>
        /// <returns> True - thread is scanning; False - otherwise. </returns>
        public bool IsScanning() => _syncState == SyncState.SCANNING;

        //  --------------------------------------------------------------------------------
        /// <summary> Check if thread is syncing. </summary>
        /// <returns> True - thread is syncing; False - otherwise. </returns>
        public bool IsSyncing() => _syncState == SyncState.SYNCING;

        //  --------------------------------------------------------------------------------
        /// <summary> Check if thread is working. </summary>
        /// <returns> True - thread is working; False - otherwise. </returns>
        public bool IsWorking() => IsScanning() || IsWorking();

        #endregion STATE METHODS

    }
}
