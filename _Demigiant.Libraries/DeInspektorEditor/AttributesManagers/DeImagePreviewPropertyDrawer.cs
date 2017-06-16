// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/06/15 18:58
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeImagePreviewAttribute))]
    public class DeImagePreviewPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) return base.GetPropertyHeight(property, label);

            if (property.objectReferenceValue == null) return EditorGUIUtility.singleLineHeight;

            if (property.objectReferenceValue is Sprite || property.objectReferenceValue is Texture) return DeImagePreviewAttribute.Height;

            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DeImagePreviewAttribute attr = (DeImagePreviewAttribute)attribute;
            DeGUI.BeginGUI();
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.ObjectReference) EditorGUI.PropertyField(position, property, label);
            else {
                bool isSprite = property.type.Contains("$Sprite");
                bool isTexture = !isSprite && property.type.Contains("$Texture");
                using (new DeGUI.ColorScope(
                    attr.emptyAlert && property.objectReferenceValue == null ? Color.red : Color.white,
                    attr.emptyAlert && property.objectReferenceValue == null ? (Color)DeGUI.colors.content.critical : Color.white)
                ) {
                    if (!isSprite && !isTexture) {
                        EditorGUI.PropertyField(position, property, label);
                    } else {
                        EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
                        position = EditorGUI.PrefixLabel(position, label);
                        UnityEngine.Object obj;
                        Rect imgR;
                        if (property.objectReferenceValue == null) {
                            imgR = position;
                        } else {
                            Vector2 textureSize;
                            if (isSprite) {
                                // Sprite
                                Rect textureRect = ((Sprite)property.objectReferenceValue).rect;
                                textureSize = new Vector2(textureRect.width, textureRect.height);
                            } else {
                                // Texture
                                Texture texture = (Texture)property.objectReferenceValue;
                                textureSize = new Vector2(texture.width, texture.height);
                            }
                            imgR = EvalImageRect(position, textureSize);
                        }
                        using (var check = new EditorGUI.ChangeCheckScope()) {
                            obj = EditorGUI.ObjectField(
                                imgR, property.objectReferenceValue, isSprite ? typeof(Sprite) : typeof(Texture), true
                            );
                            if (check.changed) property.objectReferenceValue = obj;
                        }
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        #region Helpers

        Rect EvalImageRect(Rect position, Vector2 textureSize)
        {
            float factor = textureSize.x / textureSize.y;
            return new Rect(position.x, position.y, position.height * factor, position.height);
        }

        #endregion
    }
}