// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/02/13 11:17
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.De2D
{
    /// <summary>
    /// Sets up the 2D orthographic camera it's attached to
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class De2DCamera : MonoBehaviour
    {
        public enum OrthoMode
        {
            FixedHeight, // Default ortho behaviour
            FixedWidth
        }

        public int targetSize = 960; // Target width or height, depending on CameraMode
        public int ppu = 100; // Pixels per unit
        public OrthoMode orthoMode = OrthoMode.FixedHeight;
        // Editor-only
        public bool editorAutoRefresh = true; // If TRUE, refreshes the ortho-size automatically while in editor mode

        Camera _thisCam;

        #region Unity Methods

        void Start()
        {
            Adapt();
        }

        #endregion

        #region Public Methods

        public void Adapt()
        {
            // Store the cam
            if (_thisCam == null) {
                _thisCam = this.GetComponent<Camera>();
                if (_thisCam == null) {
                    Debug.LogWarning(string.Format("De2DCamera ::: No Camera found on \"{0}\"", this.name));
                    return;
                }
            }

            // Assign ortho value
            if (!_thisCam.orthographic) {
                _thisCam.orthographic = true;
                Debug.Log(string.Format("De2DCamera ::: Setting \"{0}\" camera to orthographic", this.name));
            }
            _thisCam.orthographicSize = (targetSize / (float)ppu) * 0.5f;
            if (orthoMode == OrthoMode.FixedWidth) {
                float ratio = (float)Screen.height / Screen.width;
                _thisCam.orthographicSize *= ratio;
            }
        }

        #endregion
    }
}