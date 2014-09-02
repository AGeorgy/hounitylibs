// Author: Daniele Giardini
// Copyright (c) 2013 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2013/12/26 11:46

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Holoville.HO2DToolkit
{
    /// <summary>
    /// Allows to control menu systems with a keyboard.
    /// Doesn't detect key movememnts automatically: inputs must be called with up/down/left/right/enter.
    /// Supports a max of 10 separate button panels.
    /// // TODO complete separate panels support
    /// </summary>
    public static class HOMenuControlSystem
    {
        enum Direction
        {
            Up, Down, Left, Right
        }

        // Settings + Options
        public const string IgnoreId = "ignKeyNav"; // Used as an id for ui elements that must be ignored in key navigation
        public static bool loop; // If TRUE, after reaching the end of a row/column, the focus will go back to the first element
        public static bool rolloverEffects = true; // If TRUE simulates rollover effects when moving to an ui element
        public static float evidenceBorder;
        const int _MaxContentGroups = 10;

        public static bool active { get; private set; }

        static readonly List<ContentGroup> _ContentGroups = new List<ContentGroup>(_MaxContentGroups);
        static readonly FocusManager _FocusManager = new FocusManager();
        static IHOtk2dSlicedSprite _evidenceSprite;
        static Tweener _evidenceTween;

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        public static void Setup(IHOtk2dSlicedSprite pEvidenceSprite, bool pTweenEvidence = true, float pEvidenceBorder = 0, bool pLoop = false, bool pRolloverEffects = true)
        {
            _evidenceSprite = pEvidenceSprite;
            evidenceBorder = pEvidenceBorder;
            loop = pLoop;
            rolloverEffects = pRolloverEffects;

            if (_evidenceTween != null) {
                _evidenceTween.Rewind();
                _evidenceTween.Kill();
                _evidenceTween = null;
            }
            if (_evidenceSprite != null) {
                _evidenceSprite.gameObject.SetActive(false);
                Color toCol = _evidenceSprite.color;
                toCol.a *= 0.5f;
                if (pTweenEvidence) {
                    _evidenceTween = DOTween.To(() => _evidenceSprite.color, x => _evidenceSprite.color = x, toCol, 0.4f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutQuad)
                        .OnKill(() => _evidenceTween = null)
                        .Pause();
                }
            }
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
            if (_FocusManager.contentGroup.uiElementsToSliders.ContainsKey(_FocusManager.uiElement)) _FocusManager.contentGroup.uiElementsToSliders[_FocusManager.uiElement].DecreaseBy(0.1f);
            else MoveFocus(Direction.Left);
        }

        public static void Right()
        {
            if (_FocusManager.contentGroup.uiElementsToSliders.ContainsKey(_FocusManager.uiElement)) _FocusManager.contentGroup.uiElementsToSliders[_FocusManager.uiElement].IncreaseBy(0.1f);
            else MoveFocus(Direction.Right);
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

            // If the name wasn't found, focus on the first element
            _FocusManager.SetFocus(_ContentGroups[0], 0, 0);
        }

        /// <summary>
        /// Activates the control system on all the <see cref="HOtk2dButton"/> children of the given parent,
        /// deactivating all existing groups.
        /// </summary>
        /// <param name="parent">Panel whose children to control</param>
        public static void Activate(Transform parent)
        {
            DoActivate(parent, false, null, false, false);
        }
        /// <summary>
        /// Activates the control system on all the <see cref="HOtk2dButton"/> children of the given parent,
        /// deactivating all existing groups.
        /// </summary>
        /// <param name="parent">Panel whose children to control</param>
        /// <param name="forceRefresh">If FALSE, doesn't refresh the control system in case the panel is already active</param>
        public static void Activate(Transform parent, bool forceRefresh)
        {
            DoActivate(parent, false, null, forceRefresh, false);
        }
        /// <summary>
        /// Activates the control system on all the <see cref="HOtk2dButton"/> children of the given parent,
        /// deactivating all existing groups and setting the focus on the element with the given name, or the first element.
        /// </summary>
        /// <param name="parent">Panel whose children to control</param>
        /// <param name="focusElementName">Optional name of the button to focus on</param>
        /// <param name="forceRefresh">If FALSE, doesn't refresh the control system in case the panel is already active</param>
        public static void Activate(Transform parent, string focusElementName, bool forceRefresh = false)
        {
            DoActivate(parent, !string.IsNullOrEmpty(focusElementName), focusElementName, forceRefresh, false);
        }

        /// <summary>
        /// Activates the control system on all the <see cref="HOtk2dButton"/> children of the given parent,
        /// adding them to the existing controlled groups.
        /// </summary>
        /// <param name="parent">Panel whose children to control</param>
        public static void AddGroup(Transform parent)
        {
            DoActivate(parent, false, null, false, true);
        }
        /// <summary>
        /// Activates the control system on all the <see cref="HOtk2dButton"/> children of the given parent,
        /// adding them to the existing controlled groups.
        /// </summary>
        /// <param name="parent">Panel whose children to control</param>
        /// <param name="forceRefresh">If FALSE, doesn't refresh the control system in case the panel is already active</param>
        public static void AddGroup(Transform parent, bool forceRefresh)
        {
            DoActivate(parent, false, null, forceRefresh, true);
        }
        /// <summary>
        /// Activates the control system on all the <see cref="HOtk2dButton"/> children of the given parent,
        /// adding them to the existing controlled groups and setting the focus on the element with the given name, or the first element.
        /// </summary>
        /// <param name="parent">Panel whose children to control</param>
        /// <param name="focusElementName">Optional name of the button to focus on</param>
        /// <param name="forceRefresh">If FALSE, doesn't refresh the control system in case the panel is already active</param>
        public static void AddGroup(Transform parent, string focusElementName, bool forceRefresh = false)
        {
            DoActivate(parent, !string.IsNullOrEmpty(focusElementName), focusElementName, forceRefresh, true);
        }

        public static void DeactivateGroup(Transform parent)
        {
            int contentGroupId = _ContentGroups.GetContentGroupId(parent);
            if (contentGroupId == -1) return;

            if (_ContentGroups.Count == 1) DeactivateAll();
            else {
                bool changeFocus = _FocusManager.contentGroup.parent == parent;
                _ContentGroups.RemoveAt(contentGroupId);
                if (changeFocus) _FocusManager.SetFocus(_ContentGroups[0], 0, 0);
            }
        }

        /// <summary>
        /// Refreshes the current groups. Useful in case of layout changes withing the same group.
        /// </summary>
        public static void Refresh()
        {
            List<ContentGroup> currGroups = new List<ContentGroup>(_ContentGroups);
            string currFocusName = _FocusManager.uiElement.name;
            ContentGroup currFocusGroup = _FocusManager.contentGroup;
            DeactivateAll();
            for (int i = 0; i < currGroups.Count; ++i) {
                ContentGroup cGroup = currGroups[i];
                if (i == 0) {
                    if (currFocusGroup == cGroup) Activate(cGroup.parent, currFocusName);
                    else Activate(cGroup.parent);
                } else {
                    if (currFocusGroup == cGroup) AddGroup(cGroup.parent, currFocusName);
                    else AddGroup(cGroup.parent);
                }
            }
        }

        public static void DeactivateAll()
        {
            active = false;
            Unfocus();
            _ContentGroups.Clear();
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        static void DoActivate(Transform parent, bool setFocus, string focusElementName, bool forceRefresh, bool addAsNewGroup)
        {
            forceRefresh = forceRefresh
                || focusElementName != null
                || !addAsNewGroup && _ContentGroups.Count > 1;
            if (_ContentGroups.ContainsContentGroupParent(parent) && !forceRefresh) return;

            if (!addAsNewGroup) DeactivateAll();

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
            if (setFocus) {
                if (focusElementName != null) FocusByName(focusElementName);
                else _FocusManager.SetFocus(_ContentGroups[_ContentGroups.Count - 1], 0, 0);
            }

            active = true;
        }

        static void Unfocus()
        {
            if (!_FocusManager.hasFocus) return;

            _FocusManager.Clear();
        }

        static void MoveFocus(Direction direction)
        {
            if (!_FocusManager.hasFocus) return;

            ContentGroup moveToContentGroup = _FocusManager.contentGroup;
            int moveToRow = _FocusManager.rowId, moveToColumn = _FocusManager.columnId;
            switch (direction) {
            case Direction.Up:
                if (_FocusManager.rowId > 0) moveToRow = _FocusManager.rowId - 1;
                else {
                    ContentGroup adjacentGroup = _FocusManager.contentGroup.GetAdjacentGroup(Direction.Up);
                    if (adjacentGroup != null) moveToContentGroup = adjacentGroup;
                    else if (loop) moveToRow = _FocusManager.contentGroup.uiElements.Count - 1;
                }
                break;
            case Direction.Down:
                if (_FocusManager.rowId < _FocusManager.contentGroup.uiElements.Count - 1) moveToRow = _FocusManager.rowId + 1;
                else {
                    ContentGroup adjacentGroup = _FocusManager.contentGroup.GetAdjacentGroup(Direction.Down);
                    if (adjacentGroup != null) moveToContentGroup = adjacentGroup;
                    else if (loop) moveToRow = 0;
                }
                break;
            case Direction.Left:
                if (_FocusManager.columnId > 0) moveToColumn = _FocusManager.columnId - 1;
                else {
                    ContentGroup adjacentGroup = _FocusManager.contentGroup.GetAdjacentGroup(Direction.Left);
                    if (adjacentGroup != null) moveToContentGroup = adjacentGroup;
                    else if (loop) moveToColumn = _FocusManager.contentGroup.uiElements[_FocusManager.rowId].Count - 1;
                }
                break;
            case Direction.Right:
                if (_FocusManager.columnId < _FocusManager.contentGroup.uiElements[_FocusManager.rowId].Count - 1) moveToColumn = _FocusManager.columnId + 1;
                else {
                    ContentGroup adjacentGroup = _FocusManager.contentGroup.GetAdjacentGroup(Direction.Right);
                    if (adjacentGroup != null) moveToContentGroup = adjacentGroup;
                    else if (loop) moveToColumn = 0;
                }
                break;
            }

            bool recalculateColumn = moveToRow != _FocusManager.rowId
                || moveToContentGroup != _FocusManager.contentGroup && (direction == Direction.Up || direction == Direction.Down);
            bool recalculateRow = !recalculateColumn && moveToContentGroup != _FocusManager.contentGroup
                && (direction == Direction.Left || direction == Direction.Right);
            if (recalculateColumn) {
                // Row change - find ui element with nearest X position to previous one
                //
                // In case of contentGroup change, set correct row
                if (moveToContentGroup != _FocusManager.contentGroup) {
                    moveToRow = direction == Direction.Up ? moveToContentGroup.uiElements.Count - 1 : 0;
                }
                //
                float currElementX = _FocusManager.uiElement.trans.position.x;
                float diff = Mathf.Infinity;
                for (int c = 0; c < moveToContentGroup.uiElements[moveToRow].Count; ++c) {
                    float newDiff = Mathf.Abs(currElementX - moveToContentGroup.uiElements[moveToRow][c].trans.position.x);
                    if (newDiff < diff) {
                        diff = newDiff;
                        moveToColumn = c;
                    }
                }
            } else if (recalculateRow) {
                // Column and contentGroup change - find ui element with nearest Y position to previous one
                float currElementY = _FocusManager.uiElement.trans.position.y;
                float diff = Mathf.Infinity;
                for (int r = 0; r < moveToContentGroup.uiElements.Count; ++r) {
                    float newDiff = Mathf.Abs(currElementY - moveToContentGroup.uiElements[r][0].trans.position.y);
                    if (newDiff < diff) {
                        diff = newDiff;
                        moveToRow = r;
                    }
                }
                // In case of contentGroup change, set correct column
                if (moveToContentGroup != _FocusManager.contentGroup) {
                    moveToColumn = direction == Direction.Left ? moveToContentGroup.uiElements[moveToRow].Count - 1 : 0;
                }
            }
            // Focus
            _FocusManager.SetFocus(moveToContentGroup, moveToRow, moveToColumn);
        }

        // ===================================================================================
        // EXTENSION METHODS -----------------------------------------------------------------

        static bool ContainsContentGroupParent(this List<ContentGroup> cGroups, Transform parent)
        {
            int len = cGroups.Count;
            for (int i = 0; i < len; ++i) if (cGroups[i].parent == parent) return true;
            return false;
        }

        static int GetContentGroupId(this List<ContentGroup> cGroups, Transform parent)
        {
            int len = cGroups.Count;
            for (int i = 0; i < len; ++i) if (cGroups[i].parent == parent) return i;
            return -1;
        }

        // Returns eventual group above this one (but within this group's X bounds), or NULL if there is none
        static ContentGroup GetAdjacentGroup(this ContentGroup contentGroup, Direction direction)
        {
            if (_ContentGroups.Count <= 1) return null;

            ContentGroup adjacentGroup = null;
            foreach (ContentGroup cGroup in _ContentGroups) {
                if (cGroup == contentGroup) continue;
                switch (direction) {
                case Direction.Up:
                    if (cGroup.IsAboveOf(contentGroup)) {
                        if (adjacentGroup == null || cGroup.bounds.min.y < adjacentGroup.bounds.min.y) adjacentGroup = cGroup;
                    }
                    break;
                case Direction.Down:
                    if (cGroup.IsBelowOf(contentGroup)) {
                        if (adjacentGroup == null || cGroup.bounds.max.y > adjacentGroup.bounds.max.y) adjacentGroup = cGroup;
                    }
                    break;
                case Direction.Left:
                    if (cGroup.IsLeftOf(contentGroup)) {
                        if (adjacentGroup == null || cGroup.bounds.max.x > adjacentGroup.bounds.max.x) adjacentGroup = cGroup;
                    }
                    break;
                case Direction.Right:
                    if (cGroup.IsRightOf(contentGroup)) {
                        if (adjacentGroup == null || cGroup.bounds.min.x < adjacentGroup.bounds.min.x) adjacentGroup = cGroup;
                    }
                    break;
                }
            }
            return adjacentGroup;
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ||| INTERNAL CLASSES ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        class ContentGroup
        {
            public readonly Transform parent;
            public readonly List<List<HOtk2dButton>> uiElements; // sorted by row and column
            public readonly Dictionary<HOtk2dButton, IHOtk2dHorizontalSlider> uiElementsToSliders; // stores uiElements which are also sliders
            public readonly Bounds bounds;

            public ContentGroup(Transform parent, List<List<HOtk2dButton>> uiElements)
            {
                this.parent = parent;
                this.uiElements = uiElements;
                // Store bounds of whole group and eventual sliders
                uiElementsToSliders = new Dictionary<HOtk2dButton, IHOtk2dHorizontalSlider>(uiElements.Count);
                float minX = 9999999, minY = 9999999, minZ = 9999999, maxX = -9999999, maxY = -9999999, maxZ = -9999999;
                foreach (List<HOtk2dButton> rows in uiElements) {
                    foreach (HOtk2dButton uiElement in rows) {
                        IHOtk2dHorizontalSlider slider = uiElement.GetComponent(typeof(IHOtk2dHorizontalSlider)) as IHOtk2dHorizontalSlider;
                        if (slider != null) uiElementsToSliders.Add(uiElement, slider);
                        if (uiElement.bounds.min.x < minX) minX = uiElement.bounds.min.x;
                        if (uiElement.bounds.min.y < minY) minY = uiElement.bounds.min.y;
                        if (uiElement.bounds.min.z < minZ) minZ = uiElement.bounds.min.z;
                        if (uiElement.bounds.max.x > maxX) maxX = uiElement.bounds.max.x;
                        if (uiElement.bounds.max.y > maxY) maxY = uiElement.bounds.max.y;
                        if (uiElement.bounds.max.z > maxZ) maxZ = uiElement.bounds.max.z;
                    }
                }
                Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
                bounds = new Bounds(new Vector3(minX + (size.x * 0.5f), minY + (size.y * 0.5f), minZ + (size.z * 0.5f)), size);
            }

            public bool IsAboveOf(ContentGroup cGroup)
            {
                return bounds.min.y > cGroup.bounds.min.y && IntersectsX(cGroup.bounds);
            }

            public bool IsBelowOf(ContentGroup cGroup)
            {
                return bounds.max.y < cGroup.bounds.max.y && IntersectsX(cGroup.bounds);
            }

            public bool IsRightOf(ContentGroup cGroup)
            {
                return bounds.min.x > cGroup.bounds.min.x && IntersectsY(cGroup.bounds);
            }

            public bool IsLeftOf(ContentGroup cGroup)
            {
                return bounds.max.x < cGroup.bounds.max.x && IntersectsY(cGroup.bounds);
            }

            bool IntersectsX(Bounds b)
            {
                return bounds.max.x > b.min.x && bounds.max.x < b.max.x
                    || bounds.min.x > b.min.x && bounds.min.x < b.max.x
                    || bounds.min.x <= b.min.x && bounds.max.x >= b.max.x;
            }

            bool IntersectsY(Bounds b)
            {
                return bounds.max.y > b.min.y && bounds.max.y < b.max.y
                    || bounds.min.y > b.min.y && bounds.min.y < b.max.y
                    || bounds.min.y <= b.min.y && bounds.max.y >= b.max.y;
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
                if (rolloverEffects && uiElement != null) uiElement.SimulateRollOut();
                contentGroup = focusContentGroup;
                rowId = focusRowId;
                columnId = focusColumnId;
                uiElement = contentGroup.uiElements[focusRowId][focusColumnId];
                if (rolloverEffects) uiElement.SimulateRollOver();
                if (_evidenceSprite != null) {
                    // Set evidence sprite
                    Vector3 evidencePos = uiElement.bounds.center;
                    evidencePos.z -= 1;
                    _evidenceSprite.transform.position = evidencePos;
                    Vector2 size = uiElement.bounds.size;
                    size.x += evidenceBorder * 2;
                    size.y += evidenceBorder * 2;
                    _evidenceSprite.dimensions = size;
                    _evidenceSprite.gameObject.SetActive(true);
                    if (_evidenceTween != null) _evidenceTween.Restart();
                }
            }

            public void Clear()
            {
                if (_evidenceSprite != null) {
                    _evidenceSprite.gameObject.SetActive(false);
                    if (_evidenceTween != null) _evidenceTween.Pause();
                }
                if (rolloverEffects && uiElement != null) uiElement.SimulateRollOut();
                rowId = columnId = -1;
                contentGroup = null;
                uiElement = null;
            }
        }
    }
}