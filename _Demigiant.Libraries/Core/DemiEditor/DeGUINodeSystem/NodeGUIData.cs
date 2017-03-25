// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/12 11:43
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    public struct NodeGUIData
    {
        public Rect fullArea, dragArea;
        public Rect extraArea0;

        public NodeGUIData(Rect fullArea, Rect dragArea, Rect? extraArea0 = null)
        {
            this.fullArea = fullArea;
            this.dragArea = dragArea;
            this.extraArea0 = extraArea0 == null ? new Rect() : (Rect)extraArea0;
        }

        public override string ToString()
        {
            return string.Format("fullArea: {0} - dragArea: {1} - extraArea0: {2}", fullArea, dragArea, extraArea0);
        }
    }
}