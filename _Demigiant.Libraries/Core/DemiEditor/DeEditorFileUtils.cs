// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/05/01 01:50

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// File utils
    /// </summary>
    public static class DeEditorFileUtils
    {
        /// <summary>Path slash for AssetDatabase format</summary>
        public static readonly string ADBPathSlash = "/";
        /// <summary>Path slash to replace for AssetDatabase format</summary>
        public static readonly string ADBPathSlashToReplace = "\\";
        /// <summary>Current OS path slash</summary>
        public static readonly string PathSlash;
        /// <summary>Path slash to replace on current OS</summary>
        public static readonly string PathSlashToReplace;

        /// <summary>
        /// Full path to project directory, without final slash.
        /// </summary>
        public static string projectPath {
            get {
                if (_fooProjectPath == null) {
                    _fooProjectPath = Application.dataPath;
                    _fooProjectPath = _fooProjectPath.Substring(0, _fooProjectPath.LastIndexOf(ADBPathSlash));
                    _fooProjectPath = _fooProjectPath.Replace(ADBPathSlash, PathSlash);
                }
                return _fooProjectPath;
            }
        }
        /// <summary>
        /// Full path to project's Assets directory, without final slash.
        /// </summary>
        public static string assetsPath { get { return projectPath + PathSlash + "Assets"; } }

        static string _fooProjectPath;

        #region Constructor

        static DeEditorFileUtils()
        {
            bool useWindowsSlashes = Application.platform == RuntimePlatform.WindowsEditor;
            PathSlash = useWindowsSlashes ? "\\" : "/";
            PathSlashToReplace = useWindowsSlashes ? "/" : "\\";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the given project-relative path to a full path
        /// </summary>
        public static string ADBPathToFullPath(string adbPath)
        {
            adbPath = adbPath.Replace(ADBPathSlash, PathSlash);
            return projectPath + PathSlash + adbPath;
        }

        /// <summary>
        /// Converts the given full path to a project-relative path
        /// </summary>
        public static string FullPathToADBPath(string fullPath)
        {
            return fullPath.Substring(projectPath.Length + 1).Replace(ADBPathSlashToReplace, ADBPathSlash);
        }

        /// <summary>
        /// Returns TRUE if the file/directory at the given path exists.
        /// </summary>
        /// <param name="adbPath">Path, relative to Unity's project folder</param>
        public static bool AssetExists(string adbPath)
        {
            string fullPath = ADBPathToFullPath(adbPath);
            return File.Exists(fullPath) || Directory.Exists(fullPath);
        }

        /// <summary>
        /// Checks if the given directory (full path) is empty or not
        /// </summary>
        public static bool IsEmpty(string dir)
        {
            DirectoryInfo dInfo = new DirectoryInfo(dir);
            if (dInfo.GetFiles().Length > 0) return false;
            if (dInfo.GetDirectories().Length > 0) return false;
            return true;
        }

        /// <summary>
        /// Deletes all files and subdirectories from the given directory
        /// </summary>
        public static void MakeEmpty(string dir)
        {
            DirectoryInfo dInfo = new DirectoryInfo(dir);
            foreach (FileInfo f in dInfo.GetFiles()) {
                if (f.Extension != "meta") AssetDatabase.DeleteAsset(FullPathToADBPath(f.ToString()));
            }
            foreach (DirectoryInfo d in dInfo.GetDirectories()) {
                AssetDatabase.DeleteAsset(FullPathToADBPath(d.ToString()));
            }
        }

        /// <summary>
        /// Returns the asset path of the given GUID (relative to Unity project's folder),
        /// or an empty string if either the GUID is invalid or the related path doesn't exist.
        /// </summary>
        public static string GUIDToExistingAssetPath(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return "";
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath)) return "";
            if (AssetExists(assetPath)) return assetPath;
            return "";
        }

        /// <summary>Returns the adb path to the given ScriptableObject</summary>
        public static string MonoInstanceADBPath(ScriptableObject scriptableObj)
        {
            MonoScript ms = MonoScript.FromScriptableObject(scriptableObj);
            return AssetDatabase.GetAssetPath(ms);
        }
        /// <summary>Returns the adb path to the given MonoBehaviour</summary>
        public static string MonoInstanceADBPath(MonoBehaviour monobehaviour)
        {
            MonoScript ms = MonoScript.FromMonoBehaviour(monobehaviour);
            return AssetDatabase.GetAssetPath(ms);
        }

        /// <summary>Returns the adb directory that contains the given ScriptableObject without final slash</summary>
        public static string MonoInstanceADBDir(ScriptableObject scriptableObj)
        {
            MonoScript ms = MonoScript.FromScriptableObject(scriptableObj);
            string res = AssetDatabase.GetAssetPath(ms);
            return res.Substring(0, res.LastIndexOf(ADBPathSlash));
        }
        /// <summary>Returns the adb directory that contains the given MonoBehaviour without final slash</summary>
        public static string MonoInstanceADBDir(MonoBehaviour monobehaviour)
        {
            MonoScript ms = MonoScript.FromMonoBehaviour(monobehaviour);
            string res = AssetDatabase.GetAssetPath(ms);
            return res.Substring(0, res.LastIndexOf(ADBPathSlash));
        }

        /// <summary>
        /// Returns the adb paths to the selected folders in the Project panel, or NULL if there is none.
        /// Contrary to Selection.activeObject, which only returns folders selected in the right side of the panel,
        /// this method also works with folders selected in the left side.
        /// </summary>
        public static List<string> SelectedADBDirs()
        {
            List<string> selectedPaths = null;
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
                string adbPath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(adbPath) && Directory.Exists(ADBPathToFullPath(adbPath))) {
                    if (selectedPaths == null) selectedPaths = new List<string>();
                    selectedPaths.Add(adbPath);
                }
            }
            return selectedPaths;
        }

        #region ScriptUtils

        /// <summary>
        /// Sets the script execution order of the given MonoBehaviour
        /// </summary>
        public static void SetScriptExecutionOrder(MonoBehaviour monobehaviour, int order)
        {
            MonoScript ms = MonoScript.FromMonoBehaviour(monobehaviour);
            MonoImporter.SetExecutionOrder(ms, order);
        }

        /// <summary>
        /// Gets the script execution order of the given MonoBehaviour
        /// </summary>
        public static int GetScriptExecutionOrder(MonoBehaviour monobehaviour)
        {
            MonoScript ms = MonoScript.FromMonoBehaviour(monobehaviour);
            return MonoImporter.GetExecutionOrder(ms);
        }

        #endregion

        #endregion
    }
}