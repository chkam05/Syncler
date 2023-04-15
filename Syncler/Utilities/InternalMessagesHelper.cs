using chkam05.Tools.ControlsEx.Events;
using chkam05.Tools.ControlsEx.InternalMessages;
using Syncler.Data.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Syncler.Utilities
{
    public static class InternalMessagesHelper
    {

        //  METHODS

        #region VISUAL STYLES

        //  --------------------------------------------------------------------------------
        /// <summary> Apply visual styles for base files selector InternalMessageEx. </summary>
        /// <param name="internalMessage"> Base files selector InternalMessageEx. </param>
        public static void ApplyVisualStyle(BaseFilesSelectorInternalMessageEx internalMessage)
        {
            var configManager = ConfigManager.Instance;

            ApplyVisualStyle(internalMessage, configManager);

            internalMessage.TextBoxMouseOverBackground = configManager.AppearanceAccentMouseOverBrush;
            internalMessage.TextBoxMouseOverBorderBrush = configManager.AppearanceAccentMouseOverBrush;
            internalMessage.TextBoxMouseOverForeground = configManager.AppearanceAccentForegroundBrush;
            internalMessage.TextBoxSelectedBackground = configManager.AppearanceAccentSelectedBrush;
            internalMessage.TextBoxSelectedBorderBrush = configManager.AppearanceAccentSelectedBrush;
            internalMessage.TextBoxSelectedForeground = configManager.AppearanceAccentForegroundBrush;
            internalMessage.TextBoxSelectedTextBackground = configManager.AppearanceAccentForegroundBrush;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Apply visual styles for InternalMessageEx. </summary>
        /// <param name="internalMessage"> InternalMessageEx. </param>
        public static void ApplyVisualStyle(InternalMessageEx internalMessage)
        {
            var configManager = ConfigManager.Instance;

            ApplyVisualStyle(internalMessage, configManager);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Apply visual styles for BaseInternalMessageEx. </summary>
        /// <param name="internalMessage"> BaseInternalMessageEx. </param>
        /// <param name="configManager"> Configuration manager. </param>
        private static void ApplyVisualStyle<T>(BaseInternalMessageEx<T> internalMessage,
            ConfigManager configManager) where T : InternalMessageCloseEventArgs
        {
            internalMessage.Background = configManager.AppearanceThemeBackgroundBrush;
            internalMessage.BorderBrush = configManager.AppearanceThemeShadeBackgroundBrush;
            internalMessage.BottomBackground = configManager.AppearanceThemeShadeBackgroundBrush;
            internalMessage.BottomBorderBrush = configManager.AppearanceAccentColorBrush;
            internalMessage.BottomPadding = new Thickness(8);
            internalMessage.ButtonBackground = configManager.AppearanceAccentColorBrush;
            internalMessage.ButtonBorderBrush = configManager.AppearanceAccentColorBrush;
            internalMessage.ButtonForeground = configManager.AppearanceAccentForegroundBrush;
            internalMessage.ButtonMouseOverBackground = configManager.AppearanceAccentMouseOverBrush;
            internalMessage.ButtonMouseOverBorderBrush = configManager.AppearanceAccentMouseOverBrush;
            internalMessage.ButtonMouseOverForeground = configManager.AppearanceAccentForegroundBrush;
            internalMessage.ButtonPressedBackground = configManager.AppearanceAccentPressedBrush;
            internalMessage.ButtonPressedBorderBrush = configManager.AppearanceAccentPressedBrush;
            internalMessage.ButtonPressedForeground = configManager.AppearanceAccentForegroundBrush;
            internalMessage.ButtonPressedForeground = configManager.AppearanceAccentForegroundBrush;
            internalMessage.Foreground = configManager.AppearanceThemeForegroundBrush;
            internalMessage.HeaderBackground = configManager.AppearanceThemeShadeBackgroundBrush;
            internalMessage.HeaderBorderBrush = configManager.AppearanceAccentColorBrush;
            internalMessage.HeaderPadding = new Thickness(8);
            internalMessage.Padding = new Thickness(0);
            internalMessage.UseCustomSectionBreaksBorderBrush = true;
        }

        #endregion VISUAL STYLES

    }
}
