using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Pages.Base
{
    public class OnPageLoadedEventArgs : EventArgs
    {

        //  VARIABLES

        public BasePage LoadedPage { get; private set; }
        public BasePage PreviousPage { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> OnPageLoadedEventArgs class constructor. </summary>
        /// <param name="loadedPage"> Loaded page. </param>
        /// <param name="previousPage"> Previous page. </param>
        public OnPageLoadedEventArgs(BasePage loadedPage, BasePage previousPage)
        {
            LoadedPage = loadedPage;
            PreviousPage = previousPage;
        }

        #endregion CLASS METHODS

    }
}
