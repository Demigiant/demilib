// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/10/19

using System;
using UnityEngine;

namespace DG.DeEditorTools.Hierarchy
{
    /// <summary>
    /// Stores project-related DeHierarchy preferences
    /// </summary>
    public class DeHierarchyData : ScriptableObject
    {
        public enum EvidenceMode
        {
            Outline,
            Box
        }

        public enum SearchMode
        {
            Unset,
            Contains,
            StartsWith,
            EndsWith
        }

        #region Serialized

        public ExtraEvidenceData[] extraEvidences = new ExtraEvidenceData[0];
        public int totExtraEvidences = 0;

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        [Serializable]
        public class ExtraEvidenceData
        {
            public string componentClass;
            public string searchString;
            public EvidenceMode evidenceMode = EvidenceMode.Outline;
            public Color color = Color.cyan;
            [SerializeField] SearchMode _foo_searchMode;

            public SearchMode searchMode { get { return _foo_searchMode; } }

            public void RefreshSearchMode()
            {
                searchString = componentClass;
                if (string.IsNullOrEmpty(componentClass)) _foo_searchMode = SearchMode.Unset;
                else {
                    int strLen = componentClass.Length;
                    if (strLen > 1) {
                        if (componentClass.StartsWith("|")) {
                            _foo_searchMode = SearchMode.StartsWith;
                            searchString = componentClass.Substring(1);
                        } else if (componentClass.EndsWith("|")) {
                            _foo_searchMode = SearchMode.EndsWith;
                            searchString = componentClass.Substring(0, strLen - 1);
                        } else _foo_searchMode = SearchMode.Contains;
                    } else _foo_searchMode = SearchMode.Contains;
                }
            }
        }
    }
}