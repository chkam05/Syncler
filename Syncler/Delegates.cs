using Syncler.Components.MainMenu;
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

        //  MENUS

        public delegate void MainMenuItemSelectEventHandler(object sender, MainMenuItemSelectEventArgs e);


        //  PAGES

        public delegate void PagesManagerNavigatedEventHandler(object sender, OnPageLoadedEventArgs e);

    }
}
