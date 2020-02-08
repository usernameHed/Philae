using hedCommon.extension.runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.editor
{
    public static class ExtGUIMasks
    {
        public static int MaskField(int mask, string[] items, out bool valueHasChanged)
        {
            int oldIndex = mask;

            mask = EditorGUILayout.MaskField(mask, items);

            valueHasChanged = oldIndex != mask;

            return (mask);
        }

        //end class
    }
    //end nameSpace
}