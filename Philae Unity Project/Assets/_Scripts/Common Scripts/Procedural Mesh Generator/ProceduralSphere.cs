using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;

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
        /// calculate verticle
        /// </summary>
        private void CalculateVerticle()
        {
            _verticesObject = new Vector3[(_longitude + 1) * _latitude + 2];
            float _pi = Mathf.PI;
            float _2pi = _pi * 2f;

            _verticesObject[0] = Vector3.up * _radius;
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

                    _verticesObject[lon + lat * (_longitude + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * _radius;
                }
            }
            _verticesObject[_verticesObject.Length - 1] = Vector3.up * -_radius;
        }

        /// <summary>
        /// after having verticle, calculate normals of each points
        /// </summary>
        private void CalculateNormals()
        {
            _normalesObject = new Vector3[_verticesObject.Length];
            for (int n = 0; n < _verticesObject.Length; n++)
            {
                _normalesObject[n] = _verticesObject[n].normalized;
            }
        }

        /// <summary>
        /// calculate UV of each points;
        /// </summary>
        private void CalculateUvs()
        {
            _uvsObject = new Vector2[_verticesObject.Length];
            _uvsObject[0] = Vector2.up;
            _uvsObject[_uvsObject.Length - 1] = Vector2.zero;
            for (int lat = 0; lat < _latitude; lat++)
            {
                for (int lon = 0; lon <= _longitude; lon++)
                {
                    _uvsObject[lon + lat * (_longitude + 1) + 1] = new Vector2((float)lon / _longitude, 1f - (float)(lat + 1) / (_latitude + 1));
                }
            }
        }

        /// <summary>
        /// then save triangls of objects;
        /// </summary>
        private void CalculateTriangle()
        {
            int nbFaces = _verticesObject.Length;
            int nbTriangles = nbFaces * 2;
            int nbIndexes = nbTriangles * 3;
            _trianglesObject = new int[nbIndexes];

            //Top Cap
            int i = 0;
            for (int lon = 0; lon < _longitude; lon++)
            {
                _trianglesObject[i++] = lon + 2;
                _trianglesObject[i++] = lon + 1;
                _trianglesObject[i++] = 0;
            }

            //Middle
            for (int lat = 0; lat < _latitude - 1; lat++)
            {
                for (int lon = 0; lon < _longitude; lon++)
                {
                    int current = lon + lat * (_longitude + 1) + 1;
                    int next = current + _longitude + 1;

                    _trianglesObject[i++] = current;
                    _trianglesObject[i++] = current + 1;
                    _trianglesObject[i++] = next + 1;

                    _trianglesObject[i++] = current;
                    _trianglesObject[i++] = next + 1;
                    _trianglesObject[i++] = next;
                }
            }

            //Bottom Cap
            for (int lon = 0; lon < _longitude; lon++)
            {
                _trianglesObject[i++] = _verticesObject.Length - 1;
                _trianglesObject[i++] = _verticesObject.Length - (lon + 2) - 1;
                _trianglesObject[i++] = _verticesObject.Length - (lon + 1) - 1;
            }
        }

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
    }
}