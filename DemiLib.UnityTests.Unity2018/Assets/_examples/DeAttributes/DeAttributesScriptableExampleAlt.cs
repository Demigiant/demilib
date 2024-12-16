// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/30 23:04
// License Copyright (c) Daniele Giardini

using System;
using DG.DeInspektor.Attributes;
using UnityEngine;

namespace Assets._Examples.DeAttributes
{
    [CreateAssetMenu(fileName = "DeAttributesScriptableExampleAlt", menuName = "Test/DeAttributesScriptableExampleAlt", order = 2)]
    public class DeAttributesScriptableExampleAlt : ScriptableObject
    {
        [DeButton(DePosition.HDefault, typeof(DeAttributesScriptableExampleAlt), "CreateId")]
        public string Name;
 
        void CreateId()
        {
            Debug.Log("CreateId");
            Name = Guid.NewGuid().ToString();
        }
    }
}