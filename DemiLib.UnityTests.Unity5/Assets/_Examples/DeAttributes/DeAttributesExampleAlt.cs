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
        public GameObject[] sampleGos;
        public SampleStruct aSampleStruct = new SampleStruct();
        public string aString = "Hellow";
//        public string aString0 = "Hellow";
//        public float aFloat = 13;
//        public TextAnchor anEnum = TextAnchor.LowerLeft;


//        [DeList]
//        public int[] aListArray0 = new []{ 1, 2, 3 };
//        public string aString1 = "Hellow";
//        [DeList]
//        public List<Vector3> aList0 = new List<Vector3>() { Vector3.zero, Vector3.one };
//        [DeList]
//        public List<GameObject> aListOfGos0;
//        [DeList]
    }

    [Serializable]
    public class SampleStruct
    {
        public float a, b, c;
        public int[] anInt = new []{ 1, 2, 3 };
    }
}