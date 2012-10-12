// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/10/01 13:54

using System.Collections;
using UnityEditor;

namespace Holoville.HOEditorGUIFramework.Core
{
    /// <summary>
    /// Data used while dragging a GUI element.
    /// </summary>
    internal class GUIDragData
    {
        public readonly object draggedItem; // Dragged element
        public readonly int draggedItemIndex;
        public readonly IList draggableList; // Collection within which the drag is being executed
        public int currDragIndex = -1; // Index of current drag position
        public bool currDragSet;
        public object optionalData;

        // ***********************************************************************************
        // CONSTRUCTOR
        // ***********************************************************************************

        public GUIDragData(IList draggableList, object draggedItem, int draggedItemIndex, object optionalData)
        {
            this.draggedItem = draggedItem;
            this.draggedItemIndex = draggedItemIndex;
            this.draggableList = draggableList;
            this.optionalData = optionalData;
        }
    }
}