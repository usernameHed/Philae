using hedCommon.extension.runtime;
using hedCommon.tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace extUnityComponents
{
    public static class ExtBuildSettings
    {
        [MenuItem("PERSO/RemoveDeletedScenes")]
        public static void CleanUpDeletedScenes()
        {
            var currentScenes = EditorBuildSettings.scenes;
            var filteredScenes = currentScenes.Where(ebss => File.Exists(ebss.path)).ToArray();
            EditorBuildSettings.scenes = filteredScenes;
        }

        /// <summary>
        /// called when you are building your game
        /// index: the order of execution (if you have multiple PostProcessScene)
        /// </summary>
        [PostProcessScene(0)]
        private static void BuildCallBack()
        {
            EditorOnlyCleaning[] editorOnlyCleanings = ExtFind.GetScripts<EditorOnlyCleaning>();
            for (int i = 0; i < editorOnlyCleanings.Length; i++)
            {
                editorOnlyCleanings[i].ExecuteBuildCleaning();
            }

            List<IEditorOnly> editorHiddenComponents = ExtFind.GetInterfaces<IEditorOnly>();

            if (!Application.isPlaying)
            {
                for (int i = 0; i < editorHiddenComponents.Count; i++)
                {
                    GameObject.DestroyImmediate(editorHiddenComponents[i].GetReference());
                }
            }

            List<ICallOnBuild> callOnBuild = ExtFind.GetInterfaces<ICallOnBuild>();
            if (!Application.isPlaying)
            {
                //IEditorOnly[] editorHiddenComponents = ExtFind.GetScripts<IEditorOnly>();
                for (int i = 0; i < callOnBuild.Count; i++)
                {
                    callOnBuild[i].ExecuteOnBuild();
                }
            }
        }
    }
}