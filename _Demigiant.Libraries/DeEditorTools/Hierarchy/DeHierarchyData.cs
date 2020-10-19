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
            public EvidenceMode evidenceMode = EvidenceMode.Outline;
            public Color color = Color.cyan;
        }
    }
}