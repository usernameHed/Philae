using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using hedCommon.extension.runtime;

namespace hedCommon.procedural
{
    /// <summary>
    /// Generate a Plane with a given size, and resolution
    /// </summary>
    public class ProceduralPlane : ProceduralShape
    {
        [SerializeField, Tooltip("Length"), OnValueChanged("GenerateShape")]
        private float _length = 1f;
        [SerializeField, Tooltip("width"), OnValueChanged("GenerateShape")]
        private float _width = 1f;
        [SerializeField, Range(2, 100), OnValueChanged("GenerateShape")]
        private int _resolution = 2;


        private int _resX = 2;
        private int _resZ = 2;

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            _resX = _resZ = _resolution;

            Debug.Log("generate plane...");
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
            _vertices = new Vector3[_resX * _resZ];    //setup size of verticle

            for (int z = 0; z < _resZ; z++)          //loop thought all vecticle
            {
                // [ -length / 2, length / 2 ]
                float zPos = ((float)z / (_resZ - 1) - .5f) * _length;
                for (int x = 0; x < _resX; x++)
                {
                    // [ -width / 2, width / 2 ]
                    float xPos = ((float)x / (_resX - 1) - .5f) * _width;
                    _vertices[x + z * _resX] = new Vector3(xPos, 0f, zPos);
                }
            }
        }

        /// <summary>
        /// after having verticle, calculate normals of each points
        /// </summary>
        protected override void CalculateNormals()
        {
            _normales = new Vector3[_vertices.Length];
            for (int n = 0; n < _normales.Length; n++)
            {
                _normales[n] = transform.up;
            }
        }

        /// <summary>
        /// calculate UV of each points;
        /// </summary>
        protected override void CalculateUvs()
        {
            _uvs = new Vector2[_vertices.Length];
            for (int v = 0; v < _resZ; v++)
            {
                for (int u = 0; u < _resX; u++)
                {
                    _uvs[u + v * _resX] = new Vector2((float)u / (_resX - 1), (float)v / (_resZ - 1));
                }
            }
        }

        /// <summary>
        /// then save triangls of objects;
        /// </summary>
        protected override void CalculateTriangle()
        {
            int nbFaces = (_resX - 1) * (_resZ - 1);
            _triangles = new int[nbFaces * 6];
            int t = 0;
            for (int face = 0; face < nbFaces; face++)
            {
                // Retrieve lower left corner from face ind
                int i = face % (_resX - 1) + (face / (_resZ - 1) * _resX);

                _triangles[t++] = i + _resX;
                _triangles[t++] = i + 1;
                _triangles[t++] = i;

                _triangles[t++] = i + _resX;
                _triangles[t++] = i + _resX + 1;
                _triangles[t++] = i + 1;
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
    }
}