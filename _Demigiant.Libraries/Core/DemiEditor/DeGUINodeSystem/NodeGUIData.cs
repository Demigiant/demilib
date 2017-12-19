// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/12 11:43
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    public struct NodeGUIData
    {
        public Rect fullArea, dragArea;
        public Rect extraArea0;
        public Color mainColor; // Node color (used by miniMap and as default connector color is no startColor/gradientColor is set)
        public bool disableSnapping; // If TRUE, disables snapping for this node
        public List<Rect> connectorAreas; // Sub areas from which each connector should start. If NULL uses fullArea // TODO Implement better?
        public bool isVisible { get; internal set; } // Set by NodeProcess.Draw(node...)

        public NodeGUIData(Rect fullArea, Color? mainColor = null, Rect? extraArea0 = null) : this()
        {
            this.fullArea = fullArea;
            this.extraArea0 = extraArea0 == null ? new Rect() : (Rect)extraArea0;
            this.mainColor = mainColor == null ? new Color(0.5f, 0.5f, 0.5f, 1) : (Color)mainColor;
            dragArea = fullArea;
            connectorAreas = null;
        }
        public NodeGUIData(Rect fullArea, Rect dragArea, Rect? extraArea0 = null) : this()
        {
            this.fullArea = fullArea;
            this.dragArea = dragArea;
            this.extraArea0 = extraArea0 == null ? new Rect() : (Rect)extraArea0;
            this.mainColor = new Color(0.5f, 0.5f, 0.5f, 1);
            connectorAreas = null;
        }
        public NodeGUIData(Rect fullArea, Rect dragArea, Color mainColor, Rect? extraArea0 = null) : this()
        {
            this.fullArea = fullArea;
            this.dragArea = dragArea;
            this.extraArea0 = extraArea0 == null ? new Rect() : (Rect)extraArea0;
            this.mainColor = mainColor;
            connectorAreas = null;
        }

        /// <summary>
        /// Recalculates all rects based on the given Y shift
        /// </summary>
        public void ShiftYBy(float value)
        {
            fullArea.y += value;
            dragArea.y += value;
            extraArea0.y += value;
            if (connectorAreas != null) {
                for (int i = 0; i < connectorAreas.Count; ++i) {
                    Rect r = connectorAreas[i];
                    r.y += value;
                    connectorAreas[i] = r;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("fullArea: {0} - dragArea: {1} - extraArea0: {2}", fullArea, dragArea, extraArea0);
        }
    }
}