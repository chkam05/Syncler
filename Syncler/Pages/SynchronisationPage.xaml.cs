using chkam05.Tools.ControlsEx;
using Syncler.Data.Configuration;
using Syncler.Data.Synchronisation;
using Syncler.Pages.Base;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
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
        private void ScanFilesButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var source = ((e.Source as ButtonEx)?.DataContext as SyncThread);

            if (source is SyncThread syncThread)
            {
                syncThread.Scan();
            }
        }

        #endregion INTERACTION METHODS

    }
}
