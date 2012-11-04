// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/03 15:44

using UnityEngine;

namespace Holoville.HOEditorGUIFramework.Utils
{
    /// <summary>
    /// GUI utils.
    /// </summary>
    public static class HOGUIUtils
    {
        /// <summary>
        /// Returns TRUE if the mouse is within the currently focused inspector/window boundaries.
        /// </summary>
        public static bool PanelContainsMouse()
        {
            Vector2 mousePos = Event.current.mousePosition;
            int w = Screen.width;
            int h = Screen.height;
            return mousePos.x > 0 && mousePos.x < w && mousePos.y > 0 && mousePos.y < h;
        }
    }
}