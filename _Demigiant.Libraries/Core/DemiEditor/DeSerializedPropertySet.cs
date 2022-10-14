// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2021/07/09

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Utility class. You can either use it as is via its constructor, which automatically retrieves all serializedProperties in the instance,
    /// or you can extend it so you can add as many public SerializedProperties as the SerializedProperties you want to access
    /// (their name must be the same as the serialized field they refer to)
    /// </summary>
    public class DeSerializedPropertySet
    {
        readonly List<SerializedProperty> _props = new List<SerializedProperty>();

        /// <summary>
        /// Automatically retrieves all serializable properties on the given serializedObject,
        /// or only specific ones if propNames is specified
        /// </summary>
        public DeSerializedPropertySet(SerializedObject serializedObject, params string[] propNames)
        {
            if (propNames != null && propNames.Length > 0) {
                // Fill with only specific props
                foreach (string propName in propNames) {
                    SerializedProperty prop = serializedObject.FindProperty(propName);
                    if (prop == null) Debug.LogError("DeSerializedPropertySet ► No property named \"" + serializedObject.targetObject.GetType() + "." + propName + "\" exists");
                    else _props.Add(serializedObject.FindProperty(propName));
                }
            } else {
                if (this.GetType() != typeof(DeSerializedPropertySet)) AutoFillFromChildClassFields(serializedObject);
                else AutoFillFromSerializedObjectFields(serializedObject);
            }
        }

        #region Public Methods

        /// <summary>
        /// Draws all property fields. Remember to wrap this within <code>serializedObject.Update</code>
        /// and <code>serializedObject.ApplyModifiedProperties</code>
        /// </summary>
        public void DrawAllPropertyFields()
        {
            foreach (SerializedProperty sp in _props) EditorGUILayout.PropertyField(sp);
        }

        #endregion

        #region Methods

        // Used when using this class directly without extending it
        private void AutoFillFromSerializedObjectFields(SerializedObject serializedObject)
        {
            Type t = serializedObject.targetObject.GetType();
            FieldInfo[] fInfos = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo f in fInfos) {
                bool isSerialized = f.IsPublic && !f.IsDefined(typeof(NonSerializedAttribute), false)
                                    || f.IsDefined(typeof(SerializeField), false);
                if (!isSerialized) continue;
                SerializedProperty prop = serializedObject.FindProperty(f.Name);
                if (prop == null) continue;
                _props.Add(serializedObject.FindProperty(f.Name));
            }
        }

        // Used from class that extend this method to implement SerializedProperties by name
        private void AutoFillFromChildClassFields(SerializedObject serializedObject)
        {
            Type t = this.GetType();
            FieldInfo[] fInfos = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo f in fInfos) {
                if (f.FieldType != typeof(SerializedProperty)) continue;
                SerializedProperty prop = serializedObject.FindProperty(f.Name);
                if (prop == null) continue;
                f.SetValue(this, prop);
                _props.Add(prop);
            }
        }

        #endregion
    }
}