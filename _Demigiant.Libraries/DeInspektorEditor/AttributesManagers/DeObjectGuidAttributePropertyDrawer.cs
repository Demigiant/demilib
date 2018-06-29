// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/06/29 10:20
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeObjectGuidAttribute), true)]
    public class DeObjectGuidAttributePropertyDrawer : PropertyDrawer
    {
        const int _ObjectNameLineH = 14;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String) return base.GetPropertyHeight(property, label);

            DeObjectGuidAttribute attr = (DeObjectGuidAttribute)attribute;
            if (attr.showObjectName) return EditorGUIUtility.singleLineHeight + _ObjectNameLineH;
            else return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            DeGUI.BeginGUI();
            Styles.Init();

            DeObjectGuidAttribute attr = (DeObjectGuidAttribute)attribute;
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, new GUIContent(label.text, label.tooltip));
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            // Button
            if (GUI.Button(position, property.stringValue)) {
                string path = AssetDatabase.GUIDToAssetPath(property.stringValue);
                if (!string.IsNullOrEmpty(path)) {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
                }
            }
            // Object name
            if (attr.showObjectName) {
                position.y = position.yMax;
                position.height = _ObjectNameLineH;
                string name = AssetDatabase.GUIDToAssetPath(property.stringValue);
                if (!string.IsNullOrEmpty(name)) {
                    int lastSlashIndex = name.LastIndexOf('/');
                    if (lastSlashIndex != -1) {
                        name = string.Format("{0}<b>{1}</b>", name.Substring(0, lastSlashIndex + 1), name.Substring(lastSlashIndex));
                    } else name = string.Format("<b>{0}</b>", name);
                }
                GUI.Label(position, name, Styles.pathLabel);
            }

            EditorGUI.EndProperty();
        }

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        static class Styles
        {
            public static GUIStyle pathLabel;

            static bool _initialized;

            public static void Init()
            {
                if (_initialized) return;

                _initialized = true;

                pathLabel = new GUIStyle(GUI.skin.label).Add(9, TextAnchor.UpperRight, Format.RichText).Padding(0);
            }
        }
    }
}