using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace extUnityComponents.transform
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(AudioSource))]
    public class AudioSourceEditor : DecoratorComponentsEditor
    {
        public AudioSourceEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.AudioSourceInspector)
        {

        }

        /// <summary>
        /// This function is called at each OnInspectorGUI
        /// </summary>
        protected override void OnCustomInspectorGUI()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("test");
            }
        }
    }
}
