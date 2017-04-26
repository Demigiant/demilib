// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/01 13:25
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    public struct NodeConnectionOptions
    {
        // If TRUE allows to CTRL+drag to connect the target to another
        public bool allowManualConnections;
        // Connection mode
        public ConnectionMode connectionMode;
        // Connector mode
        public ConnectorMode connectorMode;
        // Color in case of single connection or NULL gradientColor, replaced by NodeGUIData.mainColor if it's set to Color.clear
        public Color startColor;
        // Used in case of multiple connections
        public Gradient gradientColor;

        public NodeConnectionOptions(bool allowManualConnections, ConnectorMode connectorMode = ConnectorMode.Smart)
        {
            this.allowManualConnections = allowManualConnections;
            this.connectorMode = connectorMode;

            this.connectionMode = ConnectionMode.Normal;
            this.startColor = Color.clear;
            gradientColor = null;
        }

        public NodeConnectionOptions(bool allowManualConnections, ConnectionMode connectionMode, ConnectorMode connectorMode = ConnectorMode.Smart)
        {
            this.allowManualConnections = allowManualConnections;
            this.connectionMode = connectionMode;
            this.connectorMode = connectorMode;

            this.startColor = Color.clear;
            gradientColor = null;
        }

        public NodeConnectionOptions(bool allowManualConnections, ConnectorMode connectorMode, Gradient gradientColor)
        {
            this.allowManualConnections = allowManualConnections;
            this.connectorMode = connectorMode;
            this.gradientColor = gradientColor;

            this.connectionMode = ConnectionMode.Normal;
            this.startColor = Color.clear;
        }

        public NodeConnectionOptions(bool allowManualConnections, ConnectionMode connectionMode, ConnectorMode connectorMode, Gradient gradientColor)
        {
            this.allowManualConnections = allowManualConnections;
            this.connectionMode = connectionMode;
            this.connectorMode = connectorMode;
            this.gradientColor = gradientColor;

            this.startColor = Color.clear;
        }

        public NodeConnectionOptions(bool allowManualConnections, ConnectorMode connectorMode, Color startColor, Gradient gradientColor = null)
        {
            this.allowManualConnections = allowManualConnections;
            this.connectorMode = connectorMode;
            this.startColor = startColor;
            this.gradientColor = gradientColor;

            this.connectionMode = ConnectionMode.Normal;
        }

        public NodeConnectionOptions(bool allowManualConnections, ConnectionMode connectionMode, ConnectorMode connectorMode, Color startColor, Gradient gradientColor = null)
        {
            this.allowManualConnections = allowManualConnections;
            this.connectionMode = connectionMode;
            this.connectorMode = connectorMode;
            this.startColor = startColor;
            this.gradientColor = gradientColor;
        }
    }
}