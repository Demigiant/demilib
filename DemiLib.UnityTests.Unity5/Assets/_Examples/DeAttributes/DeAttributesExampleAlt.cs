// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/15 20:10
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DeInspektor.Attributes;
using UnityEngine;

namespace Assets._Examples.DeAttributes
{
    public class DeAttributesExampleAlt : MonoBehaviour
    {
        public int[] aListArrayNoAttr = new []{ 1, 2, 3 };
        public SampleStruct[] sampleStructs = new[] {new SampleStruct(), new SampleStruct()};
        public string aString = "Hellow";
        public GameObject[] sampleGos;
        public SampleStruct aSampleStruct = new SampleStruct();
        public string aString0 = "Hellow";
        public float aFloat = 13;
        public TextAnchor anEnum = TextAnchor.LowerLeft;
    }

    [Serializable]
    public class SampleStruct
    {
        public float a, b, c;
        public int[] anInt = new []{ 1, 2, 3 };
    }
}