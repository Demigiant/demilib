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
        DeMethodButtonEditor _methodButtonEditor;

        void OnEnable()
        {
            _methodButtonEditor = new DeMethodButtonEditor(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _methodButtonEditor.Draw();
        }
    }
}