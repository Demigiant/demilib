// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/04 11:30
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DemiEditor;
using DG.DemiLib;
using DG.DemiLib.External;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DeEditorTools.Hierarchy
{
    /// <summary>
    /// Relies on <see cref="DeHierarchyComponent"/> in Core library.
    /// </summary>
    [InitializeOnLoad]
    public class DeHierarchy
    {
        public const string ADBDataPath = "Assets/-DeHierarchyProjectPreferences.asset";

        static DeHierarchyData _projectSrc;
        static public DeHierarchyComponent dehComponent { get; private set; }
        static readonly Dictionary<GameObject,GameObjectData> _GoToData = new Dictionary<GameObject,GameObjectData>();
        static string[] _extraNamespacesToIgnoreInComponents;
        static readonly GUIContent _TmpGUIContent = new GUIContent();
        static readonly List<Component> _TmpComponents = new List<Component>();

        static Rect _evidenceRShiftByVersion = new Rect();

        static bool _stylesSet;
        static Color _icoVisibilityOnColor, _icoVisibilityOffColor, _hasComponentsColor;
        static GUIStyle _evidenceStyle, _btVisibility, _btVisibilityOff, _layerBox, _layerOrderBox, _extraEvidenceBox, _extraEvidenceBoxLabel;

        static DeHierarchy()
        {
            // Delay initialization so GUI is linked and drawn after other assets that modify the Hierarchy (like DeHierarchy)
            EditorApplication.delayCall += Init;
        }

        static void Init()
        {
            EditorApplication.delayCall -= Init;
            EditorApplication.hierarchyWindowChanged -= Refresh;
            EditorApplication.hierarchyWindowItemOnGUI -= ItemOnGUI;
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            EditorApplication.hierarchyWindowChanged += Refresh;
            EditorApplication.hierarchyWindowItemOnGUI += ItemOnGUI;
            Undo.undoRedoPerformed += UndoRedoPerformed;
            Refresh();
        }

        #region GUI

        public static void Refresh()
        {
            if (DeUnityEditorVersion.MajorVersion >= 2020) {
                _evidenceRShiftByVersion = new Rect(17, -1, 1, 1);
            } else if (DeUnityEditorVersion.MajorVersion >= 2017) {
                _evidenceRShiftByVersion = new Rect(16, -1, 0, 0);
            }

            _GoToData.Clear();
            _extraNamespacesToIgnoreInComponents = DeEditorToolsPrefs.deHierarchy_ignoreCustomComponentsNamespaces.Split('\n');
            for (int i = 0; i < _extraNamespacesToIgnoreInComponents.Length; ++i) {
                _extraNamespacesToIgnoreInComponents[i] = _extraNamespacesToIgnoreInComponents[i].Trim();
                if (!_extraNamespacesToIgnoreInComponents[i].EndsWith(".")) _extraNamespacesToIgnoreInComponents[i] = _extraNamespacesToIgnoreInComponents[i] + '.';
            }

            ConnectToDeHierarchyComponent(false, true);
            if (dehComponent == null) return;

            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            // Delete customizedItems that refer to objects not in the scene anymore
            // Doesn't call Undo.RecordObject before the operation, because at this point it would mark the scene dirty even if no change is made
            List<int> missingIndexes = dehComponent.MissingItemsIndexes();
            if (missingIndexes != null) {
                Undo.RecordObject(dehComponent, "DeHierarchy");
                foreach (int missingIndex in missingIndexes) {
                    dehComponent.customizedItems.RemoveAt(missingIndex);
                    EditorUtility.SetDirty(dehComponent);
                }
            }
        }

        static void UndoRedoPerformed()
        {
            Refresh();
            EditorApplication.RepaintHierarchyWindow();
        }

        static void ItemOnGUI(int instanceID, Rect selectionRect)
        {
            bool hasDehComponent = dehComponent != null;
            bool showVisibilityButton = DeEditorToolsPrefs.deHierarchy_showVisibilityButton;
            if (showVisibilityButton) {
                if (Event.current.type == EventType.Layout) return;
            } else if (!hasDehComponent || Event.current.type != EventType.Repaint) return;

            ConnectToProjectData(true);
            DeGUI.BeginGUI();
            SetStyles();

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            bool isActiveInHierarchy = go.activeInHierarchy;
            bool requiresGoData = DeEditorToolsPrefs.deHierarchy_showCustomComponentIndicator
                                  || DeEditorToolsPrefs.deHierarchy_showSortingLayer || DeEditorToolsPrefs.deHierarchy_showOrderInLayer
                                  || _projectSrc.totExtraEvidences > 0;
            GameObjectData goData = requiresGoData
                ? GetGameObjectData(go,
                    DeEditorToolsPrefs.deHierarchy_showCustomComponentIndicator, DeEditorToolsPrefs.deHierarchy_showCustomComponentsInChildrenIndicator
                ) : null;

            // Extra evidence
            if (requiresGoData && goData.hasExtraEvidence) {
                _TmpGUIContent.text = go.name;
                Rect evidenceR = new Rect(selectionRect.x - 1, selectionRect.y + 2, GUI.skin.label.CalcSize(_TmpGUIContent).x, selectionRect.height - 2);
                evidenceR = new Rect(
                    evidenceR.x + _evidenceRShiftByVersion.x, evidenceR.y + _evidenceRShiftByVersion.y,
                    evidenceR.width + _evidenceRShiftByVersion.width, evidenceR.height + _evidenceRShiftByVersion.height
                );
                Color evColor = goData.extraEvidenceColor;
                switch (goData.extraEvidenceMode) {
                case DeHierarchyData.EvidenceMode.Box:
                    Color labelColor = goData.extraEvidenceLabelColor;
                    if (!isActiveInHierarchy) {
                        evColor.a *= 0.4f;
                        labelColor = new DeSkinColor(labelColor == Color.white ? 0.5f : 0.15f);
                    }
                    using (new DeGUI.ColorScope(evColor, labelColor)) {
                        GUI.Box(evidenceR, GUIContent.none, _extraEvidenceBox);
                        GUI.Label(evidenceR, _TmpGUIContent, _extraEvidenceBoxLabel);
                    }
                    break;
                default:
                    if (!isActiveInHierarchy) evColor.a *= 0.5f;
                    using (new DeGUI.ColorScope(null, null, evColor)) {
                        GUI.Box(evidenceR, GUIContent.none, DeGUI.styles.box.roundOutline01);
                    }
                    break;
                }
            }

            // Custom Components icon
            if (DeEditorToolsPrefs.deHierarchy_showCustomComponentIndicator && (goData.hasCustomComponents || goData.hasCustomComponentsInChildren)) {
                if (goData.hasCustomComponents) {
                    Rect compR = new Rect(selectionRect.x - 3, selectionRect.y + 5, 2, selectionRect.height - 8);
                    if (DeEditorToolsPrefs.deHierarchy_showBorder) compR.x -= 1;
                    using (new DeGUI.ColorScope(null, null, _hasComponentsColor)) {
                        GUI.DrawTexture(compR, DeStylePalette.whiteSquare);
                    }
                }
                if (goData.hasCustomComponentsInChildren) {
                    Rect subcompR = new Rect(selectionRect.x - 11, selectionRect.yMax - 2, 7, 1);
                    using (new DeGUI.ColorScope(null, null, _hasComponentsColor)) {
                        GUI.DrawTexture(subcompR, DeStylePalette.whiteSquare);
                    }
                }
            }

            Rect extraR = new Rect(selectionRect.xMax, selectionRect.y, 0, selectionRect.height);
            // Visibility button
            if (showVisibilityButton) {
                bool isActive = go.activeSelf;
                Texture2D ico = DeStylePalette.ico_visibility;
                extraR = new Rect(selectionRect.xMax - ico.width - 2, (int)(selectionRect.center.y - (ico.height * 0.5f)), ico.width, ico.height);
                using (new DeGUI.ColorScope(null, null, isActiveInHierarchy ? _icoVisibilityOnColor : _icoVisibilityOffColor)) {
                    if (DeGUI.DownButton(extraR, "", isActive ? _btVisibility : _btVisibilityOff)) {
                        Undo.RecordObject(go, go.name + " Visibility");
                        go.SetActive(!isActive);
                        EditorUtility.SetDirty(go);
                    }
                }
            }
            // Sorting layer/order
            if ((DeEditorToolsPrefs.deHierarchy_showSortingLayer || DeEditorToolsPrefs.deHierarchy_showOrderInLayer) && goData.hasRenderer) {
                if (DeEditorToolsPrefs.deHierarchy_showSortingLayer) {
                    GUIContent label = new GUIContent(goData.renderer.sortingLayerName);
                    Vector2 size = _layerOrderBox.CalcSize(label);
                    extraR = extraR.Shift(-size.x - 2, 0, 0, 0).SetY((int)(selectionRect.center.y - (size.y * 0.5f)))
                        .SetHeight(size.y).SetWidth(size.x);
                    GUI.Label(extraR, label, _layerBox);
                }
                if (DeEditorToolsPrefs.deHierarchy_showOrderInLayer) {
                    GUIContent label = new GUIContent(goData.renderer.sortingOrder.ToString());
                    Vector2 size = _layerOrderBox.CalcSize(label);
                    extraR = extraR.Shift(-size.x - 2, 0, 0, 0).SetY((int)(selectionRect.center.y - (size.y * 0.5f)))
                        .SetHeight(size.y).SetWidth(size.x);
                    GUI.Label(extraR, label, _layerOrderBox);
                }
            }

            // Tree structure lines
            if (DeEditorToolsPrefs.deHierarchy_showTreeLines && go.transform.parent != null) {
                const float columnStep = 14;
                float minColumnX = DeUnityEditorVersion.Version < 2019.3f ? 30 : 64;
                Rect r = new Rect(selectionRect.x - 14, selectionRect.y, 1, selectionRect.height);
                Color linesCol = new Color(0.4078432f, 0.4078432f, 0.4078432f, 1f);
                bool firstElementDone = false;
                while (r.x > 20) {
                    float colorDepth = columnStep / (r.x - minColumnX + columnStep);
                    if (colorDepth < 0.1f) colorDepth = 0.1f;
                    linesCol = linesCol.SetAlpha(colorDepth);
                    Rect drawR = new Rect(r.x - 8, r.y - 7, r.width, r.height);
                    DeGUI.DrawColoredSquare(drawR, linesCol); // vertical
                    drawR = new Rect(drawR.x + drawR.width, drawR.y + drawR.height - 1, 14 - drawR.width, 1);
                    if (firstElementDone) drawR.width = 2;
                    DeGUI.DrawColoredSquare(drawR, linesCol);
                    firstElementDone = true;
                    r.x -= columnStep;
                }
            }

            if (!hasDehComponent) return;

            DeHierarchyComponent.CustomizedItem customizedItem = dehComponent.GetItem(go);
            if (customizedItem == null) return;

            // Color
            Color color = customizedItem.GetColor();
            if (DeEditorToolsPrefs.deHierarchy_fadeEvidenceWhenInactive && !go.activeInHierarchy) color.a = 0.4f;
            // Icon
            if (DeEditorToolsPrefs.deHierarchy_showIco) {
                const int icoOffset = -30; // was -28
                const int icoSize = 28; // was 28
                const int icoSizeHalf = 14; // was 14
                Rect fullR = selectionRect;
                fullR.x += icoOffset;
                fullR.width += icoSize;
                if (!DeEditorToolsPrefs.deHierarchy_indentIco) {
                    // Full rect not indented
                    Transform t = go.transform;
                    while (t.parent != null) {
                        t = t.parent;
                        fullR.x -= icoSizeHalf;
                        fullR.width += icoSizeHalf;
                    }
                }
                Texture2D icoTexture;
                switch (customizedItem.icoType) {
                case DeHierarchyComponent.IcoType.Star:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_star_border : DeStylePalette.ico_star;
                    break;
                case DeHierarchyComponent.IcoType.Cog:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_cog_border : DeStylePalette.ico_cog;
                    break;
                case DeHierarchyComponent.IcoType.Comment:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_comment_border : DeStylePalette.ico_comment;
                    break;
                case DeHierarchyComponent.IcoType.UI:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_ui_border : DeStylePalette.ico_ui;
                    break;
                case DeHierarchyComponent.IcoType.Play:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_play_border : DeStylePalette.ico_play;
                    break;
                case DeHierarchyComponent.IcoType.Heart:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_heart_border : DeStylePalette.ico_heart;
                    break;
                case DeHierarchyComponent.IcoType.Skull:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_skull_border : DeStylePalette.ico_skull;
                    break;
                case DeHierarchyComponent.IcoType.Camera:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_camera_border : DeStylePalette.ico_camera;
                    break;
                case DeHierarchyComponent.IcoType.Light:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_light_border : DeStylePalette.ico_light;
                    break;
                default: // Dot
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.whiteDot_darkBorder : DeStylePalette.whiteDot;
                    break;
                }
                Rect icoR = new Rect(fullR.x + 9 - icoTexture.width * 0.5f, fullR.y + 9 - icoTexture.height * 0.5f, icoTexture.width, icoTexture.height);
                using (new DeGUI.ColorScope(null, null, color)) GUI.DrawTexture(icoR, icoTexture);
            }
            // Border
            if (DeEditorToolsPrefs.deHierarchy_showBorder) {
                using (new DeGUI.ColorScope(color)) {
                    GUI.Label(selectionRect.Shift(-3, 1, 4, -1), "", _evidenceStyle);
                }
            }
            // Separator
            if (customizedItem.separatorType != DeHierarchyComponent.SeparatorType.None) {
                color = customizedItem.GetSeparatorColor();
                bool thick = DeEditorToolsPrefs.deHierarchy_thickSeparators;
                const int offsetToLBorder = -30; // Offset to reach left border
                const int extraSize = 60;
                Rect separatorR;
                switch (customizedItem.separatorType) {
                case DeHierarchyComponent.SeparatorType.Top:
                    separatorR = new Rect(selectionRect.x, selectionRect.y - (thick ? 1 : 0), selectionRect.width, (thick ? 3 : 1));
                    break;
                default:
                    separatorR = new Rect(selectionRect.x, selectionRect.y + selectionRect.height - 1, selectionRect.width, (thick ? 3 : 1));
                    break;
                }
                separatorR = separatorR.Shift(offsetToLBorder, 0, extraSize, 0);
                DeGUI.DrawColoredSquare(separatorR, color);
            }
        }

        static void SetStyles()
        {
            if (_stylesSet) return;

            _stylesSet = true;

            _icoVisibilityOnColor = new DeSkinColor(DeGUI.IsProSkin ? 0.65f : 0.5f);
            _icoVisibilityOffColor = new DeSkinColor(DeGUI.IsProSkin ? 0.4f : 0.6f);
            _hasComponentsColor = new DeSkinColor(new Color(0.09f, 0.63f, 0.98f));

            _evidenceStyle = DeGUI.styles.button.bBlankBorder.Clone(TextAnchor.MiddleLeft).Background(DeStylePalette.squareBorderCurvedEmpty)
                .PaddingLeft(3).PaddingTop(2);
            _btVisibility = DeGUI.styles.button.flatWhite.Clone().Margin(0).Padding(0).Background(DeStylePalette.ico_visibility)
                .Width(DeStylePalette.ico_visibility.width).Height(DeStylePalette.ico_visibility.height);
            _btVisibilityOff = _btVisibility.Clone().Background(DeStylePalette.ico_visibility_off);
            _layerBox = new GUIStyle(GUI.skin.label).Add(TextAnchor.MiddleCenter, 10, new DeSkinColor(0.2f, 0.8f)).Padding(2, 2, 1, 1).Margin(0)
                .Background(DeGUI.IsProSkin ? DeStylePalette.blackSquare : DeStylePalette.whiteSquare);
            _layerOrderBox = _layerBox.Clone();

            _extraEvidenceBox = DeGUI.styles.box.roundOutline01.Clone().Background(DeStylePalette.whiteSquareCurved02);
            _extraEvidenceBoxLabel = new GUIStyle(GUI.skin.label).Add(Color.white).Padding(0)
                .ContentOffset(DeUnityEditorVersion.MajorVersion < 2020 ? 1 : 2, 0);
        }

        #endregion

        #region Public Methods

        public static void OnPreferencesRefresh(bool flagsChanged)
        {
            Refresh();
            if (flagsChanged) {
                if (dehComponent != null) SetDeHierarchyGOFlags(dehComponent.gameObject);
                EditorApplication.DirtyHierarchyWindowSorting();
            } else EditorApplication.RepaintHierarchyWindow();
        }

        public static void SetColorFor(GameObject go, DeHierarchyComponent.HColor hColor)
        {
            Object[] currSelectedObjs = Selection.objects;
            Selection.activeTransform = go.transform;
            SetColorForSelections(hColor);
            Selection.objects = currSelectedObjs;
        }

        public static void SetIconFor(GameObject go, DeHierarchyComponent.IcoType icoType)
        {
            Object[] currSelectedObjs = Selection.objects;
            Selection.activeTransform = go.transform;
            SetIconForSelections(icoType);
            Selection.objects = currSelectedObjs;
        }

        #endregion

        #region Internal Methods

        // Assumes at least one object is selected
        internal static void ResetSelections()
        {
            ConnectToDeHierarchyComponent(false);
            if (dehComponent == null) return;

            Undo.RecordObject(dehComponent, "DeHierarchy");
            bool changed = false;
            foreach (GameObject go in Selection.gameObjects) {
                changed = dehComponent.RemoveItemData(go) || changed;
            }
            if (dehComponent.customizedItems.Count == 0) {
                Undo.DestroyObjectImmediate(dehComponent.gameObject);
                dehComponent = null;
            } else if (changed) EditorUtility.SetDirty(dehComponent);

            EditorApplication.RepaintHierarchyWindow();
        }

        // Assumes at least one object is selected
        internal static void ResetSeparatorsForSelections()
        {
            ConnectToDeHierarchyComponent(false);
            if (dehComponent == null) return;

            Undo.RecordObject(dehComponent, "DeHierarchy");
            bool changed = false;
            foreach (GameObject go in Selection.gameObjects) {
                changed = dehComponent.ResetSeparator(go) || changed;
            }
            if (dehComponent.customizedItems.Count == 0) {
                Undo.DestroyObjectImmediate(dehComponent.gameObject);
                dehComponent = null;
            } else if (changed) EditorUtility.SetDirty(dehComponent);
        }

        // Assumes at least one object is selected
        // Assumes it's never called with HColor.None
        internal static void SetColorForSelections(DeHierarchyComponent.HColor hColor)
        {
            ConnectToDeHierarchyComponent(true);
            Undo.RecordObject(dehComponent, "DeHierarchy");
            foreach (GameObject go in Selection.gameObjects) {
                dehComponent.StoreItemColor(go, hColor);
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        // Assumes at least one object is selected
        internal static void SetIconForSelections(DeHierarchyComponent.IcoType icoType)
        {
            ConnectToDeHierarchyComponent(true);
            Undo.RecordObject(dehComponent, "DeHierarchy");
            foreach (GameObject go in Selection.gameObjects) {
                dehComponent.StoreItemIcon(go, icoType);
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        // Assumes at least one object is selected
        // Assumes it's never called with HColor.None
        internal static void SetSeparatorForSelections(
            DeHierarchyComponent.SeparatorType? separatorType = null, DeHierarchyComponent.HColor? separatorHColor = null
        ){
            ConnectToDeHierarchyComponent(true);
            Undo.RecordObject(dehComponent, "DeHierarchy");
            foreach (GameObject go in Selection.gameObjects) {
                dehComponent.StoreItemSeparator(go, separatorType, separatorHColor);
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        #endregion

        #region Methods

        static void ConnectToDeHierarchyComponent(bool createIfMissing, bool forceRefresh = false)
        {
            if (dehComponent != null && !forceRefresh) return;

            dehComponent = DeEditorToolsUtils.FindFirstComponentOfType<DeHierarchyComponent>();
            if (dehComponent != null || !createIfMissing) {
                if (dehComponent != null) SetDeHierarchyGOFlags(dehComponent.gameObject);
                return;
            }

            GameObject go = new GameObject(":: DeHierarchy ::");
            SetDeHierarchyGOFlags(go);
            dehComponent = go.AddComponent<DeHierarchyComponent>();
        }

        static void ConnectToProjectData(bool createIfMissing)
        {
            if (_projectSrc == null) _projectSrc = DeEditorPanelUtils.ConnectToSourceAsset<DeHierarchyData>(DeHierarchy.ADBDataPath, createIfMissing, false);
        }

        static void SetDeHierarchyGOFlags(GameObject deHierarchyGO)
        {
            if (DeEditorToolsPrefs.deHierarchy_hideObject) {
                if ((deHierarchyGO.hideFlags & (HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy)) == (HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy)) return;
                deHierarchyGO.hideFlags = HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy;
            } else {
                if ((deHierarchyGO.hideFlags | (HideFlags.DontSaveInBuild)) == (HideFlags.DontSaveInBuild)) return;
                deHierarchyGO.hideFlags = HideFlags.DontSaveInBuild;
            }
        }

        static GameObjectData GetGameObjectData(GameObject go, bool checkForCustomComponents, bool checkForCustomComponentsInChildren)
        {
            if (!_GoToData.ContainsKey(go)) _GoToData.Add(go, new GameObjectData(go, checkForCustomComponents, checkForCustomComponentsInChildren));
            return _GoToData[go];
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        public class GameObjectData
        {
            public readonly Renderer renderer;
            public readonly bool hasRenderer;
            public readonly bool hasCustomComponents;
            public readonly bool hasCustomComponentsInChildren;
            public readonly bool hasExtraEvidence;
            public readonly DeHierarchyData.EvidenceMode extraEvidenceMode;
            public readonly Color extraEvidenceColor;
            public readonly Color extraEvidenceLabelColor;

            public GameObjectData(GameObject go, bool checkForCustomComponents, bool checkForCustomComponentsInChildren)
            {
                renderer = go.GetComponent<Renderer>();
                hasRenderer = renderer != null;
                if (checkForCustomComponents) {
                    // Custom components
                    Transform parentT = go.transform;
                    if (checkForCustomComponentsInChildren) go.GetComponentsInChildren<Component>(true, _TmpComponents);
                    else go.GetComponents<Component>(_TmpComponents);
                    int len = _TmpComponents.Count;
                    for (int i = len - 1; i > 0; --i) { // Ignore 0 because it's always Transform or RectTransform
                        if (_TmpComponents[i] == null) continue; // Happens in case of missing scripts
                        if (checkForCustomComponentsInChildren) {
                            bool isChildComp = _TmpComponents[i].transform != parentT;
                            if (!isChildComp && hasCustomComponents || isChildComp && hasCustomComponentsInChildren) continue;
                            // if (comps[i].GetType().FullName.StartsWith("UnityEngine")) continue;
                            if (!IsValidCustomComponent(_TmpComponents[i].GetType().FullName)) continue;
                            if (isChildComp) hasCustomComponentsInChildren = true;
                            else hasCustomComponents = true;
                            if (hasCustomComponents && hasCustomComponentsInChildren) break;
                        } else {
                            if (!IsValidCustomComponent(_TmpComponents[i].GetType().FullName)) continue;
                            hasCustomComponents = true;
                            break;
                        }
                    }
                }
                if (_projectSrc.totExtraEvidences > 0) {
                    // Extra evidences
                    go.GetComponents<Component>(_TmpComponents);
                    int len = _TmpComponents.Count;
                    for (int i = len - 1; i > 0; --i) { // Ignore 0 because it's always Transform or RectTransform
                        if (_TmpComponents[i] == null) continue; // Happens in case of missing scripts
                        string typeName = _TmpComponents[i].GetType().FullName;
                        for (int j = 0; j < _projectSrc.totExtraEvidences; ++j) {
                            if (string.IsNullOrEmpty(_projectSrc.extraEvidences[j].componentClass)) continue;
                            if (!typeName.EndsWith(_projectSrc.extraEvidences[j].componentClass)) continue;
                            hasExtraEvidence = true;
                            extraEvidenceMode = _projectSrc.extraEvidences[j].evidenceMode;
                            extraEvidenceColor = _projectSrc.extraEvidences[j].color;
                            extraEvidenceLabelColor = DeGUI.GetVisibleContentColorOn(extraEvidenceColor);
                            break;
                        }
                        if (hasExtraEvidence) break;
                    }
                }
            }

            bool IsValidCustomComponent(string typeName)
            {
                if (typeName.StartsWith("UnityEngine.")) return false;
                for (int i = 0; i < _extraNamespacesToIgnoreInComponents.Length; ++i) {
                    if (typeName.StartsWith(_extraNamespacesToIgnoreInComponents[i])) return false;
                }
                return true;
            }
        }
    }
}