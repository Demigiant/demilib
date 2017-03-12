// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/11 20:31
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    /// <summary>
    /// Main class for DeGUI Node system. Call <see cref="BeginGUI"/> when starting a new nodes area.
    /// </summary>
    public class DeGUINodeProcess
    {
        public EditorWindow editor { get; private set; }
        public Rect area { get; private set; }
        public Vector2 areaShift { get; private set; }

        readonly bool _drawBackgroundGrid;
        readonly bool _forceDarkSkin;
        readonly Dictionary<Type,ABSDeGUINode> _typeToNodeController = new Dictionary<Type,ABSDeGUINode>();

        #region CONSTRUCTOR

        /// <summary>
        /// Creates a new DeGUINodeProcess.
        /// </summary>
        /// <param name="editor">EditorWindow for this process</param>
        /// <param name="drawBackgroundGrid">If TRUE draws a background grid</param>
        /// <param name="forceDarkSkin">If TRUE forces the grid skin to black, otherwise varies based on current Unity free/pro skin</param>
        public DeGUINodeProcess(EditorWindow editor, bool drawBackgroundGrid = false, bool forceDarkSkin = false)
        {
            this.editor = editor;
            _drawBackgroundGrid = drawBackgroundGrid;
            _forceDarkSkin = forceDarkSkin;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Begins a new area for nodes and returns TRUE if the area shift has changed (in case of a drag operation).
        /// Doesn't need to be concluded with an End.<para/>
        /// Automatically manages area drag operations with middle mouse.
        /// Also sets <code>GUI.changed</code> to TRUE if the shift changes.
        /// </summary>
        /// <param name="nodeArea">Area within which the nodes will be drawn</param>
        /// <param name="refAreaShift">Area shift (caused by dragging)</param>
        public bool BeginGUI(Rect nodeArea, ref Vector2 refAreaShift)
        {
            bool shiftChanged = false;
            area = nodeArea;
            areaShift = refAreaShift;

            if (_drawBackgroundGrid) DeGUI.BackgroundGrid(area, areaShift, _forceDarkSkin);

            // AREA MOUSE EVENTS
            switch (Event.current.type) {
            case EventType.MouseDrag:
                if (Event.current.button == 2) {
                    refAreaShift = areaShift += Event.current.delta;
                    shiftChanged = GUI.changed = true;
                    editor.Repaint();
                }
                break;
            }

            return shiftChanged;
        }

        public void Draw<T>(IEditorGUINode node, bool isDraggable = true) where T : ABSDeGUINode, new()
        {
            Type type = typeof(T);
            if (!_typeToNodeController.ContainsKey(type)) _typeToNodeController.Add(type, (T)Activator.CreateInstance(type, this));
            ABSDeGUINode guiNode = _typeToNodeController[type];
            Vector2 position = new Vector2((int)(node.guiPosition.x + areaShift.x), (int)(node.guiPosition.y + areaShift.y));
            guiNode.Draw(position, node, isDraggable);
        }

        #endregion
    }
}