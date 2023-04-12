using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Syncler.Data.Synchronisation
{
    public class SyncConfig : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<GroupConfig> _groupsCollection;


        //  GETTERS & SETTERS

        public ObservableCollection<GroupConfig> Groups
        {
            get => _groupsCollection;
            set
            {
                _groupsCollection = value;
                _groupsCollection.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(Groups)); };
                OnPropertyChanged(nameof(Groups));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncConfig class constructor. </summary>
        [JsonConstructor]
        public SyncConfig(ObservableCollection<GroupConfig> groups = null)
        {
            Groups = groups ?? new ObservableCollection<GroupConfig>();
        }

        #endregion CLASS METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler event. </summary>
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
