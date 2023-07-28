using Newtonsoft.Json;
using Syncler.Data.Events;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
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
        private bool _autoSync = false;


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

        public bool AutoSync
        {
            get => _autoSync;
            set
            {
                _autoSync = value;
                OnPropertyChanged(nameof(AutoSync));
            }
        }


        public string ListingFilePath
        {
            get
            {
                string catalog = Path.Combine(
                    Environment.GetEnvironmentVariable("APPDATA"),
                    ApplicationHelper.GetApplicationName(),
                    "Listings");

                if (!Directory.Exists(catalog))
                    Directory.CreateDirectory(catalog);

                return Path.Combine(catalog, $"{Id}.json");
            }
        }

        public string DiffrencesFilePath
        {
            get
            {
                string catalog = Path.Combine(
                    Environment.GetEnvironmentVariable("APPDATA"),
                    ApplicationHelper.GetApplicationName(),
                    "Diffrences");

                if (!Directory.Exists(catalog))
                    Directory.CreateDirectory(catalog);

                return Path.Combine(catalog, $"{Id}.json");
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

        #region VALIDATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Validate group config. </summary>
        /// <param name="errorMessage"> Output error message. </param>
        /// <param name="groupConfigNames"> Group config names already taken. </param>
        /// <returns></returns>
        public bool ValidateGroup(out string errorMessage, List<string> groupConfigNames = null)
        {
            if (string.IsNullOrEmpty(Name))
            {
                errorMessage = $"Group name cannot be null or empty.";
                return false;
            }

            if (groupConfigNames != null && groupConfigNames.Any(g => g.ToLower() == Name.ToLower()))
            {
                errorMessage = $"Group name \"{Name}\" is already taken by another group.";
                return false;
            }

            if (Items.Count < 2)
            {
                errorMessage = $"Group must contain at least two catalogs to sync files between them.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate group item path. </summary>
        /// <param name="filePath"> Group item path. </param>
        /// <param name="errorMessage"> Output error message. </param>
        /// <returns> True - group item path is valid; False - otherwise. </returns>
        public bool ValidateGroupItemPath(string filePath, out string errorMessage)
        {
            if (Items.Any(i => i.Path == filePath))
            {
                errorMessage = "Group already contains \"{Path.GetFileName(filePath)}\" catalog.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        #endregion VALIDATION METHODS

    }
}
