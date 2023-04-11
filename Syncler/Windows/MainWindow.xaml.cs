using chkam05.Tools.ControlsEx.InternalMessages;
using chkam05.Tools.ControlsEx.WindowsEx;
using Syncler.Components.MainMenu;
using Syncler.Data.Configuration;
using Syncler.Pages;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Syncler.Windows
{
    public partial class MainWindow : WindowEx
    {

        //  VARIABLES

        public ConfigManager ConfigManager { get; private set; }


        //  GETTERS & SETTERS

        public InternalMessagesExContainer InternalMessagesContainer
        {
            get => imContainer;
        }

        public PagesManager PagesManager
        {
            get => pagesManager;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MainWindow class constructor. </summary>
        public MainWindow()
        {
            //  Initialize modules.
            ConfigManager = ConfigManager.Instance;

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region MAIN MENU INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load main menu items defined in page. </summary>
        /// <param name="page"> Page. </param>
        private void LoadMenuFromPage(BasePage page)
        {
            if (page?.MainMenuItems != null)
            {
                mainMenu.ClearItems();

                if (page.MainMenuItems.Any())
                    mainMenu.AddMenuItems(page.MainMenuItems);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing BackButton in MainMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event arguments. </param>
        private void MainMenu_OnBackItemSelect(object sender, MainMenuItemSelectEventArgs e)
        {
            if (PagesManager.CanGoBack)
                PagesManager.GoBack();
        }

        #endregion MAIN MENU INTERACTION METHODS

        #region PAGES MANAGER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after navigating to previous page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Page Loaded Event Arguments. </param>
        private void PagesManager_OnPageBack(object sender, OnPageLoadedEventArgs e)
        {
            LoadMenuFromPage(e.LoadedPage);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Page Loaded Event Arguments. </param>
        private void PagesManager_OnPageNavigated(object sender, OnPageLoadedEventArgs e)
        {
            LoadMenuFromPage(e.LoadedPage);
        }

        #endregion PAGES MANAGER INTERACTION METHODS

        #region WINDOW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading window. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void WindowEx_Loaded(object sender, RoutedEventArgs e)
        {
            //  Load window size and position.
            Left = ConfigManager.Config.WindowPosition.X;
            Top = ConfigManager.Config.WindowPosition.Y;
            Height = ConfigManager.Config.WindowSize.Height;
            Width = ConfigManager.Config.WindowSize.Width;

            //  Fix position on screen.
            var screen = ApplicationHelper.GetScreenWhereIsWindow(this);

            if (screen != null)
                ApplicationHelper.AdjustWindowToScreen(this, screen);
            else
                ApplicationHelper.AdjustWindowToPrimaryScreen(this);

            //  Load home page.
            PagesManager.LoadPage(new HomePage(PagesManager));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked before closing window. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Cancel Event Arguments. </param>
        private void WindowEx_Closed(object sender, EventArgs e)
        {
            //  Save window size and position.
            ConfigManager.Config.WindowPosition = new Point(Left, Top);
            ConfigManager.Config.WindowSize = new Size(Width, Height);

            //  Save settings.
            ConfigManager.SaveSettings();
        }

        #endregion WINDOW METHODS

    }
}
