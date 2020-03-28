using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;

namespace hedCommon.procedural
{
    /// <summary>
    /// Plane Description
    /// </summary>
    public class ProceduralSphere : Generate
    {
        [SerializeField, Tooltip("radius")]
        private float _radius = 1f;
        [SerializeField, Tooltip("longitude")]
        private int _longitude = 24;
        [SerializeField, Tooltip("latitude")]
        private int _latitude = 16;

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            Debug.Log("generate Sphere...");
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
            _vertices = new Vector3[(_longitude + 1) * _latitude + 2];
            float _pi = Mathf.PI;
            float _2pi = _pi * 2f;

            _vertices[0] = Vector3.up * _radius;
            for (int lat = 0; lat < _latitude; lat++)
            {
                float a1 = _pi * (float)(lat + 1) / (_latitude + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int lon = 0; lon <= _longitude; lon++)
                {
                    float a2 = _2pi * (float)(lon == _longitude ? 0 : lon) / _longitude;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    _vertices[lon + lat * (_longitude + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * _radius;
                }
            }
            _vertices[_vertices.Length - 1] = Vector3.up * -_radius;
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
            _uvs[0] = Vector2.up;
            _uvs[_uvs.Length - 1] = Vector2.zero;
            for (int lat = 0; lat < _latitude; lat++)
            {
                for (int lon = 0; lon <= _longitude; lon++)
                {
                    _uvs[lon + lat * (_longitude + 1) + 1] = new Vector2((float)lon / _longitude, 1f - (float)(lat + 1) / (_latitude + 1));
                }
            }
        }

        /// <summary>
        /// then save triangls of objects;
        /// </summary>
        protected override void CalculateTriangle()
        {
            int nbFaces = _vertices.Length;
            int nbTriangles = nbFaces * 2;
            int nbIndexes = nbTriangles * 3;
            _triangles = new int[nbIndexes];

            //Top Cap
            int i = 0;
            for (int lon = 0; lon < _longitude; lon++)
            {
                _triangles[i++] = lon + 2;
                _triangles[i++] = lon + 1;
                _triangles[i++] = 0;
            }

            //Middle
            for (int lat = 0; lat < _latitude - 1; lat++)
            {
                for (int lon = 0; lon < _longitude; lon++)
                {
                    int current = lon + lat * (_longitude + 1) + 1;
                    int next = current + _longitude + 1;

                    _triangles[i++] = current;
                    _triangles[i++] = current + 1;
                    _triangles[i++] = next + 1;

                    _triangles[i++] = current;
                    _triangles[i++] = next + 1;
                    _triangles[i++] = next;
                }
            }

            //Bottom Cap
            for (int lon = 0; lon < _longitude; lon++)
            {
                _triangles[i++] = _vertices.Length - 1;
                _triangles[i++] = _vertices.Length - (lon + 2) - 1;
                _triangles[i++] = _vertices.Length - (lon + 1) - 1;
            }
        }

        /// <summary>
        /// fit the meshCollider to the procedural shape
        /// </summary>
        public override void GenerateCollider()
        {
            SphereCollider sphere = gameObject.GetOrAddComponent<SphereCollider>();
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            ExtColliders.AutoSizeCollider3d(meshFilter, sphere);
        }
    }
}