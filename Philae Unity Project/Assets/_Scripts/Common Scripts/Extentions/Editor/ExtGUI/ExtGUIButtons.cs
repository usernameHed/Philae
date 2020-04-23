using hedCommon.eventEditor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public static class ExtGUIButtons
    {
        private static Texture _texture;
        public delegate void MethodToCallWhenClickOnButton();

        public static bool ButtonImage(string imageName, float width = 20, float height = 20)
        {
            _texture = (Texture)EditorGUIUtility.Load("SceneView/" + imageName + ".png");

            if (GUILayout.Button(_texture, GUILayout.Width(width), GUILayout.Height(height)))
            {
                return (true);
            }
            return (false);
        }


        public static bool ButtonImage(string imageName, params GUILayoutOption[] options)
        {
            _texture = (Texture)EditorGUIUtility.Load("SceneView/" + imageName + ".png");

            if (GUILayout.Button(_texture, options))
            {
                return (true);
            }
            return (false);
        }

        public static bool ButtonImage(string imageName, string toolTipText, GUIStyle guiStyle, params GUILayoutOption[] options)
        {
            _texture = (Texture)EditorGUIUtility.Load("SceneView/" + imageName + ".png");

            GUIContent toolTip = new GUIContent(_texture, toolTipText);

            if (GUILayout.Button(toolTip, guiStyle, options))
            {
                return (true);
            }
            return (false);
        }

        public static bool ButtonImage(string imageName, string tooTipText, KeyCode keyCode, GUIStyle guiStyle = null, params GUILayoutOption[] options)
        {
            _texture = (Texture)EditorGUIUtility.Load("SceneView/" + imageName + ".png");

            GUIContent toolTip = new GUIContent(_texture, tooTipText);

            if (GUILayout.Button(toolTip, guiStyle, options) || ExtEventEditor.IsKeyUp(Event.current, keyCode))
            {
                return (true);
            }
            return (false);
        }

        public static bool ButtonImage(string text, Color color, params GUILayoutOption[] options)
        {
            _texture = (Texture)EditorGUIUtility.Load("SceneView/" + text + ".png");
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = color;
            if (GUILayout.Button(_texture, options))
            {
                GUI.backgroundColor = previous;
                return (true);
            }
            GUI.backgroundColor = previous;
            return (false);
        }

        public static bool ButtonImage(string text, Color color, GUIStyle style, params GUILayoutOption[] options)
        {
            _texture = (Texture)EditorGUIUtility.Load("SceneView/" + text + ".png");
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = color;
            if (GUILayout.Button(_texture, style, options))
            {
                GUI.backgroundColor = previous;
                return (true);
            }
            GUI.backgroundColor = previous;
            return (false);
        }

        public static bool Button(string text, string toolTipText, GUIStyle style, params GUILayoutOption[] options)
        {
            GUIContent toolTip = new GUIContent(ExtString.Truncate(text, 20, "..."), toolTipText);

            if (GUILayout.Button(toolTip, style, options))
            {
                return (true);
            }
            return (false);
        }

        public static bool Button(string text, string toolTipText, params GUILayoutOption[] options)
        {
            GUIContent toolTip = new GUIContent(ExtString.Truncate(text, 20, "..."), toolTipText);

            if (GUILayout.Button(toolTip, options))
            {
                return (true);
            }
            return (false);
        }

        public static bool Button(string text, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(ExtString.Truncate(text, 20, "..."), options))
            {
                return (true);
            }
            return (false);
        }


        public static bool ButtonAskBefore(string textButton, Color color, string titleDialog = "Warning", string titleCOntentDialog = "Are you sure ?", GUIStyle guiStyle = default, params GUILayoutOption[] options)
        {
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = color;
            if (GUILayout.Button(ExtString.Truncate(textButton, 20, "..."), (guiStyle) == null ? new GUIStyle(GUI.skin.button) : guiStyle, options))
            {
                GUI.backgroundColor = previous;
                return (ExtGUI.DrawDisplayDialog(titleDialog, titleCOntentDialog));
            }
            GUI.backgroundColor = previous;
            return (false);
        }

        public static bool Button(string text, Color color, params GUILayoutOption[] options)
        {
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = color;
            if (GUILayout.Button(ExtString.Truncate(text, 20, "..."), options))
            {
                GUI.backgroundColor = previous;
                return (true);
            }
            GUI.backgroundColor = previous;
            return (false);
        }

        public static bool Button(string text, string toolTipText, Color color, GUIStyle guiStyle, params GUILayoutOption[] options)
        {
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = color;

            GUIContent toolTip = new GUIContent(ExtString.Truncate(text, 20, "..."), toolTipText);

            if (GUILayout.Button(toolTip, guiStyle, options))
            {
                GUI.backgroundColor = previous;
                return (true);
            }
            GUI.backgroundColor = previous;
            return (false);
        }

        public static void Button(string text, MethodToCallWhenClickOnButton toCall, Color color)
        {
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = color;
            if (GUILayout.Button(ExtString.Truncate(text, 20, "...")))
            {
                toCall();
            }
            GUI.backgroundColor = previous;
        }

        //end class
    }
    //end nameSpace
}