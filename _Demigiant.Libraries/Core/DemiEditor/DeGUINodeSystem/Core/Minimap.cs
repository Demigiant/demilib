// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/08 11:25
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor.Internal;
using DG.DemiLib;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core
{
    internal class Minimap
    {
        NodeProcess _process;
        Texture2D _texture;
        bool _requiresRefresh = true; // TRUE when the texture needs to be refreshed
        static readonly Styles _Styles = new Styles();

        #region CONSTRUCTOR

        public Minimap(NodeProcess process)
        {
            _process = process;
        }

        #endregion

        #region GUI

        // Called only during Repaint
        public void Draw()
        {
            if (_process.nodes.Count == 0) return;

            Rect visibleArea = new Rect(_process.area);
            Rect relativeArea = visibleArea.ResetXY();
            Rect fullNodesArea = EvaluateFullNodesArea(); // x/y = TL node corner, width/height = exact size of area occupied by nodes without extra space
            if (fullNodesArea.width < 1 || visibleArea.Includes(fullNodesArea)) return; // Don't draw map if nodes don't exit the visible area

            _Styles.Init();
            int maxSize = _process.options.minimapMaxSize;
            float areaW, areaH;
            Vector2 shiftFromOriginalNodesAreaPos = new Vector2();
            Rect fullZeroBasedArea = new Rect();
            // Store zero-based full area (nodes plus space in all directions) and shift between original nodes position
            shiftFromOriginalNodesAreaPos = new Vector2(
                fullNodesArea.x - visibleArea.x < 0 ? -fullNodesArea.x : -visibleArea.x,
                fullNodesArea.y - visibleArea.y < 0 ? -fullNodesArea.y : -visibleArea.y
            );
            fullZeroBasedArea = new Rect(0, 0,
                Mathf.Abs(Mathf.Min(0, fullNodesArea.x - visibleArea.x)) + Mathf.Max(fullNodesArea.xMax - visibleArea.x, relativeArea.xMax),
                Mathf.Abs(Mathf.Min(0, fullNodesArea.y - visibleArea.y)) + Mathf.Max(fullNodesArea.yMax - visibleArea.y, relativeArea.yMax)
            );
            if (fullZeroBasedArea.width > fullZeroBasedArea.height) {
                areaW = maxSize;
                areaH = areaW * fullZeroBasedArea.height / fullZeroBasedArea.width;
            } else {
                areaH = maxSize;
                areaW = areaH * fullZeroBasedArea.width / fullZeroBasedArea.height;
            }

            Rect area = new Rect(visibleArea.xMax - areaW - 3, visibleArea.yMax - areaH - 3, areaW, areaH);

            // DRAW MAP
            // Background
            GUI.DrawTexture(area, DeStylePalette.blackSquareAlpha80);
            // Visible area overlay
            Rect innerArea = new Rect(area.x, area.y,
                area.width * relativeArea.width / fullZeroBasedArea.width,
                area.height * relativeArea.height / fullZeroBasedArea.height
            );
            if (fullNodesArea.x < visibleArea.x) {
                float extraXL = Mathf.Abs(fullNodesArea.x);
                float emptyX = extraXL + Mathf.Max(0, fullNodesArea.xMax - visibleArea.xMax);
                innerArea.x += (area.width - innerArea.width) * (extraXL / emptyX);
            }
            if (fullNodesArea.y < visibleArea.y) {
                float extraYL = Mathf.Abs(fullNodesArea.y);
                float emptyY = extraYL + Mathf.Max(0, fullNodesArea.yMax - visibleArea.yMax);
                innerArea.y += (area.height - innerArea.height) * (extraYL / emptyY);
            }
            GUI.Box(innerArea, "", _Styles.visibleArea);
            GUI.Box(innerArea, "", _Styles.visibleAreaOverlay);
            // Texture
            if (_requiresRefresh || _texture == null) {
                _requiresRefresh = false;
                RefreshMapTexture(fullZeroBasedArea, shiftFromOriginalNodesAreaPos);
            }
            GUI.DrawTexture(area, _texture, ScaleMode.StretchToFill);
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
                NodeGUIData nodeGuiData = _process.nodeToGUIData[node];
                Rect nodeRect = nodeGuiData.fullArea;
                int xMin = (int)((nodeRect.x + shift.x) * w / fullZeroBasedArea.width);
                int xMax = (int)((nodeRect.xMax + shift.x) * w / fullZeroBasedArea.width);
                int yMin = (int)((nodeRect.y + shift.y) * w / fullZeroBasedArea.height);
                int yMax = (int)((nodeRect.yMax + shift.y) * w / fullZeroBasedArea.height);
                for (int x = xMin; x < xMax; ++x) {
                    for (int y = yMin; y < yMax; ++y) {
                        _texture.SetPixel(x, w - y, nodeGuiData.mainColor);
                    }
                }
            }
            // Apply
            _texture.Apply();
        }

        #endregion

        #region Helpers

        Rect EvaluateFullNodesArea()
        {
            Rect area = _process.nodeToGUIData[_process.nodes[0]].fullArea;
            foreach (IEditorGUINode node in _process.nodes) area = area.Add(_process.nodeToGUIData[node].fullArea);
            return area;
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