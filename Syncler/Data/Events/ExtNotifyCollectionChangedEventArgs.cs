using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Events
{
    public class ExtNotifyCollectionChangedEventArgs
    {

        //  VARIABLES

        public NotifyCollectionChangedAction Action { get; private set; }

        public bool AutoSave { get; private set; }

        public IList CurrentItems { get; private set; }

        public IList NewItems { get; private set; }

        public IList OldItems { get; private set; }

        public int NewStartingIndex { get; private set; } = -1;

        public int OldStartingIndex { get; private set; } = -1;


        //  METHODS

        #region BASE CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtNotifyCollectionChangedEventArgs class constructor. </summary>
        /// <param name="baseEventArgs"> Notify collection changed event arguments. </param>
        /// <param name="currentItems"> Current collection items. </param>
        /// <param name="autosave"> Auto save option. </param>
        public ExtNotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs baseEventArgs,
            IList currentItems, bool autosave)
        {
            Action = baseEventArgs.Action;
            AutoSave = autosave;
            CurrentItems = currentItems;
            NewItems = baseEventArgs.NewItems;
            OldItems = baseEventArgs.OldItems;
            NewStartingIndex = baseEventArgs.NewStartingIndex;
            OldStartingIndex = baseEventArgs.OldStartingIndex;
        }

        #endregion BASE CLASS METHODS

    }
}
