using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace hedCommon.extension.runtime
{
    public static class ExtMesh
    {
        private const string DEFAULT_LOCATION_GENERATED_MESH = "Assets/Resources/Primitive/Generated/";

        /// <summary>
        /// save the MeshFilter of the selected object in Assets/Resources/Procedural/
        /// </summary>
        [MenuItem("PERSO/Procedural/Save Selected Mesh")]
        public static void SaveSelectedMesh()
        {
            GameObject activeOne = Selection.activeGameObject;
            if (activeOne == null)
                return;
            SaveSelectedMeshObj(activeOne, false);
        }

        /// <summary>
        /// delete the mesh asset in the gameObject if it exist
        /// </summary>
        /// <param name="obj"></param>
        public static void DeleteSelectedMesh(GameObject obj)
        {
            MeshFilter meshRoad = obj.GetComponent<MeshFilter>();
            if (!meshRoad)
            {
                return;
            }
            Mesh tempMesh = meshRoad.sharedMesh;
            AssetDatabase.DeleteAsset(DEFAULT_LOCATION_GENERATED_MESH + meshRoad.sharedMesh.name);
        }

        public static Mesh SaveSelectedMeshObj(GameObject activeOne, bool replaceIfExist)
        {
            if (activeOne == null)
            {
                return (null);
            }

            MeshFilter meshRoad = activeOne.GetComponent<MeshFilter>();
            if (meshRoad == null)
            {
                return (null);
            }

            Mesh meshAsset = (Mesh)UnityEngine.Object.Instantiate(meshRoad.sharedMesh);

            string path = DEFAULT_LOCATION_GENERATED_MESH + activeOne.name + ".asset";
            if (!replaceIfExist)
            {
                path = ExtPaths.RenameIncrementalFile(path, out int index, false);
            }
            else
            {

            }
            Debug.Log(path);
            AssetDatabase.CreateAsset(meshAsset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ExtSelection.Ping(meshAsset);

            return (meshAsset);
        }

        /// <summary>
        /// Returns a shared mesh to work with. If existing, it will be cleared
        /// </summary>
        public static Mesh PrepareNewShared(this MeshFilter m, string name = "Mesh")
        {
            if (m == null)
                return null;
            if (m.sharedMesh == null)
            {
                Mesh msh = new Mesh();
                msh.MarkDynamic();
                msh.name = name;
                m.sharedMesh = msh;
            }
            else
            {
                m.sharedMesh.Clear();
                m.sharedMesh.name = name;
                m.sharedMesh.subMeshCount = 0;
            }
            return m.sharedMesh;
        }

        public static void CalculateTangents(this MeshFilter m)
        {
            //speed up math by copying the mesh arrays
            int[] triangles = m.sharedMesh.triangles;
            Vector3[] vertices = m.sharedMesh.vertices;
            Vector2[] uv = m.sharedMesh.uv;
            Vector3[] normals = m.sharedMesh.normals;

            if (uv.Length == 0)
                return;

            //variable definitions
            int triangleCount = triangles.Length;
            int vertexCount = vertices.Length;

            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];

            Vector4[] tangents = new Vector4[vertexCount];

            for (int a = 0; a < triangleCount; a += 3)
            {
                int i1 = triangles[a + 0];
                int i2 = triangles[a + 1];
                int i3 = triangles[a + 2];

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector2 w1 = uv[i1];
                Vector2 w2 = uv[i2];
                Vector2 w3 = uv[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float div = s1 * t2 - s2 * t1;
                float r = div == 0.0f ? 0.0f : 1.0f / div;

                float sdirX = (t2 * x1 - t1 * x2) * r;
                float sdirY = (t2 * y1 - t1 * y2) * r;
                float sdirZ = (t2 * z1 - t1 * z2) * r;
                float tdirX = (s1 * x2 - s2 * x1) * r;
                float tdirY = (s1 * y2 - s2 * y1) * r;
                float tdirZ = (s1 * z2 - s2 * z1) * r;

                tan1[i1].x += sdirX;
                tan1[i1].y += sdirY;
                tan1[i1].z += sdirZ;

                tan1[i2].x += sdirX;
                tan1[i2].y += sdirY;
                tan1[i2].z += sdirZ;

                tan1[i3].x += sdirX;
                tan1[i3].y += sdirY;
                tan1[i3].z += sdirZ;


                tan2[i1].x += tdirX;
                tan2[i1].y += tdirY;
                tan2[i1].z += tdirZ;

                tan2[i2].x += tdirX;
                tan2[i2].y += tdirY;
                tan2[i2].z += tdirZ;

                tan2[i3].x += tdirX;
                tan2[i3].y += tdirY;
                tan2[i3].z += tdirZ;
            }


            for (int a = 0; a < vertexCount; ++a)
            {
                Vector3 n = normals[a];
                Vector3 t = tan1[a];
                Vector3.OrthoNormalize(ref n, ref t);
                tangents[a].x = t.x;
                tangents[a].y = t.y;
                tangents[a].z = t.z;

                //inlined version of float dotOfCross = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f)
                float dotOfCross = ((n.y * t.z - n.z * t.y) * tan2[a].x + (n.z * t.x - n.x * t.z) * tan2[a].y + (n.x * t.y - n.y * t.x) * tan2[a].z);
                tangents[a].w = (dotOfCross < 0.0f) ? -1.0f : 1.0f;
            }

            m.sharedMesh.tangents = tangents;
        }
    }
}