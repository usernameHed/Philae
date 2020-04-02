﻿using hedCommon.extension.editor.sceneView;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public static class ExtMeshEditor
    {
        public struct PositionAndIndex
        {
            public Vector3 Position;
            public int Index;
        }

        public static void ShowNormals(Transform transform, Vector3[] normals, Vector3[] vertices, Color color)
        {
            Gizmos.color = color;
            Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 realPosition = matrix.MultiplyPoint3x4(vertices[i]);
                Vector3 normal = matrix.MultiplyVector(normals[i]);
                Debug.DrawRay(realPosition, normal * 0.1f, color);
            }
        }

        /// <summary>
        /// the big problem is: don't show 2 or 3 vertice at the same position...
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="mesh"></param>
        /// <param name="fontSize"></param>
        public static void ShowVerticesOfMesh(Transform transform, Vector3[] vertices, int fontSize)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            List<PositionAndIndex> points = new List<PositionAndIndex>(vertices.Length);

            for (int i = 0; i < vertices.Length; i++)
            {
                points.Add(new PositionAndIndex() { Position = vertices[i], Index = i });
            }
            for (int i = points.Count - 1; i >= 0; i--)
            {
                Vector3 currentPosition = points[i].Position;
                ExtSceneView.DisplayStringIn3D(matrix.MultiplyPoint3x4(points[i].Position), (points[i].Index + 1).ToString(), Color.white, fontSize);
                points.RemoveAt(i);
                int duplicate = 0;
                for (int k = points.Count - 1; k >= 0; k--)
                {
                    if (currentPosition == points[k].Position)
                    {
                        duplicate++;
                        ExtSceneView.DisplayStringIn3D(matrix.MultiplyPoint3x4(points[k].Position + new Vector3(0, 0.02f * duplicate, 0)), (points[k].Index + 1).ToString(), Color.white, fontSize);
                        points.RemoveAt(k);
                        k = points.Count - 1;
                    }
                }
                if (duplicate > 0)
                {
                    i = points.Count - 1;
                    if (i == -1)
                    {
                        return;
                    }
                }
            }
        }
    }
}