using hedCommon.extension.runtime;
using UnityEngine;

namespace hedCommon.extension.editor
{
    static class ExtGUIStyles
    {
        public static readonly GUIStyle commandButtonStyle;
        public static readonly GUIStyle boxBackground;
        public static readonly GUIStyle helpBox;
        public static readonly GUIStyle box;

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

            box = new GUIStyle("box");

            helpBox = new GUIStyle("helpBox");
            helpBox.normal.background = ExtTexture.GetTexture2D("box");
            helpBox.wordWrap = true;
        }
    }
}