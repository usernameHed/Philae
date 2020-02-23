using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.geometry.editor
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
            SerializedProperty delta = extLine.GetPropertie("_delta");
            SerializedProperty deltaSquared = extLine.GetPropertie("_deltaSquared");
            p1Propertie.vector3Value = p1;
            p2Propertie.vector3Value = p2;
            delta.vector3Value = p2 - p1;
            deltaSquared.floatValue = ExtVector3.DotProduct(delta.vector3Value, delta.vector3Value);
        }
    }
}