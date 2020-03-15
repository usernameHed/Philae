using hedCommon.extension.editor;
using hedCommon.geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    public static class ExtLineProperty
    {
        public const string PROPERTY_MOVABLE_LINE = "_movableLine";
        public const string PROPERTY_EXT_LINE_3D = "_line3d";


        /*
        public static void SetRadius(this SerializedProperty extDonut, float radius)
        {
            ExtDonut donut = extDonut.GetValue<ExtDonut>();
            donut.ChangeRadius(radius);
            extDonut.SetValue<ExtDonut>(donut);
        }
        */
    }
}