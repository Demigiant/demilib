// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/06/07 21:51
// License Copyright (c) Daniele Giardini

using DG.De2D;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

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
                           _p_transition,
                           _p_highlightedScaleFactor, _p_pressedScaleFactor, _p_disabledScaleFactor, _p_duration,
                           _p_colors,
                           _p_showOnClick, _p_showOnPress, _p_showOnRelease,
                           _p_onClick, _p_onPress, _p_onRelease;

        #region Unity and GUI Methods

        void OnEnable()
        {
            _src = this.target as DeSpriteButton;

            _p_interactable = serializedObject.FindProperty("_interactable");
            _p_transition = serializedObject.FindProperty("_transition");
            _p_highlightedScaleFactor = serializedObject.FindProperty("_highlightedScaleFactor");
            _p_pressedScaleFactor = serializedObject.FindProperty("_pressedScaleFactor");
            _p_disabledScaleFactor = serializedObject.FindProperty("_disabledScaleFactor");
            _p_duration = serializedObject.FindProperty("_duration");
            _p_colors = serializedObject.FindProperty("colors");
            _p_showOnClick = serializedObject.FindProperty("_showOnClick");
            _p_showOnPress = serializedObject.FindProperty("_showOnPress");
            _p_showOnRelease = serializedObject.FindProperty("_showOnRelease");
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
                EditorGUILayout.PropertyField(_p_transition);
                if (!_p_transition.hasMultipleDifferentValues) {
                    // Transition
                    EditorGUI.indentLevel++;
                    switch ((DeSpriteButton.TransitionType)_p_transition.enumValueIndex) {
                    case DeSpriteButton.TransitionType.BounceScale:
                        EditorGUILayout.PropertyField(_p_highlightedScaleFactor, new GUIContent("Highlighted Factor"));
                        EditorGUILayout.PropertyField(_p_pressedScaleFactor, new GUIContent("Pressed Factor"));
                        EditorGUILayout.PropertyField(_p_disabledScaleFactor, new GUIContent("Disabled Factor"));
                        EditorGUILayout.PropertyField(_p_duration, new GUIContent("Bounce Duration"));
                        break;
                    case DeSpriteButton.TransitionType.ColorTint:
                        EditorGUILayout.PropertyField(_p_colors);
                        break;
                    }
                    EditorGUI.indentLevel--;
                }
                if (check.changed) refreshSprite = true;
            }
            GUILayout.Space(4);
            using (new GUILayout.HorizontalScope()) {
                CallbackToggle("On Click", _p_showOnClick, _p_onClick);
                CallbackToggle("On Press", _p_showOnPress, _p_onPress);
                CallbackToggle("On Release", _p_showOnRelease, _p_onRelease);
            }
            if (_p_showOnClick.boolValue) EditorGUILayout.PropertyField(_p_onClick);
            if (_p_showOnPress.boolValue) EditorGUILayout.PropertyField(_p_onPress);
            if (_p_showOnRelease.boolValue) EditorGUILayout.PropertyField(_p_onRelease);

            serializedObject.ApplyModifiedProperties();

            if (refreshSprite) RefreshSprite();
        }

        void CallbackToggle(string label, SerializedProperty property, SerializedProperty evProperty)
        {
            using (var check = new EditorGUI.ChangeCheckScope()) {
                property.boolValue = DeGUILayout.ToggleButton(property.boolValue, label);
                if (check.changed && !property.boolValue) {
                    // Clear all events if unset
                    SerializedProperty evs = evProperty.FindPropertyRelative("m_PersistentCalls.m_Calls");
                    evs.ClearArray();
                    serializedObject.ApplyModifiedProperties();
                }
            }
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