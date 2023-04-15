using chkam05.Tools.ControlsEx;
using chkam05.Tools.ControlsEx.Data;
using chkam05.Tools.ControlsEx.InternalMessages;
using Syncler.Data.Synchronisation;
using Syncler.InternalMessages;
using Syncler.Pages.Base;
using Syncler.Pages.Settings;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
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

        //  COMMANDS

        public ICommand AddGroupItemCommand { get; set; }
        public ICommand RemoveGroupConfigCommand { get; set; }
        public ICommand RemoveGroupItemCommand { get; set; }


        //  VARIABLES

        public SyncManager SyncManager { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> DataSelectorPage class constructor. </summary>
        /// <param name="pagesManager"> Pages Manager. </param>
        public DataSelectorPage(PagesManager pagesManager) : base(pagesManager)
        {
            //  Initialize commands.
            AddGroupItemCommand = new RelayCommand(OnAddGroupItem);
            RemoveGroupConfigCommand = new RelayCommand(OnRemoveGroupConfig);

            //  Initialize modules.
            SyncManager = SyncManager.Instance;

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking add group button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddGroupButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var imContainer = App.GetIMContainer();
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

            imContainer.ShowMessage(im);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing add group item button. </summary>
        /// <param name="item"> Group config object. </param>
        private void OnAddGroupItem(object item)
        {
            var imContainer = App.GetIMContainer();
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

                        var im = InternalMessageEx.CreateQuestionMessage(imContainer, title, errorMessage);

                        im.OnClose += (se, ei) =>
                        {
                            if (ei.Result == InternalMessageResult.Ok)
                                SyncManager.SyncConfig.Groups.Remove(groupConfig);
                        };

                        InternalMessagesHelper.ApplyVisualStyle(im);

                        imContainer.ShowMessage(im);

                        return;
                    }

                    groupConfig.Items.Add(new GroupItem() { Path = efs.FilePath });
                    SyncManager.SaveSettings();
                }
            };

            InternalMessagesHelper.ApplyVisualStyle(imFilesSelector);

            imContainer.ShowMessage(imFilesSelector);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing remove group config button. </summary>
        /// <param name="item"> Group config object. </param>
        private void OnRemoveGroupConfig(object item)
        {
            if (item is GroupConfig groupConfig && SyncManager.HasGroupConfig(groupConfig))
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
            }
        }

        #endregion INTERACTION METHODS

    }
}
