using chkam05.Tools.ControlsEx;
using Syncler.Data.Configuration;
using Syncler.Data.Synchronisation;
using Syncler.Pages.Base;
using Syncler.Utilities;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Syncler.Pages
{
    public partial class SynchronisationPage : BasePage
    {

        //  VARIABLES

        public SyncManager SyncManager { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SynchronisationPage class constructor. </summary>
        /// <param name="pagesManager"> Pages Manager. </param>
        public SynchronisationPage(PagesManager pagesManager) : base(pagesManager)
        {
            //  Initialize modules.
            SyncManager = SyncManager.Instance;
            SyncManager.UpdateDispatcherHandler(new DispatcherHandler(this.Dispatcher));

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking scan/stop button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void ScanFilesButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var source = ((e.Source as ButtonEx)?.DataContext as SyncThread);

            if (source is SyncThread syncThread)
            {
                switch (syncThread.SyncState)
                {
                    case SyncState.NONE:
                    case SyncState.STOPPED_SCANNING:
                    case SyncState.STOPPED_SYNCING:
                        syncThread.Scan();
                        break;

                    case SyncState.SCANNING:
                    case SyncState.SYNCING:
                        syncThread.Stop();
                        break;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after double clicking on SyncFileInfos ListViewEx. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void SyncFileInfoListViewEx_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewEx listViewEx)
            {
                if (listViewEx.SelectedItem is SyncFileInfo syncFileInfo)
                {
                    if (File.Exists(syncFileInfo.FilePath))
                    {
                        string args = string.Format("/e, /select, \"{0}\"", syncFileInfo.FilePath);

                        ProcessStartInfo info = new ProcessStartInfo();
                        info.FileName = "explorer";
                        info.Arguments = args;
                        Process.Start(info);
                    }
                }
            }
        }

        #endregion INTERACTION METHODS

    }
}
