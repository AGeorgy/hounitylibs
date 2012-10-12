// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/21 11:09

using System;
using System.Collections.Generic;
using Holoville.HOEditorGUIFramework.Core;
using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorGUIFramework
{
    /// <summary>
    /// Various GUI layout methods.
    /// </summary>
    public static class HOGUILayout
    {
        // ACTIONS //////////////////////////////////////////////////////

        static readonly GUILayoutAction _Horizontal =
        (guiStyle, backgroundShade, blockAction, options) => {
            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundShade;
            if (guiStyle == null) GUILayout.BeginHorizontal(options); else GUILayout.BeginHorizontal(guiStyle, options);
            blockAction();
            GUILayout.EndHorizontal();
            GUI.backgroundColor = prevColor;
        };

        static readonly GUILayoutAction _Vertical =
        (guiStyle, backgroundShade, blockAction, options) => {
            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundShade;
            if (guiStyle == null) GUILayout.BeginVertical(options); else GUILayout.BeginVertical(guiStyle, options);
            blockAction();
            GUILayout.EndVertical();
            GUI.backgroundColor = prevColor;
        };

        // VARS /////////////////////////////////////////////////////////

        static int _activePressButtonId = -1;


        // ===================================================================================
        // LAYOUT METHODS -------------------------------------------------------------

        /// <summary>Shows the given block delegate using an horizontal layout.</summary>
        public static void Horizontal(Action blockAction, params GUILayoutOption[] options)
        { Horizontal(null, null, GUI.backgroundColor, blockAction, options); }
        /// <summary>Shows the given block delegate using an horizontal layout.</summary>
        public static void Horizontal(RectOffset padding, Action blockAction, params GUILayoutOption[] options)
        { Horizontal(padding, null, GUI.backgroundColor, blockAction, options); }
        /// <summary>Shows the given block delegate using an horizontal layout.</summary>
        public static void Horizontal(GUIStyle guiStyle, Action blockAction, params GUILayoutOption[] options)
        { Horizontal(null, guiStyle, GUI.backgroundColor, blockAction, options); }
        /// <summary>Shows the given block delegate using an horizontal layout.</summary>
        public static void Horizontal(RectOffset padding, GUIStyle guiStyle, Action blockAction, params GUILayoutOption[] options)
        { Horizontal(padding, guiStyle, GUI.backgroundColor, blockAction, options); }
        /// <summary>Shows the given block delegate using an horizontal layout.</summary>
        public static void Horizontal(Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        { Horizontal(null, null, backgroundShade, blockAction, options); }
        /// <summary>Shows the given block delegate using an horizontal layout.</summary>
        public static void Horizontal(RectOffset padding, Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        { Horizontal(padding, null, backgroundShade, blockAction, options); }
        /// <summary>Shows the given block delegate using an horizontal layout.</summary>
        public static void Horizontal(GUIStyle guiStyle, Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        { Horizontal(null, guiStyle, backgroundShade, blockAction, options); }
        /// <summary>
        /// Shows the given block delegate using an horizontal layout.
        /// </summary>
        /// <example><code>HOGUILayout.Horizontal(myGUIStyle, () => { block code here; };</code></example>
        public static void Horizontal(RectOffset padding, GUIStyle guiStyle, Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        {
            GUIStyle style;
            if (padding != null) {
                style = guiStyle == null ? new GUIStyle() : new GUIStyle(guiStyle);
                style.padding.left += padding.left;
                style.padding.right += padding.right;
                style.padding.top += padding.top;
                style.padding.bottom += padding.bottom;
            } else {
                style = guiStyle;
            }
            _Horizontal(style, backgroundShade, blockAction, options);
        }

        /// <summary>Shows the given block delegate using a vertical layout.</summary>
        public static void Vertical(Action blockAction, params GUILayoutOption[] options)
        { Vertical(null, null, GUI.backgroundColor, blockAction, options); }
        /// <summary>Shows the given block delegate using a vertical layout.</summary>
        public static void Vertical(RectOffset padding, Action blockAction, params GUILayoutOption[] options)
        { Vertical(padding, null, GUI.backgroundColor, blockAction, options); }
        /// <summary>Shows the given block delegate using a vertical layout.</summary>
        public static void Vertical(GUIStyle guiStyle, Action blockAction, params GUILayoutOption[] options)
        { Vertical(null, guiStyle, GUI.backgroundColor, blockAction, options); }
        /// <summary>Shows the given block delegate using a vertical layout.</summary>
        public static void Vertical(RectOffset padding, GUIStyle guiStyle, Action blockAction, params GUILayoutOption[] options)
        { Vertical(padding, guiStyle, GUI.backgroundColor, blockAction, options); }
        /// <summary>Shows the given block delegate using a vertical layout.</summary>
        public static void Vertical(Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        { Vertical(null, null, backgroundShade, blockAction, options); }
        /// <summary>Shows the given block delegate using a vertical layout.</summary>
        public static void Vertical(RectOffset padding, Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        { Vertical(padding, null, backgroundShade, blockAction, options); }
        /// <summary>Shows the given block delegate using a vertical layout.</summary>
        public static void Vertical(GUIStyle guiStyle, Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        { Vertical(null, guiStyle, backgroundShade, blockAction, options); }
        /// <summary>
        /// Shows the given block delegate using a vertical layout.
        /// </summary>
        /// <example><code>HOGUILayout.Horizontal(myGUIStyle, () => { block code here; });</code></example>
        public static void Vertical(RectOffset padding, GUIStyle guiStyle, Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        {
            GUIStyle style;
            if (padding != null) {
                style = guiStyle == null ? new GUIStyle() : new GUIStyle(guiStyle);
                style.padding.left += padding.left;
                style.padding.right += padding.right;
                style.padding.top += padding.top;
                style.padding.bottom += padding.bottom;
            } else {
                style = guiStyle;
            }
            _Vertical(style, backgroundShade, blockAction, options);
        }

        /// <summary>
        /// Shows the given block delegate in an eventually disabled state.
        /// If the GUI was already disabled, nothing changes.
        /// </summary>
        /// <param name="disabled">If TRUE shows the block as disabled, otherwise as enabled</param>
        /// <param name="blockAction">Block action delegate</param>
        public static void DisabledGroup(bool disabled, Action blockAction)
        { DisabledGroup(disabled, GUI.backgroundColor, blockAction); }
        /// <summary>
        /// Shows the given block delegate in an eventually disabled state.
        /// If the GUI was already disabled, nothing changes.
        /// </summary>
        /// <param name="disabled">If TRUE shows the block as disabled, otherwise as enabled</param>
        /// <param name="backgroundShade">Shade to apply to the background</param>
        /// <param name="blockAction">Block action delegate</param>
        public static void DisabledGroup(bool disabled, Color backgroundShade, Action blockAction)
        {
            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundShade;
            bool wasEnabled = GUI.enabled;
            if (wasEnabled && disabled) GUI.enabled = false;
            blockAction();
            GUI.enabled = wasEnabled;
            GUI.backgroundColor = prevColor;
        }

        /// <summary>
        /// Shows the given block delegate with the given background shade.
        /// </summary>
        /// <param name="backgroundShade">Background shade to apply</param>
        /// <param name="blockAction">Block action delegate</param>
        public static void ShadedGroup(Color backgroundShade, Action blockAction)
        {
            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundShade;
            blockAction();
            GUI.backgroundColor = prevColor;
        }

        /// <summary>
        /// Shows the given block delegate using a menubar layout.
        /// </summary>
        /// <example><code>HOGUILayout.Menubar(() => { block code here; });</code></example>
        public static void Menubar(Action blockAction, params GUILayoutOption[] options)
        { Menubar(-1, GUI.backgroundColor, blockAction, options); }
        /// <summary>
        /// Shows the given block delegate using a menubar layout.
        /// </summary>
        /// <example><code>HOGUILayout.Menubar(() => { block code here; });</code></example>
        public static void Menubar(float height, Action blockAction, params GUILayoutOption[] options)
        { Menubar(height, GUI.backgroundColor, blockAction, options); }
        /// <summary>
        /// Shows the given block delegate using a menubar layout.
        /// </summary>
        /// <example><code>HOGUILayout.Menubar(() => { block code here; });</code></example>
        public static void Menubar(Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        { Menubar(-1, backgroundShade, blockAction, options); }
        /// <summary>
        /// Shows the given block delegate using a menubar layout.
        /// </summary>
        /// <example><code>HOGUILayout.Menubar(() => { block code here; });</code></example>
        public static void Menubar(float height, Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        {
            GUIStyle style;
            if (height <= 0) style = HOGUIStyle.menubar;
            else style = new GUIStyle(HOGUIStyle.menubar) { fixedHeight = height };
            _Horizontal(style, backgroundShade, blockAction, options);
        }

        /// <summary>
        /// Shows the given block delegate using a toolbar layout.
        /// </summary>
        /// <example><code>HOGUILayout.Toolbar(() => { block code here; });</code></example>
        public static void Toolbar(Action blockAction, params GUILayoutOption[] options)
        { Toolbar(-1, GUI.backgroundColor, blockAction, options); }
        /// <summary>
        /// Shows the given block delegate using a toolbar layout.
        /// </summary>
        /// <example><code>HOGUILayout.Toolbar(() => { block code here; });</code></example>
        public static void Toolbar(float height, Action blockAction, params GUILayoutOption[] options)
        { Toolbar(height, GUI.backgroundColor, blockAction, options); }
        /// <summary>
        /// Shows the given block delegate using a toolbar layout.
        /// </summary>
        /// <example><code>HOGUILayout.Toolbar(() => { block code here; });</code></example>
        public static void Toolbar(Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        { Toolbar(-1, backgroundShade, blockAction, options); }
        /// <summary>
        /// Shows the given block delegate using a toolbar layout.
        /// </summary>
        /// <example><code>HOGUILayout.Toolbar(() => { block code here; });</code></example>
        public static void Toolbar(float height, Color backgroundShade, Action blockAction, params GUILayoutOption[] options)
        {
            GUIStyle style;
            if (height <= 0) style = HOGUIStyle.toolbar;
            else style = new GUIStyle(HOGUIStyle.toolbar) { fixedHeight = height};
            _Horizontal(style, backgroundShade, blockAction, options);
        }

        // ===================================================================================
        // SPECIAL LAYOUT METHODS ------------------------------------------------------------

        /// <summary>Creates a drag area using a vertical layout,
        /// and returns TRUE if the drag conditions were met.</summary>
        public static bool DragAndDropArea(bool allowDragAndDrop, DragAndDropVisualMode visualMode, Action blockAction, params GUILayoutOption[] options)
        { return DragAndDropArea(allowDragAndDrop, visualMode, null, blockAction, options); }
        /// <summary>Creates a drag area using a vertical layout,
        /// and returns TRUE if the drag conditions were met.</summary>
        public static bool DragAndDropArea(Type allowedObjectsType, DragAndDropVisualMode visualMode, Action blockAction, params GUILayoutOption[] options)
        { return DragAndDropArea(allowedObjectsType, visualMode, null, blockAction, options); }
        /// <summary>Creates a drag area using a vertical layout,
        /// and returns TRUE if the drag conditions were met.</summary>
        public static bool DragAndDropArea(Type allowedObjectsType, DragAndDropVisualMode visualMode, GUIStyle guiStyle, Action blockAction, params GUILayoutOption[] options)
        {
            bool allowDragAndDrop = true;
            if (Event.current.type == EventType.DragUpdated) {
                UnityEngine.Object[] draggedObjs = DragAndDrop.objectReferences;
                if (draggedObjs.Length == 0) allowDragAndDrop = false;
                int objsCount = draggedObjs.Length;
                for (int i = 0; i < objsCount; ++i) {
                    if (draggedObjs[i].GetType() != allowedObjectsType) {
                        allowDragAndDrop = false;
                        break;
                    }
                }
            }
            return DragAndDropArea(allowDragAndDrop, visualMode, guiStyle, blockAction, options);
        }
        /// <summary>
        /// Creates a drag area using a vertical layout,
        /// and returns TRUE when a drag is concluded (meaning something was released on the area) 
        /// and drag conditions were met.
        /// </summary>
        public static bool DragAndDropArea(bool allowDragAndDrop, DragAndDropVisualMode visualMode, GUIStyle guiStyle, Action blockAction, params GUILayoutOption[] options)
        {
            _Vertical(guiStyle, GUI.backgroundColor, blockAction, options);
            if (allowDragAndDrop && Event.current.type == EventType.DragUpdated && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
                DragAndDrop.visualMode = visualMode;
            } else if (Event.current.type == EventType.DragPerform && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draws a subpanel container with a title.
        /// </summary>
        public static void Subpanel(string title, Action blockAction)
        {
            _Vertical(new GUIStyle(), GUI.backgroundColor, ()=> {
                _Horizontal(HOGUIStyle.subpanelBox, HOGUIStyle.SubpanelBgColor, () => {
                    ColoredLabel(title, HOGUIStyle.label, Color.white);
                    GUILayout.FlexibleSpace();
                }, null);
                blockAction();
            }, null);
        }

        // ===================================================================================
        // GUIELEMENTS METHODS ---------------------------------------------------------------

        // LABELS ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Draws a label with the choice for font color.
        /// </summary>
        public static void ColoredLabel(string text, Color fontColor, params GUILayoutOption[] options)
        { ColoredLabel(new GUIContent(text, ""), null, fontColor, options); }
        /// <summary>
        /// Draws a label with the choice for font color.
        /// </summary>
        public static void ColoredLabel(string text, GUIStyle guiStyle, Color fontColor, params GUILayoutOption[] options)
        { ColoredLabel(new GUIContent(text, ""), guiStyle, fontColor, options); }
        /// <summary>
        /// Draws a label with the choice for font color.
        /// </summary>
        public static void ColoredLabel(GUIContent content, Color fontColor, params GUILayoutOption[] options)
        { ColoredLabel(content, null, fontColor, options); }
        /// <summary>
        /// Draws a label with the choice for font color.
        /// </summary>
        public static void ColoredLabel(GUIContent content, GUIStyle guiStyle, Color fontColor, params GUILayoutOption[] options)
        {
            GUIStyle style = guiStyle == null ? new GUIStyle(GUI.skin.label) : new GUIStyle(guiStyle);
            style.SetFontColor(fontColor);
            GUILayout.Label(content, style, options);
        }

        // BUTTONS ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Draws a single press button with choice for background shade and text color.
        /// </summary>
        public static bool ColoredButton(string text, Color backgroundShade, Color fontColor, params GUILayoutOption[] options)
        { return ColoredButton(new GUIContent(text, ""), GUI.skin.button, backgroundShade, fontColor, options); }
        /// <summary>
        /// Draws a single press button with choice for background shade and text color.
        /// </summary>
        public static bool ColoredButton(GUIContent content, Color backgroundShade, Color fontColor, params GUILayoutOption[] options)
        { return ColoredButton(content, GUI.skin.button, backgroundShade, fontColor, options); }
        /// <summary>
        /// Draws a single press button with choice for background shade and text color.
        /// </summary>
        public static bool ColoredButton(string text, GUIStyle guiStyle, Color backgroundShade, Color fontColor, params GUILayoutOption[] options)
        { return ColoredButton(new GUIContent(text, ""), guiStyle, backgroundShade, fontColor, options); }
        /// <summary>
        /// Draws a single press button with choice for background shade and text color.
        /// </summary>
        public static bool ColoredButton(GUIContent content, GUIStyle guiStyle, Color backgroundShade, Color fontColor, params GUILayoutOption[] options)
        {
            GUIStyle style = new GUIStyle(guiStyle);
            style.SetFontColor(fontColor);
            Color guiPrevColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundShade;
            bool clicked = GUILayout.Button(content, style, options);
            GUI.backgroundColor = guiPrevColor;
            return clicked;
        }

        /// <summary>
        /// Draws a single press button with choice for background shade.
        /// </summary>
        public static bool ShadedButton(string text, Color backgroundShade, params GUILayoutOption[] options)
        { return ColoredButton(new GUIContent(text, ""), GUI.skin.button, backgroundShade, GUI.backgroundColor, options); }
        /// <summary>
        /// Draws a single press button with choice for background shade.
        /// </summary>
        public static bool ShadedButton(GUIContent content, Color backgroundShade, params GUILayoutOption[] options)
        { return ColoredButton(content, GUI.skin.button, backgroundShade, GUI.backgroundColor, options); }
        /// <summary>
        /// Draws a single press button with choice for background shade.
        /// </summary>
        public static bool ShadedButton(string text, GUIStyle guiStyle, Color backgroundShade, params GUILayoutOption[] options)
        { return ColoredButton(new GUIContent(text, ""), guiStyle, backgroundShade, GUI.backgroundColor, options); }
        /// <summary>
        /// Draws a single press button with choice for background shade.
        /// </summary>
        public static bool ShadedButton(GUIContent content, GUIStyle guiStyle, Color backgroundShade, params GUILayoutOption[] options)
        {
            Color guiPrevColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundShade;
            bool clicked = GUILayout.Button(content, guiStyle, options);
            GUI.backgroundColor = guiPrevColor;
            return clicked;
        }

        /// <summary>
        /// Draws a single press toggle button with choice for toggle background shade and text color.
        /// </summary>
        public static bool ToggleButton(bool toggled, string text, GUIStyle guiStyle, Color toggleBackgroundShade, Color toggleFontColor, params GUILayoutOption[] options)
        { return ToggleButton(toggled, new GUIContent(text, ""), guiStyle, toggleBackgroundShade, toggleFontColor, options); }
        /// <summary>
        /// Draws a single press toggle button with choice for toggle background shade and text color.
        /// </summary>
        public static bool ToggleButton(bool toggled, GUIContent content, GUIStyle guiStyle, Color toggleBackgroundShade, Color toggleFontColor, params GUILayoutOption[] options)
        {
            bool clicked = toggled
                ? ColoredButton(content, guiStyle, toggleBackgroundShade, toggleFontColor, options)
                : GUILayout.Button(content, guiStyle, options);
            if (clicked) {
                toggled = !toggled;
                GUI.changed = true;
            }
            return toggled;
        }

        /// <summary>
        /// Draws a button that returns TRUE the first time it's pressed, instead than when its released.
        /// </summary>
        public static bool PressButton(string text, GUIStyle guiStyle, params GUILayoutOption[] options)
        { return PressButton(new GUIContent(text, ""), guiStyle, options); }
        /// <summary>
        /// Draws a button that returns TRUE the first time it's pressed, instead than when its released.
        /// </summary>
        public static bool PressButton(GUIContent content, GUIStyle guiStyle, params GUILayoutOption[] options)
        {
            // NOTE: tried using RepeatButton, but doesn't work if used for dragging
            GUILayout.Button(content, guiStyle, options);
            int controlId = GUIUtility.GetControlID(FocusType.Native);
            int hotControl = GUIUtility.hotControl;
            bool pressed = hotControl > 1 && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
            if (pressed && _activePressButtonId != controlId) {
                _activePressButtonId = controlId;
                return true;
            }
            if (!pressed && hotControl < 1) _activePressButtonId = -1;
            return false;
        }

        /// <summary>
        /// Draws a single press icon button with an expand icon.
        /// </summary>
        public static bool ToolbarExpandButton(Color backgroundShade)
        { return ToolbarExpandCollapseButton(false, "", backgroundShade); }
        /// <summary>
        /// Draws a single press icon button with an expand icon.
        /// </summary>
        public static bool ToolbarExpandButton(string tooltip)
        { return ToolbarExpandCollapseButton(false, tooltip, GUI.backgroundColor); }
        /// <summary>
        /// Draws a single press icon button with an expand icon.
        /// </summary>
        public static bool ToolbarExpandButton(string tooltip, Color backgroundShade)
        { return ToolbarExpandCollapseButton(false, tooltip, backgroundShade); }
        /// <summary>
        /// Draws a single press icon button with a collapse icon.
        /// </summary>
        public static bool ToolbarCollapseButton(Color backgroundShade)
        { return ToolbarExpandCollapseButton(true, "", backgroundShade); }
        /// <summary>
        /// Draws a single press icon button with a collapse icon.
        /// </summary>
        public static bool ToolbarCollapseButton(string tooltip)
        { return ToolbarExpandCollapseButton(true, tooltip, GUI.backgroundColor); }
        /// <summary>
        /// Draws a single press icon button with a collapse icon.
        /// </summary>
        public static bool ToolbarCollapseButton(string tooltip, Color backgroundShade)
        { return ToolbarExpandCollapseButton(true, tooltip, backgroundShade); }

        static bool ToolbarExpandCollapseButton(bool isCollapse, string tooltip, Color backgroundShade)
        {
            GUIStyle style = new GUIStyle(HOGUIStyle.toolbarIconButton);
            style.padding.left += 2;
            style.padding.top += 1;
            return ShadedButton(new GUIContent(isCollapse ? HOGUITexture.collapse : HOGUITexture.expand, tooltip), style, backgroundShade);
        }

        // OTHER CONTROLS ////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Draws a button with a foldout icon and no label.
        /// </summary>
        public static bool Foldout(bool isOpen, bool stretchWidth = false, params GUILayoutOption[] options)
        { return Foldout(isOpen, "", stretchWidth, options); }
        /// <summary>
        /// Draws a button with a foldout icon and a label.
        /// </summary>
        public static bool Foldout(bool isOpen, string text, bool stretchWidth = false, params GUILayoutOption[] options)
        {
            bool noText = string.IsNullOrEmpty(text);
            GUIStyle style = isOpen
                ? noText ? HOGUIStyle.foldoutOpenButton : HOGUIStyle.foldoutOpenButtonWLabel
                : noText ? HOGUIStyle.foldoutClosedButton : HOGUIStyle.foldoutClosedButtonWLabel;
            style.stretchWidth = stretchWidth;
            if (GUILayout.Button(text, style, options)) {
                GUI.changed = true;
                return !isOpen;
            }
            return isOpen;
        }
        /// <summary>
        /// Draws a button with a foldout icon and a miniLabel.
        /// </summary>
        public static bool MiniFoldout(bool isOpen, string text, bool stretchWidth = false, params GUILayoutOption[] options)
        {
            GUIStyle style = isOpen ? HOGUIStyle.foldoutOpenButtonWMiniLabel : HOGUIStyle.foldoutClosedButtonWMiniLabel;
            style.stretchWidth = stretchWidth;
            if (GUILayout.Button(text, style, options)) {
                GUI.changed = true;
                return !isOpen;
            }
            return isOpen;
        }

        /// <summary>
        /// Draws a checkbox that can fit inside a toolbar.
        /// </summary>
        public static bool ToolbarToggle(bool toggled)
        {
            // Uses a button because a customized toggle doesn't work on Unity 4
            if (GUILayout.Button("", toggled ? HOGUIStyle.toolbarToggleButtonOn : HOGUIStyle.toolbarToggleButtonOff)) {
                toggled = !toggled;
                GUI.changed = true;
            }
            return toggled;
        }

        // ELEMENTS //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Horizontal divider.
        /// </summary>
        /// <param name="margin">Top and bottom margin</param>
        /// <param name="height">Height of the divider</param>
        public static void HorizontalDivider(int margin = 0, int height = 1)
        { HorizontalDivider(GUI.backgroundColor, margin, height); }
        /// <summary>
        /// Horizontal divider.
        /// </summary>
        /// <param name="backgroundShade">Shade to apply to the divider</param>
        /// <param name="margin">Top and bottom margin</param>
        /// <param name="height">Height of the divider</param>
        public static void HorizontalDivider(Color backgroundShade, int margin = 0, int height = 1)
        {
            GUIStyle style = new GUIStyle(HOGUIStyle.dividerBox)
                { margin = new RectOffset(0, 0, margin, margin), fixedHeight = height };
            _Horizontal(style, backgroundShade, GUILayout.FlexibleSpace, null);
        }

        /// <summary>
        /// Horizontal flat divider, where the color property will be the exact color shown, and not a shade.
        /// </summary>
        /// <param name="color">Color of the divider</param>
        /// <param name="margin">Top and bottom margin</param>
        /// <param name="height">Height of the divider</param>
        public static void FlatHorizontalDivider(Color color, int margin = 0, int height = 1)
        {
            GUIStyle style = new GUIStyle(HOGUIStyle.flatDividerBox) { margin = new RectOffset(0, 0, margin, margin), fixedHeight = height };
            _Horizontal(style, color, GUILayout.FlexibleSpace, null);
        }
    }
}