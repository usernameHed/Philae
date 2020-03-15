using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    public static class ExtShapeSerializeProperty
    {
        /// <summary>
        /// See MoveShape of ExtLine to know what's going on
        /// </summary>
        /// <param name="extLine">line to change</param>
        /// <param name="p1">new p1</param>
        /// <param name="p2">new p2</param>
        public static void MoveLineFromSerializeProperties(SerializedProperty extLine, Vector3 p1, Vector3 p2)
        {
            SerializedProperty p1Propertie = extLine.GetPropertie("_p1");
            SerializedProperty p2Propertie = extLine.GetPropertie("_p2");
            p1Propertie.vector3Value = p1;
            p2Propertie.vector3Value = p2;

            UpdateLineFromSerializeProperties(extLine);
        }

        /// <summary>
        /// See MoveShape of ExtLine to know what's going on
        /// </summary>
        /// <param name="extLine">line to change</param>
        /// <param name="p1">new p1</param>
        /// <param name="p2">new p2</param>
        public static void UpdateLineFromSerializeProperties(SerializedProperty extLine)
        {
            SerializedProperty p1Propertie = extLine.GetPropertie("_p1");
            SerializedProperty p2Propertie = extLine.GetPropertie("_p2");
            SerializedProperty delta = extLine.GetPropertie("_delta");
            SerializedProperty deltaSquared = extLine.GetPropertie("_deltaSquared");
            delta.vector3Value = p2Propertie.vector3Value - p1Propertie.vector3Value;
            deltaSquared.floatValue = ExtVector3.DotProduct(delta.vector3Value, delta.vector3Value);
        }

        public static void MoveSplinePointFromSerializePropertie(SerializedProperty extSpline, Vector3 p1)
        {
            SerializedProperty listPointsOnSpline = extSpline.GetPropertie("_listPoints");
            listPointsOnSpline.arraySize = listPointsOnSpline.arraySize + 1;

            SerializedProperty point = listPointsOnSpline.GetArrayElementAtIndex(listPointsOnSpline.arraySize - 1);
            SerializedProperty localPoint = point.GetPropertie("PointLocal");
            SerializedProperty globalPoint = point.GetPropertie("PointGlobal");

            localPoint.vector3Value = p1;
            Matrix4x4 matrix = extSpline.GetPropertie("_splinesMatrix").GetValue<Matrix4x4>();
            globalPoint.vector3Value = matrix.MultiplyPoint3x4(p1);
        }
    }
}