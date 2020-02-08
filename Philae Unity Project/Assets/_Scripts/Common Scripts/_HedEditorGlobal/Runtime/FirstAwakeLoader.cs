using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using hedCommon.mixed;
using System.Collections;

namespace hedCommon.editorGlobal
{
    /// <summary>
    /// Exexute a function when we leave / enter the play mode
    /// </summary>
    [ExecuteInEditMode, TypeInfoBox("Execute action when leaving / entering play mode")]
    public class FirstAwakeLoader : MonoBehaviour
    {
        [FoldoutGroup("Object")]
        public AbstractLinker AbstractLinker;

        private const string KEY_LOAD_EDITOR_AND_PLAY = "KEY_LOAD_EDITOR_AND_PLAY";

        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (EditorPrefs.GetInt(KEY_LOAD_EDITOR_AND_PLAY) == 1)
                {
                    EditorPrefs.SetInt(KEY_LOAD_EDITOR_AND_PLAY, 0);
                    ExecuteOutOfPlayMode(true);
                }
                else
                {
                    ExecuteOutOfPlayMode(false);
                }
            }
            else
            {
#endif
                //ExecuteInPlayMode();
#if UNITY_EDITOR
            }
#endif
        }

        /// <summary>
        /// called at awake of edit mode
        /// </summary>
        private void ExecuteOutOfPlayMode(bool fromPlay)
        {
            //Debug.Log("first awake in edit mode");
            AbstractLinker.InitFromEditor(fromPlay);
        }

        /// <summary>
        /// called at awake of play mode
        /// </summary>
        private void ExecuteInPlayMode()
        {
            Debug.Log("first awake in play mode");
            AbstractLinker.InitFromPlay();
        }

#if UNITY_EDITOR
        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                EditorPrefs.SetInt(KEY_LOAD_EDITOR_AND_PLAY, 1);
            }
        }
#endif
    }
}