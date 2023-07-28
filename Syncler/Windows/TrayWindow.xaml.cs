using chkam05.Tools.ControlsEx;
using Syncler.Data.Synchronisation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Syncler.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy TrayWindow.xaml
    /// </summary>
    public partial class TrayWindow : Window
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        public SyncManager SyncManager { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> TrayWindow class constructor. </summary>
        public TrayWindow()
        {
            //  Initialize modules.
            SyncManager = SyncManager.Instance;

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region BASE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on ShowAppButtonEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ShowAppButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var application = (App)Application.Current;
            var mainWindow = application.MainWindow;

            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Activate();

            this.Close();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on CloseAppButtonEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void CloseAppButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var application = (App)Application.Current;
            application.Shutdown();
        }

        #endregion BASE INTERACTION METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler event. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region SYNCLER INTERACTION METHODS

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

        #endregion SYNCLER INTERACTION METHODS

        #region WINDOW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking close window. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Event arguments. </param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after moving cursor outisde window. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Mouse event arguments. </param>
        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch
            {
                //  Ignore this.
            }
        }

        #endregion WINDOW METHODS

    }
}
