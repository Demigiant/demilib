// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2019/04/12

using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.Panels
{
    internal class TexturePreviewWindow : EditorWindow
    {
        const string _Title = "Texture Preview";
        Texture2D _texture;
        Vector2 _scrollPos;

        #region Unity and GUI Methods

        void OnGUI()
        {
            if (_texture == null) {
                Close();
                return;
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            DeGUI.BeginGUI();
            
            Rect imgR = new Rect(0, 0, _texture.width, _texture.height);
            GUILayoutUtility.GetRect(_texture.width, _texture.width, _texture.height, _texture.height);
            GUI.DrawTexture(imgR, _texture, ScaleMode.StretchToFill);

            GUILayout.EndScrollView();
        }

        #endregion

        #region Public Methods

        public static void Open(Texture2D textureToPreview)
        {
            TexturePreviewWindow win = GetWindow<TexturePreviewWindow>(true, _Title, true);
            win._texture = textureToPreview;
        }

        #endregion
    }
}