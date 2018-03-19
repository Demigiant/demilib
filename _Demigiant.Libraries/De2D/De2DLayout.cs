// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/19 11:26
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.De2D
{
    /// <summary>
    /// Works even if disabled (unless it's disabled in-editor)
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class De2DLayout : MonoBehaviour
    {
        #region Serialized

        [SerializeField] Camera _camera;
        [SerializeField] SpriteAlignment _alignment;
        [SerializeField] Vector2 _offset = Vector2.zero;

        #endregion

        public Camera cam { get { return _camera; } set { _camera = value; if (_camera != null) Refresh(); } }

        SpriteRenderer _spriteR;

        // Editor-only
        int _currScreenWidth, _currScreenHeight;

        #region Unity

        void Start()
        {
            if (!this.enabled) return;

            if (Application.isPlaying) {
                De2DManager.Init();
                // Disable so Update is not called every frame, and it just listens to De2DManager's events
                this.enabled = false;
                De2DManager.OnScreenSizeChanged += Refresh;
            }

            // NOTE: Refresh won't work correctly in Awake but only in Start (because otherwise Sprite bounds come out wrong)
            Refresh();
        }

        void OnDestroy()
        {
            De2DManager.OnScreenSizeChanged -= Refresh;
        }

        // Disabled at runtime
        void Update()
        {
            if (_currScreenWidth == Screen.width && _currScreenHeight == Screen.height) return;

            Refresh();
        }

        #endregion

        #region Internal Methods

        internal void Refresh()
        {
            _currScreenWidth = Screen.width;
            _currScreenHeight = Screen.height;
            if (_camera == null || !_camera.orthographic) {
                if (_camera == null) {
                    if (Application.isPlaying) Debug.LogWarning(string.Format("De2DLayout ► Camera not set for \"{0}\"", this.name), this);
                } else if (!_camera.orthographic) {
                    Debug.LogWarning(string.Format("De2DLayout ► Camera \"{0}\" (used by \"{1}\") is not set to orthographic", _camera.name, this.name), _camera);
                }
                return;
            }

            if (_spriteR == null) _spriteR = this.GetComponent<SpriteRenderer>();
            De2DManager.AlignToCamera(_spriteR, _camera, _alignment, _offset);
        }

        #endregion
    }
}