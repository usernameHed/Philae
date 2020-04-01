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
        private int _nbSides = 18;
        [SerializeField, Range(2, 100), OnValueChanged("GenerateShape")]
        private int _latitude = 17;

        protected float _topRadius = 0.5f;
        private int _longitude;

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            _topRadius = _radius;
            _latitude = (_latitude % 2 == 0) ? _latitude : _latitude + 1;
            _longitude = _nbSides;

            Debug.Log("generate Capsule...");
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

            float radius = (_height < 0) ? -_radius : _radius;

            _vertices[0] = Vector3.up * radius + new Vector3(0, _height, 0);
            for (int lat = 0; lat < _latitude; lat++)
            {
                float a1 = Mathf.PI * (float)(lat + 1) / (_latitude + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int lon = 0; lon <= _longitude; lon++)
                {
                    float a2 = PI_2 * (float)(lon == _longitude ? 0 : lon) / _longitude;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    if (lat < _latitude / 2)
                    {
                        _vertices[lon + lat * (_longitude + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius + new Vector3(0, _height, 0);
                    }
                    else
                    {
                        _vertices[lon + lat * (_longitude + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
                    }
                }
            }
            _vertices[_vertices.Length - 1] = Vector3.up * -radius;
        }


        /// <summary>
        /// after having verticle, calculate normals of each points
        /// </summary>
        protected override void CalculateNormals()
        {
            _normales = new Vector3[_vertices.Length];

            for (int lat = 0; lat < _latitude; lat++)
            {
                float a1 = Mathf.PI * (float)(lat + 1) / (_latitude + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int lon = 0; lon <= _longitude; lon++)
                {
                    float a2 = PI_2 * (float)(lon == _longitude ? 0 : lon) / _longitude;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    int indexVertice = lon + lat * (_longitude + 1) + 1;
                    if (lat < _latitude / 2)
                    {
                        Vector3 vertice = _vertices[indexVertice];
                        if (lat == (_latitude / 2) - 1)
                        {
                            vertice.y = _height;
                        }
                        _normales[indexVertice] = (vertice - new Vector3(0, _height, 0)).normalized;
                    }
                    else
                    {
                        Vector3 vertice = _vertices[indexVertice];
                        if (lat == _latitude / 2)
                        {
                            vertice.y = 0;
                        }
                        _normales[indexVertice] = (vertice).normalized;
                    }
                }
            }
            _normales[0] = Vector3.up;
            _normales[_normales.Length - 1] = Vector3.down;
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
            for (int lat = 0; lat < (_latitude) - 1; lat++)
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
                if (_vertices[_vertices.Length - 1] == Vector3.zero + new Vector3(0, _height, 0)
                    || _vertices[_vertices.Length - (lon + 2) - 1] == Vector3.zero + new Vector3(0, _height, 0)
                    || _vertices[_vertices.Length - (lon + 1) - 1] == Vector3.zero + new Vector3(0, _height, 0))
                {
                    continue;
                }

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
            CapsuleCollider mesh = gameObject.GetOrAddComponent<CapsuleCollider>();
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            ExtColliders.AutoSizeCollider3d(meshFilter, mesh);
        }
        //end class
    }
}