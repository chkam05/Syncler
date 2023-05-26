using MaterialDesignThemes.Wpf;
using Syncler.Components.MainMenu;
using Syncler.Pages.Base;
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

namespace Syncler.Pages.Settings
{
    public partial class SettingsPage : BasePage
    {

        //  GETTERS & SETTERS

        public override List<MainMenuItem> MainMenuItems
        {
            get => new List<MainMenuItem>()
            {
                new MainMenuItem("Appearance", PackIconKind.Palette, OnAppearanceMenuItemSelect),
                new MainMenuItem("Sync Directories", PackIconKind.Folders, OnSyncDirectoriesMenuItemSelect),
                new MainMenuItem("About", PackIconKind.InfoOutline, OnAboutMenuItemSelect),
            };
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsPage class constructor. </summary>
        /// <param name="pagesManager"> Pages Manager. </param>
        public SettingsPage(PagesManager pagesManager) : base(pagesManager)
        {
            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Appearance settings option button control. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AppearanceSettingsOptionButtonControl_Click(object sender, RoutedEventArgs e)
        {
            _pagesManager.LoadPage(new AppearanceSettingsPage(_pagesManager));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Info settings option button control. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void InfoSettingsOptionButtonControl_Click(object sender, RoutedEventArgs e)
        {
            _pagesManager.LoadPage(new InfoSettingsPage(_pagesManager));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Sync directories settings option button control. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SyncDirectoriesSettingsOptionButtonControl_Click(object sender, RoutedEventArgs e)
        {
            _pagesManager.LoadPage(new DataSelectorPage(_pagesManager));
        }

        #endregion INTERACTION METHODS

        #region MENU ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting appearance menu item. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void OnAppearanceMenuItemSelect(object sender, EventArgs e)
        {
            _pagesManager.LoadPage(new AppearanceSettingsPage(_pagesManager));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after about settings menu item. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void OnAboutMenuItemSelect(object sender, EventArgs e)
        {
            _pagesManager.LoadPage(new InfoSettingsPage(_pagesManager));
        }

        //  --------------------------------------------------------------------------------
        //// <summary> Method invoked after sync directories settings menu item. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void OnSyncDirectoriesMenuItemSelect(object sender, EventArgs e)
        {
            _pagesManager.LoadPage(new DataSelectorPage(_pagesManager));
        }

        #endregion MENU ITEMS METHODS

    }
}
