// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/15 21:30

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

        public Transform trans { get { if (_fooTrans == null) _fooTrans = transform; return _fooTrans; } }
        public Collider coll { get { if (_fooColl == null) _fooColl = collider; return _fooColl; } }
        public IHOtk2dSprite sprite { get { if (_fooSprite == null) _fooSprite = this.GetComponent(typeof(IHOtk2dSprite)) as IHOtk2dSprite; return _fooSprite; } }
        public Camera cam { get { if (_camera == null) _camera = Camera.main; return _camera; } }

        internal bool hasRollover { get { return _tweenColorOn == TweenMode.OnRollover || _tweenScaleOn == TweenMode.OnRollover; } }

        [SerializeField] Camera _camera;
        [SerializeField] TweenMode _tweenColorOn = TweenMode.None;
        [SerializeField] TweenMode _tweenScaleOn = TweenMode.None;
        [SerializeField] float _tweenScaleMultiplier = 1.1f;
        [SerializeField] Color _tweenColor = Color.white;

        const float _TweenDuration = 0.25f;

        bool _initialized;
        bool _isOver;
        bool _isPressed;
        Sequence _deselectTween;
        Sequence _rollout;
        Transform _fooTrans;
        Collider _fooColl;
        IHOtk2dSprite _fooSprite;

        // ===================================================================================
        // UNITY METHODS ---------------------------------------------------------------------

        protected virtual void OnEnable()
        {
            if (!_initialized) {
                _initialized = true;
                // Create tweens
                if (hasRollover) {
                    _rollout = new Sequence(new SequenceParms().UpdateType(UpdateType.TimeScaleIndependentUpdate).AutoKill(false));
                    if (_tweenScaleOn == TweenMode.OnRollover)
                        _rollout.Insert(0, HOTween.HOTween.From(sprite, _TweenDuration, "scale", sprite.scale * _tweenScaleMultiplier));
                    if (_tweenColorOn == TweenMode.OnRollover)
                        _rollout.Insert(0, HOTween.HOTween.From(sprite, _TweenDuration, "color", _tweenColor));
                    _rollout.Complete();
                }
                _deselectTween = new Sequence(new SequenceParms().UpdateType(UpdateType.TimeScaleIndependentUpdate).AutoKill(false));
                if (_tweenScaleOn == TweenMode.OnPress)
                    _deselectTween.Insert(0, HOTween.HOTween.From(sprite, _TweenDuration, "scale", sprite.scale * _tweenScaleMultiplier));
                if (_tweenColorOn == TweenMode.OnPress)
                    _deselectTween.Insert(0, HOTween.HOTween.From(sprite, _TweenDuration, "color", _tweenColor));
                _deselectTween.Complete();
            }
            HOtk2dGUIManager.AddButton(this);
        }

        protected virtual void OnDisable()
        {
            HOtk2dGUIManager.RemoveButton(this);
        }

        void OnDestroy()
        {
            if (_rollout != null) _rollout.Kill();
            if (_deselectTween != null) _deselectTween.Kill();
            RollOver = null; RollOut = null;
            Press = null; Release = null;
            Click = null;
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
            _rollout.Rewind();
            if (RollOver != null) RollOver(new HOtk2dButtonEvent(HOtk2dButtonEventType.RollOver, this));
        }

        void DoRollOut()
        {
            _isOver = false;
            _rollout.Restart();
            if (RollOut != null) RollOut(new HOtk2dButtonEvent(HOtk2dButtonEventType.RollOut, this));
        }

        void DoPress()
        {
            _isPressed = true;
            _deselectTween.Rewind();
            if (Press != null) Press(new HOtk2dButtonEvent(HOtk2dButtonEventType.Press, this));
        }

        void DoRelease(bool hasMouseFocus)
        {
            _isPressed = false;
            _deselectTween.Restart();
            if (hasMouseFocus) DoClick();
            if (Release != null) Release(new HOtk2dButtonEvent(HOtk2dButtonEventType.Release, this));
        }

        void DoClick()
        {
            if (Click != null) Click(new HOtk2dButtonEvent(HOtk2dButtonEventType.Click, this));
        }
    }
}