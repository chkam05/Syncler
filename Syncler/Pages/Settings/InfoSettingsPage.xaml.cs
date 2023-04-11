using Syncler.Context;
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
    public partial class InfoSettingsPage : BasePage
    {

        //  VARIABLES

        public InfoSettingsPageDataContext DataContainer { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InfoSettingsPage class constructor. </summary>
        /// <param name="pagesManager"> Pages Manager. </param>
        public InfoSettingsPage(PagesManager pagesManager) : base(pagesManager)
        {
            DataContainer = new InfoSettingsPageDataContext();

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

    }
}
