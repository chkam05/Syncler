using Newtonsoft.Json;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syncler.Data.Synchronisation
{
    public class GroupConfig : INotifyPropertyChanged
    {

        //  COMMANDS

        [JsonIgnore]
        public ICommand RemoveGroupItemCommand { get; set; }


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

            //  Initialize commands.
            RemoveGroupItemCommand = new RelayCommand(OnRemoveGroupItem);
        }

        #endregion CLASS METHODS

        #region DATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing remove group item button. </summary>
        /// <param name="item"> Group item object. </param>
        private void OnRemoveGroupItem(object item)
        {
            if (item is GroupItem groupItem && Items.Contains(groupItem))
                Items.Remove(groupItem);
        }

        #endregion DATA MANAGEMENT METHODS

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
