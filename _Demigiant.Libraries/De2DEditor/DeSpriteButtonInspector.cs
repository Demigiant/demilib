// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/06/07 21:51
// License Copyright (c) Daniele Giardini

using DG.De2D;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.De2DEditor
{
    [CustomEditor(typeof(DeSpriteButton)), CanEditMultipleObjects]
    public class DeSpriteButtonInspector : Editor
    {
        enum ColliderType
        {
            Box,
            Circle,
            Capsule,
            Polygon
        }

        DeSpriteButton _src;

        SerializedProperty _p_interactable,
                           _p_colors,
                           _p_onClick,
                           _p_onPress,
                           _p_onRelease;

        #region Unity and GUI Methods

        void OnEnable()
        {
            _src = this.target as DeSpriteButton;

            _p_interactable = serializedObject.FindProperty("_interactable");
            _p_colors = serializedObject.FindProperty("colors");
            _p_onClick = serializedObject.FindProperty("onClick");
            _p_onPress = serializedObject.FindProperty("onPress");
            _p_onRelease = serializedObject.FindProperty("onRelease");

            _src.OnEditorRefreshRequired += RefreshSprite;
        }

        void OnDisable()
        {
            _src.OnEditorRefreshRequired -= RefreshSprite;
        }

        public override void OnInspectorGUI()
        {
            DeGUI.BeginGUI();
            bool missingCollider = false;
            foreach (Object obj in serializedObject.targetObjects) {
                if (((DeSpriteButton)obj).GetComponent<Collider2D>() == null) {
                    missingCollider = true;
                    break;
                }
            }
            if (missingCollider) {
                EditorGUILayout.HelpBox("You need to attach a Collider2D to this object", MessageType.Warning);
                using (new DeGUI.ColorScope(null, null, Color.yellow))
                using (new GUILayout.VerticalScope(DeGUI.styles.box.def)) {
                    if (GUILayout.Button("Attach BoxCollider2D")) AttachCollider(ColliderType.Box);
                    if (GUILayout.Button("Attach CircleCollider2D")) AttachCollider(ColliderType.Circle);
                    if (GUILayout.Button("Attach CapsuleCollider2D")) AttachCollider(ColliderType.Capsule);
                    if (GUILayout.Button("Attach PolygonCollider2D")) AttachCollider(ColliderType.Polygon);
                }
            } else {
                const string message = "You need to order your sprites via z-depth, not \"Order in Layer\"." +
                                       "\nAlso note that Trigger colliders will work only if your Physics2D settings allow raycasting on Triggers.";
                EditorGUILayout.HelpBox(message, MessageType.Info);
            }

            bool refreshSprite = false;

            serializedObject.Update();

            using (var check = new EditorGUI.ChangeCheckScope()) {
                EditorGUILayout.PropertyField(_p_interactable);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_p_colors);
                EditorGUI.indentLevel--;
                if (check.changed) refreshSprite = true;
            }
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(_p_onClick);
            EditorGUILayout.PropertyField(_p_onPress);
            EditorGUILayout.PropertyField(_p_onRelease);

            serializedObject.ApplyModifiedProperties();

            if (refreshSprite) RefreshSprite();
        }

        #endregion

        #region Methods

        void RefreshSprite()
        {
            foreach (Object obj in serializedObject.targetObjects) {
                DeSpriteButton bt = (DeSpriteButton)obj;
                SpriteRenderer spriteR = bt.GetComponent<SpriteRenderer>();
                Undo.RecordObject(spriteR, "DeSpriteButton");
                spriteR.color = (bt.interactable ? bt.colors.normalColor : bt.colors.disabledColor) * bt.colors.colorMultiplier;
                EditorUtility.SetDirty(spriteR);
            }
        }

        void AttachCollider(ColliderType type)
        {
            foreach (Object obj in serializedObject.targetObjects) {
                DeSpriteButton bt = (DeSpriteButton)obj;
                Collider2D coll = bt.GetComponent<Collider2D>();
                if (coll != null) continue;
                switch (type) {
                case ColliderType.Box:
                    coll = Undo.AddComponent<BoxCollider2D>(bt.gameObject);
                    break;
                case ColliderType.Circle:
                    coll = Undo.AddComponent<CircleCollider2D>(bt.gameObject);
                    break;
                case ColliderType.Capsule:
                    coll = Undo.AddComponent<CapsuleCollider2D>(bt.gameObject);
                    break;
                case ColliderType.Polygon:
                    coll = Undo.AddComponent<PolygonCollider2D>(bt.gameObject);
                    break;
                }
                if (coll != null) coll.isTrigger = true;
            }
        }

        #endregion
    }
}