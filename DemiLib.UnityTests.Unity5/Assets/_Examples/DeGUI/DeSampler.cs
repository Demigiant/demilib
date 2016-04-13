// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/26 13:32

using DG.DeAudio;
using DG.DemiLib;
using UnityEngine;

public class DeSampler : MonoBehaviour
{
    public bool useCustomPalette = true;
    public CustomColorPalette palette = new CustomColorPalette();
    
    public bool[] toggles = new bool[3];
    public bool foldoutOpen;

    public string[] draggableLabels = new[] {
        "Label 0", "Label 1", "Label 2", "Label 3"
    };

    public DeAudioClipData deAudioClip;
}