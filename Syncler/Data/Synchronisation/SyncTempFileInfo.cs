using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Synchronisation
{
    public class SyncTempFileInfo
    {

        //  VARIABLES

        private string _filePath;
        private FileInfo _fileInfo;
        private DateTime? _lastModified;
        private string _subCatalog;


        //  GETTERS & SETTERS

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        [JsonIgnore]
        public string FileName
        {
            get => Path.GetFileName(_filePath);
        }

        [JsonIgnore]
        public FileInfo FileInfo
        {
            get
            {
                if (_fileInfo == null)
                    _fileInfo = new FileInfo(FilePath);

                return _fileInfo;
            }
            set => _fileInfo = value;
        }

        public DateTime LastModified
        {
            get
            {
                if (!_lastModified.HasValue)
                    _lastModified = FileInfo.LastWriteTime;

                return _lastModified.Value;
            }
            set => _lastModified = value;
        }

        public string SubCatalog
        {
            get => _subCatalog;
            set => _subCatalog = value;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncTempFileInfo class constructor. </summary>
        public SyncTempFileInfo()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> SyncTempFileInfo class constructor. </summary>
        /// <param name="filePath"> File path. </param>
        /// <param name="subCatalog"> Sub catalog. </param>
        public SyncTempFileInfo(string filePath, string subCatalog)
        {
            FilePath = filePath;
            SubCatalog = subCatalog;
        }

        #endregion CLASS METHODS

    }
}
