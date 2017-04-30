// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/30 21:03
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor.Internal;
using DG.DemiLib;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core
{
    internal class ContextPanel
    {
        enum GroupAlignment
        {
            Left,
            HCenter,
            Right,
            Top,
            VCenter,
            Bottom
        }

        const int _Margin = 4;
        const int _HPadding = 6;
        const int _VPadding = 4;
        const int _HeaderHeight = 12;
        const int _IcoSize = 16; // Represents height of context bar and also size of icons
        const int _IcosDistance = 4;
        NodeProcess _process;
        static readonly Styles _Styles = new Styles();
        Rect _area;
        bool _isVisible { get { return _process.selection.selectedNodes.Count > 1; } }

        #region CONSTRUCTOR

        public ContextPanel(NodeProcess process)
        {
            _process = process;
        }

        #endregion

        #region GUI

        public void Draw()
        {
            if (!_isVisible) return;

            _Styles.Init();
            _area = _process.relativeArea.SetWidth(_IcoSize * 6 + _IcosDistance * 5 + _HPadding * 2)
                .SetHeight(_HeaderHeight + _IcoSize + _VPadding * 2);
            _area = _area.SetX(_process.relativeArea.xMax - _area.width - _Margin).Shift(0, _Margin, 0, 0);
            _area.x = (int)_area.x;
            _area.y = (int)_area.y;

            // Background
            DeGUI.DrawColoredSquare(_area, Color.black);
            using (new DeGUI.ColorScope(DeGUI.colors.global.blue)) GUI.Box(_area.Expand(1), "", DeGUI.styles.box.outline01);

            // Header
            Rect headerR = _area.SetHeight(_HeaderHeight);
            GUI.Label(headerR, string.Format("{0} nodes selected", _process.selection.selectedNodes.Count), _Styles.headerLabel);

            // Align buttons
            for (int i = 0; i < 6; ++i) {
                Rect icoR = new Rect(_area.x + _HPadding + (_IcoSize + _IcosDistance) * i, _area.y + _VPadding + _HeaderHeight, _IcoSize, _IcoSize);
                if (GUI.Button(icoR, IndexToIcon(i), _Styles.btIco) && Event.current.button == 0) {
                    AlignSelectedNodes(IndexToGroupAlignment(i));
                }
            }
        }

        #endregion

        #region Public Methods

        public bool HasMouseOver()
        {
            if (!_isVisible) return false;
            return _area.Contains(Event.current.mousePosition);;
        }

        #endregion

        #region Methods

        void AlignSelectedNodes(GroupAlignment alignment)
        {
            int len = _process.selection.selectedNodes.Count;
            if (len <= 1) return;

            GUI.changed = true;
            Vector2 shift = _process.areaShift;
            Vector2 alignVector = Vector2.zero;

            switch (alignment) {
            case GroupAlignment.Left:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[_process.selection.selectedNodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(r.x, 0);
                    else if (r.x < alignVector.x) alignVector.x = r.x;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = _process.selection.selectedNodes[i];
                    node.guiPosition = new Vector2(alignVector.x - shift.x, node.guiPosition.y);
                }
                break;
            case GroupAlignment.Right:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[_process.selection.selectedNodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(r.xMax, 0);
                    else if (r.xMax > alignVector.x) alignVector.x = r.xMax;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = _process.selection.selectedNodes[i];
                    node.guiPosition = new Vector2(
                        alignVector.x - _process.nodeToGUIData[_process.selection.selectedNodes[i]].fullArea.width - shift.x,
                        node.guiPosition.y
                    );
                }
                break;
            case GroupAlignment.VCenter:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[_process.selection.selectedNodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(r.center.x, 0);
                    else if (r.center.x > alignVector.x) alignVector.x += (r.center.x - alignVector.x) * 0.5f;
                    else if (r.center.x < alignVector.x) alignVector.x -= (alignVector.x - r.center.x) * 0.5f;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = _process.selection.selectedNodes[i];
                    node.guiPosition = new Vector2(
                        alignVector.x - _process.nodeToGUIData[_process.selection.selectedNodes[i]].fullArea.width * 0.5f - shift.x,
                        node.guiPosition.y
                    );
                }
                break;
            case GroupAlignment.Top:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[_process.selection.selectedNodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(0, r.y);
                    else if (r.y < alignVector.y) alignVector.y = r.y;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = _process.selection.selectedNodes[i];
                    node.guiPosition = new Vector2(node.guiPosition.x, alignVector.y - shift.y);
                }
                break;
            case GroupAlignment.Bottom:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[_process.selection.selectedNodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(0, r.yMax);
                    else if (r.yMax > alignVector.y) alignVector.y = r.yMax;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = _process.selection.selectedNodes[i];
                    node.guiPosition = new Vector2(
                        node.guiPosition.x,
                        alignVector.y - _process.nodeToGUIData[_process.selection.selectedNodes[i]].fullArea.height - shift.y
                    );
                }
                break;
            case GroupAlignment.HCenter:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[_process.selection.selectedNodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(0, r.center.y);
                    else if (r.center.y > alignVector.y) alignVector.y += (r.center.y - alignVector.y) * 0.5f;
                    else if (r.center.y < alignVector.y) alignVector.y -= (alignVector.y - r.center.y) * 0.5f;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = _process.selection.selectedNodes[i];
                    node.guiPosition = new Vector2(
                        node.guiPosition.x,
                        alignVector.y - _process.nodeToGUIData[_process.selection.selectedNodes[i]].fullArea.height * 0.5f - shift.y
                    );
                }
                break;
            }
        }

        #endregion

        #region Helpers

        Texture2D IndexToIcon(int index)
        {
            switch (index) {
            case 1: return DeStylePalette.ico_alignVC;
            case 2: return DeStylePalette.ico_alignR;
            case 3: return DeStylePalette.ico_alignT;
            case 4: return DeStylePalette.ico_alignHC;
            case 5: return DeStylePalette.ico_alignB;
            default: return DeStylePalette.ico_alignL;
            }
        }

        GroupAlignment IndexToGroupAlignment(int index)
        {
            switch (index) {
            case 1: return GroupAlignment.VCenter;
            case 2: return GroupAlignment.Right;
            case 3: return GroupAlignment.Top;
            case 4: return GroupAlignment.HCenter;
            case 5: return GroupAlignment.Bottom;
            default: return GroupAlignment.Left;
            }
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class Styles
        {
            public GUIStyle headerLabel, btIco;
            bool _initialized;

            public void Init()
            {
                if (_initialized) return;

                _initialized = true;
                headerLabel = new GUIStyle(GUI.skin.label).Add(9, Color.white, TextAnchor.MiddleCenter).Background(DeStylePalette.blueSquare)
                    .Padding(0).Margin(0).ContentOffsetY(-1);
                btIco = DeGUI.styles.button.flatWhite.Clone().Background(null);
            }
        }
    }
}