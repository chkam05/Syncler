using chkam05.Tools.ControlsEx;
using MaterialDesignThemes.Wpf;
using Syncler.Components.MainMenu;
using Syncler.Context;
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
    public partial class HomePage : BasePage
    {

        //  VARIABLES

        public InfoSettingsPageDataContext InfoSettingsPageDataContext { get; private set; }
        public SyncManager SyncManager { get; private set; }


        //  GETTERS & SETTERS

        public override List<MainMenuItem> MainMenuItems
        {
            get => new List<MainMenuItem>()
            {
                new MainMenuItem("Home", PackIconKind.Home, OnHomeMenuItemSelect),
                new MainMenuItem("Synchronisation", PackIconKind.Sync, OnSynchronisationMenuItemSelect),
                new MainMenuItem("Logger", PackIconKind.ScriptTextOutline, OnLoggerMenuItemSelect),
                new MainMenuItem("Settings", PackIconKind.Gear, OnSettingsMenuItemSelect),
            };
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> HomePage class constructor. </summary>
        /// <param name="pagesManager"> Pages Manager. </param>
        public HomePage(PagesManager pagesManager) : base(pagesManager)
        {
            //  Initialize modules.
            InfoSettingsPageDataContext = new InfoSettingsPageDataContext();
            SyncManager = SyncManager.Instance;

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region DASHBOARD INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting main menu item. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Selection Changed Event Arguments. </param>
        private void MenuListViewEx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListViewEx)sender;
            var selectedItem = listView.SelectedItem;

            if (selectedItem != null)
            {
                var menuItem = (MainMenuItem)selectedItem;

                if (menuItem != null)
                    menuItem.InvokeAction();

                listView.SelectedItem = null;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking stop button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void StopButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var source = ((e.Source as ButtonEx)?.DataContext as SyncThread);

            if (source is SyncThread syncThread)
                syncThread.Stop();
        }

        #endregion DASHBOARD INTERACTION METHODS

        #region MENU ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting home menu item. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void OnHomeMenuItemSelect(object sender, EventArgs e)
        {
            _pagesManager.LoadPage(new HomePage(_pagesManager));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting home menu item. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void OnSynchronisationMenuItemSelect(object sender, EventArgs e)
        {
            _pagesManager.LoadPage(new SynchronisationPage(_pagesManager));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting home menu item. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void OnLoggerMenuItemSelect(object sender, EventArgs e)
        {
            _pagesManager.LoadPage(new LoggerPage(_pagesManager));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting settings menu item. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void OnSettingsMenuItemSelect(object sender, EventArgs e)
        {
            _pagesManager.LoadPage(new SettingsPage(_pagesManager));
        }

        #endregion MENU ITEMS METHODS

    }
}
