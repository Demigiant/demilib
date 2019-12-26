// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2019/12/21

using System;
using UnityEngine;

namespace Assets._Examples.DeAttributes
{
    public class DeInspektorFocus : MonoBehaviour
    {
        public float aFloat;
        public SampleObjWNestedStuff sampleObjWNestedStuff;

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        [Serializable]
        public class SampleObjWNestedStuff
        {
            public float main;
            public SampleNested[] nestedArray;
        }

        [Serializable]
        public class SampleNested
        {
            public float a;
        }
    }
}