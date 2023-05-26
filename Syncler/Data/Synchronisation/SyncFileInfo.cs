using System;
using System.Collections.Generic;
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


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _basePath;
        private string _filePath;
        private long _fileSize = 0;
        private string _checksum;
        private DateTime _createdAt;
        private DateTime _modifiedAt;

        private string _diffMessage = string.Empty;
        private bool _selected = false;


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

        public string FileName
        {
            get => Path.GetFileName(_filePath);
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

        public string ModifiedAtStr
        {
            get => _createdAt.ToString("yyyy.MM.dd HH:mm:ss");
        }

        public string DiffMessage
        {
            get => _diffMessage;
            set
            {
                _diffMessage = value;
                OnPropertyChanged(nameof(DiffMessage));
            }
        }

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncFileInfo class constructor. </summary>
        public SyncFileInfo()
        {
            //
        }

        #endregion CLASS METHODS

        #region MESSAGEM METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Append diffrence message. </summary>
        /// <param name="message"> Message. </param>
        public void AppendDiffMessage(string message)
        {
            DiffMessage = !string.IsNullOrEmpty(DiffMessage) ?
                DiffMessage + Environment.NewLine + message : message;
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

    }
}
