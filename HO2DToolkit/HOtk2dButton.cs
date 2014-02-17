// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/15 21:30

using System.Collections;
using System.Collections.Generic;
using Holoville.HO2DToolkit.Core;
using Holoville.HOTween;
using UnityEngine;

namespace Holoville.HO2DToolkit
{
    /// <summary>
    /// Attach this component to a sprite to use it as a button.
    /// </summary>
    [AddComponentMenu("2D Toolkit/Holoville/Button")]
    [RequireComponent(typeof(IHOtk2dSprite))]
    public class HOtk2dButton : MonoBehaviour
    {
        enum PreinitActionType
        {
            ToggleOn,
            ToggleOnWithoutEventDispatching,
            ToggleOff,
            ToggleOffWithoutEventDispatching
        }

        public event HOTk2dButtonDelegate RollOver;
        public event HOTk2dButtonDelegate RollOut;
        public event HOTk2dButtonDelegate Press;
        public event HOTk2dButtonDelegate Release;
        public event HOTk2dButtonDelegate Click;
        public event HOTk2dButtonDelegate Select;
        public event HOTk2dButtonDelegate Deselect;
        public event HOTk2dButtonDelegate Toggle;

        /// <summary>
        /// Returns TRUE if this button is a toggle and is actually selected
        /// </summary>
        public bool selected { get; private set; }
        public string id = "";
        public Transform trans { get { if (_fooTrans == null) _fooTrans = transform; return _fooTrans; } }
        public IHOtk2dSprite sprite { get { if (_fooSprite == null) _fooSprite = this.GetComponent(typeof(IHOtk2dSprite)) as IHOtk2dSprite; return _fooSprite; } }
        public Camera guiCamera {
            get { if (_guiCamera == null) _guiCamera = HOtk2dGUIManager.defaultGuiCamera; return _guiCamera; }
            set { ChangeCamera(value); }
        }
        public string text {
            get {
                if (_textMesh == null) _textMesh = this.GetComponentInChildren(typeof(IHOtk2dTextMesh)) as IHOtk2dTextMesh;
                return _textMesh.text;
            }
            set {
                if (_textMesh == null) _textMesh = this.GetComponentInChildren(typeof(IHOtk2dTextMesh)) as IHOtk2dTextMesh;
                _textMesh.text = value;
                _textMesh.Commit();
            }
        }
        public bool isToggle { get { return _isToggle; } }
        public Bounds bounds { get { if (!_fooBoundsSet) { _fooBounds = this.collider.bounds; _fooBoundsSet = true; } return _fooBounds; } }

        internal bool hasRollover { get { return _tweenColorOn == ButtonActionType.OnRollover || _tweenScaleOn == ButtonActionType.OnRollover; } }

#pragma warning disable 649
        [SerializeField] Camera _guiCamera;
        [SerializeField] ButtonActionType _tweenColorOn = ButtonActionType.None;
        [SerializeField] ButtonActionType _tweenScaleOn = ButtonActionType.None;
        [SerializeField] float _tweenScaleMultiplier = 1.1f;
        [SerializeField] Color _tweenColor = Color.white;
        [SerializeField] bool _tweenChildren; // only used for color, since otherwise children are scaled automatically
        [SerializeField] bool _isToggle = false;
        [SerializeField] ButtonActionType _toggleOn = ButtonActionType.OnPress;
        [SerializeField] ButtonActionType _toggleOnAnimation = ButtonActionType.OnPress;
        [SerializeField] string _toggleGroupid = "";
        [SerializeField] GameObject _tooltip; // shows tooltip on rollover if not null
#pragma warning restore 649

        const float _TweenDuration = 0.35f;
        static readonly Dictionary<string, List<HOtk2dButton>> _TogglesByGroupId = new Dictionary<string, List<HOtk2dButton>>();

        bool _initialized;
        bool _isRadioButton;
        bool _isOver;
        bool _isPressed;
        Queue<PreinitActionType> _preinitActionsQueue; // Actions that are stored in case they're called before Start
        List<IHOtk2dTextMesh> _txtMeshesToUpdate = new List<IHOtk2dTextMesh>(10);
        Sequence _rolloutTween;
        Sequence _unpressTween;
        Sequence _unclickTween;
        IHOtk2dTextMesh _textMesh; // eventual textmesh
        bool _showTooltip;
        bool _isSimulatingMouseFocus;
        Transform _fooTrans;
        IHOtk2dSprite _fooSprite;
        bool _fooBoundsSet;
        Bounds _fooBounds;

        // ===================================================================================
        // UNITY METHODS ---------------------------------------------------------------------

        protected virtual void OnEnable()
        {
            HOtk2dGUIManager.AddButton(this);
            // Add to eventual toggle group
            if (_isToggle && _toggleGroupid != "") {
                if (_toggleGroupid == "") return;
                if (!_TogglesByGroupId.ContainsKey(_toggleGroupid)) _TogglesByGroupId.Add(_toggleGroupid, new List<HOtk2dButton>());
                _TogglesByGroupId[_toggleGroupid].Add(this);
            }
        }
        void Start()
        {
            _initialized = true;

            _isRadioButton = _toggleGroupid != "";

            List<IHOtk2dBase> childrenSprites = null;
            bool hasChildrenToTween = false;
            bool hasTextMeshesToTween = false;
            if (_tweenChildren && _tweenColorOn != ButtonActionType.None) {
                Component[] children = gameObject.GetComponentsInChildren(typeof(IHOtk2dBase));
                childrenSprites = new List<IHOtk2dBase>();
                foreach (Component child in children) {
                    if (child == sprite) continue;
                    childrenSprites.Add(child as IHOtk2dBase);
                    IHOtk2dTextMesh txtMesh = child as IHOtk2dTextMesh;
                    if (txtMesh != null) _txtMeshesToUpdate.Add(txtMesh);
                }
                hasChildrenToTween = childrenSprites.Count > 0;
                hasTextMeshesToTween = _txtMeshesToUpdate.Count > 0;
            }

            // Hide eventual tooltip
            if (_tooltip != null) {
                _tooltip.SetActive(false);
                _showTooltip = true;
            }

            // Create tweens
            SequenceParms seqParms;
            if (hasRollover) {
                seqParms = new SequenceParms().UpdateType(UpdateType.TimeScaleIndependentUpdate).AutoKill(false);
                if (hasTextMeshesToTween) seqParms.OnUpdate(UpdateTextMeshes);
                _rolloutTween = new Sequence(seqParms);
                if (_tweenScaleOn == ButtonActionType.OnRollover)
                    _rolloutTween.Insert(0, HOTween.HOTween.From(trans, _TweenDuration, "localScale", trans.localScale * _tweenScaleMultiplier));
                if (_tweenColorOn == ButtonActionType.OnRollover) {
                    _rolloutTween.Insert(0, HOTween.HOTween.From(sprite, _TweenDuration, "color", _tweenColor));
                    if (hasChildrenToTween) {
                        foreach (IHOtk2dBase childSprite in childrenSprites) {
                            _rolloutTween.Insert(0, HOTween.HOTween.From(childSprite, _TweenDuration, "color", _tweenColor));
                        }
                    }
                }
                _rolloutTween.Complete();
            }
            if (_tweenColorOn == ButtonActionType.OnPress || _tweenScaleOn == ButtonActionType.OnPress) {
                seqParms = new SequenceParms().UpdateType(UpdateType.TimeScaleIndependentUpdate).AutoKill(false);
                if (hasTextMeshesToTween) seqParms.OnUpdate(UpdateTextMeshes);
                _unpressTween = new Sequence(seqParms);
                if (_tweenScaleOn == ButtonActionType.OnPress)
                    _unpressTween.Insert(0, HOTween.HOTween.From(trans, _TweenDuration, "localScale", trans.localScale * _tweenScaleMultiplier));
                if (_tweenColorOn == ButtonActionType.OnPress) {
                    _unpressTween.Insert(0, HOTween.HOTween.From(sprite, _TweenDuration, "color", _tweenColor));
                    if (hasChildrenToTween) {
                        foreach (IHOtk2dBase childSprite in childrenSprites) {
                            _rolloutTween.Insert(0, HOTween.HOTween.From(childSprite, _TweenDuration, "color", _tweenColor));
                        }
                    }
                }
                _unpressTween.Complete();
            }
            if (_tweenColorOn == ButtonActionType.OnClick || _tweenScaleOn == ButtonActionType.OnClick) {
                seqParms = new SequenceParms().UpdateType(UpdateType.TimeScaleIndependentUpdate).AutoKill(false);
                if (hasTextMeshesToTween) seqParms.OnUpdate(UpdateTextMeshes);
                _unclickTween = new Sequence(seqParms);
                if (_tweenScaleOn == ButtonActionType.OnClick)
                    _unclickTween.Insert(0.15f, HOTween.HOTween.From(trans, _TweenDuration, "localScale", trans.localScale * _tweenScaleMultiplier));
                if (_tweenColorOn == ButtonActionType.OnClick) {
                    _unclickTween.Insert(0.15f, HOTween.HOTween.From(sprite, _TweenDuration, "color", _tweenColor));
                    if (hasChildrenToTween) {
                        foreach (IHOtk2dBase childSprite in childrenSprites) {
                            _rolloutTween.Insert(0, HOTween.HOTween.From(childSprite, _TweenDuration, "color", _tweenColor));
                        }
                    }
                }
                _unclickTween.Complete();
            }

            // Execute eventual cued actions
            if (_preinitActionsQueue != null) {
                foreach (PreinitActionType visualActionType in _preinitActionsQueue) {
                    switch (visualActionType) {
                    case PreinitActionType.ToggleOn:
                        ToggleOn();
                        break;
                    case PreinitActionType.ToggleOnWithoutEventDispatching:
                        ToggleOn(false);
                        break;
                    case PreinitActionType.ToggleOff:
                        ToggleOff();
                        break;
                    case PreinitActionType.ToggleOffWithoutEventDispatching:
                        ToggleOff(false);
                        break;
                    }
                }
            }
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            if (_isOver) DoRollOut(true);
            if (_isPressed) DoRelease(false, true);
            if (_unclickTween != null && !_unclickTween.isPaused) _unclickTween.Complete();
            if (_tooltip != null) _tooltip.SetActive(false);
            HOtk2dGUIManager.RemoveButton(this);
            // Remove from eventual toggle group
            if (_toggleGroupid != "") {
                List<HOtk2dButton> bts = _TogglesByGroupId[_toggleGroupid];
                bts.RemoveAt(bts.IndexOf(this));
                if (bts.Count <= 0) _TogglesByGroupId.Remove(_toggleGroupid);
            }
        }

        void OnDestroy()
        {
            if (_rolloutTween != null) _rolloutTween.Kill();
            if (_unpressTween != null) _unpressTween.Kill();
            if (_unclickTween != null) _unclickTween.Kill();
            RollOver = null; RollOut = null;
            Press = null; Release = null;
            Click = null; Select = null; Deselect = null;
        }

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        /// <summary>
        /// Selects this button (only if it's a toggle button)
        /// </summary>
        /// <param name="dispatchEvents">If TRUE dispatches relative events, otherwise ignores them</param>
        public void ToggleOn(bool dispatchEvents = true)
        {
            if (!_initialized) {
                if (_preinitActionsQueue == null) _preinitActionsQueue = new Queue<PreinitActionType>();
                _preinitActionsQueue.Enqueue(dispatchEvents ? PreinitActionType.ToggleOn : PreinitActionType.ToggleOnWithoutEventDispatching);
                return;
            }
            if (_isToggle && !selected) DoSelect(dispatchEvents);
        }

        /// <summary>
        /// Deselects this button (only if it's a toggle button)
        /// </summary>
        /// <param name="dispatchEvents">If TRUE dispatches relative events, otherwise ignores them</param>
        public void ToggleOff(bool dispatchEvents = true)
        {
            if (!_initialized) {
                if (_preinitActionsQueue == null) _preinitActionsQueue = new Queue<PreinitActionType>();
                _preinitActionsQueue.Enqueue(dispatchEvents ? PreinitActionType.ToggleOff : PreinitActionType.ToggleOffWithoutEventDispatching);
                return;
            }
            if (_isToggle && selected) DoDeselect(dispatchEvents);
        }

        /// <summary>
        /// Disables the eventual tooltip
        /// </summary>
        public void DisableTooltip()
        {
            _showTooltip = false;
            if (_tooltip != null) _tooltip.gameObject.SetActive(false);
        }

        /// <summary>
        /// Enables the eventual tooltip (enabled by default)
        /// </summary>
        public void EnableTooltip()
        {
            _showTooltip = _tooltip != null;
            if (_isOver) _tooltip.SetActive(true);
        }

        /// <summary>
        /// Presses this button programmatically and dispatches relative events.
        /// </summary>
        public void SimulateRollOver()
        {
            _isSimulatingMouseFocus = true;
            Refresh(true, MouseState.Up, false);
        }

        /// <summary>
        /// Presses this button programmatically and dispatches relative events.
        /// </summary>
        public void SimulateRollOut()
        {
            _isSimulatingMouseFocus = false;
            Refresh(false, MouseState.Up, false);
        }

        /// <summary>
        /// Presses this button programmatically and dispatches relative events.
        /// </summary>
        public void SimulatePress()
        {
            Refresh(true, MouseState.JustPressed, false);
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        internal void Refresh(bool hasMouseFocus, MouseState mouseState, bool isMousePressed)
        {
            if (hasMouseFocus || _isSimulatingMouseFocus) {
                if ((hasRollover || _showTooltip) && !isMousePressed && !_isOver) DoRollOver();
                if (mouseState == MouseState.JustPressed && !_isPressed) {
                    DoPress();
//                    DoRelease(true);
                } else if (mouseState == MouseState.Released && _isPressed) DoRelease(true);
            } else if (_isOver || _isPressed) {
                if (_isPressed && mouseState == MouseState.Up) DoRelease(false);
                if (_isOver && !isMousePressed) DoRollOut();
            }
        }

        void ChangeCamera(Camera cam)
        {
            _guiCamera = cam;
            HOtk2dGUIManager.RemoveButton(this);
            HOtk2dGUIManager.AddButton(this);
        }

        void UpdateTextMeshes()
        {
            foreach (IHOtk2dTextMesh txtMesh in _txtMeshesToUpdate) {
                txtMesh.Commit();
            }
        }

        void DoRollOver()
        {
            if (!HOtk2dGUIManager.rolloversEnabled) return;

            _isOver = true;
            if (_isToggle && _toggleOn == ButtonActionType.OnRollover) {
                if (selected && !_isRadioButton) DoDeselect(); else DoSelect();
            } else {
                if (_rolloutTween != null) _rolloutTween.Rewind();
            }
            if (_showTooltip) _tooltip.SetActive(true);
            DispatchEvent(this, RollOver, HOtk2dGUIManager.OnRollOver, HOtk2dButtonEventType.RollOver);
        }

        void DoRollOut(bool instantTween = false)
        {
            if (!HOtk2dGUIManager.rolloversEnabled) return;

            _isOver = false;
            if (!_isToggle || _toggleOn != ButtonActionType.OnRollover && (!selected || selected && _toggleOnAnimation != ButtonActionType.OnRollover)) {
                if (_rolloutTween != null) {
                    if (instantTween) _rolloutTween.Complete();
                    else _rolloutTween.Play();
                }
            }
            if (_showTooltip) _tooltip.SetActive(false);
            DispatchEvent(this, RollOut, HOtk2dGUIManager.OnRollOut, HOtk2dButtonEventType.RollOut);
        }

        void DoPress()
        {
            _isPressed = true;
            if (_isToggle && _toggleOn == ButtonActionType.OnPress) {
                if (selected && !_isRadioButton) DoDeselect(); else DoSelect();
            } else {
                if (_unpressTween != null) _unpressTween.Rewind();
            }
            DispatchEvent(this, Press, HOtk2dGUIManager.OnPress, HOtk2dButtonEventType.Press);
        }

        void DoRelease(bool hasMouseFocus, bool instantTween = false)
        {
            _isPressed = false;
            if (!_isToggle || _toggleOn != ButtonActionType.OnPress && (!selected || selected && _toggleOnAnimation != ButtonActionType.OnPress)) {
                if (_unpressTween != null) {
                    if (instantTween) _unpressTween.Complete();
                    else _unpressTween.Restart();
                }
            }
            if (hasMouseFocus) DoClick();
            DispatchEvent(this, Release, HOtk2dGUIManager.OnRelease, HOtk2dButtonEventType.Release);
        }

        void DoClick()
        {
            if (_isToggle && _toggleOn == ButtonActionType.OnClick) {
                if (selected && !_isRadioButton) DoDeselect(); else DoSelect();
            } else {
                if (_unclickTween != null) _unclickTween.Restart();
            }
            DispatchEvent(this, Click, HOtk2dGUIManager.OnClick, HOtk2dButtonEventType.Click);
        }

        void DoSelect(bool dispatchEvents = true)
        {
            if (selected) return;
            if (_toggleGroupid != "") DeselectByGroupId(_toggleGroupid, dispatchEvents);
            selected = true;
            switch (_toggleOnAnimation) {
            case ButtonActionType.OnRollover:
                if (_rolloutTween != null) _rolloutTween.Rewind();
                break;
            case ButtonActionType.OnPress:
                if (_unpressTween != null) _unpressTween.Rewind();
                break;
            case ButtonActionType.OnClick:
                if (_unclickTween != null) _unclickTween.Rewind();
                break;
            }
            if (dispatchEvents) {
                DispatchEvent(this, Toggle, HOtk2dGUIManager.OnToggle, HOtk2dButtonEventType.Toggle);
                DispatchEvent(this, Select, HOtk2dGUIManager.OnSelect, HOtk2dButtonEventType.Select);
            }
        }

        void DoDeselect(bool dispatchEvents = true)
        {
            if (!selected) return;
            selected = false;
            switch (_toggleOnAnimation) {
            case ButtonActionType.OnRollover:
                if (_rolloutTween != null) _rolloutTween.Restart();
                break;
            case ButtonActionType.OnPress:
                if (_unpressTween != null) _unpressTween.Restart();
                break;
            case ButtonActionType.OnClick:
                if (_unclickTween != null) _unclickTween.Restart();
                break;
            }
            if (dispatchEvents) {
                DispatchEvent(this, Toggle, HOtk2dGUIManager.OnToggle, HOtk2dButtonEventType.Toggle);
                DispatchEvent(this, Deselect, HOtk2dGUIManager.OnDeselect, HOtk2dButtonEventType.Deselect);
            }
        }

        static void DeselectByGroupId(string id, bool dispatchEvents = true)
        {
            if (!_TogglesByGroupId.ContainsKey(id)) return;
            List<HOtk2dButton> buttons = _TogglesByGroupId[id];
            int len = buttons.Count - 1;
            for (int i = len; i > -1; --i) {
                if (i > buttons.Count - 1) continue;
                buttons[i].DoDeselect(dispatchEvents);
            }
        }

        static void DispatchEvent(HOtk2dButton button, HOTk2dButtonDelegate e, HOTk2dButtonDelegate eManager, HOtk2dButtonEventType type)
        {
            if (e != null) e(new HOtk2dButtonEvent(type, button));
            eManager(new HOtk2dButtonEvent(type, button));
        }
    }
}