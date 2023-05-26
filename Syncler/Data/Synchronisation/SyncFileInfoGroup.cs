using Syncler.Data.Configuration;
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
        private string _diffMessage = null;


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
            get => _diffMessage;
            set
            {
                _diffMessage = value;
                OnPropertyChanged(nameof(DiffMessage));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncFileInfoGroup class constructor. </summary>
        public SyncFileInfoGroup()
        {
            //
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

        #region VALIDATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Validate if all files are good and synchronised. </summary>
        /// <param name="SyncGroup"> Sync group. </param>
        /// <param name="validationConfig"> Validation configuration. </param>
        /// <returns></returns>
        public bool ValidateFiles(SyncGroup SyncGroup, SyncValidationConfig validationConfig)
        {
            var result = ValidateFilesCount(SyncGroup);

            if (validationConfig.ByName && !ValidateFilesByName())
                result = false;

            if (validationConfig.BySize && !ValidateFilesBySize())
                result = false;

            if (validationConfig.ByChecksum && !ValidateFilesByChecksum())
                result = false;

            return result;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate files in group - search for missing files. </summary>
        /// <param name="SyncGroup"> Sync group. </param>
        /// <returns> True - files are ok; False - files are missing. </returns>
        private bool ValidateFilesCount(SyncGroup SyncGroup)
        {
            var diffMessageSb = new StringBuilder();
            var result = true;

            if (SyncGroup.Items.Count != Files.Count)
            {
                var dirs = SyncGroup.Items
                    .Where(i => !Files.Any(f => f.BasePath == i.Name))
                    .Select(i => i.Path);

                if (dirs.Any())
                {
                    foreach (var dir in dirs)
                        diffMessageSb.AppendLine($"Missing file in {dir}");

                    result = false;
                }
            }

            if (!result)
                DiffMessage = diffMessageSb.ToString();

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
                    item.AppendDiffMessage("File name does not match the rest of the files.");
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
                    item.AppendDiffMessage("File size is diffrent than the rest of the files size.");
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
                    item.AppendDiffMessage("File checksum is diffrent than the rest of the files checksums.");
                }

                return false;
            }

            return true;
        }

        #endregion VALIDATION METHODS

    }
}
