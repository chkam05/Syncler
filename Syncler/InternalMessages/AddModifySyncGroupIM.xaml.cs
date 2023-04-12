using chkam05.Tools.ControlsEx;
using chkam05.Tools.ControlsEx.Data;
using chkam05.Tools.ControlsEx.InternalMessages;
using MaterialDesignThemes.Wpf;
using Syncler.Data.Synchronisation;
using System;
using System.Collections.Generic;
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

namespace Syncler.InternalMessages
{
    public partial class AddModifySyncGroupIM : StandardInternalMessageEx
    {

        //  VARIABLES

        private GroupConfig _groupConfig;


        //  GETTERS & SETTERS

        public GroupConfig GroupConfig
        {
            get => _groupConfig;
            set
            {
                _groupConfig = value ?? new GroupConfig();
                OnPropertyChanged(nameof(GroupConfig));

                Title = value != null ? "Edit sync group" : "Add sync group";
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> DatabaseProfileEditorIM class constructor. </summary>
        /// <param name="parentContainer"> Parent InternalMessagesEx container. </param>
        public AddModifySyncGroupIM(InternalMessagesExContainer parentContainer) : base(parentContainer)
        {
            //  Initialize user interface.
            InitializeComponent();

            //  Interface configuration.
            Buttons = new InternalMessageButtons[]
            {
                InternalMessageButtons.OkButton,
                InternalMessageButtons.CancelButton
            };
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking add catalog button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddCatalogButtonEx_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion INTERACTION METHODS

        #region TEMPLATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> When overridden in a derived class,cis invoked whenever 
        /// application code or internal processes call ApplyTemplate. </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ButtonEx okButton = GetButtonEx("okButton");
            ButtonEx cancelButton = GetButtonEx("cancelButton");

            if (okButton != null)
                okButton.Content = "Save";

            if (cancelButton != null)
                cancelButton.Content = "Cancel";
        }

        #endregion TEMPLATE METHODS

    }
}
