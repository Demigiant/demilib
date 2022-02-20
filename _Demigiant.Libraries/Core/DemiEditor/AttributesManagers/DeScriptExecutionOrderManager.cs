// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/12 13:54
// License Copyright (c) Daniele Giardini

using System;
// using System.Diagnostics;
// using System.Text;
using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

#pragma warning disable 1591
namespace DG.DemiEditor.AttributesManagers
{
    [InitializeOnLoad]
    public class DeScriptExecutionOrderManager
    {
        // static readonly StringBuilder _report = new StringBuilder();
        // static readonly Stopwatch _watch = new Stopwatch();

        static DeScriptExecutionOrderManager()
        {
            // RestartWatch();

            // int totValidMonoScripts = 0;
            // AddToReport("DeScriptExecutionOrderManager ► Start operations");
            MonoScript[] scripts = MonoImporter.GetAllRuntimeMonoScripts();
            int totScripts = scripts.Length;
            // AddToReport("- Got all monoScripts");
            for (int i = 0; i < totScripts; i++) {
                MonoScript monoScript = scripts[i];
                Type msClass = monoScript.GetClass();
                if (msClass == null) continue;
                // totValidMonoScripts++;
                Attribute[] deScriptAttributes = Attribute.GetCustomAttributes(msClass, typeof(DeScriptExecutionOrderAttribute));
                int totDeScriptAttributes = deScriptAttributes.Length;
                for (int j = 0; j < totDeScriptAttributes; j++) {
                    Attribute a = deScriptAttributes[j];
                    // AddToReport(string.Format("   - DeScriptExecutionOrder found in \"{0}\"", msClass));
                    int currentOrder = MonoImporter.GetExecutionOrder(monoScript);
                    // AddToReport(string.Format("      - Current order ({0}) retrieved", currentOrder));
                    int newOrder = ((DeScriptExecutionOrderAttribute)a).order;
                    if (currentOrder != newOrder) {
                        MonoImporter.SetExecutionOrder(monoScript, newOrder);
                        // AddToReport(string.Format("      - New order ({0}) set", newOrder));
                    }
                }
            }

            // AddToReport(string.Format("► {0} valid MonoScripts found and elaborated", totValidMonoScripts));
            // AddToReport("► Completed");
            // StopWatch();
            // Debug.Log(_report.ToString());
        }

        // static void AddToReport(string s)
        // {
        //     if (_report.Length > 0) _report.Append('\n');
        //     _report.Append(s).Append(" (").Append(ReportElapsedMS()).Append(" MS since start)");
        // }
        //
        // static void RestartWatch()
        // {
        //     _watch.Reset();
        //     _watch.Start();
        // }
        //
        // static float ReportElapsedMS(bool andStop = false)
        // {
        //     float elapsed = _watch.ElapsedMilliseconds;
        //     if (andStop) StopWatch();
        //     return elapsed;
        // }
        //
        // static void StopWatch()
        // {
        //     _watch.Stop();
        // }
    }
}