using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;

namespace hedCommon.procedural
{
    /// <summary>
    /// Plane Description
    /// </summary>
    public class ProceduralPlane : Generate
    {
        [SerializeField, Tooltip("Length")]
        private float _length = 1f;
        [SerializeField, Tooltip("width")]
        private float _width = 1f;
        [SerializeField, Range(2, 100), OnValueChanged("ChangeRes"), Tooltip("resX")]
        private int _resolution = 2;


        private int _resX = 2; // 2 minimum
        private int _resZ = 2;

        private void ChangeRes()
        {
            _resX = _resZ = _resolution;
        }

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            Debug.Log("generate plane...");
            CalculateVerticle();
            CalculateNormals();
            CalculateUvs();
            CalculateTriangle();
        }

        /// <summary>
        /// calculate verticle
        /// </summary>
        private void CalculateVerticle()
        {
            _verticesObject = new Vector3[_resX * _resZ];    //setup size of verticle

            for (int z = 0; z < _resZ; z++)          //loop thought all vecticle
            {
                // [ -length / 2, length / 2 ]
                float zPos = ((float)z / (_resZ - 1) - .5f) * _length;
                for (int x = 0; x < _resX; x++)
                {
                    // [ -width / 2, width / 2 ]
                    float xPos = ((float)x / (_resX - 1) - .5f) * _width;
                    _verticesObject[x + z * _resX] = new Vector3(xPos, 0f, zPos);
                }
            }
        }

        /// <summary>
        /// after having verticle, calculate normals of each points
        /// </summary>
        private void CalculateNormals()
        {
            _normalesObject = new Vector3[_verticesObject.Length];
            for (int n = 0; n < _normalesObject.Length; n++)
            {
                _normalesObject[n] = transform.up;
            }
        }

        /// <summary>
        /// calculate UV of each points;
        /// </summary>
        private void CalculateUvs()
        {
            _uvsObject = new Vector2[_verticesObject.Length];
            for (int v = 0; v < _resZ; v++)
            {
                for (int u = 0; u < _resX; u++)
                {
                    _uvsObject[u + v * _resX] = new Vector2((float)u / (_resX - 1), (float)v / (_resZ - 1));
                }
            }
        }

        /// <summary>
        /// then save triangls of objects;
        /// </summary>
        private void CalculateTriangle()
        {
            int nbFaces = (_resX - 1) * (_resZ - 1);
            _trianglesObject = new int[nbFaces * 6];
            int t = 0;
            for (int face = 0; face < nbFaces; face++)
            {
                // Retrieve lower left corner from face ind
                int i = face % (_resX - 1) + (face / (_resZ - 1) * _resX);

                _trianglesObject[t++] = i + _resX;
                _trianglesObject[t++] = i + 1;
                _trianglesObject[t++] = i;

                _trianglesObject[t++] = i + _resX;
                _trianglesObject[t++] = i + _resX + 1;
                _trianglesObject[t++] = i + 1;
            }
        }
    }
}