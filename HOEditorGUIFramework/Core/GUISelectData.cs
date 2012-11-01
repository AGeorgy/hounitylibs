// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/01 12:57

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoville.HOEditorGUIFramework.Core
{
    /// <summary>
    /// Data used for selection of GUI elements.
    /// </summary>
    internal class GUISelectData
    {
        public readonly ArrayList selectableList; // Clone of original IList
        public readonly List<ItemData> selectableItemsDatas = new List<ItemData>();
        public ItemData firstSelectedItemData;

        // ***********************************************************************************
        // CONSTRUCTOR
        // ***********************************************************************************

        public GUISelectData(IList selectableList)
        {
            this.selectableList = new ArrayList(selectableList);
            int len = this.selectableList.Count;
            for (int i = 0; i < len; ++i) {
                selectableItemsDatas.Add(new ItemData(i));
            }
        }

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        /// <summary>
        /// Checks if the given list is the same as the copied one.
        /// </summary>
        public bool IsStoredList(IList list)
        {
            if (list.Count != selectableList.Count) return false;
            int len = selectableList.Count;
            for (int i = 0; i < len; i++) {
                if (list[i] != selectableList[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// If this item is the only one selected, sets it as the first
        /// (useful when selecting multiple objects while pressing SHIFT).
        /// </summary>
        public void CheckFirstSelectedItem(ItemData itemData)
        {
            if (itemData.selected == false) return;
            int len = selectableItemsDatas.Count;
            for (int i = 0; i < len; i++) {
                ItemData itd = selectableItemsDatas[i];
                if (itd == itemData) continue;
                if (itd.selected) return;
            }
            firstSelectedItemData = itemData;
        }


        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ||| INTERNAL CLASSES ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public class ItemData
        {
            public int index;
            public Rect rect;
            public bool isPressed;
            public bool selected;

            public ItemData(int index)
            {
                this.index = index;
            }
        }
    }
}