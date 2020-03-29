using UnityEngine;
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
    public abstract class ProceduralShape : MonoBehaviour
    {
        [SerializeField]
        protected MeshFilter _meshFilter;
        public MeshFilter MeshFilter { get { return (_meshFilter); } }
        [SerializeField]
        protected MeshRenderer _meshRenderer;

        [SerializeField, OnValueChanged("GenerateShape")]
        private Vector3 _offsetMesh;
        [SerializeField]
        private bool _showVertices = false;

        [SerializeField, ReadOnly]
        protected Vector3[] _vertices;           //verticle of object
        protected Vector3[] _normales;           //normals of all verticles
        protected Vector2[] _uvs;                //uvs of points;
        protected int[] _triangles;              //then save triangle of objects
        protected const float PI_2 = Mathf.PI * 2f;
        protected Mesh _meshObject;

        public virtual void InitOnCreation()
        {

        }

        /// <summary>
        /// génère le mesh
        /// </summary>
        public void GenerateShape()
        {
            if (_meshFilter.sharedMesh == null)
            {
                _meshFilter.sharedMesh = new Mesh();
                _meshFilter.sharedMesh.name = "Procedural Mesh";
            }
            _meshObject = _meshFilter.sharedMesh;
            _meshObject.Clear();
            GenerateMesh();
            OffsetMesh();
            _meshObject.vertices = _vertices;
            _meshObject.normals = _normales;
            _meshObject.uv = _uvs;
            _meshObject.triangles = _triangles;
            _meshObject.Optimize();
        }

        public void Optimise()
        {
            _meshObject.Optimize();
        }

        public void OffsetMesh()
        {
            for (int i = 0; i < _vertices.Length; i++)
            {
                _vertices[i] += _offsetMesh;
            }
        }

        abstract protected void GenerateMesh();
        abstract protected void CalculateVerticle();
        abstract protected void CalculateNormals();
        abstract protected void CalculateUvs();
        abstract protected void CalculateTriangle();

        abstract public void GenerateCollider();
    }
}