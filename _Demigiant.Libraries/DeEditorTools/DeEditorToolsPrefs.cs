// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/02 18:41
// License Copyright (c) Daniele Giardini

using DG.DeEditorTools.Hierarchy;
using DG.DeEditorTools.Scene;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools
{
    public class DeEditorToolsPrefs
    {
        const string _Version = "1.0.110";

        public static bool deScene_enableContextMenu;
        public static bool deHierarchy_hideObject;
        public static bool deHierarchy_showTreeLines;
        public static bool deHierarchy_showVisibilityButton;
        public static bool deHierarchy_showSortingLayer;
        public static bool deHierarchy_showOrderInLayer;
        public static bool deHierarchy_showIco;
        public static bool deHierarchy_showIcoBorder;
        public static bool deHierarchy_indentIco;
        public static bool deHierarchy_showBorder;
        public static bool deHierarchy_fadeEvidenceWhenInactive;
        public static bool deHierarchy_thickSeparators;

        const string _SavePrefix = "DeEditorTools_";
        const string _ID_DeScene_EnableContextMenu = _SavePrefix + "deScene_enableContextMenu";
        const string _ID_DeHierarchy_HideObject = _SavePrefix + "deHierarchy_hideObject";
        const string _ID_DeHierarchy_ShowTreeLines = _SavePrefix + "deHierarchy_showTreeLines";
        const string _ID_DeHierarchy_ShowVisibilityButton = _SavePrefix + "deHierarchy_showVisibilityButton";
        const string _ID_DeHierarchy_ShowSortingLayern = _SavePrefix + "deHierarchy_showSortingLayer";
        const string _ID_DeHierarchy_ShowOrderInLayer = _SavePrefix + "deHierarchy_showOrderInLayer";
        const string _ID_DeHierarchy_ShowIco = _SavePrefix + "deHierarchy_showIco";
        const string _ID_DeHierarchy_ShowIcoBorder = _SavePrefix + "deHierarchy_showIcoBorder";
        const string _ID_DeHierarchy_IndentIco = _SavePrefix + "deHierarchy_indentIco";
        const string _ID_DeHierarchy_ShowBorder = _SavePrefix + "deHierarchy_showBorder";
        const string _ID_DeHierarchy_FadeEvidenceWhenInactive = _SavePrefix + "deHierarchy_fadeEvidenceWhenInactive";
        const string _ID_DeHierarchy_ThickSeparators = _SavePrefix + "deHierarchy_thickSeparators";

        // Elements to remove from EditorPrefs (possible leftovers from previous versions)
        static readonly string[] _PrefsToDelete = new [] {
           "De2D_enableSceneContextMenu",
           "DeEditorTools_enableSceneContextMenu",
           "DeEditorTools_hideDeHierarchyObject",
           "DeEditorTools_deHierarchy_showDot",
           "DeEditorTools_deHierarchy_showDotBorder",
           "DeEditorTools_deHierarchy_indentDot",
        };

        static DeEditorToolsPrefs()
        {
            // Delete leftover preferences
            foreach (string s in _PrefsToDelete) {
                if (EditorPrefs.HasKey(s)) EditorPrefs.DeleteKey(s);
            }
            // Load preferences
            deScene_enableContextMenu = EditorPrefs.GetBool(_ID_DeScene_EnableContextMenu, true);
            deHierarchy_hideObject = EditorPrefs.GetBool(_ID_DeHierarchy_HideObject, false);
            deHierarchy_showTreeLines = EditorPrefs.GetBool(_ID_DeHierarchy_ShowTreeLines, true);
            deHierarchy_showVisibilityButton = EditorPrefs.GetBool(_ID_DeHierarchy_ShowVisibilityButton, true);
            deHierarchy_showSortingLayer = EditorPrefs.GetBool(_ID_DeHierarchy_ShowSortingLayern, false);
            deHierarchy_showOrderInLayer = EditorPrefs.GetBool(_ID_DeHierarchy_ShowOrderInLayer, false);
            deHierarchy_showIco = EditorPrefs.GetBool(_ID_DeHierarchy_ShowIco, true);
            deHierarchy_showIcoBorder = EditorPrefs.GetBool(_ID_DeHierarchy_ShowIcoBorder, false);
            deHierarchy_indentIco = EditorPrefs.GetBool(_ID_DeHierarchy_IndentIco, false);
            deHierarchy_showBorder = EditorPrefs.GetBool(_ID_DeHierarchy_ShowBorder, true);
            deHierarchy_fadeEvidenceWhenInactive = EditorPrefs.GetBool(_ID_DeHierarchy_FadeEvidenceWhenInactive, true);
            deHierarchy_thickSeparators = EditorPrefs.GetBool(_ID_DeHierarchy_ThickSeparators, true);
        }

        [PreferenceItem("DeEditorTools")]
        public static void PreferencesGUI()
        {
            DeGUI.BeginGUI();

            GUILayout.Label("v" + _Version);

            // DeScene
            GUILayout.Label("DeScene", EditorStyles.largeLabel);
            using (new GUILayout.VerticalScope(GUI.skin.box)) {
                deScene_enableContextMenu = DeGUILayout.ToggleButton(deScene_enableContextMenu, "Enable Scene ContextMenu", GUILayout.Height(16));
                GUILayout.Label(
                    "When this is enabled, a context menu will be available in the Scene Panel with various features. Note though that enabling it will disable regular right-click (which by default is the same as middle-click, and shows the panning instrument).",
                    EditorStyles.wordWrappedMiniLabel
                );
            }

            // DeHierarchy
            GUILayout.Space(4);
            GUILayout.Label("DeHierarchy", EditorStyles.largeLabel);
            bool hierarchyChanged = false;
            bool flagsChanged = false;
            using (new GUILayout.VerticalScope(GUI.skin.box)) {
                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginChangeCheck();
                deHierarchy_hideObject = DeGUILayout.ToggleButton(deHierarchy_hideObject, "Hide DeHierarchy GameObject", GUILayout.Height(16));
                flagsChanged = EditorGUI.EndChangeCheck();
                deHierarchy_showTreeLines = EditorGUILayout.Toggle("Show tree structure", deHierarchy_showTreeLines);
                deHierarchy_showVisibilityButton = EditorGUILayout.Toggle("Show visibility button", deHierarchy_showVisibilityButton);
                deHierarchy_showSortingLayer = EditorGUILayout.Toggle("Show sorting layer", deHierarchy_showSortingLayer);
                deHierarchy_showOrderInLayer = EditorGUILayout.Toggle("Show sorting order", deHierarchy_showOrderInLayer);
                deHierarchy_showIco = EditorGUILayout.Toggle("Show colored icon", deHierarchy_showIco);
                using (new EditorGUI.DisabledScope(!deHierarchy_showIco)) {
                    deHierarchy_indentIco = EditorGUILayout.Toggle(" └ Indent icon", deHierarchy_indentIco);
                    deHierarchy_showIcoBorder = EditorGUILayout.Toggle(" └ Show icon outline", deHierarchy_showIcoBorder);
                }
                deHierarchy_showBorder = EditorGUILayout.Toggle("Show colored border", deHierarchy_showBorder);
                deHierarchy_fadeEvidenceWhenInactive = EditorGUILayout.Toggle("Fade evidence if object is inactive", deHierarchy_fadeEvidenceWhenInactive);
                deHierarchy_thickSeparators = EditorGUILayout.Toggle("Thick separators", deHierarchy_thickSeparators);
                hierarchyChanged = EditorGUI.EndChangeCheck();
            }

            if (GUI.changed) SaveAll();

            if (hierarchyChanged || flagsChanged) DeHierarchy.OnPreferencesRefresh(flagsChanged);
        }

        static void SaveAll()
        {
            EditorPrefs.SetBool(_ID_DeScene_EnableContextMenu, deScene_enableContextMenu);
            EditorPrefs.SetBool(_ID_DeHierarchy_HideObject, deHierarchy_hideObject);
            EditorPrefs.SetBool(_ID_DeHierarchy_ShowTreeLines, deHierarchy_showTreeLines);
            EditorPrefs.SetBool(_ID_DeHierarchy_ShowVisibilityButton, deHierarchy_showVisibilityButton);
            EditorPrefs.SetBool(_ID_DeHierarchy_ShowSortingLayern, deHierarchy_showSortingLayer);
            EditorPrefs.SetBool(_ID_DeHierarchy_ShowOrderInLayer, deHierarchy_showOrderInLayer);
            EditorPrefs.SetBool(_ID_DeHierarchy_ShowIco, deHierarchy_showIco);
            EditorPrefs.SetBool(_ID_DeHierarchy_ShowIcoBorder, deHierarchy_showIcoBorder);
            EditorPrefs.SetBool(_ID_DeHierarchy_IndentIco, deHierarchy_indentIco);
            EditorPrefs.SetBool(_ID_DeHierarchy_ShowBorder, deHierarchy_showBorder);
            EditorPrefs.SetBool(_ID_DeHierarchy_FadeEvidenceWhenInactive, deHierarchy_fadeEvidenceWhenInactive);
            EditorPrefs.SetBool(_ID_DeHierarchy_ThickSeparators, deHierarchy_thickSeparators);
        }
    }
}