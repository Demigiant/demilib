using System;
using System.IO;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DemiLibTester.Editor
{
    public class CreateMBsMenu : MonoBehaviour
    {
        const string _MBFile =
            "using System;" +
            "\nusing UnityEngine;" +
            "\npublic class XXXXXX : MonoBehaviour" +
            "\n{" +
            "\n}";

        // [MenuItem("Assets/Create/Test/Mass Create MonoBehaviours", false, 3)]
        // static void MassCreateMonoBehaviours(MenuCommand command)
        // {
        //     const string title = "Mass Create MonoBehaviours";
        //
        //     if (Selection.assetGUIDs.Length != 1 || !DeEditorFileUtils.IsProjectFolder(Selection.assetGUIDs[0])) {
        //         EditorUtility.DisplayDialog(title, "Select a single folder please", "Ok");
        //         return;
        //     }
        //
        //     int result = EditorUtility.DisplayDialogComplex(
        //         title,
        //         "How many do you want to create?",
        //         "1000", "3000", "Cancel"
        //     );
        //     int totToCreate = result == 0
        //         ? 1000
        //         : result == 1
        //             ? 3000
        //             : 0;
        //     if (totToCreate == 0) return;
        //
        //     string guid = Selection.assetGUIDs[0];
        //
        //     string adbDir = DeEditorFileUtils.GUIDToExistingAssetPath(guid);
        //     string fullDirWFinalSlash = DeEditorFileUtils.ADBPathToFullPath(adbDir) + DeEditorFileUtils.PathSlash;
        //     // Create files
        //     try {
        //         AssetDatabase.StartAssetEditing();
        //         int suffix = 1;
        //         for (int i = 0; i < totToCreate; ++i) {
        //             string className = "TestMB_" + suffix;
        //             string fileName = className + ".cs";
        //             string filePath = fullDirWFinalSlash + fileName;
        //             while (File.Exists(filePath)) {
        //                 suffix++;
        //                 className = "TestMB_" + suffix;
        //                 fileName = className + ".cs";
        //                 filePath = fullDirWFinalSlash + fileName;
        //             }
        //             string content = _MBFile.Replace("XXXXXX", className);
        //             File.WriteAllText(filePath, content);
        //         }
        //     } catch (Exception e) {
        //         Console.WriteLine(e);
        //         throw;
        //     } finally {
        //         AssetDatabase.StopAssetEditing();
        //         AssetDatabase.ImportAsset(adbDir);
        //         AssetDatabase.Refresh();
        //     }
        // }
    }
}