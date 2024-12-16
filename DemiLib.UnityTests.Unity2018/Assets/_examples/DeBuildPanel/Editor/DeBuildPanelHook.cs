using DG.DeEditorTools.BuildPanel;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class DeBuildPanelHook
{
    static DeBuildPanelHook()
    {
        DeBuildPanel.OnBuildNameRequest += OnBuildNameRequest;
    }

    static string OnBuildNameRequest(BuildTarget buildTarget, string buildName)
    {
        return buildName + "_hookAdded";
    }
}