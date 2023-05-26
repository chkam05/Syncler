using MaterialDesignThemes.Wpf;
using Syncler.Components.MainMenu;
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

        //  GETTERS & SETTERS

        public override List<MainMenuItem> MainMenuItems
        {
            get => new List<MainMenuItem>()
            {
                new MainMenuItem("Home", PackIconKind.Home, OnHomeMenuItemSelect),
                new MainMenuItem("Synchronisation", PackIconKind.Sync, OnSynchronisationMenuItemSelect),
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
            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

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
