using hedCommon.editor.editorWindow;
using hedCommon.extension.editor;
using hedCommon.extension.editor.editorWindow;
using hedCommon.extension.runtime;
using hedCommon.time;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace hedCommon.toolbarExtent
{
    public class TimeScaleSlider
    {
        public void DisplaySlider()
        {
            TimeEditor.timeScale = EditorGUILayout.Slider("", TimeEditor.timeScale, 0, 2, GUILayout.Width(120));
        }
    }
}