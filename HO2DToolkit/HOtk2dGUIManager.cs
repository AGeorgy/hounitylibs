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
        public static event HOTk2dButtonDelegate RollOver;
        internal static void OnRollOver(HOtk2dButtonEvent e) { if (RollOver != null) RollOver(e); }
        public static event HOTk2dButtonDelegate RollOut;
        internal static void OnRollOut(HOtk2dButtonEvent e) { if (RollOut != null) RollOut(e); }
        public static event HOTk2dButtonDelegate Press;
        internal static void OnPress(HOtk2dButtonEvent e) { if (Press != null) Press(e); }
        public static event HOTk2dButtonDelegate Release;
        internal static void OnRelease(HOtk2dButtonEvent e) { if (Release != null) Release(e); }
        public static event HOTk2dButtonDelegate Click;
        internal static void OnClick(HOtk2dButtonEvent e) { if (Click != null) Click(e); }
        public static event HOTk2dButtonDelegate Select;
        internal static void OnSelect(HOtk2dButtonEvent e) { if (Select != null) Select(e); }
        public static event HOTk2dButtonDelegate Deselect;
        internal static void OnDeselect(HOtk2dButtonEvent e) { if (Deselect != null) Deselect(e); }
        public static event HOTk2dButtonDelegate Toggle;
        internal static void OnToggle(HOtk2dButtonEvent e) { if (Toggle != null) Toggle(e); }

        /// <summary>
        /// Camera that will be used for all new buttons that don't have a camera set.
        /// Default to Camera.main if not set.
        /// </summary>
        public static Camera defaultGuiCamera {
            get { if (_defaultGuiCamera == null) _defaultGuiCamera = Camera.main; return _defaultGuiCamera; }
            set { _defaultGuiCamera = value; }
        }

        static readonly List<HOtk2dButton> _Buttons = new List<HOtk2dButton>();
        static readonly Dictionary<Transform, HOtk2dButton> _ButtonsByTrans = new Dictionary<Transform, HOtk2dButton>();
        static readonly List<Camera> _Cams = new List<Camera>(); // Set by RefreshData

        static Camera _defaultGuiCamera;
        static bool _hasRollovers; // Set by RefreshData
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
            if (isMouseDown || isMouseUp || _hasRollovers) {
                List<HOtk2dButton> hitButtons = GetOverButtons(Input.mousePosition);
                int len = _Buttons.Count - 1;
                for (int i = len; i > -1; --i) {
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
            _hasRollovers = false;
            foreach (HOtk2dButton button in _Buttons) {
                if (button.hasRollover) _hasRollovers = true;
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
            return hits;
        }
    }
}