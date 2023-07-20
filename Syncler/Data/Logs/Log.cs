using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Data.Logs
{
    public class Log : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private DateTime _dateTime;
        private string _action;
        private string _context;
        private string _message;


        //  GETTERS & SETTERS
        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                _dateTime = value;
                OnPropertyChanged(nameof(DateTime));
                OnPropertyChanged(nameof(DateTimeStr));
            }
        }

        public string DateTimeStr
        {
            get => DateTime.ToString("yyyy-MM-dd hh:mm:ss");
        }

        public string Action
        {
            get => _action;
            set
            {
                _action = value;
                OnPropertyChanged(nameof(Action));
            }
        }

        public string Context
        {
            get => _context;
            set
            {
                _context = value;
                OnPropertyChanged(nameof(Context));
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Log class constructor. </summary>
        /// <param name="dateTime"> Date time. </param>
        /// <param name="message"> Message. </param>
        /// <param name="action"> Action (type of log). </param>
        /// <param name="context"> Context (part of application). </param>
        public Log(DateTime dateTime, string message, string action = "Info", string context = "Application")
        {
            DateTime = dateTime;
            Message = message;
            Action = action;
            Context = context;
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
