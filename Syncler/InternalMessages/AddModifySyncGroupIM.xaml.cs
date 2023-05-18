using chkam05.Tools.ControlsEx;
using chkam05.Tools.ControlsEx.Data;
using chkam05.Tools.ControlsEx.InternalMessages;
using Syncler.Data.Configuration;
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
        private SyncGroup _syncGroup;
        private List<string> _syncGroupNames;


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

        public SyncGroup SyncGroup
        {
            get => _syncGroup;
            set
            {
                _syncGroup = value ?? new SyncGroup();
                OnPropertyChanged(nameof(SyncGroup));

                Title = value != null ? "Edit sync group" : "Add sync group";
            }
        }

        public List<string> SyncGroupNames
        {
            get => _syncGroupNames;
            set
            {
                _syncGroupNames = value;
                OnPropertyChanged(nameof(SyncGroupNames));
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
                    if (!SyncGroup.ValidateGroupItemPath(efs.FilePath, out string errorMessage))
                    {
                        ErrorMessage = errorMessage;
                        return;
                    }

                    SyncGroup.Items.Add(new SyncGroupItem(SyncGroup.Id) { Path = efs.FilePath });
                }
            };

            InternalMessagesHelper.ApplyVisualStyle(imFilesSelector);
            _parentContainer.ShowMessage(imFilesSelector);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking remove catalog button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void RemoveCatalogButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var source = ((e.Source as ButtonEx)?.DataContext as SyncGroupItem);

            if (source is SyncGroupItem syncGroupItem && SyncGroup.Items.Contains(syncGroupItem))
                SyncGroup.Items.Remove(syncGroupItem);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking ok button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        protected override void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (!SyncGroup.ValidateGroup(out string errorMessage, SyncGroupNames))
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
