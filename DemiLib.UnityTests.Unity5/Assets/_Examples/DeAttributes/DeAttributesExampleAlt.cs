// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/15 20:10
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DeInspektor.Attributes;
using DG.DemiLib;
using UnityEngine;

namespace Assets._Examples.DeAttributes
{
    public class DeAttributesExampleAlt : MonoBehaviour
    {
        public float aFloat0 = 4.5f;
        [Range(1, 5)] public float aRange = 2;
        [DeRange(1, 5)] public float aDeRange0 = 2;
        [DeRange(1, 5, "Custom Label")] public float aDeRange1 = 2;
        public Range aDemiLibRange0 = new Range(2, 4);
        [DeRange(1, 5, "Range w DeRange")] public Range aDemiLibRange1 = new Range(2, 4);
        public int[] aListArrayNoAttr = new []{ 1, 2, 3 };
        public SampleObj[] sampleObjs = new[] {new SampleObj(), new SampleObj()};
        public SampleObjAlt[] sampleObjsAlt = new[] {new SampleObjAlt(), new SampleObjAlt()};
        [Header("Sample header after array")]
        public string aString = "Hellow";
        public GameObject[] sampleGos;
        public SampleObj aSampleObj = new SampleObj();
        public string aString0 = "Hellow";
        public float aFloat1 = 13;
        public TextAnchor anEnum = TextAnchor.LowerLeft;
    }

    [Serializable]
    public class SampleObj
    {
        public string titleGiver = "Hellow";
        public float a, b, c;
        public int[] anInt = new []{ 1, 2, 3 };
    }

    [Serializable]
    public class SampleObjAlt
    {
        public float a, b, c;
        public int[] anInt = new []{ 1, 2, 3 };
    }
}