using System.Collections;
using System.Collections.Generic;
using DG.DemiLib.Attributes;
using UnityEngine;

public class DeAttributesExample : MonoBehaviour
{
    [DeHeader("A colored header", "ffd860", "a154df")]
    public float aFloat = 2;
    [DeHeader("A right aligned colored header", TextAnchor.MiddleRight, "ffffff", "327dcc")]
    public float anotherFloat = 2;
    [DeHeader("Another colored header then an array", "ffffff", "000000")]
    public int[] anArray = new []{ 1, 2, 3 };
    [DeColoredLabel("ffd860", "a154df")]
    public string aString = "Hellow";
}