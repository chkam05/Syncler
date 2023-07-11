using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Logs
{
    public class Logger : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private static Logger _instance;
        private static object _instanceLock = new object();
        private ObservableCollection<Log> _logsCollection;


        //  GETTERS & SETTERS

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                            _instance = new Logger();
                    }
                }

                return _instance;
            }
        }

        public ObservableCollection<Log> LogsCollection
        {
            get => _logsCollection;
            set
            {
                _logsCollection = value;
                _logsCollection.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(LogsCollection)); };
                OnPropertyChanged(nameof(LogsCollection));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Logger private singleton class constructor. </summary>
        private Logger()
        {
            LogsCollection = new ObservableCollection<Log>();
        }

        #endregion CLASS METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }
}
