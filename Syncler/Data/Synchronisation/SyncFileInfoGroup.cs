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

    }
}
