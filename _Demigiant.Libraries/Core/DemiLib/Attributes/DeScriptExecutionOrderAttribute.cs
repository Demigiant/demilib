// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/12 13:54
// License Copyright (c) Daniele Giardini

using System;

#pragma warning disable 1591
namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// Class attribute
    /// </summary>
    public class DeScriptExecutionOrderAttribute : Attribute
    {
        public int order;

        public DeScriptExecutionOrderAttribute(int order)
        {
            this.order = order;
        }
    }
}