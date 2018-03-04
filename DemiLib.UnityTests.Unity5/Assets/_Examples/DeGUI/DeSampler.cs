// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/26 13:32

using System;
using System.Collections.Generic;
using DG.DeAudio;
using DG.DemiLib;
using UnityEngine;
using _Examples.DeGUI.DeGUINode;

[ExecuteInEditMode]
public class DeSampler : MonoBehaviour
{
    public bool useCustomPalette = true;
    public CustomColorPalette palette = new CustomColorPalette();
    
    public List<string> strList0 = new List<string>() {"A", "B", "C"};
    public List<string> strList1 = new List<string>() {"1", "2", "3"};
    public List<string> strListAlt = new List<string>() {"a", "b", "c", "d", "e", "f"};
    public bool[] toggles = new bool[3];
    public bool foldoutOpen;

    public string[] draggableLabels = new[] {
        "Label 0", "Label 1", "Label 2", "Label 3"
    };

    public DeAudioClipData deAudioClip;

    // Node System

    public NodeSystem nodeSystem = new NodeSystem();

    void Reset()
    {
        nodeSystem.genericNodes = new List<GenericNode>();
        Generate();
    }

    void OnEnable()
    {
        if (nodeSystem.genericNodes == null) nodeSystem.genericNodes = new List<GenericNode>();
        if (nodeSystem.genericNodes.Count == 0) Generate();
    }

    void Generate()
    {
        for (int i = 0; i < 8; ++i) {
            GenericNode node = new GenericNode {
                id = i.ToString(),
                guiPosition = new Vector2(UnityEngine.Random.Range(50, 400), UnityEngine.Random.Range(50, 400))
            };
            if (i < 3) {
                node.type = NodeType.Multi;
                if (i < 2) {
                    // Multi default
                    while (node.connectedNodesIds.Count < 3) node.connectedNodesIds.Add(null);
                } else {
                    // NormalPlus
                    node.normalPlusConnectionMode = true;
                    while (node.connectedNodesIds.Count < 4) node.connectedNodesIds.Add(null);
                }
            } else {
                node.type = NodeType.Generic;
                if (i < 6) {
                    // Flexible
                    node.flexibleConnectionMode = true;
                    node.connectedNodesIds.Clear();
                } else {
                    // Dual
                    node.dualConnectionMode = true;
                    while (node.connectedNodesIds.Count < 2) node.connectedNodesIds.Add(null);
                }
            }
            nodeSystem.genericNodes.Add(node);
        }
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    [Serializable]
    public class NodeSystem
    {
        public Vector2 areaShift;
        public StartNode startNode = new StartNode();
        public List<GenericNode> genericNodes;
    }

}