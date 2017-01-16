// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/15 20:10
// License Copyright (c) Daniele Giardini

using DG.DemiLib.Attributes;
using UnityEngine;

namespace Assets._Examples.DeAttributes
{
    public class DeAttributesExampleAlt : MonoBehaviour
    {
        public bool conditioningBool = true;
        [DeBeginGroup]
        [DeBeginDisabled("conditioningBool")]
        [Range(0, 40)]
        public float aFloat0 = 13f;
        public bool subConditioningBool = true;
        [DeBeginGroup]
        [DeBeginDisabled("subConditioningBool")]
        public float aFloat1 = 10f;
        public string aString0 = "A String";
        [DeEndDisabled]
        [DeEndGroup]
        public string endDisabled0 = "SubConditioning ends here (included)";
        [DeButton(typeof(DeAttributesExampleAlt), "AMethod")]
        public string aString1 = "Another string";
        [DeEndDisabled]
        [DeEndGroup]
        public string endDisabled1 = "Conditioning ends here (included)";

        public string aString2 = "A string";

        [DeBeginGroup]
        [DeDisabled("conditioningBool")]
        [DeHeader("Multi ranged + enabled")]
        [DeComment("A ranged value with extra enabled conditions using DeBegin/EndDisabled")]
        [Range(0, 40)]
        [DeEndGroup]
        public float aRanged = 23;

        public string aString3 = "A string";

        void AMethod()
        {
            Debug.Log("AMethod called");
        }
    }
}