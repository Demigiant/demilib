// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/20 21:11
// License Copyright (c) Daniele Giardini

using DG.Audio;
using UnityEngine;
using UnityEditor;

namespace DG.AudioEditor
{
    public class MenuItems : MonoBehaviour
    {
        [MenuItem("GameObject/Create Other/Demigiant/DeAudioManager", false, 99)]
        static void CreateDeAudioManager(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("DeAudioManager");
            go.AddComponent<DeAudioManager>();
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}