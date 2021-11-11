// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2021/11/10

using System;
using System.Collections;
using System.Collections.Generic;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.CustomDefinesPanel
{
    /// <summary>
    /// Public so it can be extended for custom projects
    /// </summary>
    public class CustomDefinesPanel : EditorWindow
    {
        [MenuItem("Tools/Demigiant/" + _Title)]
        static void ShowWindow() { GetWindow(typeof(CustomDefinesPanel), false, _Title); }
		
        const string _Title = "Defines Panel";
        const string _SrcADBFilePath = "Assets/-CustomDefinesPanelData.asset";
        CustomDefinesPanelData _src;
        bool _waitingForCompilation;
        Vector2 _scrollPos;
        readonly GUIContent _gcRefresh = new GUIContent("Refresh", "Force refresh");
        readonly GUIContent _gcAdd = new GUIContent("+ ADD", "Add new define");
        readonly GUIContent _gcDrag = new GUIContent("≡", "Drag this element");
        readonly GUIContent _gcEnabled = new GUIContent("✓", "Disable define");
        readonly GUIContent _gcDisabled = new GUIContent("×", "Enable define");
        readonly GUIContent _gcLockedUnlocked = new GUIContent("LOCKED", "Lock/unlock define for editing");
        readonly GUIContent _gcDelete = new GUIContent("×", "Delete define");
        readonly GUIContent _gcUnapplied = new GUIContent("[!]", "Define is un-applied");
        readonly GUIContent _gcApply = new GUIContent("Apply", "Apply Changes");

        #region Unity and GUI Methods

        void OnEnable()
        {
            if (_src == null) _src = DeEditorPanelUtils.ConnectToSourceAsset<CustomDefinesPanelData>(_SrcADBFilePath, true);
            Undo.undoRedoPerformed += Repaint;
            // Get all defines
            if (_src.Refresh()) {
                Repaint();
                EditorUtility.SetDirty(_src);
            }
        }

        void OnDisable()
        { Undo.undoRedoPerformed -= Repaint; }

        void OnHierarchyChange()
        { Repaint(); }

        void OnGUI()
        {
            if (_src == null) _src = DeEditorPanelUtils.ConnectToSourceAsset<CustomDefinesPanelData>(_SrcADBFilePath, true);

            DeGUI.BeginGUI();

            if (EditorApplication.isCompiling) {
                WaitForCompilation();
                return;
            } else {
                _waitingForCompilation = false;
            }

            using (new DeGUILayout.ToolbarScope(new DeSkinColor(0.75f, 0.6f), DeGUI.styles.toolbar.def)) {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(_gcRefresh, DeGUI.styles.button.tool, GUILayout.ExpandWidth(false))) {
                    _src.Refresh();
                    EditorUtility.SetDirty(_src);
                    Repaint();
                }
                GUILayout.Space(2);
                if (GUILayout.Button(_gcAdd, DeGUI.styles.button.tool, GUILayout.ExpandWidth(false))) {
                    _src.defines.Insert(0, new DefineData(_src.GetUniqueId()));
                    GUI.changed = true;
                }
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            Undo.RecordObject(_src, _Title);

            for (int i = 0; i < _src.defines.Count; ++i) {
                DefineData def = _src.defines[i];
                // Define row
                using (new DeGUILayout.ToolbarScope(DeGUI.styles.toolbar.box, GUILayout.ExpandWidth(true))) {
                    if (DeGUILayout.PressButton(_gcDrag, DeGUI.styles.button.tool, GUILayout.Width(16))) {
                        DeGUIDrag.StartDrag(this, _src.defines, i);
                    }
                    using (new EditorGUI.DisabledScope(def.locked)) {
                        using (var check = new EditorGUI.ChangeCheckScope()) {
                            def.enabled = DeGUILayout.ToggleButton(def.enabled, def.enabled ? _gcEnabled : _gcDisabled, DeGUI.styles.button.bBlankBorderCompact, GUILayout.Width(16));
                            if (check.changed) {
                                bool alreadyMarkedForRemoval = _src.definesToRemove.Contains(def.id);
                                if (def.enabled && alreadyMarkedForRemoval) _src.definesToRemove.Remove(def.id);
                                else if (!def.enabled && !alreadyMarkedForRemoval) _src.definesToRemove.Add(def.id);
                                def.applied = false;
                            }
                        }
                        using (new DeGUI.ColorScope(def.enabled ? Color.green : Color.red)) {
                            using (var check = new EditorGUI.ChangeCheckScope()) {
                                string prevId = def.id;
                                string newId = EditorGUILayout.DelayedTextField(def.id, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
                                if (check.changed) {
                                    // Verify id change
                                    if (string.IsNullOrEmpty(newId)) {
                                        EditorUtility.DisplayDialog("Define ID Changed", "You can't have an empty define", "Ok");
                                    } else if (_src.HasDefine(newId)) {
                                        EditorUtility.DisplayDialog("Define ID Changed", string.Format("Define \"{0}\" already exists", newId), "Ok");
                                    } else if (def.applied) {
                                        _src.definesToRemove.Add(def.id);
                                        def.id = newId;
                                        def.applied = false;
                                    } else {
                                        def.id = newId;
                                    }
                                }
                            }
                        }
                    }
                    def.locked = DeGUILayout.ToggleButton(def.locked, _gcLockedUnlocked, DeGUI.colors.global.orange, Color.white, DeGUI.styles.button.bBlankBorderCompact, GUILayout.ExpandWidth(false));
                    using (new EditorGUI.DisabledScope(def.locked)) {
                        using (new DeGUI.ColorScope(Color.red)) {
                            if (GUILayout.Button(_gcDelete, DeGUI.styles.button.tool, GUILayout.Width(16))) {
                                _src.definesToRemove.Add(def.id);
                                _src.defines.RemoveAt(i);
                                i--;
                                _src.hasUnappliedChanges = true;
                                GUI.changed = true;
                            }
                        }
                    }
                    if (!def.applied) {
                        _src.hasUnappliedChanges = true;
                        GUILayout.Label(_gcUnapplied, GUILayout.Width(18));
                    }
                }
                //
                if (DeGUIDrag.Drag(_src.defines, i).outcome == DeDragResultType.Accepted) {
                    GUI.changed = true;
                }
            }

            using (new GUILayout.HorizontalScope()) {
                using (new EditorGUI.DisabledScope(!_src.hasUnappliedChanges)) {
                    if (GUILayout.Button(_gcApply, DeGUI.styles.button.tool, GUILayout.ExpandWidth(true))) {
                        bool removed = DeEditorUtils.RemoveGlobalDefines(_src.definesToRemove);
                        bool added = DeEditorUtils.AddGlobalDefines(_src.GetUnappliedIdsToAdd());
                        if (!removed && !added) _src.Refresh();
                        EditorUtility.SetDirty(_src);
                        Repaint();
                    }
                }
            }

            GUILayout.EndScrollView();
            if (GUI.changed) EditorUtility.SetDirty(_src);
        }

        void WaitForCompilation()
        {
            EditorGUILayout.HelpBox("Waiting for Unity to compile", MessageType.Info, true);
            if (GUILayout.Button("CLICK HERE TO REFRESH", GUILayout.ExpandWidth(true))) {
                DeEditorUtils.DelayedCall(0.1f, Repaint);
            }
        
            if (_waitingForCompilation) return;
        
            _waitingForCompilation = true;
            DeEditorCoroutines.StartCoroutine(WaitForCompilationCoroutine());
        }
        
        IEnumerator WaitForCompilationCoroutine()
        {
            if (!_waitingForCompilation) yield break;
            while (EditorApplication.isCompiling) {
                yield return null;
            }
            _waitingForCompilation = false;
            Repaint();
        }

        #endregion
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    public class CustomDefinesPanelData : ScriptableObject
    {
        public bool hasUnappliedChanges = false;
        public List<DefineData> defines = new List<DefineData>();
        public List<string> definesToRemove = new List<string>();

        // Returns TRUE if something changed in the current defines
        public bool Refresh()
        {
            hasUnappliedChanges = false;
            List<string> currentDefines = DeEditorUtils.GetGlobalDefinesForCurrentBuildTargetGroup();
            bool changed = false;
            foreach (string currDef in currentDefines) {
                bool found = false;
                foreach (DefineData defData in defines) {
                    if (defData.id != currDef) continue;
                    found = true;
                    defData.applied = true;
                    if (defData.enabled) continue;
                    changed = true;
                    defData.enabled = true;
                }
                if (!found) {
                    changed = true;
                    defines.Add(new DefineData(currDef, true, true, true));
                }
            }
            foreach (DefineData defData in defines) {
                if (defData.applied) continue;
                if (!defData.enabled) {
                    defData.applied = true;
                    continue;
                }
                hasUnappliedChanges = true;
                break;
            }
            return changed;
        }

        public bool HasDefine(string id)
        {
            string idLower = id.ToLower();
            foreach (DefineData defData in defines) {
                if (defData.id.ToLower() == idLower) return true;
            }
            return false;
        }

        public List<string> GetUnappliedIdsToAdd()
        {
            List<string> result = new List<string>();
            foreach (DefineData defData in defines) {
                if (!defData.applied && defData.enabled) result.Add(defData.id);
            }
            return result;
        }

        public string GetUniqueId()
        {
            string res = "NEW";
            while (HasDefine(res)) {
                res += "_";
            }
            return res;
        }
        
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    [Serializable]
    public class DefineData
    {
        public string id;
        public bool enabled;
        public bool locked;
        public bool applied;

        public DefineData(string id, bool enabled = true, bool locked = false, bool applied = false)
        {
            this.id = id;
            this.enabled = enabled;
            this.locked = locked;
            this.applied = applied;
        }
    }
}