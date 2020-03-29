using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using TMPro;
using System.Collections.Generic;
using hedCommon.extension.runtime;

namespace hedCommon.procedural
{
    /// <summary>
    /// Plane Description
    /// </summary>
    public class ProceduralIcoSphere : ProceduralShape
    {
        [SerializeField, OnValueChanged("GenerateShape")]
        private float _radius = 1f;
        [SerializeField, Range(0, 3), OnValueChanged("GenerateShape")]
        private int _recursionLevel = 3;

        private List<Vector3> _vertList = new List<Vector3>();

        private Dictionary<long, int> _middlePointIndexCache = new Dictionary<long, int>();
        private int _index = 0;

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            Debug.Log("generate Sphere...");
            CalculateVerticle();
            CalculateTriangle();
            CalculateNormals();
            CalculateUvs();
        }

        /// <summary>
        /// calculate verticle
        /// </summary>
        protected override void CalculateVerticle()
        {
            float t = (1f + Mathf.Sqrt(5f)) / 2f;

            _vertList.Add(new Vector3(-1f, t, 0f).normalized * _radius);
            _vertList.Add(new Vector3(1f, t, 0f).normalized * _radius);
            _vertList.Add(new Vector3(-1f, -t, 0f).normalized * _radius);
            _vertList.Add(new Vector3(1f, -t, 0f).normalized * _radius);

            _vertList.Add(new Vector3(0f, -1f, t).normalized * _radius);
            _vertList.Add(new Vector3(0f, 1f, t).normalized * _radius);
            _vertList.Add(new Vector3(0f, -1f, -t).normalized * _radius);
            _vertList.Add(new Vector3(0f, 1f, -t).normalized * _radius);

            _vertList.Add(new Vector3(t, 0f, -1f).normalized * _radius);
            _vertList.Add(new Vector3(t, 0f, 1f).normalized * _radius);
            _vertList.Add(new Vector3(-t, 0f, -1f).normalized * _radius);
            _vertList.Add(new Vector3(-t, 0f, 1f).normalized * _radius);
        }

        /// <summary>
        /// after having verticle, calculate normals of each points
        /// </summary>
        protected override void CalculateNormals()
        {
            _normales = new Vector3[_vertices.Length];
            for (int n = 0; n < _vertices.Length; n++)
            {
                _normales[n] = _vertices[n].normalized;
            }
        }

        /// <summary>
        /// calculate UV of each points;
        /// </summary>
        protected override void CalculateUvs()
        {
            _uvs = new Vector2[_vertices.Length];
        }

        /// <summary>
        /// then save triangls of objects;
        /// </summary>
        protected override void CalculateTriangle()
        {
            // create 20 triangles of the icosahedron
            List<TriangleIndices> faces = new List<TriangleIndices>();

            // 5 faces around point 0
            faces.Add(new TriangleIndices(0, 11, 5));
            faces.Add(new TriangleIndices(0, 5, 1));
            faces.Add(new TriangleIndices(0, 1, 7));
            faces.Add(new TriangleIndices(0, 7, 10));
            faces.Add(new TriangleIndices(0, 10, 11));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(1, 5, 9));
            faces.Add(new TriangleIndices(5, 11, 4));
            faces.Add(new TriangleIndices(11, 10, 2));
            faces.Add(new TriangleIndices(10, 7, 6));
            faces.Add(new TriangleIndices(7, 1, 8));

            // 5 faces around point 3
            faces.Add(new TriangleIndices(3, 9, 4));
            faces.Add(new TriangleIndices(3, 4, 2));
            faces.Add(new TriangleIndices(3, 2, 6));
            faces.Add(new TriangleIndices(3, 6, 8));
            faces.Add(new TriangleIndices(3, 8, 9));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(4, 9, 5));
            faces.Add(new TriangleIndices(2, 4, 11));
            faces.Add(new TriangleIndices(6, 2, 10));
            faces.Add(new TriangleIndices(8, 6, 7));
            faces.Add(new TriangleIndices(9, 8, 1));


            // refine triangles
            for (int i = 0; i < _recursionLevel; i++)
            {
                List<TriangleIndices> faces2 = new List<TriangleIndices>();
                foreach (var tri in faces)
                {
                    // replace triangle by 4 triangles
                    int a = GetMiddlePoint(tri.v1, tri.v2, ref _vertList, ref _middlePointIndexCache, _radius);
                    int b = GetMiddlePoint(tri.v2, tri.v3, ref _vertList, ref _middlePointIndexCache, _radius);
                    int c = GetMiddlePoint(tri.v3, tri.v1, ref _vertList, ref _middlePointIndexCache, _radius);

                    faces2.Add(new TriangleIndices(tri.v1, a, c));
                    faces2.Add(new TriangleIndices(tri.v2, b, a));
                    faces2.Add(new TriangleIndices(tri.v3, c, b));
                    faces2.Add(new TriangleIndices(a, b, c));
                }
                faces = faces2;
            }

            _vertices = _vertList.ToArray();

            List<int> triList = new List<int>();
            for (int i = 0; i < faces.Count; i++)
            {
                triList.Add(faces[i].v1);
                triList.Add(faces[i].v2);
                triList.Add(faces[i].v3);
            }
            _triangles = triList.ToArray();
        }

        /// <summary>
        /// return index of point in the middle of p1 and p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="vertices"></param>
        /// <param name="cache"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private static int GetMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
        {
            // first check if we have it already
            bool firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            long greaterIndex = firstIsSmaller ? p2 : p1;
            long key = (smallerIndex << 32) + greaterIndex;

            int ret;
            if (cache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it
            Vector3 point1 = vertices[p1];
            Vector3 point2 = vertices[p2];
            Vector3 middle = new Vector3
            (
                (point1.x + point2.x) / 2f,
                (point1.y + point2.y) / 2f,
                (point1.z + point2.z) / 2f
            );

            // add vertex makes sure point is on unit sphere
            int i = vertices.Count;
            vertices.Add(middle.normalized * radius);

            // store it, return index
            cache.Add(key, i);

            return i;
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
    }
}