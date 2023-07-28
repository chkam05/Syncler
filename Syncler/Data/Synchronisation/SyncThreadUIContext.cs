using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Synchronisation
{
    public class SyncThreadUIContext : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private bool _buttonScanEnabled = true;
        private string _buttonScanTitle = "Scan";
        private bool _buttonSyncEnabled = false;
        private bool _buttonCancelEnabled = false;
        private string _buttonCancelTitle = "Stop";

        private PackIconKind _stateIcon = PackIconKind.None;
        private string _stateMessage = string.Empty;

        private long _progress = 0;
        private long _progressMax = 0;
        private bool _progressVisibility = false;


        //  GETTERS & SETTERS

        public bool ButtonScanEnabled
        {
            get => _buttonScanEnabled;
        }

        public string ButtonScanTitle
        {
            get => _buttonScanTitle;
        }

        public bool ButtonSyncEnabled
        {
            get => _buttonSyncEnabled;
        }

        public bool ButtonCancelEnabled
        {
            get => _buttonCancelEnabled;
        }

        public string ButtonCancelTitle
        {
            get => _buttonCancelTitle;
        }

        public PackIconKind StateIcon
        {
            get => _stateIcon;
        }

        public string StateMessage
        {
            get => _stateMessage;
        }

        public long Progress
        {
            get => _progress;
        }

        public long ProgressMax
        {
            get => _progressMax;
        }

        public double ProcentageProgress
        {
            get => _progressMax >= 0 ? 0 : (_progress * 100) / _progressMax;
        }

        public bool ProgressVisibility
        {
            get => _progressVisibility;
            set
            {
                _progressVisibility = value;
                OnPropertyChanged(nameof(ProgressVisibility));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncThreadUIContext class constructor. </summary>
        /// <param name="syncState"> Synchronisation state. </param>
        /// <param name="anyScannedFile"> Is any file already scanned. </param>
        public SyncThreadUIContext(SyncState syncState, bool anyScannedFile = false)
        {
            UpdateUI(syncState, anyScannedFile);
        }

        #endregion CLASS METHODS

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

        #region UPDATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update ui buttons state and titles due to sync state. </summary>
        /// <param name="syncState"> Synchronisation state. </param>
        /// <param name="anyScannedFile"> Is any file already scanned. </param>
        private void UpdateComponents(SyncState syncState, bool anyScannedFile = false)
        {
            switch (syncState)
            {
                case SyncState.NONE:
                case SyncState.STOPPED_SCANNING:
                case SyncState.STOPPED_SYNCING:
                    _buttonScanEnabled = true;
                    _buttonScanTitle = anyScannedFile ? "Rescan" : "Scan";
                    _buttonSyncEnabled = anyScannedFile;
                    _buttonCancelEnabled = false;
                    _buttonCancelTitle = "Stop";
                    _progressVisibility = false;
                    break;

                case SyncState.SCANNING:
                    _buttonScanEnabled = false;
                    _buttonSyncEnabled = false;
                    _buttonCancelEnabled = true;
                    _buttonCancelTitle = "Stop scanning";
                    _progressVisibility = true;
                    break;

                case SyncState.SYNCING:
                    _buttonScanEnabled = false;
                    _buttonSyncEnabled = false;
                    _buttonCancelEnabled = true;
                    _buttonCancelTitle = "Stop sync";
                    _progressVisibility = true;
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update synchronisation progress. </summary>
        /// <param name="progress"> Progress. </param>
        /// <param name="progressMax"> Progress max value. </param>
        public void UpdateProgress(long progress, long? progressMax = null)
        {
            if (progressMax.HasValue && _progressMax != progressMax.Value && progressMax.Value >= 0)
            {
                _progressMax = progressMax.Value;
                OnPropertyChanged(nameof(ProgressMax));
            }

            _progress = Math.Max(0, Math.Min(progress, _progressMax));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(ProcentageProgress));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update synchronisation state message. </summary>
        /// <param name="message"> New message. </param>
        public void UpdateStateMessage(string message)
        {
            _stateMessage = message;
            OnPropertyChanged(nameof(StateMessage));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update state message due to sync state. </summary>
        /// <param name="syncState"> Synchronisation state. </param>
        /// <param name="anyScannedFile"> Is any file already scanned. </param>
        private void UpdateStateMessage(SyncState syncState, bool anyScannedFile = false)
        {
            switch (syncState)
            {
                case SyncState.NONE:
                    _stateMessage = "Ready.";
                    _stateIcon = PackIconKind.None;
                    break;

                case SyncState.STOPPED_SCANNING:
                    _stateMessage = "Scanning stopped.";
                    _stateIcon = PackIconKind.Cancel;
                    break;

                case SyncState.STOPPED_SYNCING:
                    _stateMessage = "Sync stopped.";
                    _stateIcon = PackIconKind.Cancel;
                    break;

                case SyncState.SCANNING:
                    _stateMessage = "Scanning...";
                    _stateIcon = PackIconKind.FileSearch;
                    break;

                case SyncState.SYNCING:
                    _stateMessage = "Synchronising...";
                    _stateIcon = PackIconKind.Sync;
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update synchronisation user interface data. </summary>
        /// <param name="syncState"> Synchronisation state. </param>
        /// <param name="anyScannedFile"> Is any file already scanned. </param>
        public void UpdateUI(SyncState syncState, bool anyScannedFile = false)
        {
            UpdateComponents(syncState, anyScannedFile);
            UpdateStateMessage(syncState, anyScannedFile);
            NotifyPropertyChanges();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Notify properites changed. </summary>
        private void NotifyPropertyChanges()
        {
            OnPropertyChanged(nameof(ButtonScanEnabled));
            OnPropertyChanged(nameof(ButtonScanTitle));
            OnPropertyChanged(nameof(ButtonSyncEnabled));
            OnPropertyChanged(nameof(ButtonCancelEnabled));
            OnPropertyChanged(nameof(ButtonCancelTitle));
            OnPropertyChanged(nameof(StateIcon));
            OnPropertyChanged(nameof(StateMessage));
            OnPropertyChanged(nameof(ProgressVisibility));
        }

        #endregion UPDATE METHODS

    }
}
