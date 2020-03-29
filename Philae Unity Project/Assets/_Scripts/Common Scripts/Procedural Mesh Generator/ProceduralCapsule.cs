using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using hedCommon.extension.runtime;

namespace hedCommon.procedural
{
    /// <summary>
    /// Plane Description
    /// </summary>
    public class ProceduralCapsule : ProceduralShape
    {
        [SerializeField, OnValueChanged("GenerateShape")]
        private float _height = 1f;
        [SerializeField, Range(0.0001f, 5), OnValueChanged("GenerateShape")]
        protected float _radius = 0.5f;
        [SerializeField, Range(2, 100), OnValueChanged("GenerateShape")]
        private int _sides = 18;

        private int nbVerticesCap;
        protected float _topRadius = 0.5f;

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            _topRadius = _radius;
            nbVerticesCap = _sides + 1;

            Debug.Log("generate Cone...");
            CalculateVerticle();
            CalculateNormals();
            CalculateUvs();
            CalculateTriangle();
        }

        /// <summary>
        /// calculate verticle
        /// </summary>
        protected override void CalculateVerticle()
        {
            //sides + top
            _vertices = new Vector3[_sides * 2];
            int vert = 0;

            // Sides
            int v = 0;
            while (vert <= _vertices.Length - 2)
            {
                float rad = (float)v / _sides * PI_2;
                _vertices[vert] = new Vector3(Mathf.Cos(rad) * _topRadius, _height, Mathf.Sin(rad) * _topRadius);
                _vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * _radius, 0, Mathf.Sin(rad) * _radius);
                vert += 2;
                v++;
            }
            //_vertices[vert] = _vertices[_sides * 2 + 2];
            //_vertices[vert + 1] = _vertices[_sides * 2 + 3];
        }

        /// <summary>
        /// after having verticle, calculate normals of each points
        /// </summary>
        protected override void CalculateNormals()
        {
            // bottom + top + sides
            _normales = new Vector3[_vertices.Length];
            int vert = 0;

            // Sides
            int v = 0;
            while (vert <= _vertices.Length - 2)
            {
                float rad = (float)v / _sides * PI_2;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                _normales[vert] = new Vector3(cos, 0f, sin);
                _normales[vert + 1] = _normales[vert];

                vert += 2;
                v++;
            }
            //_normales[vert] = _normales[_sides * 2 + 2];
            //_normales[vert + 1] = _normales[_sides * 2 + 3];
        }

        /// <summary>
        /// calculate UV of each points;
        /// </summary>
        protected override void CalculateUvs()
        {
            _uvs = new Vector2[_vertices.Length];

            int u = 0;
            // Sides
            int u_sides = 0;
            while (u <= _uvs.Length - 4)
            {
                float t = (float)u_sides / _sides;
                _uvs[u] = new Vector3(t, 1f);
                _uvs[u + 1] = new Vector3(t, 0f);
                u += 2;
                u_sides++;
            }
            _uvs[u] = new Vector2(1f, 1f);
            _uvs[u + 1] = new Vector2(1f, 0f);
        }

        /// <summary>
        /// then save triangls of objects;
        /// </summary>
        protected override void CalculateTriangle()
        {
            int nbTriangles = (_sides * 2);
            _triangles = new int[nbTriangles * 3];

            int tri = 0;
            int i = 0;
            // Sides
            while (tri < nbTriangles - 2)
            {
                _triangles[i] = tri + 2;
                _triangles[i + 1] = tri + 1;
                _triangles[i + 2] = tri + 0;
                tri++;
                i += 3;

                _triangles[i] = tri + 1;
                _triangles[i + 1] = tri + 2;
                _triangles[i + 2] = tri + 0;
                tri++;
                i += 3;
            }
            _triangles[i] = tri;
            _triangles[i + 1] = 0;
            _triangles[i + 2] = 1;
            tri++;
            i += 3;

            _triangles[i] = tri;
            _triangles[i + 1] = tri - 1;
            _triangles[i + 2] = 1;
            tri++;
            i += 3;
        }

        /// <summary>
        /// fit the meshCollider to the procedural shape
        /// </summary>
        public override void GenerateCollider()
        {
            MeshCollider mesh = gameObject.GetOrAddComponent<MeshCollider>();
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            ExtColliders.AutoSizeCollider3d(meshFilter, mesh);
        }
        //end class
    }
}