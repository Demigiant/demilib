// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/12/13 16:19
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core.DebugSystem
{
    internal static class NodeProcessDebugGUI
    {
        static readonly string[] _LabelsTxts = new string[3];
        static readonly Rect[] _LabelsRects = new Rect[3];

        #region GUI

        public static void Draw(NodeProcessDebug debug, Rect processArea)
        {
            if (Event.current.type != EventType.Repaint) return;

            const int bgPadding = 6;
            const int dist = 2;

            _LabelsTxts[0] = "<b><color=#ff0000>NodeProcess DEBUG</color></b>";
            _LabelsTxts[1] = string.Format(
                "<b>Panning Avrg <color=#4290f5>FPS</color> (Layout/Repaint/Layout+Repaint):</b> <color=#ffd845>{0:N2}</color> / <color=#ffd845>{1:N2}</color> / <color=#ffd845>{2:N2}</color>",
                debug.panningData.avrgFps_Layout,
                debug.panningData.avrgFps_Repaint,
                debug.panningData.avrgFps_LayoutAndRepaint
            );
            _LabelsTxts[2] = string.Format(
                "<b>Panning Avrg <color=#4290f5>MS</color> (Layout/Repaint/Layout+Repaint):</b> <color=#ffd845>{0:N2}</color> / <color=#ffd845>{1:N2}</color> / <color=#ffd845>{2:N2}</color>",
                debug.panningData.avrgDrawTime_Layout,
                debug.panningData.avrgDrawTime_Repaint,
                debug.panningData.avrgDrawTime_LayoutAndRepaint
            );

            Rect bgR = new Rect(processArea.x, processArea.y, 0, bgPadding * 2);
            for (int i = 0; i < _LabelsTxts.Length; ++i) {
                Vector2 labelSize = Styles.fpsLabel.CalcSize(new GUIContent(_LabelsTxts[i]));
                _LabelsRects[i] = new Rect(bgR.x + bgPadding, bgR.yMax - bgPadding, labelSize.x, labelSize.y);
                if (bgR.width < labelSize.x + bgPadding * 2) bgR.width = labelSize.x + bgPadding * 2;
                if (i > 0) bgR.height += dist;
                bgR.height += labelSize.y;
            }

            DeGUI.DrawColoredSquare(bgR, new Color(0, 0, 0, 0.8f));
            for (int i = 0; i < _LabelsTxts.Length; ++i) {
                GUI.Label(_LabelsRects[i], _LabelsTxts[i], Styles.fpsLabel);
            }
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