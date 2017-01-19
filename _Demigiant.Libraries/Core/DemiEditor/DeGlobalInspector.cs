// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/12 10:47
// License Copyright (c) Daniele Giardini

using DG.DemiEditor.AttributesManagers;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Used to draw special method attributes.
    /// If you need another generic inspector, inherit from this and override DoEnable, DoDisable and OnInspectorGUI.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)] [CanEditMultipleObjects]
    public class DeGlobalInspector : Editor
    {
        public static DeGlobalInspector I { get; private set; }
        DeMethodButtonEditor _methodButtonEditor;

        public override void OnInspectorGUI()
        {
            I = this;
            if (_methodButtonEditor == null) _methodButtonEditor = new DeMethodButtonEditor(target);

            base.OnInspectorGUI();

            _methodButtonEditor.Draw();
            if (I == this) I = null;
        }

        public static void RepaintMe()
        {
            if (I == null) return;
            I.Repaint();
        }
    }
}