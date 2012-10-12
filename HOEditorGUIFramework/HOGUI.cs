// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/30 15:39

using UnityEngine;

namespace Holoville.HOEditorGUIFramework
{
    /// <summary>
    /// Various GUI methods.
    /// </summary>
    public static class HOGUI
    {
        // ELEMENTS //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Horizontal flat divider, where the color property will be the exact color shown, and not a shade.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the divider</param>
        /// <param name="color">Color of the divider</param>
        public static void FlatDivider(Rect position, Color color)
        {
            Color prevBacgkroundColor = GUI.backgroundColor;
            GUIStyle style = new GUIStyle(HOGUIStyle.flatDividerBox) { fixedHeight = 0 };
            GUI.backgroundColor = color;
            GUI.Box(position, "", style);
            GUI.backgroundColor = prevBacgkroundColor;
        }
    }
}