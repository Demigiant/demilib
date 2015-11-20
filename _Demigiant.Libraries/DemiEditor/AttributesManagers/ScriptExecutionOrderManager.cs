// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/12 13:54
// License Copyright (c) Daniele Giardini

using System;
using DG.DemiLib.Attributes;
using UnityEditor;

#pragma warning disable 1591
namespace DG.DemiEditor.AttributesManagers
{
    [InitializeOnLoad]
    public class ScriptExecutionOrderManager
    {
        static ScriptExecutionOrderManager()
        {
            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts()) {
                if (monoScript.GetClass() == null) continue;
                foreach (Attribute a in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptExecutionOrder))) {
                    int currentOrder = MonoImporter.GetExecutionOrder(monoScript);
                    int newOrder = ((ScriptExecutionOrder)a).order;
                    if (currentOrder != newOrder) MonoImporter.SetExecutionOrder(monoScript, newOrder);
                }
            }
        }
    }
}