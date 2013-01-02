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
        public event HOTk2dButtonDelegate RollOver;
        public event HOTk2dButtonDelegate RollOut;
        public event HOTk2dButtonDelegate Press;
        public event HOTk2dButtonDelegate Release;
        public event HOTk2dButtonDelegate Click;
        public event HOTk2dButtonDelegate Select;
        public event HOTk2dButtonDelegate Deselect;
        public event HOTk2dButtonDelegate Toggle;

        enum PreinitActionType
        {
            ToggleOn,
            ToggleOff
        }

        /// <summary>
        /// Returns TRUE if this button is a toggle and is actually selected
        /// </summary>
        public bool selected { get; private set; }
        public Transform trans { get { if (_fooTrans == null) _fooTrans = transform; return _fooTrans; } }
        public IHOtk2dSprite sprite { get { if (_fooSprite == null) _fooSprite = this.GetComponent(typeof(IHOtk2dSprite)) as IHOtk2dSprite; return _fooSprite; } }
        public Camera guiCamera {
            get { if (_guiCamera == null) _guiCamera = HOtk2dGUIManager.defaultGuiCamera; return _guiCamera; }
            set { ChangeCamera(value); }
        }

        internal bool hasRollover { get { return _tweenColorOn == ButtonActionType.OnRollover || _tweenScaleOn == ButtonActionType.OnRollover; } }

        [SerializeField] Camera _guiCamera;
        [SerializeField] ButtonActionType _tweenColorOn = ButtonActionType.None;
        [SerializeField] ButtonActionType _tweenScaleOn = ButtonActionType.None;
        [SerializeField] float _tweenScaleMultiplier = 1.1f;
        [SerializeField] Color _tweenColor = Color.white;
        [SerializeField] bool _isToggle = false;
        [SerializeField] ButtonActionType _toggleOn = ButtonActionType.OnPress;
        [SerializeField] string _toggleGroupid = "";

        const float _TweenDuration = 0.25f;
        static readonly Dictionary<string, List<HOtk2dButton>> _TogglesByGroupId = new Dictionary<string, List<HOtk2dButton>>();

        bool _initialized;
        bool _isOver;
        bool _isPressed;
        Queue<PreinitActionType> _preinitActionsQueue; // Actions that are stored in case they're called before Start
        Sequence _rolloutTween;
        Sequence _unpressTween;
        Sequence _unclickTween;
        Transform _fooTrans;
        IHOtk2dSprite _fooSprite;

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

            // Create tweens
            if (hasRollover) {
                _rolloutTween = new Sequence(new SequenceParms().UpdateType(UpdateType.TimeScaleIndependentUpdate).AutoKill(false));
                if (_tweenScaleOn == ButtonActionType.OnRollover)
                    _rolloutTween.Insert(0, HOTween.HOTween.From(sprite, _TweenDuration, "scale", sprite.scale * _tweenScaleMultiplier));
                if (_tweenColorOn == ButtonActionType.OnRollover)
                    _rolloutTween.Insert(0, HOTween.HOTween.From(sprite, _TweenDuration, "color", _tweenColor));
                _rolloutTween.Complete();
            }
            if (_tweenColorOn == ButtonActionType.OnPress || _tweenScaleOn == ButtonActionType.OnPress) {
                _unpressTween = new Sequence(new SequenceParms().UpdateType(UpdateType.TimeScaleIndependentUpdate).AutoKill(false));
                if (_tweenScaleOn == ButtonActionType.OnPress)
                    _unpressTween.Insert(0, HOTween.HOTween.From(sprite, _TweenDuration, "scale", sprite.scale * _tweenScaleMultiplier));
                if (_tweenColorOn == ButtonActionType.OnPress)
                    _unpressTween.Insert(0, HOTween.HOTween.From(sprite, _TweenDuration, "color", _tweenColor));
                _unpressTween.Complete();
            }
            if (_tweenColorOn == ButtonActionType.OnClick || _tweenScaleOn == ButtonActionType.OnClick) {
                _unclickTween = new Sequence(new SequenceParms().UpdateType(UpdateType.TimeScaleIndependentUpdate).AutoKill(false));
                if (_tweenScaleOn == ButtonActionType.OnClick)
                    _unclickTween.Insert(0.15f, HOTween.HOTween.From(sprite, _TweenDuration, "scale", sprite.scale * _tweenScaleMultiplier));
                if (_tweenColorOn == ButtonActionType.OnClick)
                    _unclickTween.Insert(0.15f, HOTween.HOTween.From(sprite, _TweenDuration, "color", _tweenColor));
                _unclickTween.Complete();
            }

            // Execute eventual cued actions
            if (_preinitActionsQueue != null) {
                foreach (PreinitActionType visualActionType in _preinitActionsQueue) {
                    switch (visualActionType) {
                    case PreinitActionType.ToggleOn:
                        ToggleOn();
                        break;
                    case PreinitActionType.ToggleOff:
                        ToggleOff();
                        break;
                    }
                }
            }
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            if (_isOver) DoRollOut();
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
        public void ToggleOn()
        {
            if (!_initialized) {
                if (_preinitActionsQueue == null) _preinitActionsQueue = new Queue<PreinitActionType>();
                _preinitActionsQueue.Enqueue(PreinitActionType.ToggleOn);
                return;
            }
            if (_isToggle && !selected) DoSelect();
        }

        /// <summary>
        /// Deselects this button (only if it's a toggle button)
        /// </summary>
        public void ToggleOff()
        {
            if (!_initialized) {
                if (_preinitActionsQueue == null) _preinitActionsQueue = new Queue<PreinitActionType>();
                _preinitActionsQueue.Enqueue(PreinitActionType.ToggleOff);
                return;
            }
            if (_isToggle && selected) DoDeselect();
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        internal void Refresh(bool hasMouseFocus, MouseState mouseState, bool isMousePressed)
        {
            if (hasMouseFocus) {
                if (hasRollover && !isMousePressed && !_isOver) DoRollOver();
                if (mouseState == MouseState.JustPressed && !_isPressed) DoPress();
                else if (mouseState == MouseState.Released && _isPressed) DoRelease(true);
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

        void DoRollOver()
        {
            _isOver = true;
            if (_isToggle && _toggleOn == ButtonActionType.OnRollover) {
                if (selected) DoDeselect(); else DoSelect();
            } else {
                if (_rolloutTween != null) _rolloutTween.Rewind();
            }
            DispatchEvent(this, RollOver, HOtk2dGUIManager.OnRollOver, HOtk2dButtonEventType.RollOver);
        }

        void DoRollOut()
        {
            _isOver = false;
            if (!_isToggle || _toggleOn != ButtonActionType.OnRollover) {
                if (_rolloutTween != null) _rolloutTween.Restart();
            }
            DispatchEvent(this, RollOut, HOtk2dGUIManager.OnRollOut, HOtk2dButtonEventType.RollOut);
        }

        void DoPress()
        {
            _isPressed = true;
            if (_isToggle && _toggleOn == ButtonActionType.OnPress) {
                if (selected) DoDeselect(); else DoSelect();
            } else {
                if (_unpressTween != null) _unpressTween.Rewind();
            }
            DispatchEvent(this, Press, HOtk2dGUIManager.OnPress, HOtk2dButtonEventType.Press);
        }

        void DoRelease(bool hasMouseFocus)
        {
            _isPressed = false;
            if (!_isToggle || _toggleOn != ButtonActionType.OnPress) {
                if (_unpressTween != null) _unpressTween.Restart();
            }
            if (hasMouseFocus) DoClick();
            DispatchEvent(this, Release, HOtk2dGUIManager.OnRelease, HOtk2dButtonEventType.Release);
        }

        void DoClick()
        {
            if (_isToggle && _toggleOn == ButtonActionType.OnClick) {
                if (selected) DoDeselect(); else DoSelect();
            } else {
                if (_unclickTween != null) _unclickTween.Restart();
            }
            DispatchEvent(this, Click, HOtk2dGUIManager.OnClick, HOtk2dButtonEventType.Click);
        }

        void DoSelect()
        {
            if (selected) return;
            if (_toggleGroupid != "") DeselectByGroupId(_toggleGroupid);
            selected = true;
            switch (_toggleOn) {
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
            DispatchEvent(this, Toggle, HOtk2dGUIManager.OnToggle, HOtk2dButtonEventType.Toggle);
            DispatchEvent(this, Select, HOtk2dGUIManager.OnSelect, HOtk2dButtonEventType.Select);
        }

        void DoDeselect()
        {
            if (!selected) return;
            selected = false;
            switch (_toggleOn) {
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
            DispatchEvent(this, Toggle, HOtk2dGUIManager.OnToggle, HOtk2dButtonEventType.Toggle);
            DispatchEvent(this, Deselect, HOtk2dGUIManager.OnDeselect, HOtk2dButtonEventType.Deselect);
        }

        static void DeselectByGroupId(string id)
        {
            List<HOtk2dButton> buttons = _TogglesByGroupId[id];
            foreach (HOtk2dButton button in buttons) button.DoDeselect();
        }

        static void DispatchEvent(HOtk2dButton button, HOTk2dButtonDelegate e, HOTk2dButtonDelegate eManager, HOtk2dButtonEventType type)
        {
            if (e != null) e(new HOtk2dButtonEvent(type, button));
            eManager(new HOtk2dButtonEvent(type, button));
        }
    }
}