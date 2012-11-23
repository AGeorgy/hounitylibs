// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/15 21:30

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
        public event HOTk2dButtonDelegate Toggle;
        public event HOTk2dButtonDelegate Untoggle;

        /// <summary>
        /// Returns TRUE if this button is a toggle and is actually toggled/selected
        /// </summary>
        public bool isToggled { get; private set; }
        public Transform trans { get { if (_fooTrans == null) _fooTrans = transform; return _fooTrans; } }
        public IHOtk2dSprite sprite { get { if (_fooSprite == null) _fooSprite = this.GetComponent(typeof(IHOtk2dSprite)) as IHOtk2dSprite; return _fooSprite; } }
        public Camera cam { get { if (_camera == null) _camera = Camera.main; return _camera; } }

        internal bool hasRollover { get { return _tweenColorOn == ButtonActionType.OnRollover || _tweenScaleOn == ButtonActionType.OnRollover; } }

        [SerializeField] Camera _camera;
        [SerializeField] ButtonActionType _tweenColorOn = ButtonActionType.None;
        [SerializeField] ButtonActionType _tweenScaleOn = ButtonActionType.None;
        [SerializeField] float _tweenScaleMultiplier = 1.1f;
        [SerializeField] Color _tweenColor = Color.white;
        [SerializeField] bool _isToggle = false;
        [SerializeField] ButtonActionType _toggleOn = ButtonActionType.OnPress;
        [SerializeField] string _toggleGroupid = "";

        const float _TweenDuration = 0.25f;
        static readonly Dictionary<string, List<HOtk2dButton>> _ButtonsByGroupId = new Dictionary<string, List<HOtk2dButton>>();

        bool _initialized;
        bool _isOver;
        bool _isPressed;
        Sequence _rolloutTween;
        Sequence _unpressTween;
        Sequence _unclickTween;
        Transform _fooTrans;
        IHOtk2dSprite _fooSprite;

        // ===================================================================================
        // UNITY METHODS ---------------------------------------------------------------------

        protected virtual void OnEnable()
        {
            if (!_initialized) {
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
            }
            HOtk2dGUIManager.AddButton(this);
            // Add to eventual toggle group
            if (_isToggle && _toggleGroupid != "") {
                if (_toggleGroupid == "") return;
                if (_ButtonsByGroupId.ContainsKey(_toggleGroupid)) _ButtonsByGroupId.Add(_toggleGroupid, new List<HOtk2dButton>());
                _ButtonsByGroupId[_toggleGroupid].Add(this);
            }
        }

        protected virtual void OnDisable()
        {
            HOtk2dGUIManager.RemoveButton(this);
            // Remove from eventual toggle group
            if (_toggleGroupid != "") {
                List<HOtk2dButton> bts = _ButtonsByGroupId[_toggleGroupid];
                bts.RemoveAt(bts.IndexOf(this));
                if (bts.Count <= 0) _ButtonsByGroupId.Remove(_toggleGroupid);
            }
        }

        void OnDestroy()
        {
            if (_rolloutTween != null) _rolloutTween.Kill();
            if (_unpressTween != null) _unpressTween.Kill();
            if (_unclickTween != null) _unclickTween.Kill();
            RollOver = null; RollOut = null;
            Press = null; Release = null;
            Click = null; Toggle = null; Untoggle = null;
        }

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        /// <summary>
        /// Toggles this button (only if it's a toggle button)
        /// </summary>
        public void SelectToggle()
        {
            if (_isToggle && !isToggled) DoToggle();
        }

        /// <summary>
        /// Untoggles this button (only if it's a toggle button)
        /// </summary>
        public void DeselectToggle()
        {
            if (_isToggle && isToggled) DoUntoggle();
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

        void DoRollOver()
        {
            _isOver = true;
            if (_isToggle && _toggleOn == ButtonActionType.OnRollover) {
                if (isToggled) DoUntoggle(); else DoToggle();
            } else {
                if (_rolloutTween != null) _rolloutTween.Rewind();
            }
            DispatchEvent(RollOver, HOtk2dGUIManager.OnRollOver, HOtk2dButtonEventType.RollOver);
        }

        void DoRollOut()
        {
            _isOver = false;
            if (!_isToggle || _toggleOn != ButtonActionType.OnRollover) {
                if (_rolloutTween != null) _rolloutTween.Restart();
            }
            DispatchEvent(RollOut, HOtk2dGUIManager.OnRollOut, HOtk2dButtonEventType.RollOut);
        }

        void DoPress()
        {
            _isPressed = true;
            if (_isToggle && _toggleOn == ButtonActionType.OnPress) {
                if (isToggled) DoUntoggle(); else DoToggle();
            } else {
                if (_unpressTween != null) _unpressTween.Rewind();
            }
            DispatchEvent(Press, HOtk2dGUIManager.OnPress, HOtk2dButtonEventType.Press);
        }

        void DoRelease(bool hasMouseFocus)
        {
            _isPressed = false;
            if (!_isToggle || _toggleOn != ButtonActionType.OnPress) {
                if (_unpressTween != null) _unpressTween.Restart();
            }
            if (hasMouseFocus) DoClick();
            DispatchEvent(Release, HOtk2dGUIManager.OnRelease, HOtk2dButtonEventType.Release);
        }

        void DoClick()
        {
            if (_isToggle && _toggleOn == ButtonActionType.OnClick) {
                if (isToggled) DoUntoggle(); else DoToggle();
            } else {
                if (_unclickTween != null) _unclickTween.Restart();
            }
            DispatchEvent(Click, HOtk2dGUIManager.OnClick, HOtk2dButtonEventType.Click);
        }

        void DoToggle()
        {
            isToggled = true;
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
            DispatchEvent(Toggle, HOtk2dGUIManager.OnToggle, HOtk2dButtonEventType.Toggle);
        }

        void DoUntoggle()
        {
            isToggled = false;
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
            DispatchEvent(Untoggle, HOtk2dGUIManager.OnUntoggle, HOtk2dButtonEventType.Untoggle);
        }

        void DispatchEvent(HOTk2dButtonDelegate e, HOTk2dButtonDelegate eManager, HOtk2dButtonEventType type)
        {
            if (e != null) e(new HOtk2dButtonEvent(type, this));
            eManager(new HOtk2dButtonEvent(type, this));
        }
    }
}