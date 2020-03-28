﻿using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using hedCommon.extension.runtime;

namespace hedCommon.procedural
{
    public struct TriangleIndices
    {
        public int v1;
        public int v2;
        public int v3;

        public TriangleIndices(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    /// <summary>
    /// Generate Description
    /// </summary>
    public abstract class Generate : MonoBehaviour
    {
        [SerializeField]
        protected MeshFilter _meshFilter;
        [SerializeField]
        protected MeshRenderer _meshRenderer;

        protected Vector3[] _verticesObject;           //verticle of object
        protected Vector3[] _normalesObject;           //normals of all verticles
        protected Vector2[] _uvsObject;                //uvs of points;
        protected int[] _trianglesObject;              //then save triangle of objects

        protected Mesh _meshObject;

        /// <summary>
        /// génère le mesh
        /// </summary>
        public void GeneratePlease()
        {
            if (_meshFilter.sharedMesh == null)
            {
                _meshFilter.sharedMesh = new Mesh();
                _meshFilter.sharedMesh.name = "Procedural Mesh";
            }
            _meshObject = _meshFilter.sharedMesh;
            _meshObject.Clear();
            GenerateMesh();
            _meshObject.vertices = _verticesObject;
            _meshObject.normals = _normalesObject;
            _meshObject.uv = _uvsObject;
            _meshObject.triangles = _trianglesObject;
            _meshObject.RecalculateBounds();
        }
        abstract protected void GenerateMesh(); //appelé à l'initialisation
    }
}