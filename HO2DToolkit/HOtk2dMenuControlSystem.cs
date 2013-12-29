// Author: Daniele Giardini
// Copyright (c) 2013 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2013/12/26 11:46

using System.Collections.Generic;
using UnityEngine;

namespace Holoville.HO2DToolkit
{
    /// <summary>
    /// Allows to control menu systems with a keyboard.
    /// Doesn't detect key movememnts automatically: inputs must be called with up/down/left/right/enter.
    /// Supports a max of 5 separate button panels.
    /// // TODO complete separate panels support
    /// </summary>
    public static class HOtk2dMenuControlSystem
    {
        enum Direction
        {
            Up, Down, Left, Right
        }

        // Settings + Options
        public const string IgnoreId = "ignKeyNav"; // Used as an id for ui elements that must be ignored in key navigation
        public static bool loop; // If TRUE, after reaching the end of a row/column, the focus will go back to the first element
        public static IHOtk2dSlicedSprite evidenceSprite;
        public static float evidenceBorder;
        const int _MaxContentGroups = 5;

        static readonly List<ContentGroup> _ContentGroups = new List<ContentGroup>(_MaxContentGroups);
        static readonly FocusManager _FocusManager = new FocusManager();

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        public static void Setup(IHOtk2dSlicedSprite pEvidenceSprite, float pEvidenceBorder = 0, bool pLoop = false)
        {
            evidenceSprite = pEvidenceSprite;
            evidenceBorder = pEvidenceBorder;
            loop = pLoop;

            evidenceSprite.gameObject.SetActive(false);
        }

        public static void Up()
        {
            MoveFocus(Direction.Up);
        }

        public static void Down()
        {
            MoveFocus(Direction.Down);
        }

        public static void Left()
        {
            MoveFocus(Direction.Left);
        }

        public static void Right()
        {
            MoveFocus(Direction.Right);
        }

        public static void Enter()
        {
            if (!_FocusManager.hasFocus) return;

            _FocusManager.uiElement.SimulatePress();
        }

        public static void FocusByName(string focusElementName)
        {
            Unfocus();
            if (_ContentGroups.Count == 0) return;

            foreach (ContentGroup cg in _ContentGroups) {
                for (int r = 0; r < cg.uiElements.Count; ++r) {
                    List<HOtk2dButton> rowElements = cg.uiElements[r];
                    for (int c = 0; c < rowElements.Count; ++c) {
                        if (rowElements[c].name == focusElementName) {
                            _FocusManager.SetFocus(cg, r, c);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Activates the control system on all the <see cref="HOtk2dButton"/> children of the given parent.
        /// </summary>
        /// <param name="parent">Panel whose children to control</param>
        /// <param name="focusElementName">Optional name of the button to focus on</param>
        /// <param name="forceRefresh">If FALSE, doesn't refresh the control system in case the panel is already active</param>
        public static void Activate(Transform parent, string focusElementName = null, bool forceRefresh = false)
        {
            DoActivate(parent, focusElementName, forceRefresh, false);
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        static void DoActivate(Transform parent, string focusElementName, bool forceRefresh, bool addAsNewContentGroup)
        {
            if (_ContentGroups.ContainsContentGroupParent(parent) && !forceRefresh) return;

            if (!addAsNewContentGroup) _ContentGroups.Clear();

            HOtk2dButton[] buttons = parent.GetComponentsInChildren<HOtk2dButton>();
            if (buttons.Length == 0) return;

            // Sort content by Y
            List<HOtk2dButton> contentByY = new List<HOtk2dButton>();
            foreach (HOtk2dButton bt in buttons) {
                if (bt.enabled && bt.id != IgnoreId) contentByY.Add(bt);
            }
            if (contentByY.Count == 0) return;
            //
            contentByY.Sort((a, b) => {
                if (a.trans.position.y < b.trans.position.y) return 1;
                if (a.trans.position.y > b.trans.position.y) return -1;
                return 0;
            });
            // Fill uiElements
            List<List<HOtk2dButton>> uiElements = new List<List<HOtk2dButton>>();
            HOtk2dButton prevBt = null;
            foreach (HOtk2dButton bt in contentByY) {
                if (prevBt == null || !Mathf.Approximately(bt.trans.position.y, prevBt.trans.position.y)) {
                    // New row
                    uiElements.Add(new List<HOtk2dButton>());
                }
                uiElements[uiElements.Count - 1].Add(bt);
                prevBt = bt;
            }
            // Sort uiElements columns by X
            foreach (List<HOtk2dButton> row in uiElements) {
                row.Sort((a, b) => {
                    if (a.trans.position.x > b.trans.position.x) return 1;
                    if (a.trans.position.x < b.trans.position.x) return -1;
                    return 0;
                });
            }
            // Create contentGroup
            _ContentGroups.Add(new ContentGroup(parent, uiElements));

            // Focus
            if (focusElementName != null) FocusByName(focusElementName);
            else _FocusManager.SetFocus(_ContentGroups[_ContentGroups.Count - 1], 0, 0);
        }

        static void Unfocus()
        {
            if (!_FocusManager.hasFocus) return;

            _FocusManager.Clear();
        }

        static void MoveFocus(Direction direction)
        {
            if (!_FocusManager.hasFocus) return;

            int moveToRow = _FocusManager.rowId, moveToColumn = _FocusManager.columnId;
            switch (direction) {
            case Direction.Up:
                moveToRow = _FocusManager.rowId > 0 ? _FocusManager.rowId - 1
                    : loop ? _FocusManager.contentGroup.uiElements.Count - 1 : _FocusManager.rowId;
                break;
            case Direction.Down:
                moveToRow = _FocusManager.rowId < _FocusManager.contentGroup.uiElements.Count - 1 ? _FocusManager.rowId + 1
                    : loop ? 0 : _FocusManager.rowId;
                break;
            case Direction.Left:
                moveToColumn = _FocusManager.columnId > 0 ? _FocusManager.columnId - 1
                    : loop ? _FocusManager.contentGroup.uiElements[_FocusManager.rowId].Count - 1 : _FocusManager.columnId;
                break;
            case Direction.Right:
                moveToColumn = _FocusManager.columnId < _FocusManager.contentGroup.uiElements[_FocusManager.rowId].Count -1 ? _FocusManager.columnId + 1
                    : loop ? 0 : _FocusManager.columnId;
                break;
            }
            if (moveToRow != _FocusManager.rowId) {
                // Row change - find ui element with nearest X position to previous one
                float currElementX = _FocusManager.uiElement.trans.position.x;
                float diff = Mathf.Infinity;
                for (int c = 0; c < _FocusManager.contentGroup.uiElements[moveToRow].Count; ++c) {
                    float newDiff = Mathf.Abs(currElementX - _FocusManager.contentGroup.uiElements[moveToRow][c].trans.position.x);
                    if (newDiff < diff) {
                        diff = newDiff;
                        moveToColumn = c;
                    }
                }
            }
            // Focus
            _FocusManager.SetFocus(_FocusManager.contentGroup, moveToRow, moveToColumn);
        }

        // ===================================================================================
        // EXTENSION METHODS -----------------------------------------------------------------

        static bool ContainsContentGroupParent (this List<ContentGroup> cGroups, Transform parent)
        {
            int len = cGroups.Count;
            for (int i = 0; i < len; ++i) if (cGroups[i].parent == parent) return true;
            return false;
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ||| INTERNAL CLASSES ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        class ContentGroup
        {
            public readonly Transform parent;
            public readonly List<List<HOtk2dButton>> uiElements; // sorted by row and column

            public ContentGroup(Transform parent, List<List<HOtk2dButton>> uiElements)
            {
                this.parent = parent;
                this.uiElements = uiElements;
            }
        }

        class FocusManager
        {
            public bool hasFocus { get { return uiElement != null; } }

            public ContentGroup contentGroup;
            public HOtk2dButton uiElement;
            public int rowId;
            public int columnId;

            public void SetFocus(ContentGroup focusContentGroup, int focusRowId, int focusColumnId)
            {
                contentGroup = focusContentGroup;
                rowId = focusRowId;
                columnId = focusColumnId;
                uiElement = contentGroup.uiElements[focusRowId][focusColumnId];
                uiElement.SimulateRollOver();
                if (evidenceSprite != null) {
                    Vector3 evidencePos = uiElement.trans.position;
                    evidencePos.z -= 1;
                    evidenceSprite.transform.position = evidencePos;
                    IHOtk2dSlicedSprite uiElementSlicedSprite = uiElement.GetComponent(typeof(IHOtk2dSlicedSprite)) as IHOtk2dSlicedSprite;
                    Vector2 size;
                    if (uiElementSlicedSprite != null) size = uiElementSlicedSprite.dimensions;
                    else size = uiElement.sprite.GetBounds().size;
                    size.x += evidenceBorder * 2;
                    size.y += evidenceBorder * 2;
                    evidenceSprite.dimensions = size;
                    evidenceSprite.gameObject.SetActive(true);
                }
            }

            public void Clear()
            {
                if (evidenceSprite != null) evidenceSprite.gameObject.SetActive(false);
                if (uiElement != null) uiElement.SimulateRollOut();
                rowId = columnId = -1;
                contentGroup = null;
                uiElement = null;
            }
        }
    }
}