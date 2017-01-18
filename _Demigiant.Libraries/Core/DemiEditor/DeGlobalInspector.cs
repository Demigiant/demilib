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

        void OnEnable()
        { DoEnable(); }
        public virtual void DoEnable()
        {
            I = this;
            _methodButtonEditor = new DeMethodButtonEditor(target);
        }

        void OnDisable() { DoDisable(); }
        public virtual void DoDisable()
        {
            if (I == this) I = null;
        }

        public override void OnInspectorGUI()
        {
            I = this;
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