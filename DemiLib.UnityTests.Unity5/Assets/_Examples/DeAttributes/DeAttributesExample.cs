using System.Collections;
using System.Collections.Generic;
using DG.DemiLib.Attributes;
using UnityEngine;

public class DeAttributesExample : MonoBehaviour
{
    [DeHeader("A default header")]
    public float aFloat0 = 2;
    [DeHeader("A colored header", "ffd860", "a154df")]
    public float aFloat1 = 2;
    [DeHeader("A colored header with custom font style and size", "ffffff", "ff5e31", FontStyle.Normal, 9)]
    public float aFloat2 = 2;
    [DeHeader("More custom font style and size", "380b0b", "ffc331", FontStyle.Normal, 16)]
    public float aFloat3 = 2;
    [DeHeader("A right aligned colored header", TextAnchor.MiddleRight, "ffffff", "327dcc")]
    public float aFloat4 = 2;
    [DeHeader("Another colored header then an array", "ffffff", "000000")]
    public int[] anArray = new []{ 1, 2, 3 };
    [DeColoredLabel("ffd860", "a154df")]
    public string aString = "Hellow";
}