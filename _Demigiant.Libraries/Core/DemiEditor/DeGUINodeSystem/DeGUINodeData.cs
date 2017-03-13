// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/12 11:43
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    public struct DeGUINodeData
    {
        public Rect fullArea, dragArea;

        public DeGUINodeData(Rect fullArea, Rect dragArea)
        {
            this.fullArea = fullArea;
            this.dragArea = dragArea;
        }

        public override string ToString()
        {
            return string.Format("fullArea: {0} - dragArea: {1}", fullArea, dragArea);
        }
    }
}