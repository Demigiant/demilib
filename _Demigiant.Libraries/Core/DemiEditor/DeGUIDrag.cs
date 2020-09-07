// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/30 10:55
// License Copyright (c) Daniele Giardini

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    public enum DeDragResultType
    {
        /// <summary>Nothing is being dragged</summary>
        NoDrag,
        /// <summary>Dragging</summary>
        Dragging,
        /// <summary>Dragging concluced and accepted</summary>
        Accepted,
        /// <summary>Dragging concluced but item position didn't change</summary>
        Ineffective,
        /// <summary>Dragging canceled</summary>
        Canceled,
        /// <summary>Dragging concluced but not accepted because too short</summary>
        Click
    }

    public enum DeDragDirection
    {
        /// <summary>Automatically determines if dragged elements are horizontal, vertical, or both</summary>
        Auto,
        /// <summary>Forces vertical drag</summary>
        Vertical,
        /// <summary>Forces horizontal drag (useful to avoid initial wrong drag indicators
        /// if the users starts dragging an horizontal system vertically)</summary>
        Horizontal
    }

    public struct DeDragResult
    {
        public DeDragResultType outcome;
        public int movedFromIndex, movedToIndex;
        public DeDragResult(DeDragResultType outcome, int movedFromIndex = -1, int movedToIndex = -1)
        {
            this.outcome = outcome;
            this.movedFromIndex = movedFromIndex;
            this.movedToIndex = movedToIndex;
        }
    }

    /// <summary>
    /// Manages the dragging of GUI elements
    /// </summary>
    public static class DeGUIDrag
    {
        /// <summary>
        /// True if a GUI element is currently being dragged
        /// </summary>
        public static bool isDragging { get { return _dragData != null; } }
        /// <summary>
        /// Return the current item being dragged, or NULL if there is none
        /// </summary>
        public static object draggedItem { get { if (_dragData == null) return null; return _dragData.draggedItem; } }
        /// <summary>
        /// Type of current item being dragged, or NULL if there is none
        /// </summary>
        public static Type draggedItemType { get { if (_dragData == null) return null; return _dragData.draggedItem.GetType(); } }
        /// <summary>
        /// Starting index of current item being dragged, or NULL if there is none
        /// </summary>
        public static int draggedItemOriginalIndex { get { if (_dragData == null) return -1; return _dragData.draggedItemIndex; } }
        /// <summary>
        /// Retrieves the eventual optional data stored via the StartDrag method
        /// </summary>
        public static object optionalDragData { get { if (_dragData == null) return null; return _dragData.optionalData; } }

        // Default drag color
        public static readonly Color DefaultDragColor = new Color(0.1720873f, 0.4236527f, 0.7686567f, 0.65f);

        const float _DragDelay = 0.15f;
        static GUIDragData _dragData;
        static int _dragId; // Unused since now it just checks that the dragged list is correct
        static bool _dragLastDrawnIndexSet;
        static int _tmpDragLastDrawnIndex = -1; // Used while trying to retrieve last drawn indexes
        static bool _waitingToApplyDrag;
        static DelayedCall _dragDelayedCall; // Used to set _dragDelayElapsed to TRUE
        static bool _dragDelayElapsed;
        static Editor _editor;
        static EditorWindow _editorWindow;

        #region Public Methods

        /// <summary>
        /// Starts a drag operation on a GUI element.
        /// </summary>
        /// <param name="editor">Reference to the current editor drawing the GUI (used when a Repaint is needed)</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="optionalData">Optional data that can be retrieved via the <see cref="optionalDragData"/> static property</param>
        public static void StartDrag(Editor editor, IList draggableList, int draggedItemIndex, object optionalData = null)
        { DoStartDrag(-1, editor, null, draggableList, draggedItemIndex, optionalData); }
        /// <summary>
        /// Starts a drag operation on a GUI element.
        /// </summary>
        /// <param name="dragId">ID for this drag operation (must be the same for both StartDrag and Drag</param>
        /// <param name="editor">Reference to the current editor drawing the GUI (used when a Repaint is needed)</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="optionalData">Optional data that can be retrieved via the <see cref="optionalDragData"/> static property</param>
        [Obsolete("Use overload that doesn't require dragId instead")]
        public static void StartDrag(int dragId, Editor editor, IList draggableList, int draggedItemIndex, object optionalData = null)
        { DoStartDrag(dragId, editor, null, draggableList, draggedItemIndex, optionalData); }
        /// <summary>
        /// Starts a drag operation on a GUI element.
        /// </summary>
        /// <param name="editorWindow">Reference to the current editor drawing the GUI (used when a Repaint is needed)</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="optionalData">Optional data that can be retrieved via the <see cref="optionalDragData"/> static property</param>
        public static void StartDrag(EditorWindow editorWindow, IList draggableList, int draggedItemIndex, object optionalData = null)
        { DoStartDrag(-1, null, editorWindow, draggableList, draggedItemIndex, optionalData); }
        /// <summary>
        /// Starts a drag operation on a GUI element.
        /// </summary>
        /// <param name="dragId">ID for this drag operation (must be the same for both StartDrag and Drag</param>
        /// <param name="editorWindow">Reference to the current editor drawing the GUI (used when a Repaint is needed)</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="optionalData">Optional data that can be retrieved via the <see cref="optionalDragData"/> static property</param>
        [Obsolete("Use overload that doesn't require dragId instead")]
        public static void StartDrag(int dragId, EditorWindow editorWindow, IList draggableList, int draggedItemIndex, object optionalData = null)
        { DoStartDrag(dragId, null, editorWindow, draggableList, draggedItemIndex, optionalData); }

        static void DoStartDrag(int dragId, Editor editor, EditorWindow editorWindow, IList draggableList, int draggedItemIndex, object optionalData)
        {
            if (_dragData != null) return;
            Reset();
            _editor = editor;
            _editorWindow = editorWindow;
            _dragId = dragId;
            _dragData = new GUIDragData(draggableList, draggableList[draggedItemIndex], draggedItemIndex, optionalData);
            ClearDragDelayedCall();
            _dragDelayedCall = DeEditorUtils.DelayedCall(_DragDelay, ()=> _dragDelayElapsed = true);
        }

        /// <summary>
        /// Call this after each draggable GUI block, to calculate and draw the current drag state
        /// (or complete it if the mouse was released).
        /// </summary>
        /// <param name="dragId">ID for this drag operation (must be the same for both StartDrag and Drag</param>
        /// <param name="draggableList">List containing the draggable item and all other relative draggable items</param>
        /// <param name="currDraggableItemIndex">Current index of the draggable item being drawn</param>
        /// <param name="lastGUIRect">If NULL will calculate this automatically using <see cref="GUILayoutUtility.GetLastRect"/>.
        /// Pass this if you're creating a drag between elements that don't use GUILayout</param>
        /// <param name="direction">Drag direction. You can leave it to <see cref="DeDragDirection.Auto"/>
        /// unless you want to skip eventual layout calculations</param>
        [Obsolete("Use overload that doesn't require dragId instead")]
        public static DeDragResult Drag(
            int dragId, IList draggableList, int currDraggableItemIndex, Rect? lastGUIRect = null, DeDragDirection direction = DeDragDirection.Auto
        )
        { return Drag(draggableList, currDraggableItemIndex, DefaultDragColor, lastGUIRect, direction); }
        /// <summary>
        /// Call this after each draggable GUI block, to calculate and draw the current drag state
        /// (or complete it if the mouse was released).
        /// </summary>
        /// <param name="draggableList">List containing the draggable item and all other relative draggable items</param>
        /// <param name="currDraggableItemIndex">Current index of the draggable item being drawn</param>
        /// <param name="lastGUIRect">If NULL will calculate this automatically using <see cref="GUILayoutUtility.GetLastRect"/>.
        /// Pass this if you're creating a drag between elements that don't use GUILayout</param>
        /// <param name="direction">Drag direction. You can leave it to <see cref="DeDragDirection.Auto"/>
        /// unless you want to skip eventual layout calculations</param>
        public static DeDragResult Drag(
            IList draggableList, int currDraggableItemIndex, Rect? lastGUIRect = null, DeDragDirection direction = DeDragDirection.Auto
        )
        { return Drag(draggableList, currDraggableItemIndex, DefaultDragColor, lastGUIRect, direction); }
        /// <summary>
        /// Call this after each draggable GUI block, to calculate and draw the current drag state
        /// (or complete it if the mouse was released).
        /// </summary>
        /// <param name="dragId">ID for this drag operation (must be the same for both StartDrag and Drag</param>
        /// <param name="draggableList">List containing the draggable item and all other relative draggable items</param>
        /// <param name="currDraggableItemIndex">Current index of the draggable item being drawn</param>
        /// <param name="dragEvidenceColor">Color to use for drag divider and selection</param>
        /// <param name="lastGUIRect">If NULL will calculate this automatically using <see cref="GUILayoutUtility.GetLastRect"/>.
        /// Pass this if you're creating a drag between elements that don't use GUILayout</param>
        /// <param name="direction">Drag direction. You can leave it to <see cref="DeDragDirection.Auto"/>
        /// unless you want to skip eventual layout calculations</param>
        [Obsolete("Use overload that doesn't require dragId instead")]
        public static DeDragResult Drag(
            int dragId, IList draggableList, int currDraggableItemIndex, Color dragEvidenceColor,
            Rect? lastGUIRect = null, DeDragDirection direction = DeDragDirection.Auto
        )
        { return Drag(draggableList, currDraggableItemIndex, dragEvidenceColor, lastGUIRect, direction); }
        /// <summary>
        /// Call this after each draggable GUI block, to calculate and draw the current drag state
        /// (or complete it if the mouse was released).
        /// </summary>
        /// <param name="draggableList">List containing the draggable item and all other relative draggable items</param>
        /// <param name="currDraggableItemIndex">Current index of the draggable item being drawn</param>
        /// <param name="dragEvidenceColor">Color to use for drag divider and selection</param>
        /// <param name="lastGUIRect">If NULL will calculate this automatically using <see cref="GUILayoutUtility.GetLastRect"/>.
        /// Pass this if you're creating a drag between elements that don't use GUILayout</param>
        /// <param name="direction">Drag direction. You can leave it to <see cref="DeDragDirection.Auto"/>
        /// unless you want to skip eventual layout calculations</param>
        public static DeDragResult Drag(
            IList draggableList, int currDraggableItemIndex, Color dragEvidenceColor,
            Rect? lastGUIRect = null, DeDragDirection direction = DeDragDirection.Auto
        ){
            // Drag ID is ignored and is here only for legacy reasons
            if (_dragData == null || _dragData.draggableList == null || _dragData.draggableList != draggableList) return new DeDragResult(DeDragResultType.NoDrag);
//            if (_dragData == null || _dragId == -1 || _dragId != dragId) return new DeDragResult(DeDragResultType.NoDrag);
            if (_waitingToApplyDrag) {
                int dragFrom = _dragData.draggedItemIndex;
                int dragTo = _dragData.currDragIndex;
                if (Event.current.type == EventType.Repaint) Event.current.type = EventType.Used;
                if (Event.current.type == EventType.Used) ApplyDrag();
                return new DeDragResult(DeDragResultType.Dragging, dragFrom, dragTo);
            }

            _dragData.draggableList = draggableList; // Reassign in case of references that change every call (like with EditorBuildSettings.scenes)
            int listCount = _dragData.draggableList.Count;
            // Find first and last drawn indexes (in case of non-layout GUI that isn't drawing all list elements)
            if (currDraggableItemIndex < _dragData.firstDrawnIndex) _dragData.firstDrawnIndex = currDraggableItemIndex;
            if (!_dragLastDrawnIndexSet) {
                if (_tmpDragLastDrawnIndex > currDraggableItemIndex) {
                    _dragData.lastDrawnIndex = _tmpDragLastDrawnIndex;
                    _dragLastDrawnIndexSet = true;
                } else {
                    _dragData.lastDrawnIndex = listCount - 1;
                    _tmpDragLastDrawnIndex = currDraggableItemIndex;
                }
            }
            //
            if (currDraggableItemIndex == _dragData.firstDrawnIndex && Event.current.type == EventType.Repaint) _dragData.currDragSet = false;
            if (!_dragData.currDragSet) {
                // Find and store eventual drag position
                Rect lastRect = lastGUIRect == null ? GUILayoutUtility.GetLastRect() : (Rect)lastGUIRect;
                if (!_dragData.hasHorizontalSet && Event.current.type == EventType.Repaint) {
                    // Set drag direction
                    switch (direction) {
                    case DeDragDirection.Auto:
                        // Auto-determine if drag is also horizontal
                        if (currDraggableItemIndex == _dragData.firstDrawnIndex) _dragData.lastRect = lastRect;
                        else if (_dragData.lastRect.width > 0) {
                            _dragData.hasHorizontalSet = true;
                            _dragData.hasHorizontalEls = !Mathf.Approximately(_dragData.lastRect.xMin, lastRect.xMin);
                        }
                        break;
                    case DeDragDirection.Vertical:
                        _dragData.hasHorizontalSet = true;
                        _dragData.hasHorizontalEls = false;
                        break;
                    case DeDragDirection.Horizontal:
                        _dragData.hasHorizontalSet = true;
                        _dragData.hasHorizontalEls = true;
                        break;
                    }
                }
                Vector2 lastRectMiddleP = lastRect.center;
                Vector2 mouseP = Event.current.mousePosition;
                if (
                    currDraggableItemIndex <= _dragData.lastDrawnIndex && lastRect.Contains(mouseP)
                    && (_dragData.hasHorizontalEls && mouseP.x <= lastRectMiddleP.x || !_dragData.hasHorizontalEls && mouseP.y <= lastRectMiddleP.y)
                ) {
                    if (_dragDelayElapsed) {
                        Rect evidenceR = _dragData.hasHorizontalEls
                            ? new Rect(lastRect.xMin, lastRect.yMin, 5, lastRect.height)
                            : new Rect(lastRect.xMin, lastRect.yMin, lastRect.width, 5);
                        DeGUI.FlatDivider(evidenceR, dragEvidenceColor);
                    }
                    _dragData.currDragIndex = currDraggableItemIndex;
                    _dragData.currDragSet = true;
                } else if (
                    currDraggableItemIndex <= _dragData.lastDrawnIndex && lastRect.Contains(mouseP)
                    && (_dragData.hasHorizontalEls && mouseP.x > lastRectMiddleP.x || !_dragData.hasHorizontalEls && mouseP.y > lastRectMiddleP.y)
                ) {
                    if (_dragDelayElapsed) {
                        Rect evidenceR = _dragData.hasHorizontalEls
                            ? new Rect(lastRect.xMax - 5, lastRect.yMin, 5, lastRect.height)
                            : new Rect(lastRect.xMin, lastRect.yMax - 5, lastRect.width, 5);
                        DeGUI.FlatDivider(evidenceR, dragEvidenceColor);
                    }
                    _dragData.currDragIndex = currDraggableItemIndex + 1;
                    _dragData.currDragSet = true;
                } else if (
                    currDraggableItemIndex == _dragData.firstDrawnIndex && !lastRect.Contains(mouseP)
                    && (_dragData.hasHorizontalEls && (mouseP.x <= lastRect.x || mouseP.y < lastRect.y) || !_dragData.hasHorizontalEls && mouseP.y <= lastRectMiddleP.y)
                ) {
                    // First, with mouse above or aside drag areas
                    if (_dragDelayElapsed) {
                        Rect evidenceR = _dragData.hasHorizontalEls
                            ? new Rect(lastRect.xMin, lastRect.yMin, 5, lastRect.height)
                            : new Rect(lastRect.xMin, lastRect.yMin, lastRect.width, 5);
                        DeGUI.FlatDivider(evidenceR, dragEvidenceColor);
                    }
                    _dragData.currDragIndex = currDraggableItemIndex;
                    _dragData.currDragSet = true;
                } else if (
                    currDraggableItemIndex >= _dragData.lastDrawnIndex
                    && (_dragData.hasHorizontalEls && (mouseP.x > lastRectMiddleP.x || mouseP.y > lastRect.yMax) || !_dragData.hasHorizontalEls && mouseP.y > lastRectMiddleP.y)
                ) {
                    if (_dragDelayElapsed) {
                        Rect evidenceR = _dragData.hasHorizontalEls
                            ? new Rect(lastRect.xMax - 5, lastRect.yMin, 5, lastRect.height)
                            : new Rect(lastRect.xMin, lastRect.yMax - 5, lastRect.width, 5);
                        DeGUI.FlatDivider(evidenceR, dragEvidenceColor);
                    }
                    _dragData.currDragIndex = _dragData.lastDrawnIndex + 1;
                    _dragData.currDragSet = true;
                }
            }
            if (_dragData.draggedItemIndex == currDraggableItemIndex) {
                // Evidence dragged pool
                Color selectionColor = dragEvidenceColor;
                selectionColor.a = 0.35f;
                if (_dragDelayElapsed) DeGUI.FlatDivider(lastGUIRect == null ? GUILayoutUtility.GetLastRect() : (Rect)lastGUIRect, selectionColor);
            }

            if (GUIUtility.hotControl < 1) {
                // End drag
                if (_dragDelayElapsed) return EndDrag(true);
                EndDrag(false);
                return new DeDragResult(DeDragResultType.Click);
            }
            return new DeDragResult(DeDragResultType.Dragging, _dragData.draggedItemIndex, _dragData.currDragIndex);
        }

        /// <summary>
        /// Ends the drag operations, and eventually applies the drag outcome.
        /// Returns TRUE if the position of the dragged item actually changed.
        /// Called automatically by Drag method. Use it only if you want to force the end of a drag operation.
        /// </summary>
        /// <param name="applyDrag">If TRUE applies the drag results, otherwise simply cancels the drag</param>
        public static DeDragResult EndDrag(bool applyDrag)
        {
            if (_dragData == null) return new DeDragResult(DeDragResultType.NoDrag);

            int dragFrom = _dragData.draggedItemIndex;
            int dragTo = _dragData.currDragIndex > _dragData.draggedItemIndex ? _dragData.currDragIndex - 1 : _dragData.currDragIndex;

            if (applyDrag) {
                bool changed = _dragData.currDragIndex < _dragData.draggedItemIndex || _dragData.currDragIndex > _dragData.draggedItemIndex + 1;
                if (Event.current.type == EventType.Repaint) Event.current.type = EventType.Used;
                else if (Event.current.type == EventType.Used) ApplyDrag();
                else _waitingToApplyDrag = true;
                DeDragResultType resultType = changed ? DeDragResultType.Accepted : DeDragResultType.Ineffective;
                return new DeDragResult(resultType, dragFrom, dragTo);
            }

            Reset();
            return new DeDragResult(DeDragResultType.Canceled, dragFrom, dragTo);
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
            _dragLastDrawnIndexSet = false;
            _tmpDragLastDrawnIndex = -1;
            _waitingToApplyDrag = false;
            _dragDelayElapsed = false;
            ClearDragDelayedCall();
        }

        static void ClearDragDelayedCall()
        {
            if (_dragDelayedCall == null) return;
            DeEditorUtils.ClearDelayedCall(_dragDelayedCall);
            _dragDelayedCall = null;
        }

        #endregion
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ CLASS ███████████████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    internal class GUIDragData
    {
        public readonly object draggedItem; // Dragged element
        public readonly int draggedItemIndex;
        public IList draggableList; // Collection within which the drag is being executed
        public int currDragIndex = -1; // Index of current drag position
        public int firstDrawnIndex = int.MaxValue; // Used in case of a non-layout GUI that isn't drawing all elements because of scrolling
        public int lastDrawnIndex = -1; // Used in case of a non-layout GUI that isn't drawing all elements because of scrolling
        public bool currDragSet;
        // Used to auto-check if the drag can be horizontal too
        public Rect lastRect;
        public bool hasHorizontalSet;
        public bool hasHorizontalEls;
        //
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