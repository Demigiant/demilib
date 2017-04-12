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
        public List<Rect> connectorAreas; // Sub areas from which each connector should start - if NULL uses fullArea (//TODO)
        public Color mainColor; // Node color (used by miniMap and as default connector color is no startColor/gradientColor is set)

        public NodeGUIData(Rect fullArea, Rect dragArea, Rect? extraArea0 = null)
        {
            this.fullArea = fullArea;
            this.dragArea = dragArea;
            this.extraArea0 = extraArea0 == null ? new Rect() : (Rect)extraArea0;
            this.mainColor = new Color(0.5f, 0.5f, 0.5f, 1);
            connectorAreas = null;
            // TODO Implement connectorAreas
        }
        public NodeGUIData(Rect fullArea, Rect dragArea, Color mainColor, Rect? extraArea0 = null)
        {
            this.fullArea = fullArea;
            this.dragArea = dragArea;
            this.extraArea0 = extraArea0 == null ? new Rect() : (Rect)extraArea0;
            this.mainColor = mainColor;
            connectorAreas = null;
            // TODO Implement connectorAreas
        }

        public override string ToString()
        {
            return string.Format("fullArea: {0} - dragArea: {1} - extraArea0: {2}", fullArea, dragArea, extraArea0);
        }
    }
}