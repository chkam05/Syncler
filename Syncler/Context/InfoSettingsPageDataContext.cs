using Syncler.Pages.Base;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Syncler.Context
{
    public class InfoSettingsPageDataContext : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _title;
        private string _name;
        private string _version;
        private string _description;
        private string _company;
        private string _copyright;
        private string _location;


        //  GETTERS & SETTERS

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Company
        {
            get => _company;
            set
            {
                _company = value;
                OnPropertyChanged(nameof(Company));
            }
        }

        public string Copyright
        {
            get => _copyright;
            set
            {
                _copyright = value;
                OnPropertyChanged(nameof(Copyright));
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InfoSettingsPageDataContext class constructor. </summary>
        public InfoSettingsPageDataContext()
        {
            Title = ApplicationHelper.GetApplicationTitle();
            Name = ApplicationHelper.GetApplicationName();
            Version = ApplicationHelper.GetApplicationVersion().ToString();
            Description = ApplicationHelper.GetApplicationDescription();
            Company = ApplicationHelper.GetApplicationCompany();
            Copyright = ApplicationHelper.GetApplicationCopyright();
            Location = ApplicationHelper.GetApplicationLocation();
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
