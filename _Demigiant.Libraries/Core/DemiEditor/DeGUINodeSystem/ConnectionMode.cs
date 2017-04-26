// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/26 18:55
// License Copyright (c) Daniele Giardini
namespace DG.DemiEditor.DeGUINodeSystem
{
    public enum ConnectionMode
    {
        Normal,
        /// <summary>
        /// The connectedNodesIds list will be automatically increased/decreased when adding/removing connections
        /// (otherwise connectedNodesIds will have to be increased via custom code)
        /// </summary>
        Flexible,
        /// <summary>
        /// Requires only two connectedNodesIds (no more, no less), 
        /// uses regular CTRL+Drag to connect connection 0, CTRL+SPACE+Drag to connect connection 1
        /// </summary>
        Dual
    }
}