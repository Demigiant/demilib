// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/07/20

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DemiEditor
{
    /// <summary>
    /// Utils to manage UnityPackages import/export and file mirroring
    /// </summary>
    public static class DeEditorPackageManager
    {
        static readonly StringBuilder _Strb = new StringBuilder();

        #region Public Methods

        /// <summary>
        /// Stores all file paths (excluding metas) found in the given AssetDatabase directory and subdirectory
        /// into the given AssetDatabase file (which will be created if missing),
        /// writing them as relative to the given directory.<para/>
        /// EXAMPLE:<para/>
        /// <code>adbReadFromDirPath = "Plugins/DOTween"<para/>
        /// file "Assets/Plugins/DOTween/aScript.cs" stored as "aScript.cs"<para/>
        /// file "Assets/Plugins/DOTween/Subdir/aScript.cs" stored as "Subdir/aScript.cs"<para/>
        /// </code>
        /// </summary>
        /// <param name="adbWriteToFilePath">AssetDatabase path ("Assets/...") where the list should be written</param>
        /// <param name="adbReadFromDirPath">AssetDatabase path ("Assets/...") from which the list of files should be retrieved, without final slash</param>
        public static void WriteFileListTo(string adbWriteToFilePath, string adbReadFromDirPath)
        {
            if (adbReadFromDirPath.Length == 0) {
                Debug.LogWarning("WriteFileListTo ► parameter adbReadFromDirPath can't be empty");
                return;
            }
            if (adbReadFromDirPath.EndsWith("/") || adbReadFromDirPath.EndsWith("\\")) {
                // Trim final slash if present
                adbReadFromDirPath = adbWriteToFilePath.Substring(0, adbWriteToFilePath.Length - 1);
            }
            string fullReadFromDirPath = DeEditorFileUtils.ADBPathToFullPath(adbReadFromDirPath);
            if (!Directory.Exists(fullReadFromDirPath)) {
                Debug.LogError(string.Format("WriteFileListTo ► adbReadFromDirPath doesn't exist ({0})", adbReadFromDirPath));
                return;
            }
            string[] relFilePaths = Directory.GetFiles(fullReadFromDirPath, "*", SearchOption.AllDirectories)
                .Where(name => !name.EndsWith(".meta", true, CultureInfo.InvariantCulture)).ToArray();
            if (relFilePaths.Length == 0) {
                Debug.LogWarning("WriteFileListTo ► file list empty, canceling operation");
                return;
            }
            MakeFilePathsRelativeTo(relFilePaths, adbReadFromDirPath); // Make relative to original adbReadFromDirPath
            string fullWriteToFilePath = DeEditorFileUtils.ADBPathToFullPath(adbWriteToFilePath);
            using (StreamWriter writer = new StreamWriter(fullWriteToFilePath, false)) {
                for (int i = 0; i < relFilePaths.Length; ++i) {
                    writer.WriteLine(relFilePaths[i]);
                }
            }
            AssetDatabase.ImportAsset(DeEditorFileUtils.FullPathToADBPath(fullWriteToFilePath));
            Debug.Log(string.Format("WriteFileListTo ► {0} files written to \"{1}\"", relFilePaths.Length, fullWriteToFilePath));
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(adbWriteToFilePath));
        }

        /// <summary>
        /// Parses a file list created via <see cref="WriteFileListTo"/> and removes any files not present in the list from the given directory
        /// </summary>
        /// <param name="label">Label to use when logging the result</param>
        /// <param name="adbListFilePath">AssetDatabase path ("Assets/...") to the file containing the list</param>
        /// <param name="adbParseDirPath">AssetDatabase path ("Assets/...") to the directory to parse for extra files to remove</param>
        /// <param name="simulate">If TRUE only returns a report log and doesn't actually delete the files</param>
        public static void ParseListAndRemoveExtraFiles(string label, string adbListFilePath, string adbParseDirPath, bool simulate = false)
        {
            string fullParseDirPath = DeEditorFileUtils.ADBPathToFullPath(adbParseDirPath);
            if (!Directory.Exists(fullParseDirPath)) {
                Debug.LogError(string.Format("ParseListAndRemoveExtraFiles ► adbParseDirPath doesn't exist ({0})", adbParseDirPath));
                return;
            }
            string fullListFilePath = DeEditorFileUtils.ADBPathToFullPath(adbListFilePath);
            if (!File.Exists(fullListFilePath)) {
                Debug.LogWarning(string.Format("ParseListAndRemoveExtraFiles ► file \"{0}\" doesn't exist, canceling operation", adbListFilePath));
                return;
            }
            List<string> relListFilePaths = new List<string>();
            using (StreamReader reader = new StreamReader(fullListFilePath)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    relListFilePaths.Add(line);
                }
            }
            string[] fullFilePaths = Directory.GetFiles(fullParseDirPath, "*", SearchOption.AllDirectories)
                .Where(name => !name.EndsWith(".meta", true, CultureInfo.InvariantCulture)).ToArray();
            if (fullFilePaths.Length == 0) return;
            // Validate and eventually delete
            _Strb.Length = 0;
            AssetDatabase.StartAssetEditing();
            int totDeleted = 0;
            try {
                for (int i = 0; i < fullFilePaths.Length; ++i) {
                    string relFilePath = MakeFilePathRelativeTo(fullFilePaths[i], adbParseDirPath);
                    if (relListFilePaths.Contains(relFilePath)) continue;
                    totDeleted++;
                    string adbFilePath = DeEditorFileUtils.FullPathToADBPath(fullFilePaths[i]);
                    _Strb.Append("\n   - ").Append(adbFilePath);
                    if (!simulate) AssetDatabase.DeleteAsset(adbFilePath);
                }
            } catch (Exception e) {
                Debug.LogError(e);
            } finally {
                AssetDatabase.StopAssetEditing();
            }
            if (simulate) _Strb.Insert(0, string.Format("{0} ► SIMULATION ► Would've deleted {1} files", label, totDeleted));
            else _Strb.Insert(0, string.Format("{0} ► Deleted {1} files", label, totDeleted));
            string msg = _Strb.ToString();
            _Strb.Length = 0;
            Debug.Log(msg);
        }

        #endregion

        #region Methods

        static void MakeFilePathsRelativeTo(string[] fullFilePaths, string adbDir)
        {
            for (int i = 0; i < fullFilePaths.Length; ++i) {
                fullFilePaths[i] = MakeFilePathRelativeTo(fullFilePaths[i], adbDir);
            }
        }

        static string MakeFilePathRelativeTo(string fullFilePath, string adbDir)
        {
            return DeEditorFileUtils.FullPathToADBPath(fullFilePath).Substring(adbDir.Length + 1);
        }

        #endregion
    }
}