using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using hedCommon.extension.runtime;

namespace hedCommon.procedural
{
    /// <summary>
    /// Generate a Torus
    /// </summary>
    public class ProceduralTorus : ProceduralShape
    {
        [SerializeField, OnValueChanged("GenerateShape")]
        private float _radius = 0.5f;
        [SerializeField, Range(0.0001f, 2), OnValueChanged("GenerateShape")]
        private float _thickNess = .2f;
        [SerializeField, Range(3, 100), OnValueChanged("GenerateShape")]
        private int _nbRadSeg = 24;
        [SerializeField, Range(3, 100), OnValueChanged("GenerateShape")]
        private int _nbSides = 18;

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            Debug.Log("generate Torus...");
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
            _vertices = new Vector3[(_nbRadSeg + 1) * (_nbSides + 1)];
            float _2pi = Mathf.PI * 2f;
            for (int seg = 0; seg <= _nbRadSeg; seg++)
            {
                int currSeg = seg == _nbRadSeg ? 0 : seg;

                float t1 = (float)currSeg / _nbRadSeg * _2pi;
                Vector3 r1 = new Vector3(Mathf.Cos(t1) * _radius, 0f, Mathf.Sin(t1) * _radius);

                for (int side = 0; side <= _nbSides; side++)
                {
                    int currSide = side == _nbSides ? 0 : side;

                    Vector3 normale = Vector3.Cross(r1, Vector3.up);
                    float t2 = (float)currSide / _nbSides * _2pi;
                    Vector3 r2 = Quaternion.AngleAxis(-t1 * Mathf.Rad2Deg, Vector3.up) * new Vector3(Mathf.Sin(t2) * _thickNess, Mathf.Cos(t2) * _thickNess);

                    _vertices[side + seg * (_nbSides + 1)] = r1 + r2;
                }
            }
        }

        /// <summary>
        /// after having verticle, calculate normals of each points
        /// </summary>
        protected override void CalculateNormals()
        {
            _normales = new Vector3[_vertices.Length];
            for (int seg = 0; seg <= _nbRadSeg; seg++)
            {
                int currSeg = seg == _nbRadSeg ? 0 : seg;

                float t1 = (float)currSeg / _nbRadSeg * PI_2;
                Vector3 r1 = new Vector3(Mathf.Cos(t1) * _radius, 0f, Mathf.Sin(t1) * _radius);

                for (int side = 0; side <= _nbSides; side++)
                {
                    _normales[side + seg * (_nbSides + 1)] = (_vertices[side + seg * (_nbSides + 1)] - r1).normalized;
                }
            }
        }

        /// <summary>
        /// calculate UV of each points;
        /// </summary>
        protected override void CalculateUvs()
        {
            _uvs = new Vector2[_vertices.Length];
            for (int seg = 0; seg <= _nbRadSeg; seg++)
            {
                for (int side = 0; side <= _nbSides; side++)
                {
                    _uvs[side + seg * (_nbSides + 1)] = new Vector2((float)seg / _nbRadSeg, (float)side / _nbSides);
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

            int i = 0;
            for (int seg = 0; seg <= _nbRadSeg; seg++)
            {
                for (int side = 0; side <= _nbSides - 1; side++)
                {
                    int current = side + seg * (_nbSides + 1);
                    int next = side + (seg < (_nbRadSeg) ? (seg + 1) * (_nbSides + 1) : 0);

                    if (i < _triangles.Length - 6)
                    {
                        _triangles[i++] = current;
                        _triangles[i++] = next;
                        _triangles[i++] = next + 1;

                        _triangles[i++] = current;
                        _triangles[i++] = next + 1;
                        _triangles[i++] = current + 1;
                    }
                }
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