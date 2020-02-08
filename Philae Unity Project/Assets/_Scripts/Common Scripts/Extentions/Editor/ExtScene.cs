using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace hedCommon.extension.editor
{
    public static class ExtScene
    {
        private static string _sceneToOpen;

        public static void StartScene(string scene)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            _sceneToOpen = scene;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (_sceneToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(_sceneToOpen);
                //EditorApplication.isPlaying = true;
            }
            _sceneToOpen = null;
        }


        /// <summary>
        /// If there is dirty scene (scene with unsaved changes),
        /// ask to save them.
        /// Return FALSE if we press CANCEL
        /// Return TRUE if we press SAVE, or DON'T SAVE
        /// </summary>
        /// <returns> Return FALSE if we press CANCEL
        /// Return TRUE if we press SAVE, or DON'T SAVE</returns>
        public static bool AskToSaveDirtyScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return (false);
            }
            return (true);
        }

        /// <summary>
        /// Save all openned scene
        /// if hard is true, ensure that every scene are saved
        /// (even if they are not marked as dirty)
        /// </summary>
        /// <param name="hard">if true, mark all openned scene as dirty before saving</param>
        public static void SaveOpenScenes(bool hard)
        {
            EditorSceneManager.MarkAllScenesDirty();
            EditorSceneManager.SaveOpenScenes();
        }
    }
}