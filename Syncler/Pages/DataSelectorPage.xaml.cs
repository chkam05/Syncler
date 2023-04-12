using Syncler.Data.Synchronisation;
using Syncler.Pages.Base;
using Syncler.Pages.Settings;
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
    public partial class DataSelectorPage : BasePage
    {

        //  VARIABLES

        public SyncManager SyncManager { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> DataSelectorPage class constructor. </summary>
        /// <param name="pagesManager"> Pages Manager. </param>
        public DataSelectorPage(PagesManager pagesManager) : base(pagesManager)
        {
            //  Initialize modules.
            SyncManager = SyncManager.Instance;

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region HEADER BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking add group button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddGroupButtonEx_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion HEADER BUTTONS METHODS

    }
}
