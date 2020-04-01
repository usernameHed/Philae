using hedCommon.extension.runtime;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor
{
    static class ExtGUIStyles
    {
        public static readonly GUIStyle commandButtonStyle;
        public static readonly GUIStyle boxBackground;
        public static readonly GUIStyle helpBox;
        public static readonly GUIStyle box;
        public static readonly GUIStyle miniBox;
        public static readonly GUIStyle miniText;
        public static readonly GUIStyle microButton;


        static ExtGUIStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };

            boxBackground = new GUIStyle("boxBackground")
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold,
            };

            miniText = new GUIStyle()
            {
                fontSize = 9,
            };
            miniText.normal.textColor = Color.white;

            miniBox = new GUIStyle()
            {
                fontSize = 8,
            };
            miniBox.normal.background = ExtTexture.GetTexture2D("box");


            box = new GUIStyle("box");

            helpBox = new GUIStyle("helpBox");
            helpBox.normal.background = ExtTexture.GetTexture2D("box");
            helpBox.wordWrap = true;

            microButton = EditorStyles.miniButton;
            microButton.fontSize = 9;
        }
    }
}