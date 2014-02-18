// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/22 14:29

using System.Collections.Generic;
using Holoville.HO2DToolkit.Core;
using UnityEngine;

namespace Holoville.HO2DToolkit
{
    /// <summary>
    /// Manager for HO2DToolkit GUI elements,
    /// automatically added at runtime when needed.
    /// </summary>
    public class HOtk2dGUIManager : MonoBehaviour
    {
        private static HOTk2dButtonDelegate RollOverInvoker;
        public static event HOTk2dButtonDelegate RollOver
        {
            add { RollOverInvoker += value; }
            remove { RollOverInvoker -= value; }
        }

        private static HOTk2dButtonDelegate RollOutInvoker;
        public static event HOTk2dButtonDelegate RollOut
        {
            add { RollOutInvoker += value; }
            remove { RollOutInvoker -= value; }
        }

        private static HOTk2dButtonDelegate PressInvoker;
        public static event HOTk2dButtonDelegate Press
        {
            add { PressInvoker += value; }
            remove { PressInvoker -= value; }
        }

        private static HOTk2dButtonDelegate ReleaseInvoker;
        public static event HOTk2dButtonDelegate Release
        {
            add { ReleaseInvoker += value; }
            remove { ReleaseInvoker -= value; }
        }

        private static HOTk2dButtonDelegate ClickInvoker;
        public static event HOTk2dButtonDelegate Click
        {
            add { ClickInvoker += value; }
            remove { ClickInvoker -= value; }
        }

        private static HOTk2dButtonDelegate SelectInvoker;
        public static event HOTk2dButtonDelegate Select
        {
            add { SelectInvoker += value; }
            remove { SelectInvoker -= value; }
        }

        private static HOTk2dButtonDelegate DeselectInvoker;
        public static event HOTk2dButtonDelegate Deselect
        {
            add { DeselectInvoker += value; }
            remove { DeselectInvoker -= value; }
        }

        private static HOTk2dButtonDelegate ToggleInvoker;
        public static event HOTk2dButtonDelegate Toggle
        {
            add { ToggleInvoker += value; }
            remove { ToggleInvoker -= value; }
        }

        internal static void OnRollOver(HOtk2dButtonEvent e) { if (RollOverInvoker != null) RollOverInvoker(e); }
        internal static void OnRollOut(HOtk2dButtonEvent e) { if (RollOutInvoker != null) RollOutInvoker(e); }
        internal static void OnPress(HOtk2dButtonEvent e) { if (PressInvoker != null) PressInvoker(e); }
        internal static void OnRelease(HOtk2dButtonEvent e) { if (ReleaseInvoker != null) ReleaseInvoker(e); }
        internal static void OnClick(HOtk2dButtonEvent e) { if (ClickInvoker != null) ClickInvoker(e); }
        internal static void OnSelect(HOtk2dButtonEvent e) { if (SelectInvoker != null) SelectInvoker(e); }
        internal static void OnDeselect(HOtk2dButtonEvent e) { if (DeselectInvoker != null) DeselectInvoker(e); }
        internal static void OnToggle(HOtk2dButtonEvent e) { if (ToggleInvoker != null) ToggleInvoker(e); }

        /// <summary>
        /// Camera that will be used for all new buttons that don't have a camera set.
        /// Default to Camera.main if not set.
        /// </summary>
        public static Camera defaultGuiCamera {
            get { if (_defaultGuiCamera == null) _defaultGuiCamera = Camera.main; return _defaultGuiCamera; }
            set { _defaultGuiCamera = value; }
        }
        /// <summary>
        /// If set to FALSE, rollover events and animations won't happen
        /// </summary>
        public static bool rolloversEnabled = true;

        /// <summary>
        /// If TRUE uses 2D colliders and physics
        /// </summary>
        public static bool use2DSystem;

        static readonly List<HOtk2dButton> _Buttons = new List<HOtk2dButton>();
        static readonly Dictionary<Transform, HOtk2dButton> _ButtonsByTrans = new Dictionary<Transform, HOtk2dButton>();
        static readonly List<Camera> _Cams = new List<Camera>(); // Set by RefreshData

        static Camera _defaultGuiCamera;
//        static bool _hasRollovers; // Set by RefreshData
        static bool _requiresDataRefresh;

        // ***********************************************************************************
        // CONSTRUCTOR
        // ***********************************************************************************

        static HOtk2dGUIManager()
        {
            // Create gameObject
            GameObject go = new GameObject("- HOtk2dGUIManager");
            DontDestroyOnLoad(go);
            go.AddComponent<HOtk2dGUIManager>();
        }

        // ===================================================================================
        // UNITY METHODS ---------------------------------------------------------------------

        void Update()
        {
            if (_requiresDataRefresh) RefreshData();
            if (_Buttons.Count == 0) return;

            bool isMouseDown = Input.GetMouseButtonDown(0);
            bool isMousePressed = Input.GetMouseButton(0);
            bool isMouseUp = Input.GetMouseButtonUp(0);
            MouseState mouseState = isMouseDown ? MouseState.JustPressed : isMousePressed ? MouseState.Pressed : isMouseUp ? MouseState.Released : MouseState.Up;
            if (rolloversEnabled || isMouseDown || isMouseUp) {
                List<HOtk2dButton> hitButtons = GetOverButtons(Input.mousePosition);
                int len = _Buttons.Count - 1;
                for (int i = len; i > -1; --i) {
                    if (i > _Buttons.Count - 1) continue;
                    HOtk2dButton button = _Buttons[i];
                    button.Refresh(hitButtons.IndexOf(button) != -1, mouseState, isMousePressed);
                }
            }
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        static internal void AddButton(HOtk2dButton button)
        {
            int ind = _Buttons.IndexOf(button);
            if (ind != -1) return;
            _Buttons.Add(button);
            _ButtonsByTrans.Add(button.trans, button);
            _requiresDataRefresh = true;
        }

        static internal void RemoveButton(HOtk2dButton button)
        {
            int ind = _Buttons.IndexOf(button);
            if (ind == -1) return;
            _Buttons.RemoveAt(ind);
            _ButtonsByTrans.Remove(button.trans);
            _requiresDataRefresh = true;
        }

        static void RefreshData()
        {
            _Cams.Clear();
//            _hasRollovers = false;
            foreach (HOtk2dButton button in _Buttons) {
//                if (rolloversEnabled && button.hasRollover) _hasRollovers = true;
                if (_Cams.IndexOf(button.guiCamera) == -1) _Cams.Add(button.guiCamera);
            }
            _requiresDataRefresh = false;
        }

        // Returns the buttons which have the mouse over them
        // (max one per camera, and only if they are not covered by something else)
        static List<HOtk2dButton> GetOverButtons(Vector2 mousePos)
        {
            List<HOtk2dButton> hits = new List<HOtk2dButton>();
            int len = _Cams.Count;
            if (use2DSystem) {
                for (int i = 0; i < len; ++i) {
                    Camera cam = _Cams[i];
                    Collider2D hitObj = Physics2D.OverlapPoint(cam.ScreenToWorldPoint(mousePos));
                    if (hitObj == null) continue;
                    if (_ButtonsByTrans.ContainsKey(hitObj.transform)) hits.Add(_ButtonsByTrans[hitObj.transform]);
                }
            } else {
                for (int i = 0; i < len; ++i) {
                    Camera cam = _Cams[i];
                    Ray ray = cam.ScreenPointToRay(mousePos);
                    RaycastHit[] hitInfos = Physics.RaycastAll(ray, 1.0e8f);
                    if (hitInfos.Length == 0) continue;
                    // Sort by nearest
                    System.Array.Sort(hitInfos, (a, b) => a.distance.CompareTo(b.distance));
                    Transform trans = hitInfos[0].transform;
                    if (_ButtonsByTrans.ContainsKey(trans)) hits.Add(_ButtonsByTrans[trans]);
                }
            }
            return hits;
        }
    }
}