// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/01/03 12:22
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Handles = UnityEditor.Handles;
using Object = UnityEngine.Object;

namespace DG.DemiEditor
{
    /// <summary>
    /// Utils to use the correct method based on Unity's version
    /// </summary>
    public static class DeEditorCompatibilityUtils
    {
        static MethodInfo _miEncodeToPng;
        static MethodInfo _miFindObjectOfTypeGeneric;
        static MethodInfo _miFindObjectOfType;
        static MethodInfo _miFindObjectsOfTypeGeneric;
        static MethodInfo _miFindObjectsOfType;
        static MethodInfo _miFreeMoveHandle;
        static MethodInfo _miGetPrefabParent;
        static bool _findObjectOfType_hasIncludeInactiveParam;
        static bool _findObjectOfTypeGeneric_hasIncludeInactiveParam;
        static bool _findObjectsOfType_hasIncludeInactiveParam;
        static bool _findObjectsOfTypeGeneric_hasIncludeInactiveParam;
        static Type _findObjectsInactiveType;
        static Type _findObjectsSortModeType;

        #region Public Methods

        /// <summary>
        /// Encodes to PNG using reflection to use correct method depending if editor is version 2017 or earlier
        /// </summary>
        public static byte[] EncodeToPNG(Texture2D texture)
        {
            if (_miEncodeToPng == null) {
                _miEncodeToPng = typeof(Texture2D).GetMethod("EncodeToPNG", BindingFlags.Instance | BindingFlags.Public);
                if (_miEncodeToPng == null) {
                    _miEncodeToPng = Type.GetType("UnityEngine.ImageConversion, UnityEngine")
                        .GetMethod("EncodeToPNG", BindingFlags.Static | BindingFlags.Public);
                }
            }
            if (DeUnityEditorVersion.MajorVersion >= 2017) {
                object[] parms = new[] { texture };
                return _miEncodeToPng.Invoke(null, parms) as byte[];
            } else {
                return _miEncodeToPng.Invoke(texture, null) as byte[];
            }
        }

        /// <summary>
        /// Warning: some versions of this method don't have the includeInactive parameter so it won't be taken into account
        /// </summary>
        public static T FindObjectOfType<T>(bool includeInactive = false)
        {
            if (_miFindObjectOfTypeGeneric == null) {
                if (DeUnityEditorVersion.MajorVersion < 2023) {
                    _miFindObjectOfTypeGeneric = typeof(UnityEngine.Object).GetMethod(
                        "FindObjectOfType", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                        new[] {typeof(bool)},
                        null
                    );
                    _findObjectOfTypeGeneric_hasIncludeInactiveParam = true;
                    if (_miFindObjectOfTypeGeneric == null) {
                        MethodInfo[] ms = typeof(UnityEngine.Object).GetMethods(BindingFlags.Static | BindingFlags.Public);
                        foreach (MethodInfo m in ms) {
                            if (m.Name != "FindObjectOfType" || !m.IsGenericMethod) continue;
                            _miFindObjectOfTypeGeneric = m;
                            break;
                        }
                        _findObjectOfTypeGeneric_hasIncludeInactiveParam = false;
                    }
                    _miFindObjectOfTypeGeneric = _miFindObjectOfTypeGeneric.MakeGenericMethod(typeof(T));
                } else {
                    if (_findObjectsInactiveType == null) _findObjectsInactiveType = typeof(GameObject).Assembly.GetType("UnityEngine.FindObjectsInactive");
                    _miFindObjectOfTypeGeneric = typeof(UnityEngine.Object).GetMethod(
                        "FindAnyObjectByType", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                        new[] {_findObjectsInactiveType},
                        null
                    ).MakeGenericMethod(typeof(T));
                }
            }
            if (DeUnityEditorVersion.MajorVersion < 2023) {
                if (_findObjectOfTypeGeneric_hasIncludeInactiveParam) return (T)_miFindObjectOfTypeGeneric.Invoke(null, new object[] {includeInactive});
                else return (T)_miFindObjectOfTypeGeneric.Invoke(null, null);
            } else {
                return (T)_miFindObjectOfTypeGeneric.Invoke(null, new object[] {includeInactive ? 0 : 1});
            }
        }
        /// <summary>
        /// Warning: some versions of this method don't have the includeInactive parameter so it won't be taken into account
        /// </summary>
        public static Object FindObjectOfType(Type type, bool includeInactive = false)
        {
            if (_miFindObjectOfType == null) {
                if (DeUnityEditorVersion.MajorVersion < 2023) {
                    _miFindObjectOfType = typeof(UnityEngine.Object).GetMethod(
                        "FindObjectOfType", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                        new[] {typeof(Type), typeof(bool)},
                        null
                    );
                    _findObjectOfType_hasIncludeInactiveParam = true;
                    if (_miFindObjectOfType == null) {
                        _miFindObjectOfType = typeof(UnityEngine.Object).GetMethod(
                            "FindObjectOfType", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                            new[] {typeof(Type)},
                            null
                        );
                        _findObjectOfType_hasIncludeInactiveParam = false;
                    }
                } else {
                    if (_findObjectsInactiveType == null) _findObjectsInactiveType = typeof(GameObject).Assembly.GetType("UnityEngine.FindObjectsInactive");
                    _miFindObjectOfType = typeof(UnityEngine.Object).GetMethod(
                        "FindAnyObjectByType", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                        new[] {typeof(Type), _findObjectsInactiveType},
                        null
                    );
                }
            }
            if (DeUnityEditorVersion.MajorVersion < 2023) {
                if (_findObjectOfType_hasIncludeInactiveParam) return (Object)_miFindObjectOfType.Invoke(null, new object[] {type, includeInactive});
                else return (Object)_miFindObjectOfType.Invoke(null, new object[] {type});
            } else {
                return (Object)_miFindObjectOfType.Invoke(null, new object[] {type, includeInactive ? 0 : 1});
            }
        }

        /// <summary>
        /// Warning: some versions of this method don't have the includeInactive parameter so it won't be taken into account
        /// </summary>
        public static T[] FindObjectsOfType<T>(bool includeInactive = false)
        {
            if (_miFindObjectsOfTypeGeneric == null) {
                if (DeUnityEditorVersion.MajorVersion < 2023) {
                    _miFindObjectsOfTypeGeneric = typeof(UnityEngine.Object).GetMethod(
                        "FindObjectsOfType", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                        new[] {typeof(bool)},
                        null
                    );
                    _findObjectsOfTypeGeneric_hasIncludeInactiveParam = true;
                    if (_miFindObjectsOfTypeGeneric == null) {
                        MethodInfo[] ms = typeof(UnityEngine.Object).GetMethods(BindingFlags.Static | BindingFlags.Public);
                        foreach (MethodInfo m in ms) {
                            if (m.Name != "FindObjectsOfType" || !m.IsGenericMethod) continue;
                            _miFindObjectsOfTypeGeneric = m;
                            break;
                        }
                        _findObjectsOfTypeGeneric_hasIncludeInactiveParam = false;
                    }
                    _miFindObjectsOfTypeGeneric = _miFindObjectsOfTypeGeneric.MakeGenericMethod(typeof(T));
                } else {
                    if (_findObjectsInactiveType == null) _findObjectsInactiveType = typeof(GameObject).Assembly.GetType("UnityEngine.FindObjectsInactive");
                    if (_findObjectsSortModeType == null) _findObjectsSortModeType = typeof(GameObject).Assembly.GetType("UnityEngine.FindObjectsSortMode");
                    _miFindObjectsOfTypeGeneric = typeof(UnityEngine.Object).GetMethod(
                        "FindObjectsByType", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                        new[] {_findObjectsInactiveType, _findObjectsSortModeType},
                        null
                    ).MakeGenericMethod(typeof(T));
                }
            }
            if (DeUnityEditorVersion.MajorVersion < 2023) {
                if (_findObjectsOfTypeGeneric_hasIncludeInactiveParam) return (T[])_miFindObjectsOfTypeGeneric.Invoke(null, new object[] {includeInactive});
                else return (T[])_miFindObjectsOfTypeGeneric.Invoke(null, null);
            } else {
                return (T[])_miFindObjectsOfTypeGeneric.Invoke(null, new object[] {includeInactive ? 0 : 1, 0});
            }
        }
        /// <summary>
        /// Warning: some versions of this method don't have the includeInactive parameter so it won't be taken into account
        /// </summary>
        public static Object[] FindObjectsOfType(Type type, bool includeInactive = false)
        {
            if (_miFindObjectsOfType == null) {
                if (DeUnityEditorVersion.MajorVersion < 2023) {
                    _miFindObjectsOfType = typeof(UnityEngine.Object).GetMethod(
                        "FindObjectsOfType", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                        new[] {typeof(Type), typeof(bool)},
                        null
                    );
                    _findObjectsOfType_hasIncludeInactiveParam = true;
                    if (_miFindObjectOfType == null) {
                        _miFindObjectsOfType = typeof(UnityEngine.Object).GetMethod(
                            "FindObjectsOfType", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                            new[] {typeof(Type)},
                            null
                        );
                        _findObjectsOfType_hasIncludeInactiveParam = false;
                    }
                } else {
                    if (_findObjectsInactiveType == null) _findObjectsInactiveType = typeof(GameObject).Assembly.GetType("UnityEngine.FindObjectsInactive");
                    if (_findObjectsSortModeType == null) _findObjectsSortModeType = typeof(GameObject).Assembly.GetType("UnityEngine.FindObjectsSortMode");
                    _miFindObjectsOfType = typeof(UnityEngine.Object).GetMethod(
                        "FindObjectsByType", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                        new[] {typeof(Type), _findObjectsInactiveType, _findObjectsSortModeType},
                        null
                    );
                }
            }
            if (DeUnityEditorVersion.MajorVersion < 2023) {
                if (_findObjectsOfType_hasIncludeInactiveParam) return (Object[])_miFindObjectsOfType.Invoke(null, new object[] {type, includeInactive});
                else return (Object[])_miFindObjectsOfType.Invoke(null, new object[] {type});
            } else {
                return (Object[])_miFindObjectsOfType.Invoke(null, new object[] {type, includeInactive ? 0 : 1, 0});
            }
        }

        public static Vector3 FreeMoveHandle(Vector3 position, Quaternion rotation, float size, Vector3 snap, Handles.CapFunction capFunction)
        {
            if (_miFreeMoveHandle == null) {
                if (DeUnityEditorVersion.Version < 2022) {
                    _miFreeMoveHandle = typeof(Handles).GetMethod(
                        "FreeMoveHandle", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                        new[] {typeof(Vector3), typeof(Quaternion), typeof(float), typeof(Vector3), typeof(Handles.CapFunction)},
                        null
                    );
                } else {
                    _miFreeMoveHandle = typeof(Handles).GetMethod(
                        "FreeMoveHandle", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                        new[] {typeof(Vector3), typeof(float), typeof(Vector3), typeof(Handles.CapFunction)},
                        null
                    );
                }
            }
            if (DeUnityEditorVersion.Version < 2022) {
                return (Vector3)_miFreeMoveHandle.Invoke(null, new object[] {position, rotation, size, snap, capFunction});
            }
            return (Vector3)_miFreeMoveHandle.Invoke(null, new object[] {position, size, snap, capFunction});
        }

        /// <summary>
        /// Returns the prefab parent by using different code on Unity 2018 or later
        /// </summary>
        public static Object GetPrefabParent(GameObject instance)
        {
            if (_miGetPrefabParent == null) {
                if (DeUnityEditorVersion.Version < 2017.2f) {
                    _miGetPrefabParent = typeof(PrefabUtility).GetMethod("GetPrefabParent", BindingFlags.Public | BindingFlags.Static);
                } else {
                    _miGetPrefabParent = typeof(PrefabUtility).GetMethod("GetCorrespondingObjectFromSource", BindingFlags.Public | BindingFlags.Static);
                    if (_miGetPrefabParent.IsGenericMethod) {
                        _miGetPrefabParent = _miGetPrefabParent.MakeGenericMethod(typeof(Object));
                    }
                }
            }
            return (Object)_miGetPrefabParent.Invoke(null, new object[] {instance});
        }

        #endregion
    }
}