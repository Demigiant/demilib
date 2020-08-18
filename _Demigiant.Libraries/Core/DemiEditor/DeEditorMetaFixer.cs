// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/08/14

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Exception = System.Exception;
using File = System.IO.File;

namespace DG.DemiEditor
{
    /// <summary>
    /// Framework used to fix missing monoScript reference in GameObjects when a script's meta guid changes
    /// </summary>
    public static class DeEditorMetaFixer
    {
        static string _currSceneADBFilePath;
        static IEnumerator _opsCoroutine;

        #region Public Methods

        /// <summary>
        /// Retrieves the GUID in the given meta file and returns it, or NULL if it's not found
        /// </summary>
        /// <param name="metaFilePath">Full filePath to the meta file</param>
        public static string RetrieveMetaGuid(string metaFilePath)
        {
            if (!File.Exists(metaFilePath)) {
                Debug.LogWarning(string.Format("DeEditorMetaFixer.RetrieveMetaGuid ► meta file doesn't exist ({0})", metaFilePath));
                return null;
            }
            using (StreamReader reader = new StreamReader(metaFilePath)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (!line.StartsWith("guid:")) continue;
                    string guid = line.Substring(line.IndexOf(':') + 1);
                    return guid.Trim();
                }
            }
            Debug.LogWarning(string.Format("DeEditorMetaFixer.RetrieveMetaGuid ► GUID not found while reading \"{0}\"", metaFilePath));
            return null;
        }

        /// <summary>
        /// Fixes all wrong Component GUIDs in scenes and prefabs
        /// </summary>
        /// <param name="cDatas"><see cref="ComponentData"/> objects to use for the operation</param>
        public static void FixComponentsGuidsInAllScenesAndPrefabs(params ComponentData[] cDatas)
        {
            if (_opsCoroutine != null) {
                Debug.LogWarning("DeEditorMetaFixer.FixComponentsGuidsInAllScenesAndPrefabs ► Ignored because another operation is already running");
                return;
            }
            _opsCoroutine = DeEditorCoroutines.StartCoroutine(CO_FixComponentsGuidsInAllScenesAndPrefabs(cDatas));
        }
        static IEnumerator CO_FixComponentsGuidsInAllScenesAndPrefabs(ComponentData[] cDatas)
        {
            if (!BeginFixScenesOperation()) {
                ClearOps();
                yield break;
            }
            EditorUtility.DisplayProgressBar("Fix MissingScripts", "Fixing MissingScript errors in all scenes and prefabs", 0);
            yield return null;
            ModInfo modInfo = new ModInfo();
            int totGuidsFixed = 0;
            string[] allSceneAndPrefabFiles = Directory.GetFiles(DeEditorFileUtils.assetsPath, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".unity") || f.EndsWith(".prefab")).ToArray();
            int totFiles = allSceneAndPrefabFiles.Length;
            List<string> modifiedSceneAndPrefabFiles = new List<string>();
            int totModifiedScenes = 0;
            int totModifiedPrefabs = 0;
            AssetDatabase.StartAssetEditing();
            for (int i = 0; i < totFiles; i++) {
                float progress = (i + 1) / (float)totFiles;
                string sceneOrPrefabFile = allSceneAndPrefabFiles[i];
                EditorUtility.DisplayProgressBar("Fix MissingScripts", "Checking/fixing MissingScript errors in " + Path.GetFileName(sceneOrPrefabFile), progress);
                try {
                    string content = File.ReadAllText(sceneOrPrefabFile);
                    bool modified = false;
                    foreach (ComponentData cData in cDatas) {
                        int tot = FixComponentGuidInSceneOrPrefabString(cData, ref content, DeEditorFileUtils.FullPathToADBPath(sceneOrPrefabFile), modInfo);
                        totGuidsFixed += tot;
                        if (tot > 0) modified = true;
                    }
                    if (modified) {
                        EditorUtility.DisplayProgressBar("Fix MissingScripts", "Write file " + Path.GetFileName(sceneOrPrefabFile), progress);
                        File.WriteAllText(sceneOrPrefabFile, content);
                        modifiedSceneAndPrefabFiles.Add(sceneOrPrefabFile);
                        if (sceneOrPrefabFile.EndsWith(".unity")) totModifiedScenes++;
                        else totModifiedPrefabs++;
                    }
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
            // Complete
            AssetDatabase.StopAssetEditing();
            int totModifiedFiles = modifiedSceneAndPrefabFiles.Count;
            StringBuilder strb = new StringBuilder()
                .Append(totGuidsFixed).Append(" GUIDs fixed in ")
                .Append(totModifiedScenes).Append(" scenes and ").Append(totModifiedPrefabs).Append(" prefabs");
            if (totModifiedFiles > 0) {
                for (int i = 0; i < totModifiedFiles; i++) {
                    string adbModifiedFile = DeEditorFileUtils.FullPathToADBPath(modifiedSceneAndPrefabFiles[i]);
                    AssetDatabase.ImportAsset(adbModifiedFile, ImportAssetOptions.ForceUpdate);
                    modInfo.AddDetailedModInfoTo(strb, adbModifiedFile);
                }
            }
            // Log result
            EditorUtility.ClearProgressBar();
            Debug.Log(strb.ToString());
            yield return null;
            strb.Length = 0;
            strb.Append(totGuidsFixed).Append(" GUIDs fixed in ")
                .Append(totModifiedScenes).Append(" scenes and ").Append(totModifiedPrefabs).Append(" prefabs.");
            foreach (ComponentData cData in cDatas) {
                strb.Append("\n- ").Append(cData.id).Append(": ").Append(cData.totGuidsFixed).Append(" GUIDs fixed");
            }
            EditorUtility.DisplayDialog("Fix MissingScripts", strb.ToString(), "Ok");
            EndFixScenesOperation();
        }

        /// <summary>
        /// Fixes all wrong Component GUIDs in the active scene and returns the total number of Components fixed
        /// </summary>
        /// <param name="cDatas"><see cref="ComponentData"/> objects to use for the operation</param>
        public static void FixComponentsGuidsInActiveScene(params ComponentData[] cDatas)
        {
            if (_opsCoroutine != null) {
                Debug.LogWarning("DeEditorMetaFixer.FixComponentsGuidsInActiveScene ► Ignored because another operation is already running");
                return;
            }
            _opsCoroutine = DeEditorCoroutines.StartCoroutine(CO_FixComponentsGuidsInActiveScene(cDatas));
        }
        static IEnumerator CO_FixComponentsGuidsInActiveScene(ComponentData[] cDatas)
        {
            if (!BeginFixScenesOperation()) yield break;
            // Open new scene
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            //
            EditorUtility.DisplayProgressBar("Fix MissingScripts", "Fixing MissingScript errors in active scene", 0);
            yield return null;
            // Parse and replace
            ModInfo modInfo = new ModInfo();
            string currSceneFullPath = DeEditorFileUtils.ADBPathToFullPath(_currSceneADBFilePath);
            int totCDatas = cDatas.Length;
            int totGuidsFixed = 0;
            string sceneFileString = File.ReadAllText(currSceneFullPath);
            for (int i = 0; i < totCDatas; i++) {
                ComponentData rcData = cDatas[i];
                EditorUtility.DisplayProgressBar("Fix MissingScripts", "Checking/fixing MissingScript errors for " + rcData.id, i / (float)totCDatas);
                totGuidsFixed += FixComponentGuidInSceneOrPrefabString(rcData, ref sceneFileString, _currSceneADBFilePath, modInfo);
                yield return null;
            }
            // Save if necessary
            if (totGuidsFixed > 0) {
                EditorUtility.DisplayProgressBar("Fix MissingScripts", "Completed: saving scene", 1);
                AssetDatabase.StartAssetEditing();
                try {
                    File.WriteAllText(currSceneFullPath, sceneFileString);
                } catch (Exception e) {
                    Debug.LogException(e);
                }
                AssetDatabase.StopAssetEditing();
                AssetDatabase.ImportAsset(_currSceneADBFilePath, ImportAssetOptions.ForceUpdate);
                yield return null;
            }
            // Log result
            EditorUtility.ClearProgressBar();
            StringBuilder strb = new StringBuilder();
            strb.Append(totGuidsFixed + " GUIDs fixed in active scene");
            modInfo.AddDetailedModInfoTo(strb, _currSceneADBFilePath);
            Debug.Log(strb.ToString());
            //
            strb.Length = 0;
            foreach (ComponentData cData in cDatas) {
                strb.Append("\n- ").Append(cData.id).Append(": ").Append(cData.totGuidsFixed).Append(" GUIDs fixed");
            }
            EditorUtility.DisplayDialog("Fix MissingScripts", totGuidsFixed + " GUIDs fixed in active scene.\n" + strb.ToString(), "Ok");
            EndFixScenesOperation();
        }

        #endregion

        #region Methods

        // Returns FALSE if the operation should be interrupted
        static bool BeginFixScenesOperation()
        {
            _currSceneADBFilePath = SceneManager.GetActiveScene().path;
            // Save current scene
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                ClearOps();
            }
            return true;
        }

        static void EndFixScenesOperation()
        {
            ClearOps();
            EditorSceneManager.OpenScene(_currSceneADBFilePath, OpenSceneMode.Single);
        }

        static void ClearOps()
        {
            EditorUtility.ClearProgressBar();
            _opsCoroutine = null;
        }

        /// <summary>
        /// Finds all MonoBehaviour/Behaviour/Component in the given scene/prefab file string
        /// that contain the given <see cref="ComponentData.serializedIdentifiers"/>
        /// and replaces their GUID with the one passed (if different).<para/>
        /// Returns the total number of Component GUIDs that were fixed
        /// </summary>
        static int FixComponentGuidInSceneOrPrefabString(ComponentData cData, ref string sceneOrPrefabFileString, string sceneOrPrefabADBFile, ModInfo modInfo)
        {
            Queue<int> linesToRewrite = null;
            // Read scene file and detect component
            int totProperties = cData.serializedIdentifiers.Length;
            string currGuid = null;
            int totComponentsWDiffGuidFound = 0;
            using (StringReader stringReader = new StringReader(sceneOrPrefabFileString)) {
                string line;
                int lineIndex = -1;
                bool seekingForGameObjectName = false;
                bool seekingWithinComponent = false;
                string currGameObjectName = null;
                string currComponentGuidLine = null;
                int currComponentGuidLineIndex = -1;
                int totPropertiesFound = 0;
                while ((line = stringReader.ReadLine()) != null) {
                    lineIndex++;
                    switch (line) {
                    case "GameObject:":
                        seekingForGameObjectName = true;
                        break;
                    case "MonoBehaviour:":
                    case "Behaviour:":
                    case "Component:":
                        seekingWithinComponent = true;
                        break;
                    default:
                        if (line.StartsWith("-")) {
                            seekingForGameObjectName = seekingWithinComponent = false;
                            currComponentGuidLine = null;
                            currComponentGuidLineIndex = -1;
                            totPropertiesFound = 0;
                        }
                        break;
                    }
                    if (seekingForGameObjectName) {
                        string trimmedLine = line.TrimStart(' ');
                        if (trimmedLine.StartsWith("m_Name: ")) {
                            currGameObjectName = trimmedLine.Substring(8);
                            seekingForGameObjectName = false;
                        }
                    } else if (seekingWithinComponent) {
                        string trimmedLine = line.TrimStart(' ');
                        if (trimmedLine.StartsWith("m_Script: ")) {
                            currComponentGuidLine = line;
                            currComponentGuidLineIndex = lineIndex;
                            continue;
                        }
                        int colonIndex = trimmedLine.IndexOf(':');
                        if (colonIndex == -1) continue;
                        string propName = trimmedLine.Substring(0, colonIndex);
                        if (Array.IndexOf(cData.serializedIdentifiers, propName) != -1) totPropertiesFound++;
                        if (totPropertiesFound == totProperties) {
                            // Component found, store data so it can be rewritten when the loop ends
                            int guidStartIndex = currComponentGuidLine.IndexOf("guid: ") + 6;
                            int guidEndIndex = currComponentGuidLine.IndexOf(',', guidStartIndex) - 1;
                            string componentGuid = currComponentGuidLine.Substring(guidStartIndex, guidEndIndex - guidStartIndex + 1);
                            if (componentGuid != cData.correctGuid && string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(componentGuid))) {
                                // GUID returns no asset path and is different from correctGuid
                                modInfo.AddModifiedGameObjectName(sceneOrPrefabADBFile, currGameObjectName);
                                currGuid = componentGuid;
                                if (linesToRewrite == null) linesToRewrite = new Queue<int>();
                                linesToRewrite.Enqueue(currComponentGuidLineIndex);
                                totComponentsWDiffGuidFound++;
                            }
                            seekingWithinComponent = false;
                            currComponentGuidLine = null;
                            currComponentGuidLineIndex = -1;
                            totPropertiesFound = 0;
                        }
                    }
                }
            }
            cData.totGuidsFixed += totComponentsWDiffGuidFound;
            if (currGuid == null) return 0; // Component not found in scene or already correct
            // Replace correct GUID in Component GUID lines
            using (StringReader stringReader = new StringReader(sceneOrPrefabFileString)) {
                using (StringWriter writer = new StringWriter()) {
                    string line;
                    int lineIndex = -1;
                    int nextLineToRewrite = linesToRewrite.Dequeue();
                    while ((line = stringReader.ReadLine()) != null) {
                        lineIndex++;
                        if (lineIndex == nextLineToRewrite) {
                            writer.WriteLine(line.Replace(currGuid, cData.correctGuid));
                            if (linesToRewrite.Count > 0) nextLineToRewrite = linesToRewrite.Dequeue();
                        } else writer.WriteLine(line);
                    }
                    sceneOrPrefabFileString = writer.ToString();
                }
            }
            return totComponentsWDiffGuidFound;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        public class ComponentData
        {
            public readonly string id;
            public readonly string correctGuid;
            public readonly string[] serializedIdentifiers;
            public int totGuidsFixed = 0;

            public ComponentData(string id, string correctGuid, params string[] serializedIdentifiers)
            {
                this.id = id;
                this.correctGuid = correctGuid;
                this.serializedIdentifiers = serializedIdentifiers;
            }
        }

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class ModInfo
        {
            readonly Dictionary<string, List<string>> _sceneOrPrefabADBFileToGosNames = new Dictionary<string, List<string>>();

            // Adds the gameObject name to the dictionary only if it doesn't already exist in the key list
            public void AddModifiedGameObjectName(string sceneOrPrefabADBFile, string goName)
            {
                if (!_sceneOrPrefabADBFileToGosNames.ContainsKey(sceneOrPrefabADBFile)) {
                    _sceneOrPrefabADBFileToGosNames.Add(sceneOrPrefabADBFile, new List<string>());
                }
                if (!_sceneOrPrefabADBFileToGosNames[sceneOrPrefabADBFile].Contains(goName)) {
                    _sceneOrPrefabADBFileToGosNames[sceneOrPrefabADBFile].Add(goName);
                }
            }

            public List<string> GetModifiedGameObjectNamesFor(string sceneOrPrefabADBFile)
            {
                if (!_sceneOrPrefabADBFileToGosNames.ContainsKey(sceneOrPrefabADBFile)) return null;
                return _sceneOrPrefabADBFileToGosNames[sceneOrPrefabADBFile];
            }

            public void AddDetailedModInfoTo(StringBuilder addToStrb, string adbModifiedFile)
            {
                List<string> modGosForFile = GetModifiedGameObjectNamesFor(adbModifiedFile);
                if (modGosForFile == null) return;
                addToStrb.Append("\n- ").Append(adbModifiedFile.Substring(8));
                addToStrb.Append(" ► GOs modified: ");
                for (int j = 0; j < modGosForFile.Count; j++) {
                    if (j > 0) addToStrb.Append(", ");
                    string goName = modGosForFile[j];
                    addToStrb.Append('\"').Append(goName).Append('\"');
                }
            }
        }
    }
}