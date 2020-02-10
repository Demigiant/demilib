// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/08 11:25
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core
{
    internal class Minimap
    {
        static readonly Styles _Styles = new Styles();
        NodeProcess _process;
        Texture2D _texture;
        bool _requiresRefresh = true; // TRUE when the texture needs to be refreshed
        // Layout data
        bool _draw;
        Rect _area, _visibleArea, _relativeArea, _fullNodesArea, _fullZeroBasedArea;
        Vector2 _shiftFromOriginalNodesAreaPos;

        #region CONSTRUCTOR

        public Minimap(NodeProcess process)
        {
            _process = process;
        }

        #endregion

        #region GUI

        // Called to draw button (before nodes are drawn, so they don't cover the interaction)
        public void DrawButton()
        {
            Setup();
            if (!_draw) return;

            // Button (uses mouseDown and area, button is there only to capture clicks)
            if (_process.options.minimapClickToGoto) {
                EditorGUIUtility.AddCursorRect(_area, MouseCursor.Arrow);
                if (Event.current.type == EventType.MouseDown && _area.Contains(Event.current.mousePosition)) {
                    Event.current.Use();
                    JumpToMousePosition();
                }
                using (new DeGUI.ColorScope(null, null, Color.clear)) GUI.Button(_area, "");
            }
        }

        // Called only during Repaint
        public void Draw()
        {
            if (!_draw) return;

            // DRAW MAP
            // Background
            GUI.DrawTexture(_area, DeStylePalette.blackSquareAlpha80);
            // Texture map
            if (_requiresRefresh || _texture == null) {
                _requiresRefresh = false;
                RefreshMapTexture(_fullZeroBasedArea, _shiftFromOriginalNodesAreaPos);
            }
            GUI.DrawTexture(_area, _texture, ScaleMode.StretchToFill);
            // Visible area overlay
            Rect innerArea = new Rect(_area.x, _area.y,
                _area.width * _relativeArea.width / _fullZeroBasedArea.width,
                _area.height * _relativeArea.height / _fullZeroBasedArea.height
            );
            if (_fullNodesArea.x < _visibleArea.x) {
                float extraXL = Mathf.Abs(_fullNodesArea.x);
                float emptyX = extraXL + Mathf.Max(0, _fullNodesArea.xMax - _visibleArea.xMax);
                innerArea.x += (_area.width - innerArea.width) * (extraXL / emptyX);
            }
            if (_fullNodesArea.y < _visibleArea.y) {
                float extraYL = Mathf.Abs(_fullNodesArea.y);
                float emptyY = extraYL + Mathf.Max(0, _fullNodesArea.yMax - _visibleArea.yMax);
                innerArea.y += (_area.height - innerArea.height) * (extraYL / emptyY);
            }
            using (new DeGUI.ColorScope(null, null, new DeSkinColor(0.4f))) {
                GUI.Box(innerArea, "", _process.guiScale < 1f ? DeGUI.styles.box.outline02 : DeGUI.styles.box.outline01);
            }
        }

        // Called when drawing the map button, to setup and store all areas
        void Setup()
        {
            if (Event.current.type != EventType.Repaint) return;

            _draw = true;
            if (_process.nodes.Count == 0) {
                _draw = false;
                return;
            }

            _visibleArea = new Rect(_process.relativeArea);
            _relativeArea = _visibleArea.ResetXY();
            _fullNodesArea = _process.EvaluateFullNodesArea(); // x/y = TL node corner, width/height = exact size of area occupied by nodes without extra space
            if (_fullNodesArea.width < 1 || _visibleArea.Includes(_fullNodesArea)) {
                _draw = false;
                return; // Don't draw map if nodes don't exit the visible area
            }

            _Styles.Init();
            int maxSize = _process.options.minimapMaxSize;
            float areaW, areaH;
            _shiftFromOriginalNodesAreaPos = new Vector2();
            _fullZeroBasedArea = new Rect();
            // Store zero-based full area (nodes plus space in all directions) and shift between original nodes position
            _shiftFromOriginalNodesAreaPos = new Vector2(
                _fullNodesArea.x - _visibleArea.x < 0 ? -_fullNodesArea.x : -_visibleArea.x,
                _fullNodesArea.y - _visibleArea.y < 0 ? -_fullNodesArea.y : -_visibleArea.y
            );
            _fullZeroBasedArea = new Rect(0, 0,
                Mathf.Abs(Mathf.Min(0, _fullNodesArea.x - _visibleArea.x)) + Mathf.Max(_fullNodesArea.xMax - _visibleArea.x, _relativeArea.xMax),
                Mathf.Abs(Mathf.Min(0, _fullNodesArea.y - _visibleArea.y)) + Mathf.Max(_fullNodesArea.yMax - _visibleArea.y, _relativeArea.yMax)
            );
            if (_fullZeroBasedArea.width > _fullZeroBasedArea.height) {
                areaW = maxSize;
                areaH = areaW * _fullZeroBasedArea.height / _fullZeroBasedArea.width;
            } else {
                areaH = maxSize;
                areaW = areaH * _fullZeroBasedArea.width / _fullZeroBasedArea.height;
            }
            areaW /= _process.guiScale;
            areaH /= _process.guiScale;

            _area = new Rect(_visibleArea.xMax - areaW - 3, _visibleArea.yMax - areaH - 3, areaW, areaH);
        }

        #endregion

        #region Public Methods

        public void RefreshMapTextureOnNextPass()
        {
            _requiresRefresh = true;
        }

        #endregion

        #region Methods

        void RefreshMapTexture(Rect fullZeroBasedArea, Vector2 shift)
        {
            int w = _process.options.minimapResolution == ProcessOptions.MinimapResolution.Big ? 256
                : _process.options.minimapResolution == ProcessOptions.MinimapResolution.Small ? 64
                : 128;
            if (_texture == null || _texture.width != w) {
                _texture = new Texture2D(w, w, TextureFormat.ARGB32, true) {
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear
                };
            }
            Color32[] pixels = new Color32[w*w];
            // Clear the texture
            int len = pixels.Length;
            for (int i = 0; i < len; ++i) pixels[i] = new Color32(0, 0, 0, 0);
            _texture.SetPixels32(pixels);
            // Draw the elements
            foreach (IEditorGUINode node in _process.nodes) {
                bool evidenceAsEndNode = false;
                if (_process.options.minimapEvidenceEndNodes && !_process.nodeToConnectionOptions[node].neverMarkAsEndNode) {
                    foreach (string connId in node.connectedNodesIds) {
                        if (!string.IsNullOrEmpty(connId)) continue;
                        evidenceAsEndNode = true;
                        break;
                    }
                }
                NodeGUIData nodeGuiData = _process.nodeToGUIData[node];
                Rect nodeRect = nodeGuiData.fullArea;
                int xMin = (int)((nodeRect.x + shift.x) * w / fullZeroBasedArea.width);
                int xMax = (int)((nodeRect.xMax + shift.x) * w / fullZeroBasedArea.width);
                int yMin = (int)((nodeRect.y + shift.y) * w / fullZeroBasedArea.height);
                int yMax = (int)((nodeRect.yMax + shift.y) * w / fullZeroBasedArea.height);
                Color color = evidenceAsEndNode ? Color.red : nodeGuiData.mainColor;
                for (int x = xMin; x < xMax; ++x) {
                    for (int y = yMin; y < yMax; ++y) {
                        _texture.SetPixel(x, w - y, color);
                    }
                }
            }
            // Apply
            _texture.Apply();
        }

        // Jumps to mouse position in minimap area
        void JumpToMousePosition()
        {
            Vector2 relativeMouseP = Event.current.mousePosition - new Vector2(_area.x, _area.y);
            float perc = _fullZeroBasedArea.width / _area.width;
            Vector2 p = _shiftFromOriginalNodesAreaPos - relativeMouseP * perc; // TL corner
            p += new Vector2(_visibleArea.width * 0.5f, _visibleArea.height * 0.5f);
            _process.ShiftAreaBy(p);
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class Styles
        {
            public GUIStyle visibleArea, visibleAreaOverlay;
            bool _initialized;

            public void Init()
            {
                if (_initialized) return;

                _initialized = true;
                visibleArea = DeGUI.styles.box.flat.Clone().Background(DeStylePalette.whiteSquareAlpha15);
                visibleAreaOverlay = new GUIStyle(GUI.skin.box);
            }
        }
    }
}