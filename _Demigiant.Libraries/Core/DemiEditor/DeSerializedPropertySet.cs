// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2021/07/09

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Utility class meant to be extended so you can add as many public fields as the SerializedProperties you want to access.
    /// They should have the same name as the original property so that calling <see cref="Init"/> will initialize them correctly
    /// </summary>
    public class DeSerializedPropertySet
    {
        bool _initialized;
        FieldInfo[] _fInfos;

        /// <summary>
        /// Initialized the <see cref="SerializedProperty"/> fields, required before any other operation/access
        /// </summary>
        /// <param name="so"></param>
        public void Init(SerializedObject so)
        {
            if (_initialized) return;

            _initialized = true;

            _fInfos = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo f in _fInfos) {
                if (f.FieldType != typeof(SerializedProperty)) continue;
                f.SetValue(this, so.FindProperty(f.Name));
            }
        }

        /// <summary>
        /// Draws all property fields
        /// </summary>
        public void DrawAllPropertyFields()
        {
            foreach (FieldInfo fi in _fInfos) {
                EditorGUILayout.PropertyField(fi.GetValue(this) as SerializedProperty);
            }
        }
    }
}