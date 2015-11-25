// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/20 21:11
// License Copyright (c) Daniele Giardini

using DG.DeAudio;
using UnityEngine;
using UnityEditor;

namespace DG.DeAudioEditor
{
    public class MenuItems : MonoBehaviour
    {
        [MenuItem("GameObject/Demigiant/DeAudioManager", false, 10)]
        static void CreateDeAudioManager(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("DeAudioManager");
            go.AddComponent<DeAudioManager>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

//        [MenuItem("GameObject/Demigiant/DeAudioCollection", false, 10)]
//        static void CreateDeAudioCollection(MenuCommand menuCommand)
//        {
//            GameObject go = new GameObject("DeAudioCollection");
//            go.AddComponent<DeAudioCollection>();
//            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
//            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
//            Selection.activeObject = go;
//        }
    }
}