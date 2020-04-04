using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.saveLastSelection;
using hedCommon.sceneWorkflow;
using philae.architecture;
using philae.editor.editorGlobal;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.mixed
{
    public class CustomSceneButtons
    {
        private const int _heightText = 8;
        private const int _widthButtons = 17;
        private const int _heightButtons = 14;

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
            
        }

        public void DisplayScenesButton()
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

            GUILayout.FlexibleSpace();

            using (VerticalScope vertical = new VerticalScope())
            {
                GUILayout.Label("Scenes", ExtGUIStyles.miniText, GUILayout.Height(_heightText));
                using (HorizontalScope horizontal = new HorizontalScope())
                {
                    if (ExtGUIButtons.ButtonImage("pin", "Show where scenes are listed", ExtGUIStyles.microButton, GUILayout.Width(_widthButtons), GUILayout.Height(_heightButtons)))
                    {
                        ExtSelection.PingAndSelect(_refGameAsset);
                    }
                    int currentLoaded = _refGameAsset.LastIndexUsed;
                    for (int i = 0; i < _refGameAsset.CountSceneToLoad; i++)
                    {
                        bool currentLoadedScene = currentLoaded == i;
                        string levelIndex = (i + 1).ToString();
                        currentLoadedScene = GUILayout.Toggle(currentLoadedScene, new GUIContent(levelIndex, "Load all scenes inside the level " + levelIndex), ExtGUIStyles.microButton, GUILayout.Width(_widthButtons), GUILayout.Height(_heightButtons));
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
                            _refGameAsset.LoadScenesByIndex(i, true, OnLoadedScenes, false);
                        }
                    }
                }
            }
            GUILayout.FlexibleSpace();
        }

        private void OnLoadedScenes(SceneAssetLister lister)
        {
            Debug.Log("all scenes are loaded here, we can now initialize our game");
            if (Application.isPlaying)
            {
                Debug.Log("initialize in play mode & build");
                AbstractLinker.Instance?.InitFromPlay();
            }
        }

        //end class
    }
}