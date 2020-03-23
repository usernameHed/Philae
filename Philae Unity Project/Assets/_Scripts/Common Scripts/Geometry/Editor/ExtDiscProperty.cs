using hedCommon.extension.editor;
using hedCommon.geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    public static class ExtDiscProperty
    {
        public const string PROPERTY_EXT_DISC = "_disc";
        public const string PROPERTY_MATRIX = "_discMatrix";

        public const string PROPERTY_RADIUS = "_radius";
        public const string PROPERTY_RADIUS_SQUARED = "_radiusSquared";
        public const string PROPERTY_REAL_RADIUS = "_realRadius";
        public const string PROPERTY_REAL_RADIUS_SQUARED = "_realSquaredRadius";

        /*
        public static float GetThickNess(this SerializedProperty extDonut)
        {
            return (extDonut.GetPropertie(PROPERTY_THICKNESS).floatValue);
        }
        */
    }
}