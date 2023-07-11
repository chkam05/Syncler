using Syncler.Data.Configuration;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Synchronisation
{
    public class SyncFileInfoGroup : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _catalog;
        private string _fileName;
        private ObservableCollection<SyncFileInfo> _files;
        private List<string> _diffMessages;


        //  GETTERS & SETTERS

        public string Catalog
        {
            get => _catalog;
            set
            {
                _catalog = value;
                OnPropertyChanged(nameof(Catalog));
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        public ObservableCollection<SyncFileInfo> Files
        {
            get => _files;
            set
            {
                _files = value;
                _files.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(Files)); };
                OnPropertyChanged(nameof(Files));
            }
        }

        public string DiffMessage
        {
            get => _diffMessages != null && _diffMessages.Any() 
                ? string.Join(Environment.NewLine, _diffMessages) : string.Empty;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncFileInfoGroup class constructor. </summary>
        public SyncFileInfoGroup()
        {
            _diffMessages = new List<string>();
        }

        #endregion CLASS METHODS

        #region MESSAGEM METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add diffrence message. </summary>
        /// <param name="message"> Message. </param>
        public void AddDiffMessage(string message)
        {
            _diffMessages.Add(message);
            OnPropertyChanged(nameof(DiffMessage));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove diffrence message. </summary>
        /// <param name="message"> Message. </param>
        private void RemoveDiffMessage(string message)
        {
            if (_diffMessages.Contains(message))
                _diffMessages.Remove(message);
            OnPropertyChanged(nameof(DiffMessage));
        }

        #endregion MESSAGE METHODS

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

        //  --------------------------------------------------------------------------------
        /// <summary> Method for update SyncFileMode in SyncFileInfo. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Property Changed Event Arguments. </param>
        public void OnFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SyncFileInfo.SyncFileMode))
            {
                var currSyncFileInfo = (SyncFileInfo)sender;
                var diffrences = EnumHelper.GetEnumValues<SyncFileDiffrence>().ToArray();

                if (currSyncFileInfo.SyncFileModeUpdateLock)
                    return;

                foreach (var syncFileInfo in Files.Where(f => f != currSyncFileInfo))
                {
                    var isEqual = currSyncFileInfo.CompareBy(syncFileInfo, diffrences);

                    switch (currSyncFileInfo.SyncFileMode)
                    {
                        case SyncFileMode.NONE:
                            break;

                        case SyncFileMode.COPY:
                            if (isEqual)
                            {
                                syncFileInfo.UpdateSyncFileMode(SyncFileMode.NONE, true);
                            }
                            else if (!new[] { SyncFileMode.REMOVE, SyncFileMode.RENAME }.Contains(syncFileInfo.SyncFileMode))
                            {
                                syncFileInfo.UpdateSyncFileMode(SyncFileMode.REMOVE, true);
                            }
                            break;

                        case SyncFileMode.RENAME:
                            if (isEqual)
                            {
                                syncFileInfo.UpdateSyncFileMode(SyncFileMode.RENAME, true);
                            }
                            else if (!new[] { SyncFileMode.COPY, SyncFileMode.REMOVE }.Contains(syncFileInfo.SyncFileMode))
                            {
                                syncFileInfo.UpdateSyncFileMode(SyncFileMode.COPY, true);
                            }
                            break;

                        case SyncFileMode.REMOVE:
                            if (isEqual)
                            {
                                syncFileInfo.UpdateSyncFileMode(SyncFileMode.REMOVE, true);
                            }
                            else if (!new[] { SyncFileMode.COPY, SyncFileMode.REMOVE }.Contains(syncFileInfo.SyncFileMode))
                            {
                                syncFileInfo.UpdateSyncFileMode(SyncFileMode.COPY, true);
                            }
                            break;
                    }
                }
            }
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region VALIDATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Validate if all files are good and synchronised. </summary>
        /// <param name="SyncGroup"> Sync group. </param>
        /// <param name="diffrences"> Array of diffrences by which check will be performed. </param>
        /// <returns></returns>
        public bool ValidateFiles(SyncGroup SyncGroup, SyncFileDiffrence[] diffrences)
        {
            var result = ValidateFilesCount(SyncGroup);

            if (diffrences != null && diffrences.Any())
            {
                if (diffrences.Contains(SyncFileDiffrence.Name) && !ValidateFilesByName())
                    result = false;

                if (diffrences.Contains(SyncFileDiffrence.Size) && !ValidateFilesBySize())
                    result = false;

                if (diffrences.Contains(SyncFileDiffrence.Checksum) && !ValidateFilesByChecksum())
                    result = false;
            }

            return result;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate files in group - search for missing files. </summary>
        /// <param name="SyncGroup"> Sync group. </param>
        /// <returns> True - files are ok; False - files are missing. </returns>
        private bool ValidateFilesCount(SyncGroup SyncGroup)
        {
            var result = true;

            if (SyncGroup.Items.Count != Files.Count)
            {
                var dirs = SyncGroup.Items
                    .Where(i => !Files.Any(f => f.BasePath == i.Name))
                    .Select(i => i.Path);

                if (dirs.Any())
                {
                    foreach (var dir in dirs)
                        AddDiffMessage($"Missing file in {dir}");

                    result = false;
                }
            }

            return result;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate files by name - search for files with not matching name. </summary>
        /// <returns> True - files names are the same; False - otherwise. </returns>
        private bool ValidateFilesByName()
        {
            var nameGroups = Files.GroupBy(f => f.FileName);

            if (nameGroups.Count() > 1)
            {
                var group = nameGroups.OrderBy(g => g.Count()).FirstOrDefault();

                foreach (var item in group)
                {
                    item.AddDiffMessage("File name does not match the rest of the files.");
                }

                return false;
            }

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate files by size - search for files with not matching size. </summary>
        /// <returns> True - files sizes are the same; False - otherwise. </returns>
        private bool ValidateFilesBySize()
        {
            var sizeGroups = Files.GroupBy(f => f.FileSize);

            if (sizeGroups.Count() > 1)
            {
                var group = sizeGroups.OrderBy(g => g.Count()).FirstOrDefault();

                foreach (var item in group)
                {
                    item.AddDiffMessage("File size is diffrent than the rest of the files size.");
                }

                return false;
            }

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate files by checksum - search for files with not matching checksum. </summary>
        /// <returns> True - files checksum are the same; False - otherwise. </returns>
        private bool ValidateFilesByChecksum()
        {
            var checksumGroups = Files.GroupBy(f => f.Checksum);

            if (checksumGroups.Count() > 1)
            {
                var group = checksumGroups.OrderBy(g => g.Count()).FirstOrDefault();

                foreach (var item in group)
                {
                    item.AddDiffMessage("File checksum is diffrent than the rest of the files checksums.");
                }

                return false;
            }

            return true;
        }

        #endregion VALIDATION METHODS

    }
}
