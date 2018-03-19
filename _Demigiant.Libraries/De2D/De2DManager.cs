// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/19 11:35
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEngine;

namespace DG.De2D
{
    /// <summary>
    /// Global-permanent runtime component that fires callbacks in case screen changes
    /// (used to avoid having Update calls on all 2D components)
    /// </summary>
    public class De2DManager : MonoBehaviour
    {
        enum HorizontalAlignment
        {
            Left, Middle, Right
        }
        enum VerticalAlignment
        {
            Top, Middle, Bottom
        }

        #region EVENTS

        public static event Action OnScreenSizeChanged;
        static void Dispatch_OnScreenSizeChanged() { if (OnScreenSizeChanged != null) OnScreenSizeChanged(); }

        #endregion

        static De2DManager I;
        static int _currScreenWidth, _currScreenHeight;
        static MethodInfo _miGetEditorGameViewSize;

        #region Unity + INIT

        public static void Init()
        {
            if (I != null) return;

            I = new GameObject("[De2DManager]").AddComponent<De2DManager>();
            DontDestroyOnLoad(I.gameObject);
        }

        void Start()
        {
            _currScreenWidth = Screen.width;
            _currScreenHeight = Screen.height;
        }

        void OnDestroy()
        {
            if (I == this) I = null;
        }

        void Update()
        {
            if (_currScreenWidth == Screen.width && _currScreenHeight == Screen.height) return;

            _currScreenWidth = Screen.width;
            _currScreenHeight = Screen.height;
            Dispatch_OnScreenSizeChanged();
        }

        #endregion

        #region Public Methods

        public static void AlignToCamera(SpriteRenderer sprite, Camera camera, SpriteAlignment alignment, Vector2 extraOffset)
        {
            if (!camera.orthographic) {
                Debug.LogError(string.Format("Camera \"{0}\" is not set as orthographic", camera.name));
                return;
            }

            Vector2 gameviewSize = GetGameViewSize();
            float camVerticalSpanHalf = camera.orthographicSize;
            float camHorizontalSpanHalf = camVerticalSpanHalf * (gameviewSize.x / gameviewSize.y);
            Rect camRect = new Rect(
                camera.transform.position.x - camHorizontalSpanHalf, camera.transform.position.y - camVerticalSpanHalf,
                camHorizontalSpanHalf * 2, camVerticalSpanHalf * 2
            );

            Vector3 toPos = new Vector3(0, 0, sprite.transform.position.z);
            Bounds bounds = sprite.bounds;
            Vector2 pivotRatio = new Vector2(sprite.sprite.pivot.x / sprite.sprite.rect.width, sprite.sprite.pivot.y / sprite.sprite.rect.height);
            HorizontalAlignment horAlignment = GetHorizontalAlignment(alignment);
            VerticalAlignment vertAlignment = GetVerticalAlignment(alignment);
            switch (horAlignment) {
            case HorizontalAlignment.Left:
                toPos.x = camRect.xMin + bounds.size.x * pivotRatio.x;
                break;
            case HorizontalAlignment.Middle:
                toPos.x = camRect.center.x - (bounds.extents.x - bounds.size.x * pivotRatio.x);
                break;
            case HorizontalAlignment.Right:
                toPos.x = camRect.xMax - (bounds.size.x - bounds.size.x * pivotRatio.x);
                break;
            }
            switch (vertAlignment) {
            case VerticalAlignment.Top:
                toPos.y = camRect.yMax - (bounds.size.y - bounds.size.y * pivotRatio.y);
                break;
            case VerticalAlignment.Middle:
                toPos.y = camRect.center.y - (bounds.extents.y - bounds.size.y * pivotRatio.y);
                break;
            case VerticalAlignment.Bottom:
                toPos.y = camRect.yMin + bounds.size.y * pivotRatio.y;
                break;
            }

            sprite.transform.position = toPos + new Vector3(extraOffset.x, extraOffset.y, 0);
        }

        #endregion

        #region Methods

        static Vector2 GetGameViewSize()
        {
            if (Application.isPlaying) return new Vector2(Screen.width, Screen.height);
            else {
                if (_miGetEditorGameViewSize == null) {
                    Type t = Type.GetType("UnityEditor.Handles,UnityEditor.dll");
                    _miGetEditorGameViewSize = t.GetMethod("GetMainGameViewSize", BindingFlags.Static | BindingFlags.Public);
                }
                return (Vector2)_miGetEditorGameViewSize.Invoke(null, null);
            }
        }

        #endregion

        #region Helpers

        static HorizontalAlignment GetHorizontalAlignment(SpriteAlignment alignment)
        {
            switch (alignment) {
            case SpriteAlignment.BottomLeft:
            case SpriteAlignment.LeftCenter:
            case SpriteAlignment.TopLeft:
                return HorizontalAlignment.Left;
            case SpriteAlignment.BottomRight:
            case SpriteAlignment.RightCenter:
            case SpriteAlignment.TopRight:
                return HorizontalAlignment.Right;
            default:
                return HorizontalAlignment.Middle;
            }
        }

        static VerticalAlignment GetVerticalAlignment(SpriteAlignment alignment)
        {
            switch (alignment) {
            case SpriteAlignment.TopLeft:
            case SpriteAlignment.TopRight:
            case SpriteAlignment.TopCenter:
                return VerticalAlignment.Top;
            case SpriteAlignment.BottomLeft:
            case SpriteAlignment.BottomRight:
            case SpriteAlignment.BottomCenter:
                return VerticalAlignment.Bottom;
            default:
                return VerticalAlignment.Middle;
            }
        }

        #endregion
    }
}