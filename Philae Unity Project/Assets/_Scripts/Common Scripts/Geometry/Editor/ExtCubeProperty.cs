using hedCommon.extension.editor;
using hedCommon.geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    public static class ExtCubeProperty
    {
        public const string PROPERTY_MOVABLE_CUBE = "_movableCube";
        public const string PROPERTY_EXT_CUBE = "_cube";


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