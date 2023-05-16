using Newtonsoft.Json;
using Syncler.Data.Events;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Configuration
{
    public class SyncGroup : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _id;
        private string _name;
        private ObservableCollection<SyncGroupItem> _itemsCollection;


        //  GETTERS & SETTERS

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<SyncGroupItem> Items
        {
            get => _itemsCollection;
            set
            {
                _itemsCollection = value;
                _itemsCollection.CollectionChanged += OnItemsCollectionChanged;
                OnPropertyChanged(nameof(Items));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SyncGroup class constructor. </summary>
        [JsonConstructor]
        public SyncGroup(string id = null, ObservableCollection<SyncGroupItem> items = null)
        {
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString("N").ToLower();
            Items = items ?? new ObservableCollection<SyncGroupItem>();
        }

        #endregion CLASS METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoekd after changing items collection. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        protected void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Items));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler event. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }
}
