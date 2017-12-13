// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/12/13 16:19
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core.DebugSystem
{
    internal static class NodeProcessDebugGUI
    {
        #region GUI

        public static void Draw(NodeProcessDebug debug, Rect processArea)
        {
            if (Event.current.type != EventType.Repaint) return;

            const int bgPadding = 6;

            string panningAvrgFps = "<b>Panning Avrg FPS:</b> " + debug.GetPanningAvrgFps().ToString("N2");

            Vector2 panningSize = Styles.fpsLabel.CalcSize(new GUIContent(panningAvrgFps));

            Rect bgR = new Rect(processArea.x, processArea.y, panningSize.x + bgPadding * 2, panningSize.y + bgPadding * 2);
            Rect panningR = new Rect(bgR.x + bgPadding, bgR.y + bgPadding, panningSize.x, panningSize.y);

            DeGUI.DrawColoredSquare(bgR, new Color(0, 0, 0, 0.8f));
            GUI.Label(panningR, panningAvrgFps, Styles.fpsLabel);
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        static class Styles
        {
            public static GUIStyle fpsLabel;

            static Styles()
            {
                fpsLabel = new GUIStyle(GUI.skin.label).Add(TextAnchor.MiddleLeft, Color.white, Format.RichText).Padding(0).Margin(0);
            }
        }
    }
}