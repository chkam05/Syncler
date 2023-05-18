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
        private string _subCatalog;


        //  GETTERS & SETTERS

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        public string FileName
        {
            get => Path.GetFileName(_filePath);
        }

        public FileInfo FileInfo
        {
            get => _fileInfo;
            set => _fileInfo = value;
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
        /// <param name="filePath"> File path. </param>
        /// <param name="fileInfo"> File information. </param>
        /// <param name="subCatalog"> Sub catalog. </param>
        public SyncTempFileInfo(string filePath, FileInfo fileInfo, string subCatalog)
        {
            FilePath = filePath;
            FileInfo = fileInfo;
            SubCatalog = subCatalog;
        }

        #endregion CLASS METHODS

    }
}
