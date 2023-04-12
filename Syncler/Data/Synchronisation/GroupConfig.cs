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
    public class GroupConfig : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _name;
        private ObservableCollection<GroupItem> _itemsCollection;


        //  GETTERS & SETTERS

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<GroupItem> Items
        {
            get => _itemsCollection;
            set
            {
                _itemsCollection = value;
                _itemsCollection.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(Items)); };
                OnPropertyChanged(nameof(Items));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> GroupConfig class constructor. </summary>
        [JsonConstructor]
        public GroupConfig(ObservableCollection<GroupItem> items = null)
        {
            Items = items ?? new ObservableCollection<GroupItem>();
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
