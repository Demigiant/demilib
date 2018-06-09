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
        public UnityEvent onClick, onPress, onRelease;

        #endregion

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
            Refresh();
        }

        void OnMouseDown()
        {
            _state = State.Press;
            _isDown = true;
            Refresh();
            if (onPress != null) onPress.Invoke();
        }

        void OnMouseUpAsButton()
        {
            _state = State.Rollover;
            _isDown = false;
            Refresh();
            if (onClick != null) onClick.Invoke();
        }

        void OnMouseUp()
        {
            _state = _isOver ? State.Rollover : State.Normal;
            _isDown = false;
            Refresh();
            if (onRelease != null) onRelease.Invoke();
        }

        void OnMouseExit()
        {
            _state = _isDown ? State.Press : State.Normal;
            _isOver = false;
            Refresh();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Force-refreshes the graphics of this button
        /// </summary>
        public void Refresh()
        {
            Init();

            if (!_interactable) {
                TweenColorTo(colors.disabledColor, colors.fadeDuration);
            } else {
                switch (_state) {
                case State.Rollover:
                    TweenColorTo(colors.highlightedColor, colors.fadeDuration);
                    break;
                case State.Press:
                    TweenColorTo(colors.pressedColor, -1);
                    break;
                default:
                    TweenColorTo(colors.normalColor, colors.fadeDuration);
                    break;
                }
            }
        }

        #endregion

        #region Methods

        void SetInteractable(bool value)
        {
            if (_interactable == value) return;

            _interactable = value;
            Refresh();
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