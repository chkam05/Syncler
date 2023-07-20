using Microsoft.VisualBasic.FileIO;
using Syncler.Data.Configuration;
using Syncler.Data.Logs;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
        private SyncThreadUIContext _syncThreadUIContext;


        //  GETTERS & SETTERS

        public SyncState SyncState
        {
            get => _syncState;
            set
            {
                _syncState = value;
                OnPropertyChanged(nameof(SyncState));
                SyncThreadUIContext.UpdateUI(SyncState, SyncFileGroups?.Any() ?? false);
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

        public SyncThreadUIContext SyncThreadUIContext
        {
            get => _syncThreadUIContext;
            private set
            {
                _syncThreadUIContext = value;
                OnPropertyChanged(nameof(SyncThreadUIContext));
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
            SyncThreadUIContext = new SyncThreadUIContext(SyncState);
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

        #region FILES SCAN MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add sync file group to sync file groups collection.</summary>
        /// <param name="syncFileInfoGroup"> Sync file group to add. </param>
        private void AddSyncFileGroup(SyncFileInfoGroup syncFileInfoGroup)
        {
            var action = new Action(() =>
            {
                SyncFileGroups.Add(syncFileInfoGroup);
                Logger.Instance.AddLog(DateTime.Now, $"Found diffrence in file: \"{syncFileInfoGroup.FileName}\"", "Scan", SyncGroup.Name);
            });

            if (!_dispatherHandler?.TryInvoke(action) ?? false)
                action.Invoke();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clear sync file groups collection. </summary>
        private void ClearSyncFileGroups()
        {
            var action = new Action(() =>
            {
                SyncFileGroups = new ObservableCollection<SyncFileInfoGroup>();
                Logger.Instance.AddLog(DateTime.Now, "Cleared scan results", "Scan", SyncGroup.Name);
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
                StateMessageUpdate($"Scanned {filesCount} files.");
            }

            foreach (var directoryPath in Directory.GetDirectories(path))
            {
                if (_bwScanner.CancellationPending)
                    break;

                ScanCatalog(basePath, directoryPath, result, ref filesCount);
            }
        }

        #endregion FILES SCAN MANAGEMENT METHODS

        #region FILES SYNC MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        private void CopyFile(SyncFileInfo syncFileInfo, IEnumerable<string> catalogs)
        {
            var action = new Action(() =>
            {
                foreach (var catalog in catalogs)
                {
                    var destPath = Path.Combine(catalog, syncFileInfo.FileName);
                    Logger.Instance.AddLog(DateTime.Now, $"Copying file \"{syncFileInfo.FileName}\" to \"{catalog}\"", "Sync", SyncGroup.Name);
                    File.Copy(syncFileInfo.FilePath, destPath);
                }
            });

            if (!_dispatherHandler?.TryInvoke(action) ?? false)
                action.Invoke();
        }

        //  --------------------------------------------------------------------------------
        private void RemoveFile(SyncFileInfo syncFileInfo)
        {
            var action = new Action(() =>
            {
                Logger.Instance.AddLog(DateTime.Now, $"Removing file \"{syncFileInfo.FileName}\"", "Sync", SyncGroup.Name);
                FileSystem.DeleteFile(syncFileInfo.FilePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            });

            if (!_dispatherHandler?.TryInvoke(action) ?? false)
                action.Invoke();
        }

        //  --------------------------------------------------------------------------------
        private void RenameAndCopyFile(SyncFileInfo syncFileInfo, IEnumerable<string> catalogs)
        {
            var action = new Action(() =>
            {
                Logger.Instance.AddLog(DateTime.Now, $"Renaming file \"{syncFileInfo.FileName}\" to \"{syncFileInfo.NewFileName}\"", "Sync", SyncGroup.Name);

                foreach (var catalog in catalogs)
                {
                    var destPath = Path.Combine(catalog, syncFileInfo.NewFileName);
                    Logger.Instance.AddLog(DateTime.Now, $"Copying file \"{syncFileInfo.NewFileName}\" to \"{catalog}\"", "Sync", SyncGroup.Name);
                    File.Copy(syncFileInfo.FilePath, destPath);
                }

                FileSystem.DeleteFile(syncFileInfo.FilePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            });

            if (!_dispatherHandler?.TryInvoke(action) ?? false)
                action.Invoke();
        }

        //  --------------------------------------------------------------------------------
        private void RemoveSyncFileGroups(List<SyncFileInfoGroup> completedSyncFileGroups)
        {
            var action = new Action(() =>
            {
                foreach (var syncFileGroup in completedSyncFileGroups)
                {
                    Logger.Instance.AddLog(DateTime.Now, $"Sync file \"{syncFileGroup.FileName}\" completed", "Sync", SyncGroup.Name);
                    SyncFileGroups.Remove(syncFileGroup);
                }
            });

            if (!_dispatherHandler?.TryInvoke(action) ?? false)
                action.Invoke();
        }

        #endregion FILES SYNC MANAGEMENT METHODS

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

            Logger.Instance.AddLog(DateTime.Now, $"Started scanning", "Scan", SyncGroup.Name);
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
                    Thread.Sleep(1000);
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

                        var syncFileInfo = new SyncFileInfo()
                        {
                            BasePath = Path.GetFileName(basePath),
                            FilePath = f.FilePath,
                            CreatedAt = fileInfo.CreationTime,
                            ModifiedAt = fileInfo.LastWriteTime,
                            Checksum = CalculateChecksum(fileInfo),
                            FileSize = fileInfo.Length,
                        };

                        syncFileInfo.PropertyChanged += syncFileInfoGroup.OnFilePropertyChanged;

                        return syncFileInfo;
                    });

                    syncFileInfoGroup.Files = new ObservableCollection<SyncFileInfo>(syncFiles);

                    if (!syncFileInfoGroup.ValidateFiles(SyncGroup, EnumHelper.GetEnumValues<SyncFileDiffrence>().ToArray()))
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
                StateMessageUpdate($"Comparing files {e.ProgressPercentage}% ...");
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after finishing scanning files for diffrences. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Run Worker Completed Event Arguments. </param>
        private void OnScanComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            SyncState = e.Cancelled ? SyncState.STOPPED_SCANNING : SyncState.NONE;
            Logger.Instance.AddLog(DateTime.Now, $"Finished scanning", "Scan", SyncGroup.Name);
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
            Logger.Instance.AddLog(DateTime.Now, $"Started synchronisation", "Sync", SyncGroup.Name);
            _bwSyncler.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        private void Sync(object sender, DoWorkEventArgs e)
        {
            int groupsCounter = 0;
            var completedSyncFileGroups = new List<SyncFileInfoGroup>();

            foreach (var syncFileGroup in SyncFileGroups)
            {
                if (_bwSyncler.CancellationPending)
                    break;

                groupsCounter++;

                var catalogs = SyncGroup.Items.Select(sgi
                    => Path.Combine(sgi.Path, syncFileGroup.Catalog.Replace("/", "")));

                var allowComplete = syncFileGroup.Files.Any(f => f.SyncFileMode == SyncFileMode.COPY
                    || f.SyncFileMode == SyncFileMode.REMOVE);

                foreach (var syncFileInfo in syncFileGroup.Files.OrderBy(o
                    => OrderBySyncFileMode(o.SyncFileMode)))
                {
                    if (_bwSyncler.CancellationPending)
                        break;

                    var filePaths = syncFileGroup.Files.Select(f => Path.GetDirectoryName(f.FilePath));

                    switch (syncFileInfo.SyncFileMode)
                    {
                        case SyncFileMode.NONE:
                            break;

                        case SyncFileMode.REMOVE:
                            RemoveFile(syncFileInfo);
                            break;

                        case SyncFileMode.RENAME:
                            RenameAndCopyFile(syncFileInfo, catalogs.Where(cat => !filePaths.Any(p => cat == p)
                                || !File.Exists(Path.Combine(cat, syncFileInfo.NewFileName))));
                            break;

                        case SyncFileMode.COPY:
                            CopyFile(syncFileInfo, catalogs.Where(cat => !filePaths.Any(p => cat == p)));
                            break;
                    }
                }

                if (allowComplete)
                    completedSyncFileGroups.Add(syncFileGroup);

                _bwSyncler.ReportProgress(100 * groupsCounter / SyncFileGroups.Count());
            }

            RemoveSyncFileGroups(completedSyncFileGroups);
        }

        //  --------------------------------------------------------------------------------
        private void OnSyncProgress(object sender, ProgressChangedEventArgs e)
        {
            StateMessageUpdate($"Synchronising files {e.ProgressPercentage}% ...");
        }

        //  --------------------------------------------------------------------------------
        private void OnSyncComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            SyncState = e.Cancelled ? SyncState.STOPPED_SYNCING : SyncState.NONE;
            Logger.Instance.AddLog(DateTime.Now, $"Finished synchronisation", "Sync", SyncGroup.Name);
        }

        //  --------------------------------------------------------------------------------
        public void Stop()
        {
            if (_bwScanner != null && _bwScanner.IsBusy)
            {
                Logger.Instance.AddLog(DateTime.Now, $"Stopping scanning", "Scan", SyncGroup.Name);
                _bwScanner.CancelAsync();
            }

            else if (_bwSyncler != null && _bwSyncler.IsBusy)
            {
                Logger.Instance.AddLog(DateTime.Now, $"Stopping synchronisation", "Sync", SyncGroup.Name);
                _bwSyncler.CancelAsync();
            }
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

        //  --------------------------------------------------------------------------------
        /// <summary> Sync file mode order. </summary>
        /// <param name="syncFileMode"> Sync file mode. </param>
        /// <returns> Order of sync file mode. </returns>
        private int OrderBySyncFileMode(SyncFileMode syncFileMode)
        {
            switch (syncFileMode)
            {
                case SyncFileMode.REMOVE:
                    return 1;
                case SyncFileMode.RENAME:
                    return 2;
                case SyncFileMode.COPY:
                    return 3;
                case SyncFileMode.NONE:
                default:
                    return 0;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update message displayed during files scan/sync. </summary>
        /// <param name="message"> Message. </param>
        private void StateMessageUpdate(string message)
        {
            var action = new Action(() =>
            {
                SyncThreadUIContext.UpdateStateMessage(message);
            });

            if (!_dispatherHandler?.TryInvoke(action) ?? false)
                action.Invoke();
        }

        #endregion UTILITY METHODS

    }
}
