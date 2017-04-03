// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/01 13:25
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    public struct NodeConnectionOptions
    {
        public bool allowManualConnections; // If TRUE allows to CTRL+drag to connect the target to another
        public ConnectorMode connectorMode;
        public Color startColor; // Color in case of single connection or NULL gradientColor
        public Gradient gradientColor; // Used in case of multiple connections

        public NodeConnectionOptions(bool allowManualConnections, ConnectorMode connectorMode = ConnectorMode.Smart)
        {
            this.allowManualConnections = allowManualConnections;
            this.connectorMode = connectorMode;

            this.startColor = new DeSkinColor(0.3f, 0.85f);
            gradientColor = null;
        }

        public NodeConnectionOptions(bool allowManualConnections, ConnectorMode connectorMode, Color startColor, Gradient gradientColor = null)
        {
            this.allowManualConnections = allowManualConnections;
            this.connectorMode = connectorMode;
            this.startColor = startColor;
            this.gradientColor = gradientColor;
        }
    }
}