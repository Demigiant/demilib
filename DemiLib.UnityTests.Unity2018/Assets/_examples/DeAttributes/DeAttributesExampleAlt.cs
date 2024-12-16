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
        [DeComment("Test comment on single line")]
        [Range(1, 5)] public float aRange = 2;
        [DeComment("Test comment on multiple line by jove this is gonna be long or maybe not who knows")]
        [DeRange(1, 5)] public float aDeRange0 = 2;
        [DeComment("Another comment about life and everything and that's it", style = DeCommentStyle.BoxExtended)]
        [DeRange(1, 5)] public float comment0 = 2;
        [DeComment("Another comment about life and everything and that's it", style = DeCommentStyle.TextOnly)]
        [DeRange(1, 5)] public float comment1 = 2;
        [DeComment("Another comment about life and everything and that's it", style = DeCommentStyle.TextInValueArea)]
        [DeRange(1, 5)] public float comment2 = 2;
        [DeComment("Another comment about life and everything and that's it", style = DeCommentStyle.WrapNextLine)]
        [DeRange(1, 5)] public float comment3 = 2;
        [DeComment("Another comment about life and everything and that's it")]
        [DeRange(1, 5)] public float comment4 = 2;
        [DeRange(1, 5, "Custom Label")] public float aDeRange1 = 2;
        public Range aDemiLibRange0 = new Range(2, 4);
        [DeRange(1, 5, "Range w DeRange")] public Range aDemiLibRange1 = new Range(2, 4);
        public int[] aListArrayNoAttr = new []{ 1, 2, 3 };
        public SampleObj[] sampleObjs = new[] {new SampleObj(), new SampleObj()};
        public SampleObjAlt[] sampleObjsAlt = new[] {new SampleObjAlt(), new SampleObjAlt()};
        [Header("Sample header after array")]
        public string aString = "Hellow";
        [DeHeader("Header-comment-divider")]
        [DeComment("I'm a comment")]
        [DeDivider(order = 1)]
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