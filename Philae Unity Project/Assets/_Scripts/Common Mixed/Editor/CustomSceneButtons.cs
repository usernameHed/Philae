using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using philae.architecture;
using philae.editor.editorGlobal;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace hedCommon.mixed
{
    public class CustomSceneButtons
    {
        private Texture _wheelTexture;
        private Texture _viperTexture;
        private RefGamesAsset _refGameAsset;

        public void InitTextures()
        {
            //texture has to be in  Assets/Editor Default Resources/
            _wheelTexture = (Texture)EditorGUIUtility.Load("SceneView/wheel.png");
            _viperTexture = (Texture)EditorGUIUtility.Load("SceneView/viper.png");
            _refGameAsset = ExtFind.GetAssetByGenericType<RefGamesAsset>();

        }

        public void OnLeftToolbarGUI()
        {
            if (_refGameAsset == null)
            {
                _refGameAsset = ExtFind.GetAssetByGenericType<RefGamesAsset>();
                if (_refGameAsset == null)
                {
                    Debug.LogWarning("no refGameAsset present in scene...");
                    return;
                }
            }
            //ExtReflection.ClearConsole();

            GUILayout.FlexibleSpace();

            if (ExtGUIButtons.ButtonImage("pin"))
            {
                ExtSelection.PingAndSelect(_refGameAsset);
            }

            GUILayout.Label("", GUILayout.Width(5));

            int currentLoaded = _refGameAsset.LastIndexUsed;
            for (int i = 0; i < _refGameAsset.CountSceneToLoad; i++)
            {
                GUILayout.Label("", GUILayout.Width(5));
                bool currentLoadedScene = currentLoaded == i;
                string levelIndex = (i + 1).ToString();
                currentLoadedScene = GUILayout.Toggle(currentLoadedScene, new GUIContent(levelIndex, "Load all scenes inside the level " + levelIndex), EditorStyles.miniButton, GUILayout.Width(30), GUILayout.Height(25));
                if (currentLoadedScene != (currentLoaded == i))
                {
                    if (!Application.isPlaying)
                    {
                        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            return;
                        }
                    }
                    _refGameAsset.LastIndexUsed = i;
                    CurrentEditorGlobal.LoadSceneByIndex(i);
                }
            }
            
            
            GUILayout.Label("", GUILayout.Width(30));
        }

        public void OnRightToolbarGUI()
        {
            GUILayout.FlexibleSpace();
        }
    }
}