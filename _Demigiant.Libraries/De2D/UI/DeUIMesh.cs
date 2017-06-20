// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/06/20 13:59
// License Copyright (c) Daniele Giardini

using UnityEngine;
using UnityEngine.UI;

namespace DG.De2D.UI
{
    /// <summary>
    /// Used to draw a custom mesh inside Unity's UI
    /// </summary>
    public class DeUIMesh : MaskableGraphic
    {
        #region Serialized

    [Header("Mesh Data")]
        [SerializeField] Vector2[] _vertices;
        [SerializeField] Vector2[] _uvs;
        [SerializeField] int[] _trisIndexes;
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Draws a mesh with the given data. Coordinates are Canvas-based.
        /// </summary>
        public void Draw(Vector2[] vertices, int[] trisIndexes, Vector2[] uvs, Color color)
        {
            _vertices = vertices;
            _trisIndexes = trisIndexes;
            _uvs = uvs;
            this.color = color;
            SetAllDirty();
        }

        #endregion

        #region Methods

        protected override void OnPopulateMesh(VertexHelper vHelper)
        {
            if (_vertices == null || _uvs == null || _trisIndexes == null) return;
            int verticesLen = _vertices.Length;
            int uvsLen = _uvs.Length;
            int trisIndexesLen = _trisIndexes.Length;
            if (verticesLen != uvsLen) return;
            if (trisIndexesLen % 3 != 0) return;

			vHelper.Clear();

            for (int i = 0; i < verticesLen; ++i) vHelper.AddVert(_vertices[i], color, _uvs[i]);
            for (int i = 0; i < trisIndexesLen; i += 3) vHelper.AddTriangle(_trisIndexes[i], _trisIndexes[i+1], _trisIndexes[i+2]);
		}

        #endregion
    }
}