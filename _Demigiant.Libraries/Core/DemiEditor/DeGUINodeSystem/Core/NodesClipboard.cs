// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/23 19:32
// License Copyright (c) Daniele Giardini

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core
{
    /// <summary>
    /// Stores cloned nodes for pasting
    /// </summary>
    internal class NodesClipboard
    {
        public readonly List<IEditorGUINode> currClones = new List<IEditorGUINode>();
        public bool hasContent { get { return EditorGUIUtility.systemCopyBuffer == _copyBufferId && _originalNodes.Count > 0; } }

        NodeProcess _process;
        readonly string _copyBufferId; // Store in OS clipboard to indicate that there's nodes ready to be pasted for this process
        readonly List<IEditorGUINode> _originalNodes = new List<IEditorGUINode>();
        readonly Dictionary<IEditorGUINode,NodeConnectionOptions> _originalNodeToConnectionOptions = new Dictionary<IEditorGUINode,NodeConnectionOptions>();
        readonly Dictionary<IEditorGUINode,NodeGUIData> _originalNodeToGuiData = new Dictionary<IEditorGUINode,NodeGUIData>();
        readonly Dictionary<string,string> _originalIdToCloneId = new Dictionary<string,string>();
        readonly Dictionary<IEditorGUINode,IEditorGUINode> _cloneToOriginalNode = new Dictionary<IEditorGUINode,IEditorGUINode>();
        Func<IEditorGUINode,IEditorGUINode,bool> _onCloneNodeCallback; // Returns FALSE if cloning shouldn't happen
        bool _requiresRecloningOnNextPaste = false;


        #region CONSTRUCTOR

        public NodesClipboard(NodeProcess process)
        {
            _process = process;
            _copyBufferId = "[[[[!?NODES?!]]]]" + Guid.NewGuid().ToString();
        }

        #endregion

        #region Public Methods

        public void Clear()
        {
            currClones.Clear();
            _originalNodes.Clear();
            _originalIdToCloneId.Clear();
            _originalNodeToConnectionOptions.Clear();
            _originalNodeToGuiData.Clear();
            _cloneToOriginalNode.Clear();
            _requiresRecloningOnNextPaste = false;
        }

        // When added has a clone ready (used to verify onCloneNodeCallback).
        // When multiple copies are pasted, new clones are created
        public void Add(IEditorGUINode node, IEditorGUINode clone, NodeConnectionOptions connectionOptions, Func<IEditorGUINode,IEditorGUINode,bool> onCloneNodeCallback)
        {
            _originalNodes.Add(node);
            _originalIdToCloneId.Add(node.id, clone.id);
            _originalNodeToConnectionOptions.Add(node, connectionOptions);
            _originalNodeToGuiData.Add(node, _process.nodeToGUIData[node]);
            _cloneToOriginalNode.Add(clone, node);
            _onCloneNodeCallback = onCloneNodeCallback;
            currClones.Add(clone);
            EditorGUIUtility.systemCopyBuffer = _copyBufferId;
        }

        /// <summary>
        /// Returns a list of pasteable nodes, with their GUID recreated and their connections adapted
        /// </summary>
        /// <returns></returns>
        public List<T> GetNodesToPaste<T>() where T : IEditorGUINode, new()
        {
            if (_originalNodes.Count == 0) return null;
            List<T> result = new List<T>();
            if (_requiresRecloningOnNextPaste) {
                // Create new clones
                currClones.Clear();
                _originalIdToCloneId.Clear();
                _cloneToOriginalNode.Clear();
                foreach (IEditorGUINode original in _originalNodes) {
                    T clone = CloneNode<T>(original);
                    if (_onCloneNodeCallback != null && !_onCloneNodeCallback(original, clone)) continue;
                    _cloneToOriginalNode.Add(clone, original);
                    _originalIdToCloneId[original.id] = clone.id;
                    currClones.Add(clone);
                    result.Add(clone);
                }
            } else {
                foreach (IEditorGUINode clone in currClones) result.Add((T)clone);
            }
            // Replace all interconnected ids with new ones, and eliminate others
            foreach (IEditorGUINode original in _originalNodes) {
                NodeConnectionOptions connectionOptions = _originalNodeToConnectionOptions[original];
                IEditorGUINode clone = GetCloneByOriginalId(original.id);
                for (int c = clone.connectedNodesIds.Count - 1; c > -1; --c) {
                    string connectionId = clone.connectedNodesIds[c];
                    if (IsConnectionToCopiedNode(connectionId)) {
                        // Replace connection ID with clone's ID
                        clone.connectedNodesIds[c] = _originalIdToCloneId[connectionId];
                    } else {
                        // Clear or delete connection
                        switch (connectionOptions.connectionMode) {
                        case ConnectionMode.Flexible:
                            clone.connectedNodesIds.RemoveAt(c);
                            break;
                        default:
                            clone.connectedNodesIds[c] = null;
                            break;
                        }
                    }
                }
            }
            _requiresRecloningOnNextPaste = true;
            return result;
        }

        /// <summary>
        /// Returns a deep clone of the given node but doesn't clone UnityEngine references.
        /// A new ID will be automatically generated.
        /// </summary>
        public T CloneNode<T>(IEditorGUINode node) where T : IEditorGUINode, new()
        {
            T clone = (T)CloneObject(node);
            clone.id = Guid.NewGuid().ToString();
            return clone;

//            T clone = new T();
//            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
//            FieldInfo[] srcFields = node.GetType().GetFields(flags);
//            FieldInfo[] destFields = clone.GetType().GetFields(flags);
//            foreach (FieldInfo srcField in srcFields) {
//                FieldInfo destField = destFields.FirstOrDefault(field => field.Name == srcField.Name);
//                if (destField == null || destField.IsLiteral || srcField.FieldType != destField.FieldType) continue;
//                object srcValue = srcField.GetValue(node);
//                if (srcValue != null) {
//                    Type valueType = srcValue.GetType();
//                    if (valueType.IsArray) {
//                        // Clone Array
//                        string typeStr = valueType.FullName.Replace("[]", string.Empty);
//                        Type arrayType = Type.GetType(typeStr);
//                        if (arrayType == null) {
//                            // Try finding type within Unity's project qualified assemblies
//                            arrayType = Type.GetType(typeStr + ", Assembly-CSharp-firstpass");
//                            if (arrayType == null) arrayType = Type.GetType(typeStr + ", Assembly-CSharp");
//                            // If type is not found yet, let it become an error.
//                            // The alternative would be to go through all assemblies with a
//                            // foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
//                            // but it's expensive and let's ignore it for now
//                        }
//                        Array srcArray = srcValue as Array;
//                        Array clonedArray = Array.CreateInstance(arrayType, srcArray.Length);
//                        for (int i = 0; i < srcArray.Length; ++i) clonedArray.SetValue(srcArray.GetValue(i), i);
//                        destField.SetValue(clone, clonedArray);
//                    } else if (valueType.IsGenericType) {
//                        // Clone List
//                        Type listType = Type.GetType(valueType.FullName.Replace("[]", string.Empty));
//                        if (listType == null) {
//                            Debug.LogWarning(string.Format("Couldn't clone correctly the {0} field, a shallow copy will be used", srcField.Name));
//                            destField.SetValue(clone, srcField.GetValue(node));
//                        } else {
//                            IList srcList = srcValue as IList;
//                            IList clonedList = Activator.CreateInstance(listType) as IList;
//                            for (int i = 0; i < srcList.Count; ++i) clonedList.Add(srcList[i]);
//                            destField.SetValue(clone, clonedList);
//                        }
//                    } else {
//                        destField.SetValue(clone, srcField.GetValue(node));
//                    }
//                }
//            }
//            clone.id = Guid.NewGuid().ToString();
//            return clone;
        }

        object CloneObject(object original)
        {
            if (original == null) return null;
            Type t = original.GetType();
            if (t.IsValueType || t == typeof(string) || t.Namespace != null && t.Namespace.StartsWith("UnityEngine")) return original; // struct

            object clone = Activator.CreateInstance(t);
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] srcFields = t.GetFields(flags);
            FieldInfo[] destFields = clone.GetType().GetFields(flags);
            foreach (FieldInfo srcField in srcFields) {
                FieldInfo destField = destFields.FirstOrDefault(field => field.Name == srcField.Name);
                if (destField == null || destField.IsLiteral || srcField.FieldType != destField.FieldType) continue;
                object srcValue = srcField.GetValue(original);
                if (srcValue != null) {
                    Type valueType = srcValue.GetType();
                    if (valueType.IsArray) {
                        // Clone Array
                        string typeStr = valueType.FullName.Replace("[]", string.Empty);
                        Type arrayType = Type.GetType(typeStr);
                        if (arrayType == null) {
                            // Try finding type within Unity's project qualified assemblies
                            arrayType = Type.GetType(typeStr + ", Assembly-CSharp-firstpass");
                            if (arrayType == null) arrayType = Type.GetType(typeStr + ", Assembly-CSharp");
                            // If type is not found yet, let it become an error.
                            // The alternative would be to go through all assemblies with a
                            // foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                            // but it's expensive and let's ignore it for now
                        }
                        Array srcArray = srcValue as Array;
                        Array clonedArray = Array.CreateInstance(arrayType, srcArray.Length);
                        for (int i = 0; i < srcArray.Length; ++i) {
                            clonedArray.SetValue(CloneObject(srcArray.GetValue(i)), i);
                        }
                        destField.SetValue(clone, clonedArray);
                    } else if (valueType.IsGenericType) {
                        // Clone List
                        Type listType = Type.GetType(valueType.FullName.Replace("[]", string.Empty));
                        if (listType == null) {
                            Debug.LogWarning(string.Format("Couldn't clone correctly the {0} field, a shallow copy will be used", srcField.Name));
                            destField.SetValue(clone, CloneObject(srcField.GetValue(original)));
                        } else {
                            IList srcList = srcValue as IList;
                            IList clonedList = Activator.CreateInstance(listType) as IList;
                            for (int i = 0; i < srcList.Count; ++i) clonedList.Add(CloneObject(srcList[i]));
                            destField.SetValue(clone, clonedList);
                        }
                    } else {
                        destField.SetValue(clone, CloneObject(srcField.GetValue(original)));
                    }
                }
            }
            return clone;
        }

        public IEditorGUINode GetCloneByOriginalId(string id)
        {
            string cloneId = _originalIdToCloneId[id];
            foreach (IEditorGUINode node in currClones) {
                if (node.id == cloneId) return node;
            }
            return null;
        }

        public NodeGUIData GetGuiDataByCloneId(string cloneId)
        {
            foreach (IEditorGUINode clone in currClones) {
                if (clone.id == cloneId) return _originalNodeToGuiData[_cloneToOriginalNode[clone]];
            }
            return new NodeGUIData();
        }

        public NodeConnectionOptions GetConnectionOptionsByCloneId(string cloneId)
        {
            foreach (IEditorGUINode clone in currClones) {
                if (clone.id == cloneId) return _originalNodeToConnectionOptions[_cloneToOriginalNode[clone]];
            }
            return new NodeConnectionOptions();
        }

        #endregion

        #region Helpers

        bool IsConnectionToCopiedNode(string connectionId)
        {
            foreach (IEditorGUINode node in _originalNodes) {
                if (node.id == connectionId) return true;
            }
            return false;
        }

        #endregion
    }
}