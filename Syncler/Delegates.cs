using Syncler.Components.MainMenu;
using Syncler.Data.Events;
using Syncler.Pages.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncler
{
    public static class Delegates
    {

        //  DATA

        public delegate void ErrorRelayEventHandler(object sender, ErrorRelayEventArgs e);
        public delegate void ExtNotifyCollectionChangedEventHandler(object sender, ExtNotifyCollectionChangedEventArgs e);


        //  MENUS

        public delegate void MainMenuItemSelectEventHandler(object sender, MainMenuItemSelectEventArgs e);


        //  PAGES

        public delegate void PagesManagerNavigatedEventHandler(object sender, OnPageLoadedEventArgs e);

    }
}
