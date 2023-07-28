using Newtonsoft.Json;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Synchronisation
{
    public class SyncFileInfo : INotifyPropertyChanged
    {

        //  CONST

        private static readonly int FILE_SIZE_SCALE = 1024;
        private static readonly string[] FILE_SIZE_SUFFIXES = { "B", "KB", "MB", "GB", "TB", "PB" };
        private static readonly Dictionary<string, string> VALIDATION_MESSAGES = new Dictionary<string, string>()
        {
            { "EMPTY", "File name can not be empty." },
            { "DUPLICATE", "File with that name already exists." }
        };


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _basePath;
        private string _filePath;
        private long _fileSize = 0;
        private string _checksum;
        private DateTime _createdAt;
        private DateTime _modifiedAt;
        private string _newFileName;

        private List<string> _diffMessages;
        private SyncFileMode _syncFileMode = SyncFileMode.NONE;
        private bool _syncFileModeUpdateLock = false;


        //  GETTERS & SETTERS

        public string BasePath
        {
            get => _basePath;
            set
            {
                _basePath = value;
                OnPropertyChanged(nameof(BasePath));
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
                OnPropertyChanged(nameof(FileName));
            }
        }

        [JsonIgnore]
        public string FileName
        {
            get => Path.GetFileName(_filePath);
        }

        public string NewFileName
        {
            get => _newFileName;
            set
            {
                _newFileName = value;
                OnPropertyChanged(nameof(NewFileName));
                ValidateNewFileName();
            }
        }

        public long FileSize
        {
            get => _fileSize;
            set
            {
                _fileSize = value;
                OnPropertyChanged(nameof(FileSize));
                OnPropertyChanged(nameof(FileSizeStr));
            }
        }

        [JsonIgnore]
        public string FileSizeStr
        {
            get
            {
                if (_fileSize <= 0)
                {
                    return "0 " + FILE_SIZE_SUFFIXES[0];
                }

                int magnitude = (int)Math.Log(_fileSize, FILE_SIZE_SCALE);
                double adjustedSize = (double)_fileSize / Math.Pow(FILE_SIZE_SCALE, magnitude);

                return $"{adjustedSize:0.##} {FILE_SIZE_SUFFIXES[magnitude]}";
            }
        }

        public string Checksum
        {
            get => _checksum;
            set
            {
                _checksum = value;
                OnPropertyChanged(nameof(Checksum));
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                OnPropertyChanged(nameof(CreatedAt));
                OnPropertyChanged(nameof(CreatedAtStr));
            }
        }

        [JsonIgnore]
        public string CreatedAtStr
        {
            get => _createdAt.ToString("yyyy.MM.dd HH:mm:ss");
        }

        public DateTime ModifiedAt
        {
            get => _modifiedAt;
            set
            {
                _modifiedAt = value;
                OnPropertyChanged(nameof(ModifiedAt));
                OnPropertyChanged(nameof(ModifiedAtStr));
            }
        }

        [JsonIgnore]
        public string ModifiedAtStr
        {
            get => _createdAt.ToString("yyyy.MM.dd HH:mm:ss");
        }

        [JsonIgnore]
        public string DiffMessage
        {
            get => _diffMessages?.Any() ?? false ? string.Join(Environment.NewLine, _diffMessages) : string.Empty;
        }

        public List<string> DiffMessages
        {
            get => _diffMessages;
            set => _diffMessages = value;
        }

        public SyncFileMode SyncFileMode
        {
            get => _syncFileMode;
            set
            {
                _syncFileMode = value;
                OnPropertyChanged(nameof(SyncFileMode));
            }
        }

        [JsonIgnore]
        public bool SyncFileModeUpdateLock
        {
            get => _syncFileModeUpdateLock;
            private set
            {
                _syncFileModeUpdateLock = value;
                OnPropertyChanged(nameof(SyncFileModeUpdateLock));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncFileInfo class constructor. </summary>
        public SyncFileInfo()
        {
            _diffMessages = new List<string>();
        }

        #endregion CLASS METHODS

        #region COMPARE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Compare with other sync file info. </summary>
        /// <param name="syncFileInfo"> Sync file info to compare with. </param>
        /// <param name="diffrences"> Array of diffrences by which check will be performed. </param>
        /// <returns></returns>
        public bool CompareBy(SyncFileInfo syncFileInfo, SyncFileDiffrence[] diffrences)
        {
            var result = true;

            if (diffrences != null && diffrences.Any())
            {
                foreach (var diffrence in diffrences)
                {
                    switch (diffrence)
                    {
                        case SyncFileDiffrence.Name:
                            if (FileName != syncFileInfo.FileName)
                                result = false;
                            break;

                        case SyncFileDiffrence.Size:
                            if (FileSize != syncFileInfo.FileSize)
                                result = false;
                            break;

                        case SyncFileDiffrence.Checksum:
                            if (Checksum != syncFileInfo.Checksum)
                                result = false;
                            break;
                    }
                }
            }

            return result;
        }

        #endregion COMPARE METHODS

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

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region UPDATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update sync file mode. </summary>
        /// <param name="syncFileMode"> New sync file mode. </param>
        /// <param name="updateLock"> Lock update. </param>
        public void UpdateSyncFileMode(SyncFileMode syncFileMode, bool updateLock = false)
        {
            SyncFileModeUpdateLock = updateLock;
            SyncFileMode = syncFileMode;
            SyncFileModeUpdateLock = false;
        }

        #endregion UPDATE METHODS

        #region VALIDATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Validate new file name. </summary>
        private void ValidateNewFileName()
        {
            if (SyncFileMode == SyncFileMode.RENAME)
            {
                if (string.IsNullOrEmpty(NewFileName) && !_diffMessages.Contains(VALIDATION_MESSAGES["EMPTY"]))
                    AddDiffMessage(VALIDATION_MESSAGES["EMPTY"]);
                else if (_diffMessages.Contains(VALIDATION_MESSAGES["EMPTY"]))
                    RemoveDiffMessage(VALIDATION_MESSAGES["EMPTY"]);

                if (File.Exists(Path.Combine(Path.GetDirectoryName(FilePath), NewFileName)) && !_diffMessages.Contains(VALIDATION_MESSAGES["DUPLICATE"]))
                    AddDiffMessage(VALIDATION_MESSAGES["DUPLICATE"]);
                else if (_diffMessages.Contains(VALIDATION_MESSAGES["DUPLICATE"]))
                    RemoveDiffMessage(VALIDATION_MESSAGES["DUPLICATE"]);
            }
            else
            {
                if (_diffMessages.Contains(VALIDATION_MESSAGES["EMPTY"]))
                    RemoveDiffMessage(VALIDATION_MESSAGES["EMPTY"]);

                if (_diffMessages.Contains(VALIDATION_MESSAGES["DUPLICATE"]))
                    RemoveDiffMessage(VALIDATION_MESSAGES["DUPLICATE"]);
            }
        }

        #endregion VALIDATION METHODS

    }
}
