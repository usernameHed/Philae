using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using hedCommon.extension.runtime;

namespace hedCommon.procedural
{
    /// <summary>
    /// Plane Description
    /// </summary>
    public class ProceduralCone : Generate
    {
        [SerializeField]
        private float _height = 1f;
        [SerializeField]
        protected float _radius = 0.5f;
        [SerializeField]
        private int _sides = 18;

        private int nbVerticesCap;
        protected float _topRadius = 0f;

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
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
            // bottom + top + sides
            _vertices = new Vector3[nbVerticesCap + nbVerticesCap + _sides * 2 + 2];
            int vert = 0;

            // Bottom cap
            _vertices[vert++] = new Vector3(0f, 0f, 0f);
            while (vert <= _sides)
            {
                float rad = (float)vert / _sides * PI_2;
                _vertices[vert] = new Vector3(Mathf.Cos(rad) * _radius, 0f, Mathf.Sin(rad) * _radius);
                vert++;
            }

            // Top cap
            _vertices[vert++] = new Vector3(0f, _height, 0f);
            while (vert <= _sides * 2 + 1)
            {
                float rad = (float)(vert - _sides - 1) / _sides * PI_2;
                _vertices[vert] = new Vector3(Mathf.Cos(rad) * _topRadius, _height, Mathf.Sin(rad) * _topRadius);
                vert++;
            }

            // Sides
            int v = 0;
            while (vert <= _vertices.Length - 4)
            {
                float rad = (float)v / _sides * PI_2;
                _vertices[vert] = new Vector3(Mathf.Cos(rad) * _topRadius, _height, Mathf.Sin(rad) * _topRadius);
                _vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * _radius, 0, Mathf.Sin(rad) * _radius);
                vert += 2;
                v++;
            }
            _vertices[vert] = _vertices[_sides * 2 + 2];
            _vertices[vert + 1] = _vertices[_sides * 2 + 3];
        }

        /// <summary>
        /// after having verticle, calculate normals of each points
        /// </summary>
        protected override void CalculateNormals()
        {
            // bottom + top + sides
            _normales = new Vector3[_vertices.Length];
            int vert = 0;

            // Bottom cap
            while (vert <= _sides)
            {
                _normales[vert++] = Vector3.down;
            }

            // Top cap
            while (vert <= _sides * 2 + 1)
            {
                _normales[vert++] = Vector3.up;
            }

            // Sides
            int v = 0;
            while (vert <= _vertices.Length - 4)
            {
                float rad = (float)v / _sides * PI_2;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                _normales[vert] = new Vector3(cos, 0f, sin);
                _normales[vert + 1] = _normales[vert];

                vert += 2;
                v++;
            }
            _normales[vert] = _normales[_sides * 2 + 2];
            _normales[vert + 1] = _normales[_sides * 2 + 3];
        }

        /// <summary>
        /// calculate UV of each points;
        /// </summary>
        protected override void CalculateUvs()
        {
            _uvs = new Vector2[_vertices.Length];

            // Bottom cap
            int u = 0;
            _uvs[u++] = new Vector2(0.5f, 0.5f);
            while (u <= _sides)
            {
                float rad = (float)u / _sides * PI_2;
                _uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
                u++;
            }

            // Top cap
            _uvs[u++] = new Vector2(0.5f, 0.5f);
            while (u <= _sides * 2 + 1)
            {
                float rad = (float)u / _sides * PI_2;
                _uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
                u++;
            }

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
            int nbTriangles = _sides + _sides + _sides * 2;
            _triangles = new int[nbTriangles * 3 + 3];

            // Bottom cap
            int tri = 0;
            int i = 0;
            while (tri < _sides - 1)
            {
                _triangles[i] = 0;
                _triangles[i + 1] = tri + 1;
                _triangles[i + 2] = tri + 2;
                tri++;
                i += 3;
            }
            _triangles[i] = 0;
            _triangles[i + 1] = tri + 1;
            _triangles[i + 2] = 1;
            tri++;
            i += 3;

            // Top cap
            //tri++;
            while (tri < _sides * 2)
            {
                _triangles[i] = tri + 2;
                _triangles[i + 1] = tri + 1;
                _triangles[i + 2] = nbVerticesCap;
                tri++;
                i += 3;
            }

            _triangles[i] = nbVerticesCap + 1;
            _triangles[i + 1] = tri + 1;
            _triangles[i + 2] = nbVerticesCap;
            tri++;
            i += 3;
            tri++;

            // Sides
            while (tri <= nbTriangles)
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