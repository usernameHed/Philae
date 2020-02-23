using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.editor
{
    public static class ExtGUIScopes
    {
        public static HorizontalScope Horiz(Color color, params GUILayoutOption[] options)
        {
            if (color == Color.clear)
            {
                return (Horiz(null, options));
            }
            GUIStyle BackGroundWhite = new GUIStyle();
            BackGroundWhite.normal.background = ExtTexture.MakeTex(600, 1, color);
            return (Horiz(BackGroundWhite, options));
        }

        public static HorizontalScope Horiz(params GUILayoutOption[] options)
        {
            return (Horiz(null, options));
        }

        public static HorizontalScope Horiz(GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style != null)
            {
                return (new HorizontalScope(style, options));
            }
            else
            {
                return (new HorizontalScope(options));
            }
        }

        public static VerticalScope Verti(Color color, params GUILayoutOption[] options)
        {
            if (color == Color.clear)
            {
                return (Verti(null, options));
            }
            GUIStyle BackGroundWhite = new GUIStyle();
            BackGroundWhite.normal.background = ExtTexture.MakeTex(600, 1, color);
            return (Verti(BackGroundWhite, options));
        }

        public static VerticalScope Verti(params GUILayoutOption[] options)
        {
            return (Verti(null, options));
        }

        public static VerticalScope Verti(GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style != null)
            {
                return (new VerticalScope(style, options));
            }
            else
            {
                return (new VerticalScope(options));
            }
        }


        //end class
    }

    //end nameSpace
}