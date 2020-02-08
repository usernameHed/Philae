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
    public static class ExtGUILabel
    {
        private static Texture _texture;


        public static void Label(string text, Color color, params GUILayoutOption[] options)
        {
            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.normal.textColor = color;
            GUILayout.Label(text, centeredStyle, options);
        }

        public static void Label(string text, params GUILayoutOption[] options)
        {
            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
            GUILayout.Label(text, centeredStyle, options);
        }

        public static void Label(string text, int fontSize = 14, params GUILayoutOption[] options)
        {
            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.fontSize = fontSize;
            GUILayout.Label(text, centeredStyle, options);
        }

        public static void LabelImage(string imageName, float width = 20, float height = 20)
        {
            _texture = (Texture)EditorGUIUtility.Load("SceneView/" + imageName + ".png");
            GUILayout.Label(_texture, GUILayout.Width(20), GUILayout.Height(20));
        }


        //end class
    }
    //end nameSpace
}