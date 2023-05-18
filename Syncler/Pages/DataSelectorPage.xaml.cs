using chkam05.Tools.ControlsEx;
using chkam05.Tools.ControlsEx.Data;
using chkam05.Tools.ControlsEx.InternalMessages;
using Syncler.Data.Configuration;
using Syncler.Data.Events;
using Syncler.Data.Synchronisation;
using Syncler.InternalMessages;
using Syncler.Pages.Base;
using Syncler.Pages.Settings;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Syncler.Pages
{
    public partial class DataSelectorPage : BasePage
    {

        //  VARIABLES

        private bool _dataModified = false;
        private BasePage _exitPage = null;
        private bool _exitRequest = false;

        private ObservableCollection<SyncGroup> _syncGroupsCollection;

        public ConfigManager ConfigManager { get; private set; }


        //  GETTERS & SETTERS

        public ObservableCollection<SyncGroup> SyncGroupCollection
        {
            get => _syncGroupsCollection;
            set
            {
                _syncGroupsCollection = value;
                _syncGroupsCollection.CollectionChanged += OnSyncGroupCollectionChanged;
                OnPropertyChanged(nameof(SyncGroupCollection));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> DataSelectorPage class constructor. </summary>
        /// <param name="pagesManager"> Pages Manager. </param>
        public DataSelectorPage(PagesManager pagesManager) : base(pagesManager)
        {
            //  Initialize modules.
            ConfigManager = ConfigManager.Instance;

            //  Setup data.
            SetupData();

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking add sync group button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddSyncGroupButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var imContainer = App.GetIMContainer();
            var im = new AddModifySyncGroupIM(imContainer);

            im.SyncGroup = new SyncGroup();
            im.SyncGroupNames = SyncGroupCollection.Select(g => g.Name).ToList();

            im.OnClose += (s, es) =>
            {
                if (es.Result == InternalMessageResult.Ok)
                {
                    SyncGroupCollection.Add(im.SyncGroup);
                }
            };

            imContainer.ShowMessage(im);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking add sync group item button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddSyncGroupItemButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var source = ((e.Source as ButtonEx)?.DataContext as SyncGroup);

            if (source is SyncGroup syncGroup && SyncGroupCollection.Any(i => i.Id == syncGroup.Id))
            {
                var imContainer = App.GetIMContainer();
                var imFilesSelector = FilesSelectorInternalMessageEx.CreateSelectDirectoryInternalMessageEx(imContainer);

                imFilesSelector.AllowCreate = false;
                imFilesSelector.InitialDirectory = imFilesSelector.CurrentDirectory;
                imFilesSelector.OnClose += (s, efs) =>
                {
                    if (efs.Result == InternalMessageResult.Ok && Directory.Exists(efs.FilePath))
                    {
                        if (!syncGroup.ValidateGroupItemPath(efs.FilePath, out string errorMessage))
                        {
                            string title = "Could not add group item.";

                            var im = InternalMessageEx.CreateErrorMessage(imContainer, title, errorMessage);

                            InternalMessagesHelper.ApplyVisualStyle(im);

                            imContainer.ShowMessage(im);

                            return;
                        }

                        syncGroup.Items.Add(new SyncGroupItem(syncGroup.Id) { Path = efs.FilePath });
                        _dataModified = true;
                    }
                };

                InternalMessagesHelper.ApplyVisualStyle(imFilesSelector);
                imContainer.ShowMessage(imFilesSelector);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking remove sync group button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void RemoveSyncGroupButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var source = ((e.Source as ButtonEx)?.DataContext as SyncGroup);

            if (source is SyncGroup syncGroup && SyncGroupCollection.Any(i => i.Id == syncGroup.Id))
            {
                string title = "Remove group config";
                string message = $"Do you want to remove \"{syncGroup.Name}\" group config?";

                var imContainer = App.GetIMContainer();
                var im = InternalMessageEx.CreateQuestionMessage(imContainer, title, message);

                im.OnClose += (s, ec) =>
                {
                    if (ec.Result == InternalMessageResult.Yes)
                        SyncGroupCollection.Remove(syncGroup);
                };

                InternalMessagesHelper.ApplyVisualStyle(im);
                imContainer.ShowMessage(im);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking remove sync group item button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void RemoveSyncGroupItemButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var source = ((e.Source as ButtonEx)?.DataContext as SyncGroupItem);

            if (source is SyncGroupItem syncGroupItem)
            {
                var syncGroup = SyncGroupCollection.FirstOrDefault(s => s.Id == syncGroupItem.ParentId);

                if (syncGroup != null && syncGroup.Items.Contains(syncGroupItem))
                {
                    syncGroup.Items.Remove(syncGroupItem);
                    _dataModified = true;
                }
            }
        }

        #endregion INTERACTION METHODS

        #region INTERACTIONS WITH PAGES MANAGER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked by PagesManager when GoBack is called. </summary>
        /// <param name="previousPage"> Page to return to. </param>
        /// <returns> True - allow to go back; False - otherwise. </returns>
        public override bool OnGoBackFromPage(BasePage previousPage)
        {
            if (_dataModified && !_exitRequest)
            {
                if (!ValidateGroupsBeforeSave(out string errorMessage))
                {
                    OnValidateGroupsBeforeSave(errorMessage);
                    return false;
                }

                SaveSyncGroups();
            }

            if (_exitRequest)
            {
                _exitPage = null;
                _exitRequest = false;
            }

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked by PagesManager when Load(another)Page is called. </summary>
        /// <param name="pageToLoad"> Page to load. </param>
        /// <returns> True - allow to load another page; False - otherwise. </returns>
        public override bool OnGoForwardFromPage(BasePage pageToLoad)
        {
            if (_dataModified && !_exitRequest)
            {
                if (!ValidateGroupsBeforeSave(out string errorMessage))
                {
                    OnValidateGroupsBeforeSave(errorMessage, pageToLoad);
                    return false;
                }

                SaveSyncGroups();
            }

            if (_exitRequest)
            {
                _exitPage = null;
                _exitRequest = false;
            }

            return true;
        }

        #endregion INTERACTIONS WITH PAGES MANAGER METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after modifying sync groups collection. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void OnSyncGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _dataModified = true;
            OnPropertyChanged(nameof(SyncGroupCollection));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region SETUP & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Save sync groups. </summary>
        private void SaveSyncGroups()
        {
            if (_dataModified)
            {
                ConfigManager.SyncGroups = SyncGroupCollection.ToList();
                ConfigManager.SaveSettings();
            }

            _dataModified = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data containers. </summary>
        private void SetupData()
        {
            SyncGroupCollection = new ObservableCollection<SyncGroup>(ConfigManager.SyncGroups);
            _dataModified = false;
        }

        #endregion SETUP & SAVE METHODS

        #region SYNC MANAGER VALIDATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Validate sync groups data before save. </summary>
        /// <param name="errorMessage"> Output error message. </param>
        /// <returns> True - validation successfull; False - otherwise. </returns>
        protected bool ValidateGroupsBeforeSave(out string errorMessage)
        {
            StringBuilder sb = new StringBuilder();
            bool result = true;

            var syncGroupNamesWithoutItems = SyncGroupCollection.Where(g => g.Items.Count < 2);
            var syncGroupNamesWithDuplicatedItems = SyncGroupCollection.Where(g => 
            {
                var groups = g.Items.GroupBy(i => i.Path);

                if (groups.Any(i => i.Count() > 1))
                    return true;

                return false;
            });

            if (syncGroupNamesWithoutItems.Any())
            {
                foreach (var item in syncGroupNamesWithoutItems)
                    sb.Append($"{item.Name} contains less than one catalog.");

                result = false;
            }

            if (syncGroupNamesWithDuplicatedItems.Any())
            {
                foreach (var item in syncGroupNamesWithDuplicatedItems)
                    sb.Append($"{item.Name} contains duplicated catalog.");

                result = false;
            }

            errorMessage = sb.ToString();
            return result;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after failed groups data validation. </summary>
        /// <param name="errorMessage"> Validation error message. </param>
        /// <param name="pageToLoad"> Page to load. </param>
        protected void OnValidateGroupsBeforeSave(string errorMessage, BasePage pageToLoad = null)
        {
            string title = "Could not save data.";
            string message = "Data validation faled. Do you want to abandon changes and exit?"
                + Environment.NewLine + errorMessage;

            var imContainer = App.GetIMContainer();
            var im = InternalMessageEx.CreateQuestionMessage(imContainer, title, message);

            im.OnClose += (s, ie) =>
            {
                if (ie.Result == InternalMessageResult.Yes)
                {
                    _exitPage = pageToLoad;
                    _exitRequest = true;

                    if (_exitPage != null)
                        _pagesManager.LoadPage(_exitPage, force: true);
                    else
                        _pagesManager.GoBack(force: true);
                }
            };

            InternalMessagesHelper.ApplyVisualStyle(im);
            imContainer.ShowMessage(im);
        }

        #endregion SYNC MANAGER VALIDATION METHODS

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after unloading page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BasePage_Unloaded(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion PAGE METHODS

    }
}
