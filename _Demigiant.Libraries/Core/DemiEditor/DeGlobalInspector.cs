// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/12 10:47
// License Copyright (c) Daniele Giardini

using DG.DemiEditor.AttributesManagers;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Used to draw special method attributes
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)] [CanEditMultipleObjects]
    public class DeGlobalInspector : Editor
    {
        public static DeGlobalInspector I { get; private set; }
        DeMethodButtonEditor _methodButtonEditor;

        void OnEnable()
        {
            I = this;
            _methodButtonEditor = new DeMethodButtonEditor(target);
        }

        void OnDisable()
        {
            if (I == this) I = null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _methodButtonEditor.Draw();
        }

        public static void RepaintMe()
        {
            if (I == null) return;
            I.Repaint();
        }
    }
}