// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/23 13:06
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Property attribute</code><para/>
    /// Only for object reference or string fields: shows them red if empty/null
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeEmptyAlertAttribute : PropertyAttribute
    {
        internal bool alsoMarkIfOk;

        /// <summary>
        /// Only for object reference or string fields: shows them red if empty/null
        /// </summary>
        /// <param name="alsoMarkIfOk">If TRUE, also colorizes the field in green if non-empty</param>
        public DeEmptyAlertAttribute(bool alsoMarkIfOk = true)
        {
            this.alsoMarkIfOk = alsoMarkIfOk;
        }
    }
}