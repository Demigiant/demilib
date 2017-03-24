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
    public bool[] toggles = new bool[3];
    public bool foldoutOpen;

    public string[] draggableLabels = new[] {
        "Label 0", "Label 1", "Label 2", "Label 3"
    };

    public DeAudioClipData deAudioClip;

    // Node System

    public NodeSystem nodeSystem = new NodeSystem();

    void OnEnable()
    {
        if (nodeSystem.genericNodes == null) nodeSystem.genericNodes = new List<GenericNode>();
        if (nodeSystem.genericNodes.Count == 0) {
            for (int i = 0; i < 8; ++i) {
                GenericNode node = new GenericNode();
                node.id = i.ToString();
                node.guiPosition = new Vector2(UnityEngine.Random.Range(50, 400), UnityEngine.Random.Range(50, 400));
                nodeSystem.genericNodes.Add(node);
            }
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