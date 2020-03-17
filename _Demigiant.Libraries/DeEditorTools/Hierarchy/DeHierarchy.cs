// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/04 11:30
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
using DG.DemiLib;
using DG.DemiLib.External;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Hierarchy
{
    /// <summary>
    /// Relies on <see cref="DeHierarchyComponent"/> in Core library.
    /// </summary>
    [InitializeOnLoad]
    public class DeHierarchy
    {
        static public DeHierarchyComponent dehComponent { get; private set; }
        static readonly Dictionary<GameObject,Renderer> _GoToRenderer = new Dictionary<GameObject, Renderer>();

        static Color _icoVisibilityOnColor, _icoVisibilityOffColor;
        static GUIStyle _evidenceStyle, _btVisibility, _btVisibilityOff, _layerBox, _layerOrderBox;

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

        static void Refresh()
        {
            _GoToRenderer.Clear();

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

            DeGUI.BeginGUI();
            SetStyles();

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            Rect extraR = new Rect(selectionRect.xMax, selectionRect.y, 0, selectionRect.height);
            // Visibility button
            if (showVisibilityButton) {
                bool isActive = go.activeSelf;
                bool isActiveInHierarchy = go.activeInHierarchy;
                Texture2D ico = DeStylePalette.ico_visibility;
                extraR = new Rect(selectionRect.xMax - ico.width - 2, (int)(selectionRect.center.y - (ico.height * 0.5f)), ico.width, ico.height);
                using (new DeGUI.ColorScope(null, null, isActiveInHierarchy ? _icoVisibilityOnColor : _icoVisibilityOffColor)) {
                    if (GUI.Button(extraR, "", isActive ? _btVisibility : _btVisibilityOff)) {
                        Undo.RecordObject(go, go.name + " Visibility");
                        go.SetActive(!isActive);
                        EditorUtility.SetDirty(go);
                    }
                }
            }
            // Sorting layer/order
            bool showSortingLayer = DeEditorToolsPrefs.deHierarchy_showSortingLayer;
            bool showSortingOrder = DeEditorToolsPrefs.deHierarchy_showOrderInLayer;
            if (showSortingLayer || showSortingOrder) {
                if (!_GoToRenderer.ContainsKey(go)) _GoToRenderer.Add(go, go.GetComponent<Renderer>());
                Renderer rend = _GoToRenderer[go];
                if (rend != null) {
                    if (showSortingLayer) {
                        GUIContent label = new GUIContent(rend.sortingLayerName);
                        Vector2 size = _layerOrderBox.CalcSize(label);
                        extraR = extraR.Shift(-size.x - 2, 0, 0, 0).SetY((int)(selectionRect.center.y - (size.y * 0.5f)))
                            .SetHeight(size.y).SetWidth(size.x);
                        GUI.Label(extraR, label, _layerBox);
                    }
                    if (showSortingOrder) {
                        GUIContent label = new GUIContent(rend.sortingOrder.ToString());
                        Vector2 size = _layerOrderBox.CalcSize(label);
                        extraR = extraR.Shift(-size.x - 2, 0, 0, 0).SetY((int)(selectionRect.center.y - (size.y * 0.5f)))
                            .SetHeight(size.y).SetWidth(size.x);
                        GUI.Label(extraR, label, _layerOrderBox);
                    }
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
            if (_evidenceStyle != null) return;

            _icoVisibilityOnColor = new DeSkinColor(DeGUI.IsProSkin ? 0.65f : 0.5f);
            _icoVisibilityOffColor = new DeSkinColor(DeGUI.IsProSkin ? 0.4f : 0.6f);

            _evidenceStyle = DeGUI.styles.button.bBlankBorder.Clone(TextAnchor.MiddleLeft).Background(DeStylePalette.squareBorderCurvedEmpty)
                .PaddingLeft(3).PaddingTop(2);
            _btVisibility = DeGUI.styles.button.flatWhite.Clone().Margin(0).Padding(0).Background(DeStylePalette.ico_visibility)
                .Width(DeStylePalette.ico_visibility.width).Height(DeStylePalette.ico_visibility.height);
            _btVisibilityOff = _btVisibility.Clone().Background(DeStylePalette.ico_visibility_off);
            _layerBox = new GUIStyle(GUI.skin.label).Add(TextAnchor.MiddleCenter, 10, new DeSkinColor(0.2f, 0.8f)).Padding(2, 2, 1, 1).Margin(0)
                .Background(DeGUI.IsProSkin ? DeStylePalette.blackSquare : DeStylePalette.whiteSquare);
            _layerOrderBox = _layerBox.Clone();
        }

        #endregion

        #region Public Methods

        public static void OnPreferencesRefresh(bool flagsChanged)
        {
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

        #endregion
    }
}