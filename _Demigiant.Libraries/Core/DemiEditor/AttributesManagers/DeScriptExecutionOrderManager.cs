// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/12 13:54
// License Copyright (c) Daniele Giardini

using System;
using DG.DemiLib.Attributes;
using UnityEditor;
using Debug = UnityEngine.Debug;
#if DEBUG
using System.Diagnostics;
using System.Text;
#endif

#pragma warning disable 1591
namespace DG.DemiEditor.AttributesManagers
{
    [InitializeOnLoad]
    public class DeScriptExecutionOrderManager
    {
#if DEBUG
        static readonly StringBuilder _Report = new StringBuilder();
        static readonly Stopwatch _Watch = new Stopwatch();
#endif

        static DeScriptExecutionOrderManager()
        {
            if (DeEditorUtils.isUnityReady) Execute();
            else {
                DeEditorNotification.OnUnityReady -= Execute;
                DeEditorNotification.OnUnityReady += Execute;
            }
        }
    
        static void Execute()
        {
            DeEditorNotification.OnUnityReady -= Execute;

#if DEBUG            
            RestartWatch();
            AddToReport("DeScriptExecutionOrderManager ► Start operations");
            int totValidMonoScripts = 0;
#endif

            MonoScript[] scripts = MonoImporter.GetAllRuntimeMonoScripts();
            int totScripts = scripts.Length;
#if DEBUG
            AddToReport(string.Format("- Got all monoScripts ({0})", totScripts));
#endif
            if (totScripts > 0) {
                Type attrType = typeof(DeScriptExecutionOrderAttribute);
                for (int i = 0; i < totScripts; i++) {
                    MonoScript monoScript = scripts[i];
                    Type msClass = monoScript.GetClass();
                    if (msClass == null) continue;
#if DEBUG
                    totValidMonoScripts++;
#endif
                    if (!msClass.IsDefined(attrType, true)) continue;
                    object[] deScriptAttributes = msClass.GetCustomAttributes(attrType, true);
                    int totDeScriptAttributes = deScriptAttributes.Length;
                    for (int j = 0; j < totDeScriptAttributes; j++) {
                        Attribute a = (Attribute)deScriptAttributes[j];
#if DEBUG
                        AddToReport(string.Format("<color=#ffd26d>   - DeScriptExecutionOrder found in \"{0}\"</color>", msClass));
#endif
                        int currentOrder = MonoImporter.GetExecutionOrder(monoScript);
#if DEBUG
                        AddToReport(string.Format("      - Current order ({0}) retrieved", currentOrder));
#endif
                        int newOrder = ((DeScriptExecutionOrderAttribute)a).order;
                        if (currentOrder != newOrder) {
                            MonoImporter.SetExecutionOrder(monoScript, newOrder);
#if DEBUG
                            AddToReport(string.Format("<color=#00ff00>      - New order ({0}) set</color>", newOrder));
#endif
                        }
                    }
                }
            }
    
#if DEBUG
            StopWatch();
            AddToReport(string.Format("► {0} valid MonoScripts found and elaborated", totValidMonoScripts));
            AddToReport("► Completed");
            Debug.Log(_Report.ToString());
#endif
        }

#if DEBUG
        static void AddToReport(string s)
        {
            if (_Report.Length > 0) _Report.Append('\n');
            _Report.Append(s).Append(" (").Append(ReportElapsedMS()).Append(" MS since start)");
        }
        
        static void RestartWatch()
        {
            _Watch.Reset();
            _Watch.Start();
        }
        
        static float ReportElapsedMS(bool andStop = false)
        {
            float elapsed = _Watch.ElapsedMilliseconds;
            if (andStop) StopWatch();
            return elapsed;
        }
        
        static void StopWatch()
        {
            _Watch.Stop();
        }
#endif
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ CLASS ███████████████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    // class DeScriptExecutionOrderManagerPostprocessor : AssetPostprocessor
    // {
    //     const bool _DebugLogs = true;
    //     static StringBuilder _report;
    //     static Stopwatch _watch;
    //
    //     static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    //     {
    //         if (_DebugLogs) {
    //             _report = new StringBuilder();
    //             _watch = new Stopwatch();
    //             RestartWatch();
    //             AddToReport(string.Format("DeScriptExecutionOrderManagerPostprocessor ► Start operations ► Imported {0} files", importedAssets.Length));
    //         }
    //         Type attrType = typeof(DeScriptExecutionOrderAttribute);
    //         Type monoScriptType = typeof(MonoScript);
    //         foreach (string adbFilePath in importedAssets) {
    //             Type t = AssetDatabase.GetMainAssetTypeAtPath(adbFilePath);
    //             if (t == monoScriptType) {
    //                 MonoImporter importer = (MonoImporter)AssetImporter.GetAtPath(adbFilePath);
    //                 Type msClass = importer.GetScript().GetClass();
    //                 if (!msClass.IsDefined(attrType, true)) continue;
    //                 AddToReport("   ► DEFINED IN " + msClass);
    //             }
    //             AddToReport("  - " + AssetDatabase.GetMainAssetTypeAtPath(adbFilePath));
    //             // string txt = File.ReadAllText(DeEditorFileUtils.ADBPathToFullPath(adbFilePath));
    //             // if (txt.Contains("[DeScriptExecutionOrder(")) {
    //             //     // TODO
    //             // }
    //         }
    //         if (_DebugLogs) {
    //             StopWatch();
    //             AddToReport("► Completed");
    //             Debug.Log(_report.ToString());
    //         }
    //     }
    //
    //     static void AddToReport(string s)
    //     {
    //         if (_report.Length > 0) _report.Append('\n');
    //         _report.Append(s).Append(" (").Append(ReportElapsedMS()).Append(" MS since start)");
    //     }
    //     
    //     static void RestartWatch()
    //     {
    //         _watch.Reset();
    //         _watch.Start();
    //     }
    //     
    //     static float ReportElapsedMS(bool andStop = false)
    //     {
    //         float elapsed = _watch.ElapsedMilliseconds;
    //         if (andStop) StopWatch();
    //         return elapsed;
    //     }
    //     
    //     static void StopWatch()
    //     {
    //         _watch.Stop();
    //     }
    // }
}