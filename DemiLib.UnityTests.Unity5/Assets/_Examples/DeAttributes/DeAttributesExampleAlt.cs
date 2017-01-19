// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/15 20:10
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DemiLib.Attributes;
using UnityEngine;

namespace Assets._Examples.DeAttributes
{
    public class DeAttributesExampleAlt : MonoBehaviour
    {
        public int[] aListArrayNoAttr = new []{ 1, 2, 3 };
        public string aString0 = "Hellow";
        [DeList]
        public int[] aListArray0 = new []{ 1, 2, 3 };
        public string aString1 = "Hellow";
        [DeList]
        public List<Vector3> aList0 = new List<Vector3>() { Vector3.zero, Vector3.one };
        [DeList]
        public List<GameObject> aListOfGos0;
        [DeList]
        public SampleStruct[] sampleStructs;
    }

    [Serializable]
    public struct SampleStruct
    {
        public float a, b, c;
    }
}