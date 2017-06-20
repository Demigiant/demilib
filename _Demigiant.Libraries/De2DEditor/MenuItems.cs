// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/06/20 14:00
// License Copyright (c) Daniele Giardini

using DG.De2D.UI;
using UnityEditor;
using UnityEngine;

namespace DG.De2DEditor
{
    public static class MenuItems
    {
        [MenuItem("GameObject/UI/DeUIMesh", false)]
        static void CreateUIMesh(MenuCommand menuCommand)
        {
            GameObject contextGO = menuCommand.context as GameObject;
            if (contextGO == null || contextGO.GetComponentInParent<Canvas>() == null) {
                EditorUtility.DisplayDialog("Add DeUIMesh", "DeUIMesh can only be added inside a UI Canvas", "Ok");
                return;
            }
            GameObject go = new GameObject("DeUIMesh");
            go.AddComponent<DeUIMesh>();
            GameObjectUtility.SetParentAndAlign(go, contextGO);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}