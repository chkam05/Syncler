using chkam05.Tools.ControlsEx.Colors;
using chkam05.Tools.ControlsEx.Events;
using Syncler.Data.Configuration;
using Syncler.Pages.Base;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class AppearanceSettingsPage : BasePage
    {

        //  VARIABLES

        private ObservableCollection<AppearanceThemeType> _appearanceThemeTypesCollection;
        private ObservableCollection<ColorPaletteItem> _palleteColorsCollection;

        public ConfigManager ConfigManager { get; private set; }


        //  GETTERS & SETTERS

        public ObservableCollection<AppearanceThemeType> AppearanceThemeTypesCollection
        {
            get => _appearanceThemeTypesCollection;
            private set
            {
                _appearanceThemeTypesCollection = value;
                _appearanceThemeTypesCollection.CollectionChanged +=
                   (s, e) => { OnPropertyChanged(nameof(AppearanceThemeTypesCollection)); };
                OnPropertyChanged(nameof(AppearanceThemeTypesCollection));
            }
        }

        public ObservableCollection<ColorPaletteItem> PalleteColorsCollection
        {
            get => _palleteColorsCollection;
            set
            {
                _palleteColorsCollection = value;
                _palleteColorsCollection.CollectionChanged +=
                   (s, e) => { OnPropertyChanged(nameof(PalleteColorsCollection)); };
                OnPropertyChanged(nameof(PalleteColorsCollection));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AppearanceSettingsPage class constructor. </summary>
        /// <param name="pagesManager"> Pages Manager. </param>
        public AppearanceSettingsPage(PagesManager pagesManager) : base(pagesManager)
        {
            //  Setup components.
            ConfigManager = ConfigManager.Instance;

            //  Setup data.
            SetupData();

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing color in Appearance colors palette. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Colors Palette Selection Changed Event Arguments. </param>
        private void AppearanceColorsPaletteEx_ColorSelectionChanged(object sender, ColorsPaletteSelectionChangedEventArgs e)
        {
            if (e?.SelectedColorItem != null)
            {
                ConfigManager.Instance.AppearanceAccentColor = e.SelectedColorItem.Color;
                ConfigManager.Instance.AppearanceColorsList = PalleteColorsCollection
                    .Select(c => new AppearanceColor(c)).ToList();
            }
        }

        #endregion INTERACTION METHODS

        #region PAGES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after unloading page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BasePage_Unloaded(object sender, RoutedEventArgs e)
        {
            ConfigManager.SaveSettings();
        }

        #endregion PAGES METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data collections. </summary>
        private void SetupData()
        {
            AppearanceThemeTypesCollection = new ObservableCollection<AppearanceThemeType>(
                EnumHelper.GetEnumValues<AppearanceThemeType>());

            var colorsList = ConfigManager.Instance?.AppearanceColorsList;

            PalleteColorsCollection = new ObservableCollection<ColorPaletteItem>(
                (colorsList != null && colorsList.Any()
                    ? colorsList
                    : AppearanceConfig.Default.AppearanceColorsList)
                .Select(c => c.ToColorPaletteItem()));
        }

        #endregion SETUP METHODS

    }
}
