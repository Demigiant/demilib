// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2022/06/01

using System;
using UnityEngine;

public class DeAttributesBugTest : MonoBehaviour
{
    public Nested nested;
    
    [Serializable]
    public class Nested
    {
        public float[] nestedArray = new float[5];
    }
}