using chkam05.Tools.ControlsEx;
using chkam05.Tools.ControlsEx.Data;
using chkam05.Tools.ControlsEx.InternalMessages;
using Syncler.Data.Synchronisation;
using Syncler.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Syncler.InternalMessages
{
    public partial class AddModifySyncGroupIM : StandardInternalMessageEx
    {

        


        //  VARIABLES

        private string _errorMessage = null;
        private GroupConfig _groupConfig;
        private List<string> _groupConfigNames;


        //  GETTERS & SETTERS

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

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

        public List<string> GroupConfigNames
        {
            get => _groupConfigNames;
            set
            {
                _groupConfigNames = value;
                OnPropertyChanged(nameof(GroupConfigNames));
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
            var imFilesSelector = FilesSelectorInternalMessageEx.CreateSelectDirectoryInternalMessageEx(_parentContainer);

            imFilesSelector.AllowCreate = false;
            imFilesSelector.InitialDirectory = imFilesSelector.CurrentDirectory;
            imFilesSelector.OnClose += (s, efs) =>
            {
                if (efs.Result == InternalMessageResult.Ok && Directory.Exists(efs.FilePath))
                {
                    if (!GroupConfig.ValidateGroupItemPath(efs.FilePath, out string errorMessage))
                    {
                        ErrorMessage = errorMessage;
                        return;
                    }

                    GroupConfig.Items.Add(new GroupItem() { Path = efs.FilePath });
                }
            };

            InternalMessagesHelper.ApplyVisualStyle(imFilesSelector);

            _parentContainer.ShowMessage(imFilesSelector);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking ok button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        protected override void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (!GroupConfig.ValidateGroup(out string errorMessage, GroupConfigNames))
            {
                ErrorMessage = errorMessage;
                return;
            }

            base.OnOkClick(sender, e);
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
