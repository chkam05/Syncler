using Syncler.Data.Configuration;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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
        private string _syncStateMessage = string.Empty;
        private SyncGroup _syncGroup;
        private ObservableCollection<SyncFileInfoGroup> _syncFileGroups;


        //  GETTERS & SETTERS

        public SyncState SyncState
        {
            get => _syncState;
            set
            {
                _syncState = value;
                OnPropertyChanged(nameof(SyncState));
                OnPropertyChanged(nameof(ScanButtonLabel));

                switch (value)
                {
                    case SyncState.NONE:
                        SyncStateMessage = "Ready.";
                        break;

                    case SyncState.SCANNING:
                        SyncStateMessage = "Scanning...";
                        break;

                    case SyncState.SYNCING:
                        SyncStateMessage = "Synchronising...";
                        break;

                    case SyncState.STOPPED_SCANNING:
                        SyncStateMessage = "Scanning stopped.";
                        break;

                    case SyncState.STOPPED_SYNCING:
                        SyncStateMessage = "Sync stopped.";
                        break;
                }
            }
        }

        public string SyncStateMessage
        {
            get => _syncStateMessage;
            set
            {
                _syncStateMessage = value;
                OnPropertyChanged(nameof(SyncStateMessage));
            }
        }

        public string ScanButtonLabel
        {
            get
            {
                bool anyScannedFile = SyncFileGroups != null && SyncFileGroups.Any();

                switch (SyncState)
                {
                    case SyncState.NONE:
                        return anyScannedFile ? "Rescan" : "Scan";

                    case SyncState.SCANNING:
                        return "Stop scan";

                    case SyncState.SYNCING:
                        return "Stop sync";

                    case SyncState.STOPPED_SCANNING:
                    case SyncState.STOPPED_SYNCING:
                        return "Rescan";

                    default:
                        return anyScannedFile ? "Rescan" : "Scan";
                }
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

        public ObservableCollection<SyncFileInfoGroup> SyncFileGroups
        {
            get => _syncFileGroups;
            set
            {
                _syncFileGroups = value;
                _syncFileGroups.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(SyncFileGroups)); };
                OnPropertyChanged(nameof(SyncFileGroups));
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
            Stop();
        }

        #endregion CLASS METHODS

        #region DATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update dispatcher handler. </summary>
        /// <param name="dispatcherHandler"> New dispatcher handler. </param>
        public void UpdateDispatcherHandler(DispatcherHandler dispatcherHandler)
        {
            _dispatherHandler = dispatcherHandler;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update sync group. </summary>
        /// <param name="syncGroup"> Sync group with changed data. </param>
        public void UpdateSyncGroup(SyncGroup syncGroup)
        {
            var currentSyncState = SyncState;
            var stopped = false;

            if (syncGroup.Id == SyncGroup.Id)
            {
                if (IsWorking())
                {
                    stopped = true;
                    Stop();
                }

                SyncGroup = syncGroup;

                if (stopped && currentSyncState == SyncState.SCANNING)
                    Scan();
            }
        }

        #endregion DATA MANAGEMENT METHODS

        #region FILES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add sync file group to sync file groups collection.</summary>
        /// <param name="syncFileInfoGroup"> Sync file group to add. </param>
        private void AddSyncFileGroup(SyncFileInfoGroup syncFileInfoGroup)
        {
            var action = new Action(() => SyncFileGroups.Add(syncFileInfoGroup));

            if (!_dispatherHandler?.TryInvoke(action) ?? false)
                action.Invoke();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update message displayed during files comparing. </summary>
        /// <param name="percent"> Current compared files count. </param>
        private void AddSyncFileMessageUpdate(int percent)
        {
            var action = new Action(() => SyncStateMessage = $"Comparing files {percent}% ...");

            if (!_dispatherHandler?.TryInvoke(action) ?? false)
                action.Invoke();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clear sync file groups collection. </summary>
        private void ClearSyncFileGroups()
        {
            var action = new Action(() =>
            {
                if (SyncFileGroups == null)
                {
                    SyncFileGroups = new ObservableCollection<SyncFileInfoGroup>();
                    return;
                }

                SyncFileGroups.Clear();
            });

            if (!_dispatherHandler?.TryInvoke(action) ?? false)
                action.Invoke();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Scan files in catalog for diffrences. </summary>
        /// <param name="basePath"> Base/start catalog path. </param>
        /// <param name="path"> Current subcatalog path. </param>
        /// <param name="result"> List of found files. </param>
        private void ScanCatalog(string basePath, string path, List<SyncTempFileInfo> result, ref long filesCount)
        {
            foreach (var filePath in Directory.GetFiles(path))
            {
                if (_bwScanner.CancellationPending)
                    break;

                FileInfo fileInfo = new FileInfo(filePath);
                string subCatalog = path.Replace(basePath, string.Empty)
                    .Replace("\\", "/");

                result.Add(new SyncTempFileInfo(filePath, fileInfo, subCatalog));
                filesCount++;
                ScanMessageUpdate(filesCount);
            }

            foreach (var directoryPath in Directory.GetDirectories(path))
            {
                if (_bwScanner.CancellationPending)
                    break;

                ScanCatalog(basePath, directoryPath, result, ref filesCount);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update message displayed during files scan. </summary>
        /// <param name="filesCount"> Current scanned files count. </param>
        private void ScanMessageUpdate(long filesCount)
        {
            var action = new Action(() => SyncStateMessage = $"Scanned {filesCount} files.");

            if (!_dispatherHandler?.TryInvoke(action) ?? false)
                action.Invoke();
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

        #region THREADS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Scan files for differences. </summary>
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
        /// <summary> Scan files for differences - work. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Do Work Event Arguments. </param>
        private void Scan(object sender, DoWorkEventArgs e)
        {
            ClearSyncFileGroups();

            try
            {
                List<SyncTempFileInfo> files = new List<SyncTempFileInfo>();
                var paths = SyncGroup.Items.Select(i => i.Path);
                long filesCounter = 0;

                //  Scan files and directories.
                foreach (var path in paths)
                {
                    if (_bwScanner.CancellationPending)
                        break;

                    ScanCatalog(path, path, files, ref filesCounter);
                }

                if (_bwScanner.CancellationPending)
                    return;

                //  Group files and directories.
                var grouppedFiles = files.GroupBy(f => new { f.FileName, f.SubCatalog });
                int groupsCounter = 0;

                foreach (var fileGroup in grouppedFiles)
                {
                    if (_bwScanner.CancellationPending)
                        break;

                    groupsCounter++;

                    var syncFileInfoGroup = new SyncFileInfoGroup()
                    {
                        Catalog = fileGroup.Key.SubCatalog,
                        FileName = fileGroup.Key.FileName
                    };

                    var syncFiles = fileGroup.Select(f =>
                    {
                        var fileInfo = new FileInfo(f.FilePath);
                        var basePath = Path.GetDirectoryName(f.FilePath);

                        if (!string.IsNullOrEmpty(f.SubCatalog))
                            basePath = basePath.Replace(f.SubCatalog.Replace("/", "\\"), string.Empty);

                        return new SyncFileInfo()
                        {
                            BasePath = Path.GetFileName(basePath),
                            FilePath = f.FilePath,
                            CreatedAt = fileInfo.CreationTime,
                            ModifiedAt = fileInfo.LastWriteTime,
                            Checksum = CalculateChecksum(fileInfo),
                            FileSize = fileInfo.Length,
                        };
                    });

                    syncFileInfoGroup.Files = new ObservableCollection<SyncFileInfo>(syncFiles);

                    if (!syncFileInfoGroup.ValidateFiles(SyncGroup, SyncValidationConfig.Default))
                        _bwScanner.ReportProgress(100 * groupsCounter / grouppedFiles.Count(), syncFileInfoGroup);
                }
            }
            catch (Exception)
            {
                SyncState = SyncState.STOPPED_SCANNING;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after reporting in scanning files for diffrences process. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Progress Changed Event Arguments. </param>
        private void OnScanProgress(object sender, ProgressChangedEventArgs e)
        {
            var syncFileInfoGroup = e.UserState as SyncFileInfoGroup;

            if (syncFileInfoGroup != null)
            {
                AddSyncFileGroup(syncFileInfoGroup);
                AddSyncFileMessageUpdate(e.ProgressPercentage);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after finishing scanning files for diffrences. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Run Worker Completed Event Arguments. </param>
        private void OnScanComplete(object sender, RunWorkerCompletedEventArgs e)
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
        private void OnSyncProgress(object sender, ProgressChangedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnSyncComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            SyncState = e.Cancelled ? SyncState.STOPPED_SYNCING : SyncState.NONE;
        }

        //  --------------------------------------------------------------------------------
        public void Stop()
        {
            if (_bwScanner != null && _bwScanner.IsBusy)
                _bwScanner.CancelAsync();

            else if (_bwSyncler != null && _bwSyncler.IsBusy)
                _bwSyncler.CancelAsync();
        }

        #endregion THREADS METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate file checksum. </summary>
        /// <param name="fileInfo"> System file informations. </param>
        /// <returns> Calculated checksum. </returns>
        private string CalculateChecksum(FileInfo fileInfo)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = fileInfo.OpenRead())
                {
                    byte[] checksumBytes = md5.ComputeHash(stream);
                    return BitConverter.ToString(checksumBytes).Replace("-", string.Empty);
                }
            }
        }

        #endregion UTILITY METHODS

    }
}
