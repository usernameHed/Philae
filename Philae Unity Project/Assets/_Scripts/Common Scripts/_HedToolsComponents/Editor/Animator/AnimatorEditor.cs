using UnityEngine;
using UnityEditor;
using System.Reflection;
using static UnityEditor.EditorGUILayout;
using hedCommon.extension.editor;

namespace extUnityComponents
{
    [CustomEditor(typeof(Animator))]
    public class AnimatorEditor : DecoratorComponentsEditor
    {
        private Texture _playButton;
        private Texture _pauseButton;
        private bool _isPlaying = true;

        public AnimatorEditor()
                : base(editorTypeName:  BUILT_IN_EDITOR_COMPONENTS.AnimatorInspector,
                      showExtension:    false,
                      tinyEditorName:   "Animator Tool")
        {

        }

        public override void OnCustomEnable()
        {
            _playButton = (Texture)EditorGUIUtility.Load("SceneView/play.png");
            _pauseButton = (Texture)EditorGUIUtility.Load("SceneView/pause.png");
        }

        /// <summary>
        /// get called by the decorator to show a tiny editor
        /// </summary>
        public override void ShowTinyEditorContent()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                if (_isPlaying && GUILayout.Button(new GUIContent(_playButton, "Play Animation"), ExtGUIStyles.commandButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    ExecutePlayButton();
                }
                else if (!_isPlaying && GUILayout.Button(new GUIContent(_pauseButton, "Pause Animation"), ExtGUIStyles.commandButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    ExecutePlayButton();
                }
                GUILayout.Label((_isPlaying) ? "  Play" : "  Pause");
            }
        }

        private void ExecutePlayButton()
        {
            ExtReflection.SetPlayButton(out _isPlaying);
        }
    }
}