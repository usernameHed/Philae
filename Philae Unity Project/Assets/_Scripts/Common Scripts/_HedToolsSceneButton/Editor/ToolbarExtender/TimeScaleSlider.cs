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
using static UnityEditor.EditorGUILayout;

namespace hedCommon.toolbarExtent
{
    public class TimeScaleSlider
    {
        private const int _heightText = 8;
        private const int _heightButtons = 15;

        public void Init()
        {
            TimeEditor.timeScale = 1;
        }

        public void DisplaySlider()
        {
            GUILayout.FlexibleSpace();
            using (VerticalScope vertical = new VerticalScope())
            {
                GUILayout.Label("timeScale", ExtGUIStyles.miniText, GUILayout.Height(_heightText));
                float timeScale = EditorGUILayout.Slider("", TimeEditor.timeScale, 0, 1, GUILayout.Width(120), GUILayout.Height(_heightButtons));
                if (timeScale != TimeEditor.timeScale)
                {
                    TimeEditor.timeScale = timeScale;
                }
            }
        }
    }
}