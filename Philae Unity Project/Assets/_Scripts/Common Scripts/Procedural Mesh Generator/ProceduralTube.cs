using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using hedCommon.extension.runtime;

namespace hedCommon.procedural
{
    /// <summary>
    /// Plane Description
    /// </summary>
    public class ProceduralTube : ProceduralShape
    {
        [SerializeField, OnValueChanged("GenerateShape")]
        private float _height = 1f;
        [SerializeField, Range(3, 100), OnValueChanged("GenerateShape")]
        private int _nbSides = 24;

        [Space(10)]
        // Outter shell is at radius1 + radius2 / 2, inner shell at radius1 - radius2 / 2
        [SerializeField, Range(0, 10), OnValueChanged("OnRadiusChanged")]
        private float _topRadius = 0.5f;
        [SerializeField, Range(0, 10), OnValueChanged("OnRadiusChanged")]
        private float _bottomRadius = 0.5f;
        [Space(10)]
        [SerializeField, PropertyRange(0, "_topRadiusX2"), OnValueChanged("GenerateShape")]
        private float _topThickNess = 0.15f;
        [SerializeField, PropertyRange(0, "_bottomRadiusX2"), OnValueChanged("GenerateShape")]
        private float _bottomThickNess = 0.15f;

        private int _nbVerticesCap;
        private int _nbVerticesSides;
        private float _topRadiusX2 = 1f;
        private float _bottomRadiusX2 = 1f;

        private void OnRadiusChanged()
        {
            _topRadiusX2 = _topRadius * 2;
            _bottomRadiusX2 = _bottomRadius * 2;
            _topThickNess = ExtMathf.SetBetween(_topThickNess, 0, _topRadiusX2);
            _bottomThickNess = ExtMathf.SetBetween(_bottomThickNess, 0, _bottomRadiusX2);

            base.GenerateShape();
        }

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            _nbVerticesCap = _nbSides * 2 + 2;
            _nbVerticesSides = _nbSides * 2 + 2;

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
            _vertices = new Vector3[_nbVerticesCap * 2 + _nbVerticesSides * 2];
            int vert = 0;
            float _2pi = Mathf.PI * 2f;

            // Bottom cap
            int sideCounter = 0;
            while (vert < _nbVerticesCap)
            {
                sideCounter = sideCounter == _nbSides ? 0 : sideCounter;

                float r1 = (float)(sideCounter++) / _nbSides * _2pi;
                float cos = Mathf.Cos(r1);
                float sin = Mathf.Sin(r1);
                _vertices[vert] = new Vector3(cos * (_bottomRadius - _bottomThickNess * .5f), 0f, sin * (_bottomRadius - _bottomThickNess * .5f));
                _vertices[vert + 1] = new Vector3(cos * (_bottomRadius + _bottomThickNess * .5f), 0f, sin * (_bottomRadius + _bottomThickNess * .5f));
                vert += 2;
            }

            // Top cap
            sideCounter = 0;
            while (vert < _nbVerticesCap * 2)
            {
                sideCounter = sideCounter == _nbSides ? 0 : sideCounter;

                float r1 = (float)(sideCounter++) / _nbSides * _2pi;
                float cos = Mathf.Cos(r1);
                float sin = Mathf.Sin(r1);
                _vertices[vert] = new Vector3(cos * (_topRadius - _topThickNess * .5f), _height, sin * (_topRadius - _topThickNess * .5f));
                _vertices[vert + 1] = new Vector3(cos * (_topRadius + _topThickNess * .5f), _height, sin * (_topRadius + _topThickNess * .5f));
                vert += 2;
            }

            // Sides (out)
            sideCounter = 0;
            while (vert < _nbVerticesCap * 2 + _nbVerticesSides)
            {
                sideCounter = sideCounter == _nbSides ? 0 : sideCounter;

                float r1 = (float)(sideCounter++) / _nbSides * _2pi;
                float cos = Mathf.Cos(r1);
                float sin = Mathf.Sin(r1);

                _vertices[vert] = new Vector3(cos * (_topRadius + _topThickNess * .5f), _height, sin * (_topRadius + _topThickNess * .5f));
                _vertices[vert + 1] = new Vector3(cos * (_bottomRadius + _bottomThickNess * .5f), 0, sin * (_bottomRadius + _bottomThickNess * .5f));
                vert += 2;
            }

            // Sides (in)
            sideCounter = 0;
            while (vert < _vertices.Length)
            {
                sideCounter = sideCounter == _nbSides ? 0 : sideCounter;

                float r1 = (float)(sideCounter++) / _nbSides * _2pi;
                float cos = Mathf.Cos(r1);
                float sin = Mathf.Sin(r1);

                _vertices[vert] = new Vector3(cos * (_topRadius - _topThickNess * .5f), _height, sin * (_topRadius - _topThickNess * .5f));
                _vertices[vert + 1] = new Vector3(cos * (_bottomRadius - _bottomThickNess * .5f), 0, sin * (_bottomRadius - _bottomThickNess * .5f));
                vert += 2;
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
            while (vert < _nbVerticesCap)
            {
                _normales[vert++] = Vector3.down;
            }

            // Top cap
            while (vert < _nbVerticesCap * 2)
            {
                _normales[vert++] = Vector3.up;
            }

            // Sides (out)
            int sideCounter = 0;
            while (vert < _nbVerticesCap * 2 + _nbVerticesSides)
            {
                sideCounter = sideCounter == _nbSides ? 0 : sideCounter;

                float r1 = (float)(sideCounter++) / _nbSides * PI_2;

                _normales[vert] = new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1));
                _normales[vert + 1] = _normales[vert];
                vert += 2;
            }

            // Sides (in)
            sideCounter = 0;
            while (vert < _vertices.Length)
            {
                sideCounter = sideCounter == _nbSides ? 0 : sideCounter;

                float r1 = (float)(sideCounter++) / _nbSides * PI_2;

                _normales[vert] = -(new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1)));
                _normales[vert + 1] = _normales[vert];
                vert += 2;
            }
        }

        /// <summary>
        /// calculate UV of each points;
        /// </summary>
        protected override void CalculateUvs()
        {
            _uvs = new Vector2[_vertices.Length];

            int vert = 0;
            // Bottom cap
            int sideCounter = 0;
            while (vert < _nbVerticesCap)
            {
                float t = (float)(sideCounter++) / _nbSides;
                _uvs[vert++] = new Vector2(0f, t);
                _uvs[vert++] = new Vector2(1f, t);
            }

            // Top cap
            sideCounter = 0;
            while (vert < _nbVerticesCap * 2)
            {
                float t = (float)(sideCounter++) / _nbSides;
                _uvs[vert++] = new Vector2(0f, t);
                _uvs[vert++] = new Vector2(1f, t);
            }

            // Sides (out)
            sideCounter = 0;
            while (vert < _nbVerticesCap * 2 + _nbVerticesSides)
            {
                float t = (float)(sideCounter++) / _nbSides;
                _uvs[vert++] = new Vector2(t, 0f);
                _uvs[vert++] = new Vector2(t, 1f);
            }

            // Sides (in)
            sideCounter = 0;
            while (vert < _vertices.Length)
            {
                float t = (float)(sideCounter++) / _nbSides;
                _uvs[vert++] = new Vector2(t, 0f);
                _uvs[vert++] = new Vector2(t, 1f);
            }
        }

        /// <summary>
        /// then save triangls of objects;
        /// </summary>
        protected override void CalculateTriangle()
        {
            int nbFace = _nbSides * 4;
            int nbTriangles = nbFace * 2;
            int nbIndexes = nbTriangles * 3;
            _triangles = new int[nbIndexes];

            // Bottom cap
            int i = 0;
            int sideCounter = 0;
            while (sideCounter < _nbSides)
            {
                int current = sideCounter * 2;
                int next = sideCounter * 2 + 2;

                _triangles[i++] = next + 1;
                _triangles[i++] = next;
                _triangles[i++] = current;
                
                _triangles[i++] = current + 1;
                _triangles[i++] = next + 1;
                _triangles[i++] = current;

                sideCounter++;
            }

            // Top cap
            while (sideCounter < _nbSides * 2)
            {
                int current = sideCounter * 2 + 2;
                int next = sideCounter * 2 + 4;

                _triangles[i++] = current;
                _triangles[i++] = next;
                _triangles[i++] = next + 1;
                
                _triangles[i++] = current;
                _triangles[i++] = next + 1;
                _triangles[i++] = current + 1;

                sideCounter++;
            }

            // Sides (out)
            while (sideCounter < _nbSides * 3)
            {
                int current = sideCounter * 2 + 4;
                int next = sideCounter * 2 + 6;

                _triangles[i++] = current;
                _triangles[i++] = next;
                _triangles[i++] = next + 1;
                
                _triangles[i++] = current;
                _triangles[i++] = next + 1;
                _triangles[i++] = current + 1;

                sideCounter++;
            }


            // Sides (in)
            while (sideCounter < _nbSides * 4)
            {
                int current = sideCounter * 2 + 6;
                int next = sideCounter * 2 + 8;

                _triangles[i++] = next + 1;
                _triangles[i++] = next;
                _triangles[i++] = current;
                
                _triangles[i++] = current + 1;
                _triangles[i++] = next + 1;
                _triangles[i++] = current;

                sideCounter++;
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