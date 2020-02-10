// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/30 21:03
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
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

            const int totIcons = 8;
            const int totAlignIcons = 6;
            const int dividerW = 2; // Divider between align and distribute buttons

            _Styles.Init();
            _area = _process.relativeArea.SetWidth(_IcoSize * totIcons + _IcosDistance * totIcons - 1 + _HPadding * 2 + dividerW)
                .SetHeight(_HeaderHeight + _IcoSize + _VPadding * 2);
            _area = _area.SetX(_process.relativeArea.xMax - _area.width - _Margin).Shift(0, _Margin, 0, 0);
            _area.x = (int)_area.x;
            _area.y = (int)_area.y;
            Rect dividerR = new Rect(_area.x + _IcoSize * totAlignIcons + _IcosDistance * totAlignIcons + _HPadding, _area.y, dividerW, _area.height);

            // Background
            DeGUI.DrawColoredSquare(_area, Color.black);
            using (new DeGUI.ColorScope(DeGUI.colors.global.blue)) GUI.Box(_area.Expand(1), "", DeGUI.styles.box.outline01);
            // Distribute buttons divider
            DeGUI.DrawColoredSquare(new Rect(dividerR), DeGUI.colors.global.blue);

            // Header
            Rect headerR = _area.SetHeight(_HeaderHeight);
            GUI.Label(headerR, string.Format("{0} nodes selected", _process.selection.selectedNodes.Count), _Styles.headerLabel);

            // Align buttons
            Rect icoR = new Rect(_area.x + _HPadding, _area.y + _VPadding + _HeaderHeight, _IcoSize, _IcoSize);
            for (int i = 0; i < totIcons; ++i) {
                if (i == totAlignIcons) icoR.x += dividerW + _IcosDistance;
                if (GUI.Button(icoR, IndexToIcon(i), _Styles.btIco) && Event.current.button == 0) {
                    if (i > totAlignIcons - 1) ArrangeSelectedNodes(i > totAlignIcons, _process.selection.selectedNodes);
                    AlignSelectedNodes(IndexToGroupAlignment(i), _process.selection.selectedNodes);
                }
                icoR.x += _IcoSize + _IcosDistance;
            }
        }

        #endregion

        #region Public Methods

        public bool HasMouseOver()
        {
            if (!_isVisible) return false;
            return _area.Contains(Event.current.mousePosition);;
        }

        public void AlignAndArrangeNodes(bool horizontally, List<IEditorGUINode> nodes)
        {
            ArrangeSelectedNodes(horizontally, nodes);
            AlignSelectedNodes(horizontally ? GroupAlignment.Top : GroupAlignment.Left, nodes);
        }

        #endregion

        #region Methods

        void AlignSelectedNodes(GroupAlignment alignment, List<IEditorGUINode> nodes)
        {
            int len = nodes.Count;
            if (len <= 1) return;

            GUI.changed = true;
            Vector2 shift = _process.areaShift;
            Vector2 alignVector = Vector2.zero;

            switch (alignment) {
            case GroupAlignment.Left:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[nodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(r.x, 0);
                    else if (r.x < alignVector.x) alignVector.x = r.x;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = nodes[i];
                    node.guiPosition = new Vector2(alignVector.x - shift.x, node.guiPosition.y);
                }
                break;
            case GroupAlignment.Right:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[nodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(r.xMax, 0);
                    else if (r.xMax > alignVector.x) alignVector.x = r.xMax;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = nodes[i];
                    node.guiPosition = new Vector2(
                        alignVector.x - _process.nodeToGUIData[nodes[i]].fullArea.width - shift.x,
                        node.guiPosition.y
                    );
                }
                break;
            case GroupAlignment.VCenter:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[nodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(r.center.x, 0);
                    else if (r.center.x > alignVector.x) alignVector.x += (r.center.x - alignVector.x) * 0.5f;
                    else if (r.center.x < alignVector.x) alignVector.x -= (alignVector.x - r.center.x) * 0.5f;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = nodes[i];
                    node.guiPosition = new Vector2(
                        alignVector.x - _process.nodeToGUIData[nodes[i]].fullArea.width * 0.5f - shift.x,
                        node.guiPosition.y
                    );
                }
                break;
            case GroupAlignment.Top:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[nodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(0, r.y);
                    else if (r.y < alignVector.y) alignVector.y = r.y;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = nodes[i];
                    node.guiPosition = new Vector2(node.guiPosition.x, alignVector.y - shift.y);
                }
                break;
            case GroupAlignment.Bottom:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[nodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(0, r.yMax);
                    else if (r.yMax > alignVector.y) alignVector.y = r.yMax;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = nodes[i];
                    node.guiPosition = new Vector2(
                        node.guiPosition.x,
                        alignVector.y - _process.nodeToGUIData[nodes[i]].fullArea.height - shift.y
                    );
                }
                break;
            case GroupAlignment.HCenter:
                for (int i = 0; i < len; ++i) {
                    Rect r = _process.nodeToGUIData[nodes[i]].fullArea;
                    if (i == 0) alignVector = new Vector2(0, r.center.y);
                    else if (r.center.y > alignVector.y) alignVector.y += (r.center.y - alignVector.y) * 0.5f;
                    else if (r.center.y < alignVector.y) alignVector.y -= (alignVector.y - r.center.y) * 0.5f;
                }
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = nodes[i];
                    node.guiPosition = new Vector2(
                        node.guiPosition.x,
                        alignVector.y - _process.nodeToGUIData[nodes[i]].fullArea.height * 0.5f - shift.y
                    );
                }
                break;
            }
            _process.MarkLayoutAsDirty();
            _process.DispatchOnGUIChange(NodeProcess.GUIChangeType.DragNodes);
        }

        void ArrangeSelectedNodes(bool horizontally, List<IEditorGUINode> nodes)
        {
            nodes.Sort((a, b) => {
                if (horizontally) {
                    if (a.guiPosition.x < b.guiPosition.x) return -1;
                    if (a.guiPosition.x > b.guiPosition.x) return 1;
                    if (a.guiPosition.y > b.guiPosition.y) return 1;
                    if (a.guiPosition.y < b.guiPosition.y) return -1;
                } else {
                    if (a.guiPosition.y < b.guiPosition.y) return -1;
                    if (a.guiPosition.y > b.guiPosition.y) return 1;
                    if (a.guiPosition.x > b.guiPosition.x) return 1;
                    if (a.guiPosition.x < b.guiPosition.x) return -1;
                }
                return 0;
            });
            for (int i = 1; i < nodes.Count; ++i) {
                IEditorGUINode prevINode = nodes[i-1];
                NodeGUIData prevNodeGUIData = _process.nodeToGUIData[prevINode];
                prevNodeGUIData.fullArea.x = prevINode.guiPosition.x;
                prevNodeGUIData.fullArea.y = prevINode.guiPosition.y;
                IEditorGUINode iNode = nodes[i];
                iNode.guiPosition = horizontally
                    ? new Vector2(prevNodeGUIData.fullArea.xMax + NodeProcess.SnapOffset, prevINode.guiPosition.y)
                    : new Vector2(prevINode.guiPosition.x, prevNodeGUIData.fullArea.yMax + NodeProcess.SnapOffset);
            }
            _process.DispatchOnGUIChange(NodeProcess.GUIChangeType.DragNodes);
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
            case 6: return DeStylePalette.ico_distributeVAlignL; // Align and distribute
            case 7: return DeStylePalette.ico_distributeHAlignT; // Align and distribute
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
            case 6: return GroupAlignment.Left; // Align and distribute
            case 7: return GroupAlignment.Top; // Align and distribute
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