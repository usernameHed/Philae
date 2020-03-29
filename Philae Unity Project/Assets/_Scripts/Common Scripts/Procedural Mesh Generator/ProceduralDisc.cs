using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using hedCommon.extension.runtime;

namespace hedCommon.procedural
{
    /// <summary>
    /// Plane Description
    /// </summary>
    public class ProceduralDisc : ProceduralShape
    {
        [SerializeField, Range(0.0001f, 10), OnValueChanged("GenerateShape")]
        protected float _radius = 0.5f;
        [SerializeField, Range(3, 100), OnValueChanged("GenerateShape")]
        private int _sides = 18;

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            Debug.Log("generate Disc...");
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
            _vertices = new Vector3[_sides + 1];
            int vert = 0;

            // Bottom cap
            _vertices[vert++] = new Vector3(0f, 0f, 0f);
            while (vert <= _sides)
            {
                float rad = (float)vert / _sides * PI_2;
                _vertices[vert] = new Vector3(Mathf.Cos(rad) * _radius, 0f, Mathf.Sin(rad) * _radius);
                vert++;
            }
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
                _normales[vert++] = Vector3.up;
            }
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
        }

        /// <summary>
        /// then save triangls of objects;
        /// </summary>
        protected override void CalculateTriangle()
        {
            int nbTriangles = _sides;
            _triangles = new int[nbTriangles * 3 + 3];

            // Bottom cap
            int tri = 0;
            int i = 0;
            while (tri < _sides - 1)
            {
                _triangles[i] = tri + 2;
                _triangles[i + 1] = tri + 1;
                _triangles[i + 2] = 0;
                tri++;
                i += 3;
            }
            _triangles[i] = 1;
            _triangles[i + 1] = tri + 1;
            _triangles[i + 2] = 0;
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