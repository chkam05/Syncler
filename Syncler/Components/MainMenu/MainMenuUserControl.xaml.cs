using chkam05.Tools.ControlsEx;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Syncler.Delegates;

namespace Syncler.Components.MainMenu
{
    public partial class MainMenuUserControl : UserControl, INotifyPropertyChanged
    {

        //  CONST

        public const double DEFAULT_EXPANDED_WIDTH = 192d;
        public const double DEFAULT_COLLAPSED_WIDTH = 44d;


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ExpandedWidthProperty = DependencyProperty.Register(
            nameof(ExpandedWidth),
            typeof(double),
            typeof(MainMenuUserControl),
            new FrameworkPropertyMetadata(
                DEFAULT_EXPANDED_WIDTH,
                new PropertyChangedCallback(OnExpandedWidthPropertyChanged)));

        public static readonly DependencyProperty CollapsedWidthProperty = DependencyProperty.Register(
            nameof(CollapsedWidth),
            typeof(double),
            typeof(MainMenuUserControl),
            new FrameworkPropertyMetadata(
                DEFAULT_COLLAPSED_WIDTH,
                new PropertyChangedCallback(OnCollapsedWidthPropertyChanged)));

        public static readonly DependencyProperty ExpandOnMouseEnterProperty = DependencyProperty.Register(
            nameof(ExpandOnMouseEnter),
            typeof(bool),
            typeof(MainMenuUserControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty CollapseOnMouseLeaveProperty = DependencyProperty.Register(
            nameof(CollapseOnMouseLeave),
            typeof(bool),
            typeof(MainMenuUserControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty MenuBackItemVisibleProperty = DependencyProperty.Register(
            nameof(MenuBackItemVisible),
            typeof(bool),
            typeof(MainMenuUserControl),
            new FrameworkPropertyMetadata(
                false,
                new PropertyChangedCallback(OnMenuBackItemVisiblePropertyChanged)));

        public static readonly DependencyProperty MenuHeaderItemVisibleProperty = DependencyProperty.Register(
            nameof(MenuHeaderItemVisible),
            typeof(bool),
            typeof(MainMenuUserControl),
            new FrameworkPropertyMetadata(
                true,
                new PropertyChangedCallback(OnHeaderItemVisiblePropertyChanged)));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;
        public event MainMenuItemSelectEventHandler OnBackItemSelect;
        public event MainMenuItemSelectEventHandler OnHeaderItemSelect;


        //  VARIABLES

        private bool _expandCollapseAnimationStarted = false;
        private bool _expanded = false;
        private double _expandMenuStoryboardTargetWidth = DEFAULT_COLLAPSED_WIDTH;

        private ObservableCollection<MainMenuItem> _menuItems;

        private MainMenuItem _menuBackItem;
        private MainMenuItem _menuHeaderItem;


        //  GETTERS & SETTERS

        public double ExpandMenuStoryboardTargetWidth
        {
            get => _expandMenuStoryboardTargetWidth;
            set
            {
                _expandMenuStoryboardTargetWidth = value;
                OnPropertyChanged(nameof(ExpandMenuStoryboardTargetWidth));
            }
        }

        public double ExpandedWidth
        {
            get => (double)GetValue(ExpandedWidthProperty);
            set
            {
                SetValue(ExpandedWidthProperty, value);
                OnPropertyChanged(nameof(ExpandedWidth));
            }
        }

        public double CollapsedWidth
        {
            get => (double)GetValue(CollapsedWidthProperty);
            set
            {
                SetValue(CollapsedWidthProperty, value);
                OnPropertyChanged(nameof(CollapsedWidth));
            }
        }

        public bool ExpandOnMouseEnter
        {
            get => (bool)GetValue(ExpandOnMouseEnterProperty);
            set
            {
                SetValue(ExpandOnMouseEnterProperty, value);
                OnPropertyChanged(nameof(ExpandOnMouseEnter));
            }
        }

        public bool CollapseOnMouseLeave
        {
            get => (bool)GetValue(CollapseOnMouseLeaveProperty);
            set
            {
                SetValue(CollapseOnMouseLeaveProperty, value);
                OnPropertyChanged(nameof(CollapseOnMouseLeave));
            }
        }

        public ObservableCollection<MainMenuItem> MenuItems
        {
            get => _menuItems;
            private set
            {
                _menuItems = value;
                _menuItems.CollectionChanged += OnMenuItemsCollectionChanged;
                OnPropertyChanged(nameof(MenuItems));
            }
        }

        public bool MenuBackItemVisible
        {
            get => (bool)GetValue(MenuBackItemVisibleProperty);
            set
            {
                SetValue(MenuBackItemVisibleProperty, value);
                OnPropertyChanged(nameof(MenuBackItemVisible));
            }
        }

        public bool MenuHeaderItemVisible
        {
            get => (bool)GetValue(MenuHeaderItemVisibleProperty);
            set
            {
                SetValue(MenuHeaderItemVisibleProperty, value);
                OnPropertyChanged(nameof(MenuHeaderItemVisible));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MainMenuUserControl class constructor. </summary>
        public MainMenuUserControl()
        {
            //  Setup data.
            SetupMainMenu();

            //  Initialize user interface.
            InitializeComponent();

            //  Start as collapsed menu.
            mainMenuBorder.Width = CollapsedWidth;
        }

        #endregion CLASS METHODS

        #region ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Run expand main menu animation. </summary>
        private void ExpandMainMenu()
        {
            ExpandMenuStoryboardTargetWidth = ExpandedWidth;
            Storyboard storyboard = Resources["ExpandCollapseMainMenuStoryboard"] as Storyboard;
            _expandCollapseAnimationStarted = true;
            storyboard?.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Run collapse main menu animation. </summary>
        private void CollapseMainMenu()
        {
            ExpandMenuStoryboardTargetWidth = CollapsedWidth;
            Storyboard storyboard = Resources["ExpandCollapseMainMenuStoryboard"] as Storyboard;
            _expandCollapseAnimationStarted = true;
            storyboard?.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after expand/collapse animation finishes. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Event Arguments. </param>
        private void ExpandCollapseMainMenuStoryboard_Completed(object sender, EventArgs e)
        {
            _expanded = !_expanded;
            _expandCollapseAnimationStarted = false;
        }

        #endregion ANIMATION METHODS

        #region COMPONENT INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after moving cursor over main menu. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void MainMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ExpandOnMouseEnter && !_expanded)
                ExpandMainMenu();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after moving cursor out from main menu. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void MainMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            if (CollapseOnMouseLeave && _expanded)
                CollapseMainMenu();
        }

        #endregion COMPONENT INTERACTION METHODS

        #region MENU ITEMS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting main menu item. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Selection Changed Event Arguments. </param>
        private void MainMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListViewEx)sender;
            var selectedItem = listView.SelectedItem;

            if (selectedItem != null)
            {
                var menuItem = (MainMenuItem)selectedItem;

                if (menuItem != null)
                    menuItem.InvokeAction();

                listView.SelectedItem = null;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting header menu item. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Event Arguments. </param>
        private void OnBackItemSelected(object sender, EventArgs e)
        {
            OnBackItemSelect?.Invoke(sender, new MainMenuItemSelectEventArgs((MainMenuItem)sender));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting header menu item. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Event Arguments. </param>
        private void OnHeaderItemSelected(object sender, EventArgs e)
        {
            if (!_expandCollapseAnimationStarted)
            {
                if (_expanded)
                    CollapseMainMenu();
                else
                    ExpandMainMenu();
            }

            OnHeaderItemSelect?.Invoke(sender, new MainMenuItemSelectEventArgs((MainMenuItem)sender));
        }

        #endregion MENU ITEMS INTERACTION METHODS

        #region MENU ITEMS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add item to main menu. </summary>
        /// <param name="menuItem"> Main menu item. </param>
        public void AddMenuItem(MainMenuItem menuItem)
        {
            if (menuItem != null && !MenuItems.Contains(menuItem))
                MenuItems.Add(menuItem);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add items to main menu. </summary>
        /// <param name="menuItems"> List of main menu items to add. </param>
        public void AddMenuItems(List<MainMenuItem> menuItems)
        {
            if (menuItems != null)
                menuItems.ForEach(i => AddMenuItem(i));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove item from main menu. </summary>
        /// <param name="menuItem"> Menu item to remove. </param>
        public void RemoveItem(MainMenuItem menuItem)
        {
            if (MenuItems.Contains(menuItem) && menuItem != _menuHeaderItem && menuItem != _menuBackItem)
                MenuItems.Remove(menuItem);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove items from main menu. </summary>
        /// <param name="menuItems"> List of menu items to remove. </param>
        public void RemoveItems(List<MainMenuItem> menuItems)
        {
            if (menuItems != null)
                menuItems.ForEach(i => RemoveItem(i));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clear main menu. </summary>
        public void ClearItems()
        {
            while (MenuItems.Count > 2)
                MenuItems.RemoveAt(2);
        }

        #endregion MENU ITEMS MANAGEMENT METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after updating value in Expanded Width Property. </summary>
        /// <param name="sender"> Dependency object from which method has been invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void OnExpandedWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var mainMenu = sender as MainMenuUserControl;

            if (mainMenu != null)
            {
                if (mainMenu._expanded && !mainMenu._expandCollapseAnimationStarted)
                    mainMenu.Width = mainMenu.ExpandedWidth;

                else if (!mainMenu._expanded && mainMenu._expandCollapseAnimationStarted)
                    mainMenu.ExpandMenuStoryboardTargetWidth = mainMenu.ExpandedWidth;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after updating value in Collapsed Width Property. </summary>
        /// <param name="sender"> Dependency object from which method has been invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void OnCollapsedWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var mainMenu = sender as MainMenuUserControl;

            if (mainMenu != null)
            {
                if (!mainMenu._expanded && !mainMenu._expandCollapseAnimationStarted)
                    mainMenu.Width = mainMenu.CollapsedWidth;

                else if (mainMenu._expanded && mainMenu._expandCollapseAnimationStarted)
                    mainMenu.ExpandMenuStoryboardTargetWidth = mainMenu.CollapsedWidth;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing elements in menu items collection. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Notify Collection CHanged Event Arguments. </param>
        private void OnMenuItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(MenuItems));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after updating value in Menu Back Item Visible Property. </summary>
        /// <param name="sender"> Dependency object from which method has been invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void OnMenuBackItemVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var mainMenu = sender as MainMenuUserControl;

            if (mainMenu != null)
            {
                if (mainMenu._menuBackItem != null)
                    mainMenu._menuBackItem.Visible = (bool)e.NewValue;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after updating value in Menu Header Item Visible Property. </summary>
        /// <param name="sender"> Dependency object from which method has been invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void OnHeaderItemVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var mainMenu = sender as MainMenuUserControl;

            if (mainMenu != null)
            {
                if (mainMenu._menuHeaderItem != null)
                    mainMenu._menuHeaderItem.Visible = (bool)e.NewValue;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler event. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup main menu default items and items container. </summary>
        private void SetupMainMenu()
        {
            _menuBackItem = new MainMenuItem("Back", PackIconKind.ArrowLeft, OnBackItemSelected)
            {
                Visible = MenuBackItemVisible
            };

            _menuHeaderItem = new MainMenuItem("Main Menu", PackIconKind.HamburgerMenu, OnHeaderItemSelected)
            {
                Visible = MenuHeaderItemVisible
            };

            var menuItems = new List<MainMenuItem>()
            {
                _menuBackItem,
                _menuHeaderItem
            };

            MenuItems = new ObservableCollection<MainMenuItem>(menuItems);
        }

        #endregion SETUP METHODS

    }
}
