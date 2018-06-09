// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/06/07 21:49
// License Copyright (c) Daniele Giardini

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DG.De2D
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DeSpriteButton : MonoBehaviour
    {
        enum State
        {
            Normal,
            Rollover,
            Press
        }

        #region EVENTS

        public event Action OnEditorRefreshRequired;
        void Dispatch_OnEditorRefreshRequired() { if (OnEditorRefreshRequired != null) OnEditorRefreshRequired(); }

        #endregion

        #region Serialized

        [SerializeField] bool _interactable = true;
        public ColorBlock colors = ColorBlock.defaultColorBlock;
        [SerializeField] bool _showOnClick = true; // Editor-only
        [SerializeField] bool _showOnPress, _showOnRelease; // Editor-only
        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onPress = new UnityEvent();
        public UnityEvent onRelease = new UnityEvent();

        #endregion

        // Extra non-serialized events
        public UnityEvent onEnter { get { if (_onEnter == null) _onEnter = new UnityEvent(); return _onEnter; } }
        public UnityEvent onExit { get { if (_onExit == null) _onExit = new UnityEvent(); return _onExit; } }
        UnityEvent _onEnter;
        UnityEvent _onExit;
        //
        public bool interactable { get { return _interactable; } set { SetInteractable(value); } }

        bool _initialized;
        State _state = State.Normal;
        SpriteRenderer _spriteR;
        bool _isOver;
        bool _isDown;
        Coroutine _coColorTween;

        #region Unity + INIT

        void Init()
        {
            if (_initialized) return;

            _initialized = true;

            _spriteR = this.GetComponent<SpriteRenderer>();
        }

        void OnEnable()
        {
            Refresh(true);
        }

        void Awake()
        {
            Init();
        }

        void Reset()
        {
            Dispatch_OnEditorRefreshRequired();
        }

        void OnMouseEnter()
        {
            _state = _isDown ? State.Press : State.Rollover;
            _isOver = true;
            if (!_interactable) return;

            Refresh();
            if (onEnter != null) onEnter.Invoke();
        }

        void OnMouseDown()
        {
            _state = State.Press;
            _isDown = true;
            if (!_interactable) return;

            Refresh();
            if (onPress != null) onPress.Invoke();
        }

        void OnMouseUpAsButton()
        {
            _state = State.Rollover;
            _isDown = false;
            if (!_interactable) return;

            Refresh();
            if (onClick != null) onClick.Invoke();
        }

        void OnMouseUp()
        {
            _state = _isOver ? State.Rollover : State.Normal;
            _isDown = false;
            if (!_interactable) return;

            Refresh();
            if (onRelease != null) onRelease.Invoke();
        }

        void OnMouseExit()
        {
            _state = _isDown ? State.Press : State.Normal;
            _isOver = false;
            if (!_interactable) return;

            Refresh();
            if (onExit != null) onExit.Invoke();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Force-refreshes the graphics of this button
        /// </summary>
        public void Refresh(bool immediate = false)
        {
            Init();

            if (!_interactable) {
                TweenColorTo(colors.disabledColor, immediate ? -1 : 0.1f);
            } else {
                switch (_state) {
                case State.Rollover:
                    TweenColorTo(colors.highlightedColor, immediate ? -1 : colors.fadeDuration);
                    break;
                case State.Press:
                    TweenColorTo(colors.pressedColor, -1);
                    break;
                default:
                    TweenColorTo(colors.normalColor, immediate ? -1 : colors.fadeDuration);
                    break;
                }
            }
        }

        #endregion

        #region Methods

        void SetInteractable(bool value, bool immediate = false)
        {
            if (_interactable == value) return;

            _interactable = value;
            Refresh(immediate);
            if (_isOver && onExit != null) onExit.Invoke();
            if (_isDown && onRelease != null) onRelease.Invoke();
        }

        #region Tweens

        void TweenColorTo(Color color, float duration)
        {
            if (_coColorTween != null) {
                this.StopCoroutine(_coColorTween);
                _coColorTween = null;
            }
            if (duration <= 0) {
                _spriteR.color = color;
                return;
            }
            if (_spriteR.color.Equals(color)) return;

            _coColorTween = this.StartCoroutine(CO_ColorTo(color, duration));
        }

        IEnumerator CO_ColorTo(Color color, float duration)
        {
            if (duration <= 0) {
                _spriteR.color = color;
                yield break;
            }
            Color startColor = _spriteR.color;
            float startTime = Time.realtimeSinceStartup;
            bool complete = false;
            while (!complete) {
                float elapsed = Time.realtimeSinceStartup - startTime;
                float elapsedPerc = Mathf.Min(1, elapsed / duration);
                _spriteR.color = Color.Lerp(startColor, color, elapsedPerc);
                if (elapsed > duration) complete = true;
                else yield return null;
            }
            _coColorTween = null;
        }

        #endregion

        #endregion
    }
}