// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/21 13:25

using Holoville.HOEditorGUIFramework.Core;
using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorGUIFramework
{
    /// <summary>
    /// Collection of various GUIStyles.
    /// </summary>
    public static class HOGUIStyle
    {
        /// <summary>
        /// Width of icon buttons.
        /// </summary>
        public const int IconButtonWidth = 20;
        /// <summary>
        /// TRUE if editor is using pro skin.
        /// </summary>
        public static readonly bool IsProSkin;

        /// <summary>
        /// Editor-style skinned element.
        /// </summary>
        public static GUIStyle label,
                               labelBold,
                               centeredLabel,
                               centeredLabelBold,
                               wordWrapLabel,
                               wordWrapLabelBold,
                               wordWrapCenteredLabel,
                               wordWrapCenteredLabelBold,
                               miniLabel,
                               miniLabelBold,
                               wordWrapMiniLabel,
                               wordWrapMiniLabelBold,
                               wordWrapCenteredMiniLabel,
                               wordWrapCenteredMiniLabelBold,
                               prefixLabel,
                               button,
                               iconButton,
                               miniButton,
                               toolbar,
                               toolbarLabel,
                               toolbarLabelBold,
                               toolbarButton,
                               toolbarIconButton,
                               toolbarIconButtonBold,
                               toolbarTextField,
                               menubar,
                               menubarLabel,
                               menubarLabelBold,
                               menubarTitle,
                               menubarTitleBold,
                               menubarButton,
                               menubarIconButton,
                               emptyBox,
                               miniBox,
                               blankBox,
                               rowBox,
                               miniRowBox;

        internal static readonly Color SubpanelBgColor = new Color(0.15f, 0.15f, 0.15f, 1);

        internal static GUIStyle dividerBox,
                                 flatDividerBox,
                                 toolbarToggleButtonOn,
                                 toolbarToggleButtonOff,
                                 foldoutClosedButton,
                                 foldoutOpenButton,
                                 foldoutClosedButtonWLabel,
                                 foldoutOpenButtonWLabel,
                                 foldoutClosedButtonWMiniLabel,
                                 foldoutOpenButtonWMiniLabel,
                                 subpanelBox;

        // ***********************************************************************************
        // CONSTRUCTOR
        // ***********************************************************************************

        static HOGUIStyle()
        {
            IsProSkin = EditorGUIUtility.isProSkin;
            StoreStyles();
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        static void StoreStyles()
        {
            // General labels ///////////////////////////////////////
            RectOffset labelRectOffset = new RectOffset(1, 2, 1, 1);
            label = new GUIStyle(EditorStyles.label) { stretchWidth = false, padding = labelRectOffset };
            labelBold = new GUIStyle(label) { font = EditorStyles.boldFont };
            centeredLabel = new GUIStyle(label) { alignment = TextAnchor.MiddleCenter, stretchWidth = true };
            centeredLabelBold = new GUIStyle(centeredLabel) { font = EditorStyles.boldFont };
            wordWrapLabel = new GUIStyle(EditorStyles.wordWrappedLabel) { padding = labelRectOffset };
            wordWrapLabelBold = new GUIStyle(wordWrapLabel) { padding = labelRectOffset, font = EditorStyles.boldFont };
            wordWrapCenteredLabel = new GUIStyle(centeredLabel) { wordWrap = true};
            wordWrapCenteredLabelBold = new GUIStyle(wordWrapCenteredLabel) { font = EditorStyles.boldFont };
            labelRectOffset.bottom = 0;
            miniLabel = new GUIStyle(EditorStyles.miniLabel) { stretchWidth = false, padding = labelRectOffset };
            miniLabelBold = new GUIStyle(miniLabel) { font = EditorStyles.miniBoldFont };
            wordWrapMiniLabel = new GUIStyle(EditorStyles.wordWrappedMiniLabel) { padding = labelRectOffset };
            wordWrapMiniLabelBold = new GUIStyle(wordWrapMiniLabel) { padding = labelRectOffset, font = EditorStyles.miniBoldFont };
            wordWrapCenteredMiniLabel = new GUIStyle(wordWrapMiniLabel) { font = EditorStyles.miniFont, alignment = TextAnchor.MiddleCenter, stretchWidth = true } ;
            wordWrapCenteredMiniLabelBold = new GUIStyle(wordWrapCenteredMiniLabel) { font = EditorStyles.miniBoldFont };
            prefixLabel = new GUIStyle(miniLabelBold) { alignment = TextAnchor.MiddleRight };

            // Buttons //////////////////////////////////////////////
            button = new GUIStyle(GUI.skin.button) { margin = new RectOffset(2, 4, 1, 1) };
            iconButton = new GUIStyle(button) { fixedWidth = IconButtonWidth };
            iconButton.padding.left -= 5;
            iconButton.padding.top -= 1;
            iconButton.padding.bottom += 1;
            miniButton = new GUIStyle(EditorStyles.miniButton);

            // Special buttons //////////////////////////////////////
            // Foldout
            GUIStyle defFoldout = new GUIStyle(EditorStyles.foldout);
            foldoutClosedButton = new GUIStyle(GUI.skin.button) {
                alignment = TextAnchor.UpperLeft, active = { background = null }, fixedWidth = 14,
                normal = { background = defFoldout.normal.background }, border = defFoldout.border
            };
            foldoutClosedButton.padding.left += 8;
            foldoutClosedButton.padding.top -= 2;
            foldoutClosedButton.margin.top -= IsProSkin ? 2 : 1;
            foldoutOpenButton = new GUIStyle(foldoutClosedButton) { normal = { background = defFoldout.onNormal.background } };
            foldoutClosedButtonWLabel = new GUIStyle(foldoutClosedButton) { fixedWidth = 0, stretchWidth = false };
            foldoutOpenButtonWLabel = new GUIStyle(foldoutOpenButton) { fixedWidth = 0, stretchWidth = false };
            foldoutClosedButtonWMiniLabel = new GUIStyle(foldoutClosedButtonWLabel) {
                font = EditorStyles.miniFont, padding = { top = 2 } };
            foldoutOpenButtonWMiniLabel = new GUIStyle(foldoutOpenButtonWLabel) {
                font = EditorStyles.miniFont, padding = { top = 2 } };
            // Toggle
            toolbarToggleButtonOn = new GUIStyle(EditorStyles.toggle) {
                padding = new RectOffset(4, 4, 4, 4),
                margin = new RectOffset(0, 0, 3, 0),
                fixedWidth = HOGUITexture.CheckboxOnFree.width,
                fixedHeight = HOGUITexture.CheckboxOnFree.height,
                border = new RectOffset(0, 0, 0, 0),
                overflow = new RectOffset(0, 0, 0, 0),
                imagePosition = ImagePosition.ImageOnly
            };
            toolbarToggleButtonOn.SetBackground(HOGUITexture.checkboxOn, true, true);
            toolbarToggleButtonOff = new GUIStyle(toolbarToggleButtonOn);
            toolbarToggleButtonOff.SetBackground(HOGUITexture.checkboxOff, true, true);

            // Controls /////////////////////////////////////////////
            toolbarTextField = new GUIStyle(EditorStyles.toolbarTextField);

            // Bars /////////////////////////////////////////////////
            // Toolbar
            toolbar = new GUIStyle(EditorStyles.toolbar);
            toolbarLabel = new GUIStyle(EditorStyles.miniLabel) { padding = { top = -1, bottom = 0 }, stretchWidth = false };
            toolbarLabelBold = new GUIStyle(toolbarLabel) { font = EditorStyles.miniBoldFont };
            toolbarButton = new GUIStyle(EditorStyles.toolbarButton) {
                fixedHeight = 0, stretchHeight = true, padding = { left = 6, right = 9, top = -2 }
            };
            toolbarIconButton = new GUIStyle(toolbarButton) { fixedWidth = IconButtonWidth, padding = { left = 2, right = 5 } };
            toolbarIconButtonBold = new GUIStyle(toolbarIconButton) { font = EditorStyles.miniBoldFont, padding = { right = 3 } };
            // Menubar
            const int toolbarDiffH = 8;
            const int toolbarDiffHHalf = (int)(toolbarDiffH * 0.5f);
            menubar = new GUIStyle(toolbar) { padding = new RectOffset(0, 0, 0, 0) };
            menubar.fixedHeight += toolbarDiffH;
            menubarLabel = new GUIStyle(toolbarLabel);
            menubarLabel.padding.top += toolbarDiffHHalf;
            menubarLabelBold = new GUIStyle(menubarLabel) { font = EditorStyles.miniBoldFont };
            menubarTitle = new GUIStyle(menubarLabel) { font = EditorStyles.standardFont };
            menubarTitle.padding.top -= 2;
            menubarTitleBold = new GUIStyle(menubarTitle) { font = EditorStyles.boldFont };
            menubarButton = new GUIStyle(toolbarButton);
            menubarIconButton = new GUIStyle(toolbarIconButton);
            menubarIconButton.fixedWidth += toolbarDiffH;
            menubarIconButton.padding.left += toolbarDiffHHalf;
            menubarIconButton.padding.right += toolbarDiffHHalf;

            // Boxes ////////////////////////////////////////////////
            miniBox = new GUIStyle(GUI.skin.box) { font = EditorStyles.miniFont, padding = new RectOffset(3, 5, 4, 4) };
            rowBox = new GUIStyle(GUI.skin.box)
                { margin = new RectOffset(0, 0, 0, 0), padding = new RectOffset(5, 6, 3, 3), overflow = new RectOffset(0, 1, 0, 1) };
            miniRowBox = new GUIStyle(rowBox) { font = EditorStyles.miniFont };
            blankBox = new GUIStyle(GUI.skin.box) { margin = new RectOffset(0, 0, 0, 0), padding = new RectOffset(0, 0, 0, 0) };
            blankBox.SetBackground(EditorGUIUtility.whiteTexture);
            emptyBox = new GUIStyle(blankBox);
            emptyBox.SetBackground(null);
            subpanelBox = new GUIStyle(blankBox) { padding = new RectOffset(6, 6, 10, 6) };
            dividerBox = new GUIStyle(EditorStyles.toolbar) { margin = new RectOffset(0, 0, 0, 0), padding = new RectOffset(0, 0, 0, 0), fixedHeight = 1 };
            flatDividerBox = new GUIStyle(blankBox) { fixedHeight = 1 };
        }
    }
}