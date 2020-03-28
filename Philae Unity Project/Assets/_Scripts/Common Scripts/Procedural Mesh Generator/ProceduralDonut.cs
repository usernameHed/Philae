using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using hedCommon.extension.runtime;

namespace hedCommon.procedural
{
    /// <summary>
    /// Plane Description
    /// </summary>
    public class ProceduralDonut : Generate
    {
        [SerializeField]
        private float radius1 = 1f;
        [SerializeField]
        private float radius2 = .3f;
        [SerializeField]
        private int nbRadSeg = 24;
        [SerializeField]
        private int nbSides = 18;

        /// <summary>
        /// here generate the mesh...
        /// </summary>
        protected override void GenerateMesh()
        {
            Debug.Log("generate Donut...");
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
            _vertices = new Vector3[(nbRadSeg + 1) * (nbSides + 1)];
            float _2pi = Mathf.PI * 2f;
            for (int seg = 0; seg <= nbRadSeg; seg++)
            {
                int currSeg = seg == nbRadSeg ? 0 : seg;

                float t1 = (float)currSeg / nbRadSeg * _2pi;
                Vector3 r1 = new Vector3(Mathf.Cos(t1) * radius1, 0f, Mathf.Sin(t1) * radius1);

                for (int side = 0; side <= nbSides; side++)
                {
                    int currSide = side == nbSides ? 0 : side;

                    Vector3 normale = Vector3.Cross(r1, Vector3.up);
                    float t2 = (float)currSide / nbSides * _2pi;
                    Vector3 r2 = Quaternion.AngleAxis(-t1 * Mathf.Rad2Deg, Vector3.up) * new Vector3(Mathf.Sin(t2) * radius2, Mathf.Cos(t2) * radius2);

                    _vertices[side + seg * (nbSides + 1)] = r1 + r2;
                }
            }
        }

        /// <summary>
        /// after having verticle, calculate normals of each points
        /// </summary>
        protected override void CalculateNormals()
        {
            _normales = new Vector3[_vertices.Length];
            for (int seg = 0; seg <= nbRadSeg; seg++)
            {
                int currSeg = seg == nbRadSeg ? 0 : seg;

                float t1 = (float)currSeg / nbRadSeg * PI_2;
                Vector3 r1 = new Vector3(Mathf.Cos(t1) * radius1, 0f, Mathf.Sin(t1) * radius1);

                for (int side = 0; side <= nbSides; side++)
                {
                    _normales[side + seg * (nbSides + 1)] = (_vertices[side + seg * (nbSides + 1)] - r1).normalized;
                }
            }
        }

        /// <summary>
        /// calculate UV of each points;
        /// </summary>
        protected override void CalculateUvs()
        {
            _uvs = new Vector2[_vertices.Length];
            for (int seg = 0; seg <= nbRadSeg; seg++)
            {
                for (int side = 0; side <= nbSides; side++)
                {
                    _uvs[side + seg * (nbSides + 1)] = new Vector2((float)seg / nbRadSeg, (float)side / nbSides);
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
            for (int seg = 0; seg <= nbRadSeg; seg++)
            {
                for (int side = 0; side <= nbSides - 1; side++)
                {
                    int current = side + seg * (nbSides + 1);
                    int next = side + (seg < (nbRadSeg) ? (seg + 1) * (nbSides + 1) : 0);

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