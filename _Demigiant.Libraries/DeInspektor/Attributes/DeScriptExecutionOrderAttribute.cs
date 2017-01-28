// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/12 13:54
// License Copyright (c) Daniele Giardini

using System;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Class attribute</code><para/>
    /// Sets the script execution order index
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DeScriptExecutionOrderAttribute : Attribute
    {
        internal int order;

        /// <summary>
        /// Sets the script execution order for this class
        /// </summary>
        /// <param name="order">Script execution order index</param>
        public DeScriptExecutionOrderAttribute(int order)
        {
            this.order = order;
        }
    }
}