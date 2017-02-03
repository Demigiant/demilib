// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/26 13:32

using System.Collections.Generic;
using DG.DeAudio;
using DG.DemiLib;
using UnityEngine;

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
}