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
        [SerializeField, OnValueChanged("GenerateShape")]
        private int _latitude = 20;

        private int nbVerticesCap;
        protected float _topRadius = 0.5f;

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            _topRadius = _radius;
            nbVerticesCap = _nbSides + 1;

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
            //sides + top + Bottom
            int capsule = _nbSides * 2;
            int sphere = (_nbSides + 1) * _latitude + 2;
            _vertices = new Vector3[capsule + sphere];
            int vert = 0;

            // Sides
            int v = 0;
            while (vert <= capsule - 2)
            {
                float rad = (float)v / _nbSides * PI_2;
                _vertices[vert] = new Vector3(Mathf.Cos(rad) * _topRadius, _height, Mathf.Sin(rad) * _topRadius);
                _vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * _radius, 0, Mathf.Sin(rad) * _radius);
                vert += 2;
                v++;
            }

            int middleLatitude = _latitude / 2;

            //up
            _vertices[vert] = Vector3.up * _radius;
            for (int lat = 0; lat < middleLatitude; lat++)
            {
                float a1 = Mathf.PI * (float)(lat + 1) / (_latitude + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int lon = 0; lon <= _nbSides; lon++)
                {
                    float a2 = PI_2 * (float)(lon == _nbSides ? 0 : lon) / _nbSides;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    _vertices[vert + lon + lat * (_nbSides + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * _radius + new Vector3(0, _height, 0);
                }
            }

            //down
            for (int lat = middleLatitude; lat < _latitude; lat++)
            {
                float a1 = Mathf.PI * (float)(lat + 1) / (_latitude + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int lon = 0; lon <= _nbSides; lon++)
                {
                    float a2 = PI_2 * (float)(lon == _nbSides ? 0 : lon) / _nbSides;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    _vertices[vert + lon + lat * (_nbSides + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * _radius;
                }
            }

            _vertices[vert + sphere - 1] = Vector3.up * -_radius;
        }

        /// <summary>
        /// after having verticle, calculate normals of each points
        /// </summary>
        protected override void CalculateNormals()
        {
            int capsule = _nbSides * 2;
            int sphere = (_nbSides + 1) * _latitude + 2;

            // bottom + top + sides
            _normales = new Vector3[_vertices.Length];
            int vert = 0;

            // Sides
            int v = 0;
            while (vert <= capsule - 2)
            {
                float rad = (float)v / _nbSides * PI_2;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                _normales[vert] = new Vector3(cos, 0f, sin);
                _normales[vert + 1] = _normales[vert];

                vert += 2;
                v++;
            }

            int middleLatitude = _latitude / 2;

            //top
            for (int lat = 0; lat < middleLatitude; lat++)
            {
                float a1 = Mathf.PI * (float)(lat + 1) / (_latitude + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int lon = 0; lon <= _nbSides; lon++)
                {
                    float a2 = PI_2 * (float)(lon == _nbSides ? 0 : lon) / _nbSides;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    int indexVertice = vert + lon + lat * (_nbSides + 1) + 1;
                    _normales[indexVertice] = (_vertices[indexVertice] - new Vector3(0, _height, 0)).normalized;
                }
            }

            //down
            for (int lat = middleLatitude; lat < _latitude; lat++)
            {
                float a1 = Mathf.PI * (float)(lat + 1) / (_latitude + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int lon = 0; lon <= _nbSides; lon++)
                {
                    float a2 = PI_2 * (float)(lon == _nbSides ? 0 : lon) / _nbSides;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    int indexVertice = vert + lon + lat * (_nbSides + 1) + 1;
                    _normales[indexVertice] = (_vertices[indexVertice]).normalized;
                }
            }
        }

        /// <summary>
        /// calculate UV of each points;
        /// </summary>
        protected override void CalculateUvs()
        {
            int capsule = _nbSides * 2;
            int sphere = (_nbSides + 1) * _latitude + 2;

            _uvs = new Vector2[_vertices.Length];

            int u = 0;
            // Sides
            int u_sides = 0;
            while (u <= capsule - 4)
            {
                float t = (float)u_sides / _nbSides;
                _uvs[u] = new Vector3(t, 1f);
                _uvs[u + 1] = new Vector3(t, 0f);
                u += 2;
                u_sides++;
            }
            _uvs[u] = new Vector2(1f, 1f);
            _uvs[u + 1] = new Vector2(1f, 0f);

            /*
            //top
            _uvs[0] = Vector2.up;
            _uvs[_uvs.Length - 1] = Vector2.zero;
            for (int lat = _latitudeStart; lat < _latitudeEnd; lat++)
            {
                for (int lon = _longitudeStart; lon <= _longitudeEnd; lon++)
                {
                    _uvs[lon + lat * (_longitude + 1) + 1] = new Vector2((float)lon / _longitude, 1f - (float)(lat + 1) / (_latitude + 1));
                }
            }
            */
        }

        /// <summary>
        /// then save triangls of objects;
        /// </summary>
        protected override void CalculateTriangle()
        {
            int nbTriangles = (_nbSides * 2);
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