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
        // If TRUE the connectedNodesIds list will be automatically increased/decreased when adding/removing connections
        // (otherwise connectedNodesIds will have to be increased via custom code)
        public bool flexibleConnections;
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

            this.flexibleConnections = false;
            this.startColor = Color.clear;
            gradientColor = null;
        }

        public NodeConnectionOptions(bool allowManualConnections, bool flexibleConnections, ConnectorMode connectorMode = ConnectorMode.Smart)
        {
            this.allowManualConnections = allowManualConnections;
            this.flexibleConnections = flexibleConnections;
            this.connectorMode = connectorMode;

            this.startColor = Color.clear;
            gradientColor = null;
        }

        public NodeConnectionOptions(bool allowManualConnections, ConnectorMode connectorMode, Gradient gradientColor)
        {
            this.allowManualConnections = allowManualConnections;
            this.connectorMode = connectorMode;
            this.gradientColor = gradientColor;

            this.startColor = Color.clear;
            this.flexibleConnections = false;
        }

        public NodeConnectionOptions(bool allowManualConnections, bool flexibleConnections, ConnectorMode connectorMode, Gradient gradientColor)
        {
            this.allowManualConnections = allowManualConnections;
            this.flexibleConnections = flexibleConnections;
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

            this.flexibleConnections = false;
        }

        public NodeConnectionOptions(bool allowManualConnections, bool flexibleConnections, ConnectorMode connectorMode, Color startColor, Gradient gradientColor = null)
        {
            this.allowManualConnections = allowManualConnections;
            this.flexibleConnections = flexibleConnections;
            this.connectorMode = connectorMode;
            this.startColor = startColor;
            this.gradientColor = gradientColor;

        }
    }
}