using chkam05.Tools.ControlsEx.InternalMessages;
using chkam05.Tools.ControlsEx.WindowsEx;
using Syncler.Components.MainMenu;
using Syncler.Data.Configuration;
using Syncler.Data.Synchronisation;
using Syncler.Pages;
using Syncler.Pages.Base;
using Syncler.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace Syncler.Windows
{
    public partial class MainWindow : WindowEx
    {

        //  VARIABLES

        public ConfigManager ConfigManager { get; private set; }
        public System.Windows.Forms.NotifyIcon TrayIcon;


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
            InitializeTrayIcon();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing Window state. </summary>
        /// <param name="e"> Event arguments. </param>
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
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

        #region SYSTEM TRAY

        //  --------------------------------------------------------------------------------
        /// <summary> Initialize system tray icon. </summary>
        private void InitializeTrayIcon()
        {
            TrayIcon = new System.Windows.Forms.NotifyIcon();
            TrayIcon.BalloonTipText = "Syncler has been minimised. Click the tray icon to show.";
            TrayIcon.BalloonTipTitle = "Show Syncler";
            TrayIcon.Text = "Syncler";
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Syncler;component/Icon.ico")).Stream;
            TrayIcon.Icon = new System.Drawing.Icon(iconStream);
            TrayIcon.MouseClick += TrayIcon_MouseClick;
            TrayIcon.DoubleClick += TrayIcon_DoubleClick;
            TrayIcon.Visible = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on tray icon. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Mouse event arguments. </param>
        private void TrayIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks < 2)
            {
                var trayWindow = Application.Current.Windows.Cast<Window>()
                    .FirstOrDefault(w => w.GetType() == typeof(TrayWindow));

                if (trayWindow == null)
                    trayWindow = new TrayWindow();
                trayWindow.Show();

                var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                trayWindow.Top = desktopWorkingArea.Bottom - (trayWindow.ActualHeight + 8);
                trayWindow.Left = desktopWorkingArea.Right - (trayWindow.ActualWidth + 8);
                trayWindow.Activate();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after double clicking on tray icon. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Event arguments. </param>
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        #endregion SYSTEM TRAY

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

            //  Dispose modules.
            SyncManager.Instance.Dispose();
            TrayIcon.Dispose();

            var trayWindow = Application.Current.Windows.Cast<Window>()
                .FirstOrDefault(w => w.GetType() == typeof(TrayWindow));

            if (trayWindow != null)
                trayWindow.Close();
        }

        #endregion WINDOW METHODS

    }
}
