// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2019/04/12

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core
{
    internal static class ScreenshotManager
    {
        #region Public Methods

        public static void CaptureScreenshot(NodeProcess process, NodeProcess.ScreenshotMode screenshotMode, Action<Texture2D> onComplete, float allNodesScaleFactor, bool useProgressBar)
        {
            process.editor.Focus();
            switch (screenshotMode) {
            case NodeProcess.ScreenshotMode.VisibleArea:
                DeEditorCoroutines.StartCoroutine(CaptureScreenshot_VisibleArea(process, onComplete, useProgressBar));
                break;
            case NodeProcess.ScreenshotMode.AllNodes:
                DeEditorCoroutines.StartCoroutine(CaptureScreenshot_AllNodes(process, onComplete, allNodesScaleFactor, useProgressBar));
                break;
            }
        }

        #endregion

        #region Methods

        static IEnumerator CaptureScreenshot_VisibleArea(NodeProcess process, Action<Texture2D> onComplete, bool useProgressBar)
        {
            if (useProgressBar) EditorUtility.DisplayProgressBar("Screenshot", "Capturing...", 0.5f);
            process.editor.Repaint();
            yield return DeEditorCoroutines.WaitForSeconds(0.001f);
            process.editor.Focus();
            yield return DeEditorCoroutines.WaitForSeconds(0.001f);

            Rect captureR = process.position.ResetXY();
            Texture2D tx = new Texture2D((int)captureR.width, (int)captureR.height, TextureFormat.RGB24, false);
            tx.ReadPixels(captureR, 0, 0, false);
            tx.Apply();
            if (useProgressBar) EditorUtility.ClearProgressBar();
            onComplete(tx);
        }

        static IEnumerator CaptureScreenshot_AllNodes(NodeProcess process, Action<Texture2D> onComplete, float scaleFactor, bool useProgressBar)
        {
            bool hasMinimap = process.options.showMinimap;
            float orGuiScale = process.guiScale;
            Vector2 orShift = process.areaShift;
            Rect orEditorPosition = process.editor.position;
            process.guiScale = scaleFactor;
            const int padding = 20;
            const int cropVal = 1; // Applied on both sides to avoid capturing window border
            if (useProgressBar) EditorUtility.DisplayProgressBar("Screenshot", "Capturing...", 0.3f);
            process.options.showMinimap = false;
            process.editor.position = new Rect(0, 0, 4096, 4096);
            process.editor.Repaint();
            yield return DeEditorCoroutines.WaitForSeconds(0.001f);
            Rect fullNodesArea = process.EvaluateFullNodesArea();
            fullNodesArea = fullNodesArea.Shift(-padding, -padding, padding * 2, padding * 2);
            fullNodesArea.x -= process.areaShift.x;
            fullNodesArea.y -= process.areaShift.y;
            fullNodesArea.width *= scaleFactor;
            fullNodesArea.height *= scaleFactor;
            Rect captureR = process.position.ResetXY();
            int cols = Mathf.CeilToInt(fullNodesArea.width / captureR.width);
            int rows = Mathf.CeilToInt(fullNodesArea.height / captureR.height);
            int blockW = (int)captureR.width;
            int blockH = (int)captureR.height;
            float cropValOffset = cropVal * 2 / scaleFactor;
            Texture2D tx = new Texture2D(
                (int)fullNodesArea.width - cropVal * 2 * cols,
                (int)fullNodesArea.height - cropVal * 2 * rows,
                TextureFormat.RGB24, false
            );
            int y = tx.height;
            for (int r = 0; r < rows; ++r) {
                bool shiftTxY = true;
                for (int c = 0; c < cols; ++c) {
                    int w = blockW;
                    int h = blockH;
                    if (c == cols - 1) w = blockW - (blockW * (c + 1) - (int)fullNodesArea.width);
                    if (r == rows - 1) h = blockH - (blockH * (r + 1) - (int)fullNodesArea.height);
                    if (shiftTxY) {
                        shiftTxY = false;
                        y -= h - cropVal * 2;
                    }
                    Vector2 areaShift = new Vector2(
                        -fullNodesArea.x - blockW / scaleFactor * c - cropValOffset,
                        -fullNodesArea.y - blockH / scaleFactor * r - cropValOffset
                    );
                    process.SetAreaShift(areaShift);
                    Texture2D subTx = new Texture2D(w, h, TextureFormat.RGB24, false);
                    yield return DeEditorCoroutines.WaitForSeconds(0.001f);
                    process.editor.Focus();
                    yield return DeEditorCoroutines.WaitForSeconds(0.001f);
                    //
//                    Debug.Log("c-r: " + c + "-" + r + " ► block: " + w + "x" + h + " ► full: " + tx.width + "x" + tx.height + " ► x-y: " + (blockW * c) + "-" + y);
//                    Debug.Log("   - process.position: " + process.position + " - areaShift: " + areaShift);
//                    continue;
                    subTx.ReadPixels(new Rect(0, blockH - h, w, h), 0, 0, false);
                    subTx.Apply();
                    Color[] pixels = subTx.GetPixels(cropVal, cropVal, w - cropVal * 2, h - cropVal * 2);
                    tx.SetPixels((blockW - cropVal * 2) * c, y, w - cropVal * 2, h - cropVal * 2, pixels);
                }
            }
            tx.Apply(false);
            if (hasMinimap) process.options.showMinimap = hasMinimap;
            process.guiScale = orGuiScale;
            process.editor.position = orEditorPosition;
            process.SetAreaShift(orShift);
            if (useProgressBar) EditorUtility.ClearProgressBar();
            onComplete(tx);
        }

        #endregion
    }
}