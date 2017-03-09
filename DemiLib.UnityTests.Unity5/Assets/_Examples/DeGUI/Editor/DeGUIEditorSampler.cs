// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/09 11:43
// License Copyright (c) Daniele Giardini

using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

public class DeGUIEditorSampler : EditorWindow
{
		
    const string _Title = "DeGUIEditorSampler";
    Vector2 _scrollPos;

    #region Open

    public static void ShowWindow() { GetWindow(typeof(DeGUIEditorSampler), true, _Title); }

    #endregion

    #region Unity and GUI Methods

    void OnHierarchyChange()
    { Repaint(); }

    void OnGUI()
    {
        if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed") Repaint();
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);
        DeGUI.BeginGUI();
			
        DeGUI.BackgroundGrid(new Rect(0, 0, this.position.width, this.position.height), Vector2.zero);

        GUILayout.EndScrollView();
    }

    #endregion
}