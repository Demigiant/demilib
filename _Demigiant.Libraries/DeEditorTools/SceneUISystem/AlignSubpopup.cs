// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/01 19:25
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DG.DeEditorTools.SceneUISystem
{
    public class AlignSubpopup : PopupWindowContent
    {
        enum HorizontalAlignment
        {
            Left, Middle, Right
        }
        enum VerticalAlignment
        {
            Top, Middle, Bottom
        }

        const int _Padding = 2;
        const int _IcoDistance = 2;
        static int _icoSize;
        Camera _alignToCamera;
        List<SpriteRenderer> _spriteRenderers;

        static GUIStyle _icoBt;

        public AlignSubpopup(Camera alignToCamera, List<SpriteRenderer> spriteRenderers)
        {
            _alignToCamera = alignToCamera;
            _spriteRenderers = spriteRenderers;
        }

        #region GUI

        public override Vector2 GetWindowSize()
        {
            _icoSize = DeStylePalette.ico_alignTL.width;
            float panelSize = _icoSize * 3 + _Padding * 2 + _IcoDistance * 2;
            return new Vector2(panelSize, panelSize); 
        }

        public override void OnGUI(Rect rect)
        {
            SetStyles();

            bool closeAllPopups = false;

            Rect icoRect = new Rect(0, 0, _icoSize, _icoSize);
            for (int c = 0; c < 3; ++c) {
                icoRect.y = _Padding + _icoSize * c + _IcoDistance * c;
                for (int r = 0; r < 3; r++) {
                    icoRect.x = _Padding + _icoSize * r + _IcoDistance * r;
                    bool executed = DrawAlignButton(icoRect, c, r);
                    if (!closeAllPopups && executed) closeAllPopups = true;
                }
            }

            if (closeAllPopups) SceneUIPopup.CloseAll();
            else if (Event.current.type == EventType.MouseMove) this.editorWindow.Repaint();
        }

        // Returns TRUE if an align operation happened
        bool DrawAlignButton(Rect r, int column, int row)
        {
            TextAnchor alignment;
            Texture2D icoTex;
            switch (column) {
            case 0:
                switch (row) {
                case 0:
                    alignment = TextAnchor.UpperLeft;
                    icoTex = DeStylePalette.ico_alignTL;
                    break;
                case 1:
                    alignment = TextAnchor.UpperCenter;
                    icoTex = DeStylePalette.ico_alignTC;
                    break;
                default:
                    alignment = TextAnchor.UpperRight;
                    icoTex = DeStylePalette.ico_alignTR;
                    break;
                }
                break;
            case 1:
                switch (row) {
                case 0:
                    alignment = TextAnchor.MiddleLeft;
                    icoTex = DeStylePalette.ico_alignCL;
                    break;
                case 1:
                    alignment = TextAnchor.MiddleCenter;
                    icoTex = DeStylePalette.ico_alignCC;
                    break;
                default:
                    alignment = TextAnchor.MiddleRight;
                    icoTex = DeStylePalette.ico_alignCR;
                    break;
                }
                break;
            default:
                switch (row) {
                case 0:
                    alignment = TextAnchor.LowerLeft;
                    icoTex = DeStylePalette.ico_alignBL;
                    break;
                case 1:
                    alignment = TextAnchor.LowerCenter;
                    icoTex = DeStylePalette.ico_alignBC;
                    break;
                default:
                    alignment = TextAnchor.LowerRight;
                    icoTex = DeStylePalette.ico_alignBR;
                    break;
                }
                break;
            }
            if (Event.current.type == EventType.Repaint && r.Contains(Event.current.mousePosition)) {
                // Evidence background
                GUI.DrawTexture(r, DeStylePalette.blueSquare);
            }
            if (GUI.Button(r, icoTex, _icoBt)) {
                AlignToCamera(alignment, _alignToCamera, _spriteRenderers);
                return true;
            }
            return false;
        }

        static void SetStyles()
        {
            if (_icoBt != null) return;

            _icoBt = new GUIStyle(GUI.skin.button).Padding(0).Margin(0).Background(null);
        }

        #endregion

        #region Methods

        static void AlignToCamera(TextAnchor alignment, Camera camera, List<SpriteRenderer> spriteRenderers)
        {
            Vector2 gameviewSize = DeEditorUtils.GetGameViewSize();
            float camVerticalSpanHalf = camera.orthographicSize;
            float camHorizontalSpanHalf = camVerticalSpanHalf * (gameviewSize.x / gameviewSize.y);
            Rect camRect = new Rect(
                camera.transform.position.x - camHorizontalSpanHalf, camera.transform.position.y - camVerticalSpanHalf,
                camHorizontalSpanHalf * 2, camVerticalSpanHalf * 2
            );
            foreach (SpriteRenderer sr in spriteRenderers) {
                Vector3 toPos = new Vector3(0, 0, sr.transform.position.z);
                Bounds bounds = sr.bounds;
                Vector2 pivotRatio = new Vector2(sr.sprite.pivot.x / sr.sprite.rect.width, sr.sprite.pivot.y / sr.sprite.rect.height);
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
                Undo.RecordObject(sr.transform, "Align to Camera");
                sr.transform.position = toPos;
            }
            EditorSceneManager.MarkAllScenesDirty();
        }

        #endregion

        #region Helpers

        static HorizontalAlignment GetHorizontalAlignment(TextAnchor alignment)
        {
            switch (alignment) {
            case TextAnchor.LowerLeft:
            case TextAnchor.MiddleLeft:
            case TextAnchor.UpperLeft:
                return HorizontalAlignment.Left;
            case TextAnchor.LowerRight:
            case TextAnchor.MiddleRight:
            case TextAnchor.UpperRight:
                return HorizontalAlignment.Right;
            default:
                return HorizontalAlignment.Middle;
            }
        }

        static VerticalAlignment GetVerticalAlignment(TextAnchor alignment)
        {
            switch (alignment) {
            case TextAnchor.UpperLeft:
            case TextAnchor.UpperRight:
            case TextAnchor.UpperCenter:
                return VerticalAlignment.Top;
            case TextAnchor.LowerLeft:
            case TextAnchor.LowerRight:
            case TextAnchor.LowerCenter:
                return VerticalAlignment.Bottom;
            default:
                return VerticalAlignment.Middle;
            }
        }

        #endregion
    }
}