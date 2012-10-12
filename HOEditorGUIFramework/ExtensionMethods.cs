// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/22 15:26

using UnityEngine;

namespace Holoville.HOEditorGUIFramework
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        // ===================================================================================
        // GUIStyle EXTENSIONS ---------------------------------------------------------------

        /// <summary>
        /// Sets the color of the given style's font.
        /// </summary>
        public static void SetFontColor(this GUIStyle guiStyle, Color color, bool setTurnedOnState = true, bool setTurnedOffState = true)
        {
            if (setTurnedOffState)
                guiStyle.normal.textColor = guiStyle.hover.textColor = guiStyle.active.textColor = guiStyle.focused.textColor = color;
            if (setTurnedOnState)
                guiStyle.onNormal.textColor = guiStyle.onHover.textColor = guiStyle.onActive.textColor = guiStyle.onFocused.textColor = color;
        }

        /// <summary>
        /// Sets the texture of the given style's background.
        /// </summary>
        public static void SetBackground(this GUIStyle guiStyle, Texture2D texture, bool setTurnedOnState = true, bool setTurnedOffState = true)
        {
            if (setTurnedOffState)
                guiStyle.normal.background = guiStyle.hover.background = guiStyle.active.background = guiStyle.focused.background = texture;
            if (setTurnedOnState)
                guiStyle.onNormal.background = guiStyle.onHover.background = guiStyle.onActive.background = guiStyle.onFocused.background = texture;
        }

        /// <summary>
        /// Clones the given style and changes its font color.
        /// </summary>
        public static GUIStyle Clone(this GUIStyle guiStyle, Color color, bool setTurnedOnState = true, bool setTurnedOffState = true)
        {
            GUIStyle style = new GUIStyle(guiStyle);
            style.SetFontColor(color, setTurnedOnState, setTurnedOffState);
            return style;
        }
    }
}