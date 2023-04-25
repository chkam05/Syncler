using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Events
{
    public class ErrorRelayEventArgs : EventArgs
    {

        //  VARIABLES

        public bool Error { get; private set; }
        public string ErrorMessage { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ErrorRelayEventArgs class constructor. </summary>
        /// <param name="errorMessage"> Error message. </param>
        /// <param name="raiseError"> Raise error or not. </param>
        public ErrorRelayEventArgs(string errorMessage, bool raiseError = true)
        {
            Error = raiseError;
            ErrorMessage = errorMessage;
        }

        #endregion CLASS METHODS

    }
}
