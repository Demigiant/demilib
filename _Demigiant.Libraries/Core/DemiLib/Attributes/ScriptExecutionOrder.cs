// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/12 13:54
// License Copyright (c) Daniele Giardini

using System;

#pragma warning disable 1591
namespace DG.DemiLib.Attributes
{
    public class ScriptExecutionOrder : Attribute
    {
        public int order;

        public ScriptExecutionOrder(int order)
        {
            this.order = order;
        }
    }
}