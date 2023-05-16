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
using GroupItem = Syncler.Data.Synchronisation.GroupItem;

namespace Syncler.Pages
{
    public partial class DataSelectorPage : BasePage
    {

        //  VARIABLES

        private bool _exitBack = false;
        private BasePage _exitPage = null;
        private bool _exitRequest = false;

        private ObservableCollection<SyncGroup> _syncGroupsCollection;
        private bool _syncGroupsCollectionModified = false;

        public ConfigManager ConfigManager { get; private set; }


        //  GETTERS & SETTERS

        public ObservableCollection<SyncGroup> SyncGroupCollection
        {
            get => _syncGroupsCollection;
            set
            {
                _syncGroupsCollection = value;
                _syncGroupsCollection.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(SyncGroupCollection)); };
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
            SyncGroupCollection = new ObservableCollection<SyncGroup>(ConfigManager.Config.SyncGroups);

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        private void AddSyncGroupButtonEx_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void AddSyncGroupItemButtonEx_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void RemoveSyncGroupButtonEx_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void RemoveSyncGroupItemButtonEx_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking add group button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddGroupButtonEx_Click(object sender, RoutedEventArgs e)
        {
            /*var imContainer = App.GetIMContainer();
            var im = new AddModifySyncGroupIM(imContainer);

            im.GroupConfig = new GroupConfig();
            im.GroupConfigNames = SyncManager.GetSyncGroupNames();

            im.OnClose += (s, es) =>
            {
                if (es.Result == InternalMessageResult.Ok)
                {
                    SyncManager.AddGroupConfig(im.GroupConfig, true);
                }
            };

            imContainer.ShowMessage(im);*/
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing add group item button. </summary>
        /// <param name="item"> Group config object. </param>
        private void OnAddGroupItem(object item)
        {
            /*var imContainer = App.GetIMContainer();
            var imFilesSelector = FilesSelectorInternalMessageEx.CreateSelectDirectoryInternalMessageEx(imContainer);

            imFilesSelector.AllowCreate = false;
            imFilesSelector.InitialDirectory = imFilesSelector.CurrentDirectory;
            imFilesSelector.OnClose += (s, efs) =>
            {
                if (efs.Result == InternalMessageResult.Ok && Directory.Exists(efs.FilePath))
                {
                    var groupConfig = (GroupConfig)item;

                    if (!groupConfig.ValidateGroupItemPath(efs.FilePath, out string errorMessage))
                    {
                        string title = "Could not add group item.";

                        var im = InternalMessageEx.CreateErrorMessage(imContainer, title, errorMessage);

                        InternalMessagesHelper.ApplyVisualStyle(im);

                        imContainer.ShowMessage(im);

                        return;
                    }

                    groupConfig.AddGroupItem(new GroupItem() { Path = efs.FilePath }, true);
                }
            };

            InternalMessagesHelper.ApplyVisualStyle(imFilesSelector);

            imContainer.ShowMessage(imFilesSelector);*/
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing remove group config button. </summary>
        /// <param name="item"> Group config object. </param>
        private void OnRemoveGroupConfig(object item)
        {
            /*if (item is GroupConfig groupConfig && SyncManager.HasGroupConfig(groupConfig))
            {
                string title = "Remove group config";
                string message = $"Do you want to remove \"{groupConfig.Name}\" group config?";

                var imContainer = App.GetIMContainer();
                var im = InternalMessageEx.CreateQuestionMessage(imContainer, title, message);

                im.OnClose += (s, e) =>
                {
                    if (e.Result == InternalMessageResult.Ok)
                        SyncManager.RemoveGroupConfig(groupConfig, true);
                };

                InternalMessagesHelper.ApplyVisualStyle(im);

                imContainer.ShowMessage(im);
            }*/
        }

        #endregion INTERACTION METHODS

        #region INTERACTIONS WITH PAGES MANAGER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked by PagesManager when GoBack is called. </summary>
        /// <param name="previousPage"> Page to return to. </param>
        /// <returns> True - allow to go back; False - otherwise. </returns>
        public override bool OnGoBackFromPage(BasePage previousPage)
        {
            /*_exitBack = true;
            _exitRequest = true;

            if (!SyncManager.SaveData())
                return false;*/

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked by PagesManager when Load(another)Page is called. </summary>
        /// <param name="pageToLoad"> Page to load. </param>
        /// <returns> True - allow to load another page; False - otherwise. </returns>
        public override bool OnGoForwardFromPage(BasePage pageToLoad)
        {
            /*_exitPage = pageToLoad;
            _exitRequest = true;

            if (!SyncManager.SaveData())
                return false;*/

            return true;
        }

        #endregion INTERACTIONS WITH PAGES MANAGER METHODS

        #region SYNC MANAGER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after raising error from sync manager. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Error Relay Event Arguments. </param>
        protected void OnErrorRelayRaised(object sender, ErrorRelayEventArgs e)
        {
            string title = "Could not save data.";
            string message = _exitRequest
                ? e.ErrorMessage + Environment.NewLine + "Do you want to abandon changes and exit?"
                : e.ErrorMessage;

            var imContainer = App.GetIMContainer();
            var im = _exitRequest
                ? InternalMessageEx.CreateQuestionMessage(imContainer, title, message)
                : InternalMessageEx.CreateErrorMessage(imContainer, title, message);

            if (_exitRequest)
            {
                im.OnClose += (s, ie) =>
                {
                    _exitRequest = false;

                    if (ie.Result == InternalMessageResult.Yes)
                    {
                        if (_exitBack)
                        {
                            _pagesManager.GoBack(force: true);
                            _exitBack = false;
                            return;
                        }

                        if (_exitPage != null)
                        {
                            _pagesManager.LoadPage(_exitPage, force: true);
                            _exitPage = null;
                            return;
                        }
                    }
                };
            }

            InternalMessagesHelper.ApplyVisualStyle(im);

            imContainer.ShowMessage(im);
        }

        #endregion SYNC MANAGER INTERACTION METHODS

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
