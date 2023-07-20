using chkam05.Tools.ControlsEx.Data;
using chkam05.Tools.ControlsEx.InternalMessages;
using MaterialDesignThemes.Wpf;
using Syncler.Data.Logs;
using Syncler.Pages.Base;
using Syncler.Pages.Settings;
using Syncler.Utilities;
using Syncler.Windows;
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
    public partial class LoggerPage : BasePage
    {

        //  VARIABLES

        public Logger Logger { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> LoggerPage class constructor. </summary>
        /// <param name="pagesManager"> Pages Manager. </param>
        public LoggerPage(PagesManager pagesManager) : base(pagesManager)
        {
            //  Initialize modules.
            Logger = ((App)Application.Current).Logger;

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking clear logs button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void ClearLogsButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var imContainer = ((MainWindow)((App)Application.Current).MainWindow).InternalMessagesContainer;
            var imQuestion = InternalMessageEx.CreateQuestionMessage(imContainer, "Clear logs", "Do you wnat to clear logs?");

            InternalMessagesHelper.ApplyVisualStyle(imQuestion);
            imQuestion.IconKind = PackIconKind.Delete;

            imQuestion.OnClose += (s, ime) =>
            {
                if (ime.Result == InternalMessageResult.Yes)
                    Logger.ClearLogs();
            };

            imContainer.ShowMessage(imQuestion);
        }

        #endregion INTERACTION METHODS

    }
}
