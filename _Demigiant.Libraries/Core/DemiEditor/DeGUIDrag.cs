// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/30 10:55
// License Copyright (c) Daniele Giardini

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Manages the dragging of GUI elements
    /// </summary>
    public static class DeGUIDrag
    {
        /// <summary>
        /// True if a GUI element is currently being dragged
        /// </summary>
        static public bool isDragging { get { return _dragData != null; } }
        /// <summary>
        /// Return the current item being dragged, or NULL if there is none.
        /// </summary>
        static public object draggedItem { get { if (_dragData == null) return null; return _dragData.draggedItem; } }
        /// <summary>
        /// Type of current item being dragged, or NULL if there is none.
        /// </summary>
        static public Type draggedItemType { get { if (_dragData == null) return null; return _dragData.draggedItem.GetType(); } }
        /// <summary>
        /// Retrieves the eventual optional data stored via the StartDrag method.
        /// </summary>
        static public object optionalDragData { get { if (_dragData == null) return null; return _dragData.optionalData; } }

        // Default drag color
        static readonly Color _DefDragColor = new Color(0.1720873f, 0.4236527f, 0.7686567f, 0.35f);

        public static GUIDragData _dragData;
        static int _dragId;
        static bool _waitingToApplyDrag;
        static Editor _editor;
        static EditorWindow _editorWindow;

        #region Public Methods

        /// <summary>
        /// Starts a drag operation on a GUI element.
        /// </summary>
        /// <param name="dragId">ID for this drag operation (must be the same for both StartDrag and Drag</param>
        /// <param name="editor">Reference to the current editor drawing the GUI (used when a Repaint is needed)</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="dragItem">Item being dragged</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="optionalData">Optional data that can be retrieved via the <see cref="optionalDragData"/> static property</param>
        public static void StartDrag(int dragId, Editor editor, IList draggableList, object dragItem, int draggedItemIndex, object optionalData = null)
        {
            if (_dragData != null) return;
            Reset();
            _editorWindow = null;
            _editor = editor;
            _dragId = dragId;
            _dragData = new GUIDragData(draggableList, dragItem, draggedItemIndex, optionalData);
        }
        /// <summary>
        /// Starts a drag operation on a GUI element.
        /// </summary>
        /// <param name="dragId">ID for this drag operation (must be the same for both StartDrag and Drag</param>
        /// <param name="editorWindow">Reference to the current editor drawing the GUI (used when a Repaint is needed)</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="dragItem">Item being dragged</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="optionalData">Optional data that can be retrieved via the <see cref="optionalDragData"/> static property</param>
        public static void StartDrag(int dragId, EditorWindow editorWindow, IList draggableList, object dragItem, int draggedItemIndex, object optionalData = null)
        {
            if (_dragData != null) return;
            Reset();
            _editor = null;
            _editorWindow = editorWindow;
            _dragId = dragId;
            _dragData = new GUIDragData(draggableList, dragItem, draggedItemIndex, optionalData);
        }

        /// <summary>
        /// Call this after each draggable GUI block, to calculate and draw the current drag state
        /// (or complete it if the mouse was released).
        /// Returns TRUE if the drag operation was concluded and accepted.
        /// </summary>
        /// <param name="dragId">ID for this drag operation (must be the same for both StartDrag and Drag</param>
        /// <param name="draggableList">List containing the draggable item and all other relative draggable items</param>
        /// <param name="currDraggableItemIndex">Current index of the draggable item being drawn</param>
        static public bool Drag(int dragId, IList draggableList, int currDraggableItemIndex)
        { return Drag(dragId, draggableList, currDraggableItemIndex, _DefDragColor); }

        /// <summary>
        /// Call this after each draggable GUI block, to calculate and draw the current drag state
        /// (or complete it if the mouse was released).
        /// Returns TRUE if the drag operation was concluded and accepted.
        /// </summary>
        /// <param name="dragId">ID for this drag operation (must be the same for both StartDrag and Drag</param>
        /// <param name="draggableList">List containing the draggable item and all other relative draggable items</param>
        /// <param name="currDraggableItemIndex">Current index of the draggable item being drawn</param>
        /// <param name="dragEvidenceColor">Color to use for drag divider and selection</param>
        static public bool Drag(int dragId, IList draggableList, int currDraggableItemIndex, Color dragEvidenceColor)
        {
            if (_dragData == null || _dragId != dragId) return false;
            if (_waitingToApplyDrag) {
                if (Event.current.type == EventType.Repaint) Event.current.Use();
                if (Event.current.type == EventType.Used) ApplyDrag();
                return false;
            }

            _dragData.draggableList = draggableList; // Reassign in case of references that change every call (like with EditorBuildSettings.scenes)
            int listCount = _dragData.draggableList.Count;
            if (currDraggableItemIndex == 0 && Event.current.type == EventType.Repaint) _dragData.currDragSet = false;
            if (!_dragData.currDragSet) {
                // Find and store eventual drag position
                Rect lastRect = GUILayoutUtility.GetLastRect();
                float lastRectMiddleY = lastRect.yMin + lastRect.height * 0.5f;
                float mouseY = Event.current.mousePosition.y;
                if (currDraggableItemIndex <= listCount - 1 && mouseY <= lastRectMiddleY) {
                    DeGUI.FlatDivider(new Rect(lastRect.xMin, lastRect.yMin - 1, lastRect.width, 2), dragEvidenceColor);
                    _dragData.currDragIndex = currDraggableItemIndex;
                    _dragData.currDragSet = true;
                } else if (currDraggableItemIndex >= listCount - 1 && mouseY > lastRectMiddleY) {
                    DeGUI.FlatDivider(new Rect(lastRect.xMin, lastRect.yMax - 1, lastRect.width, 2), dragEvidenceColor);
                    _dragData.currDragIndex = listCount;
                    _dragData.currDragSet = true;
                }
            }
            if (_dragData.draggedItemIndex == currDraggableItemIndex) {
                // Evidence dragged pool
                Color selectionColor = dragEvidenceColor;
                selectionColor.a = 0.35f;
                DeGUI.FlatDivider(GUILayoutUtility.GetLastRect(), selectionColor);
            }

            if (GUIUtility.hotControl < 1) {
                // End drag
                return EndDrag(true);
            }
            return false;
        }

        /// <summary>
        /// Ends the drag operations, and eventually applies the drag result.
        /// Returns TRUE if the position of the dragged item actually changed.
        /// </summary>
        /// <param name="applyDrag">If TRUE applies the drag results, otherwise simply cancels the drag</param>
        public static bool EndDrag(bool applyDrag)
        {
            if (_dragData == null) return false;

            if (applyDrag) {
                bool changed = _dragData.currDragIndex < _dragData.draggedItemIndex || _dragData.currDragIndex > _dragData.draggedItemIndex + 1;
                if (Event.current.type == EventType.Repaint) Event.current.Use();
                else if (Event.current.type == EventType.Used) ApplyDrag();
                else _waitingToApplyDrag = true;
                return changed;
            }

            Reset();
            return true;
        }

        #endregion

        #region Methods

        static void ApplyDrag()
        {
            if (_dragData == null) return;

            int fromIndex = _dragData.draggedItemIndex;
            int toIndex = _dragData.currDragIndex > _dragData.draggedItemIndex ? _dragData.currDragIndex - 1 : _dragData.currDragIndex;
            if (toIndex != fromIndex) {
                int index = fromIndex;
                while (index > toIndex) {
                    index--;
                    _dragData.draggableList[index + 1] = _dragData.draggableList[index];
                    _dragData.draggableList[index] = _dragData.draggedItem;
                }
                while (index < toIndex) {
                    index++;
                    _dragData.draggableList[index - 1] = _dragData.draggableList[index];
                    _dragData.draggableList[index] = _dragData.draggedItem;
                }
            }
            Reset();
            Repaint();
        }

        static void Repaint()
        {
            if (_editor != null) _editor.Repaint();
            else if (_editorWindow != null) _editorWindow.Repaint();
        }

        static void Reset()
        {
            _dragData = null;
            _dragId = -1;
            _waitingToApplyDrag = false;
        }

        #endregion
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ CLASS ███████████████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    public class GUIDragData
    {
        public readonly object draggedItem; // Dragged element
        public readonly int draggedItemIndex;
        public IList draggableList; // Collection within which the drag is being executed
        public int currDragIndex = -1; // Index of current drag position
        public bool currDragSet;
        public object optionalData;

        // ***********************************************************************************
        // CONSTRUCTOR
        // ***********************************************************************************

        public GUIDragData(IList draggableList, object draggedItem, int draggedItemIndex, object optionalData)
        {
            this.draggedItem = draggedItem;
            this.draggedItemIndex = draggedItemIndex;
            this.draggableList = draggableList;
            this.optionalData = optionalData;
        }
    }
}