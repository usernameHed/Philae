using hedCommon.extension.editor;
using hedCommon.geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    public static class ExtTriangleProperty
    {
        public const string PROPERTY_MOVABLE_TRIANGLE = "_movableTriangle";
        //public const string PROPERTY_EXT_DONUT = "_donut";
        //public const string PROPERTY_RADIUS = "_radius";
        //public const string PROPERTY_MATRIX = "_donutMatrix";

        /*
        public static float GetThickNess(this SerializedProperty extDonut)
        {
            return (extDonut.GetPropertie(PROPERTY_THICKNESS).floatValue);
        }
        */
    }
}