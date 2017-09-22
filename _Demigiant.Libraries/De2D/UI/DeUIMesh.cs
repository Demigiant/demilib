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
        [SerializeField] Vector2[] _uv0;
        [SerializeField] Vector2[] _uv1;
        [SerializeField] int[] _trisIndexes;
        
        #endregion

        static Vector3 _defNormal = Vector3.back;
        static Vector4 _defTangent = new Vector4(1f, 0f, 0f, -1f);

        CanvasRenderer _cRenderer {
            get {
                if (_fooCRenderer == null) _fooCRenderer = this.GetComponent<CanvasRenderer>();
                return _fooCRenderer;
            }
        }
        CanvasRenderer _fooCRenderer;

        #region Public Methods

        /// <summary>
        /// Draws a mesh with the given data. Coordinates are Canvas-based.
        /// </summary>
        public void Draw(Vector2[] vertices, int[] trisIndexes, Vector2[] uvs, Color color)
        { Draw(vertices, trisIndexes, uvs, null, color); }
        /// <summary>
        /// Draws a mesh with the given data. Coordinates are Canvas-based.
        /// </summary>
        public void Draw(Vector2[] vertices, int[] trisIndexes, Vector2[] uv0, Vector2[] uv1, Color color)
        {
            _cRenderer.cull = false;
            _vertices = vertices;
            _trisIndexes = trisIndexes;
            _uv0 = uv0;
            _uv1 = uv1;
            this.color = color;
            SetAllDirty();
        }

        /// <summary>
        /// Clears the drawn mesh (by activating culling so it doesn't show)
        /// </summary>
        public void Clear()
        {
            _cRenderer.cull = true;
        }



        #endregion

        #region Methods

        protected override void OnPopulateMesh(VertexHelper vHelper)
        {
            if (_vertices == null || _uv0 == null || _trisIndexes == null) return;
            int verticesLen = _vertices.Length;
            int uvsLen = _uv0.Length;
            int trisIndexesLen = _trisIndexes.Length;
            if (verticesLen != uvsLen) return;
            if (trisIndexesLen % 3 != 0) return;

			vHelper.Clear();

            for (int i = 0; i < verticesLen; ++i) {
                if (_uv1 != null) vHelper.AddVert(_vertices[i], color, _uv0[i], _uv1[i], _defNormal, _defTangent);
                else vHelper.AddVert(_vertices[i], color, _uv0[i]);
            }
            for (int i = 0; i < trisIndexesLen; i += 3) vHelper.AddTriangle(_trisIndexes[i], _trisIndexes[i+1], _trisIndexes[i+2]);
		}

        #endregion
    }
}