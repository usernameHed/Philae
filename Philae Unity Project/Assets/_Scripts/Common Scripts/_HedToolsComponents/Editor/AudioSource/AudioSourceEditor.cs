using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace ExtUnityComponents.transform
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(AudioSource))]
    public class AudioSourceEditor : DecoratorComponentsEditor
    {
        //private PreviewRenderUtility m_PreviewUtility;

        //private Editor previewEditor;

        public AudioSourceEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.AudioSourceInspector)
        {

        }

        /// <summary>
        /// called on enable
        /// </summary>
        public override void OnCustomEnable()
        {

        }

        /// <summary>
        /// need to clean when quitting
        /// </summary>
        public override void OnCustomDisable()
        {

        }

        /// <summary>
        /// This is called at the first OnInspectorGUI()
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
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
            /*
            GUIStyle bgColor = new GUIStyle();
            bgColor.normal.background = EditorGUIUtility.whiteTexture;

            if (previewEditor == null)
                previewEditor = Editor.CreateEditor(GetTarget<AudioSource>());

            previewEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor);
            */
        }
    }
}
