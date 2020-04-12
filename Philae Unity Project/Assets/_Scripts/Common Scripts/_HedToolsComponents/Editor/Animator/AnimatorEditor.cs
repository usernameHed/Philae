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
        private bool _isPlaying = true;

        public AnimatorEditor()
                : base(editorTypeName:  BUILT_IN_EDITOR_COMPONENTS.AnimatorInspector,
                      showExtension:    false,
                      tinyEditorName:   "Animator Tool")
        {

        }

        /// <summary>
        /// get called by the decorator to show a tiny editor
        /// </summary>
        public override void ShowTinyEditorContent()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                if (_isPlaying && GUILayout.Button("Play", ExtGUIStyles.microButton))
                {
                    ExecutePlayButton();
                }
                else if (!_isPlaying && GUILayout.Button("Pause", ExtGUIStyles.microButton))
                {
                    ExecutePlayButton();
                }
            }
        }

        private void ExecutePlayButton()
        {
            ExtReflection.SetPlayButton(out _isPlaying);
        }
    }
}