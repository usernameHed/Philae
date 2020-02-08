using hedCommon.extension.runtime;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System;
using hedCommon.mixed;

namespace philae.architecture
{
    [CreateAssetMenu(fileName = "RefGamesAsset", menuName = "")]
    public class RefGamesAsset : ScriptableObject
    {
        [InlineEditor]
        public List<SceneAssetLister> _listSceneToLoad = new List<SceneAssetLister>();
        public int CountSceneToLoad { get { return (_listSceneToLoad.Count); } }

        public int LastIndexUsed = -1;

        private List<AsyncOperation> _asyncOperations = new List<AsyncOperation>(20);
        public delegate void FuncToCallOnComplete(SceneAssetLister loadedScenes);
        private FuncToCallOnComplete _refFuncToCallOnComplete = null;

        private SceneAssetLister _lastSceneAssetUsed = null;
        private bool _activeAfterLoaded = true;

        /// <summary>
        /// Load a list of scene: ALL scene present in game
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScenesByIndex(int index, bool activeAfterLoaded, FuncToCallOnComplete funcToCallOnComplete, bool hardReload)
        {
            if (_refFuncToCallOnComplete != null && !hardReload)
            {
                throw new System.Exception("can't load twice in a row...");
            }
            LoadSceneFromLister(_listSceneToLoad[index], activeAfterLoaded, funcToCallOnComplete);
        }

        private void LoadSceneFromLister(SceneAssetLister lister, bool activeAfterLoaded, FuncToCallOnComplete funcToCallOnComplete)
        {
            _activeAfterLoaded = activeAfterLoaded;
            _lastSceneAssetUsed = lister;
            _refFuncToCallOnComplete = funcToCallOnComplete;

            if (Application.isPlaying)
            {
                _asyncOperations.Clear();
                Debug.Log("load main scene");
                for (int i = 0; i < lister.SceneToLoad.Count; i++)
                {
                    AsyncOperation asyncOperation;
                    if (i == 0)
                    {
                        asyncOperation = SceneManager.LoadSceneAsync(lister.SceneToLoad[0].ScenePath, LoadSceneMode.Single);
                        asyncOperation.completed += FirstSceneLoaded;
                        asyncOperation.allowSceneActivation = true;
                    }
                    else
                    {
                        asyncOperation = SceneManager.LoadSceneAsync(lister.SceneToLoad[i].ScenePath, LoadSceneMode.Additive);
                        asyncOperation.allowSceneActivation = false;
                        asyncOperation.completed += OnSceneCompleted;
                        _asyncOperations.Add(asyncOperation);
                    }
                }
            }
#if UNITY_EDITOR
            else
            {
                for (int i = 0; i < lister.SceneToLoad.Count; i++)
                {
                    EditorSceneManager.OpenScene(lister.SceneToLoad[i].ScenePath,
                        (i == 0) ? OpenSceneMode.Single : OpenSceneMode.Additive);
                }
                CalledWhenAllSceneAreLoaded();
            }
#endif
        }

        

        private void FirstSceneLoaded(AsyncOperation obj)
        {
            AbstractLinker.Instance.StartCoroutine(WaitForLoading());
        }

        private IEnumerator WaitForLoading()
        {
            yield return new WaitUntil(() => AllAditiveSceneAreReady());
            ActiveAllScenes();
        }

        private bool AllAditiveSceneAreReady()
        {
            for (int i = 0; i < _asyncOperations.Count; i++)
            {
                if (_asyncOperations[i].progress < 0.9f)
                {
                    return (false);
                }
            }
            return (true);
        }

        private void OnSceneCompleted(AsyncOperation obj)
        {
            _asyncOperations.Remove(obj);
            if (_asyncOperations.Count == 0)
            {
                CalledWhenAllSceneAreLoaded();
            }
        }

        private void ActiveAllScenes()
        {
            for (int i = 0; i < _asyncOperations.Count; i++)
            {
                _asyncOperations[i].allowSceneActivation = true;
            }
        }

        public void CalledWhenAllSceneAreLoaded()
        {
            ExtLog.Log("here all scene are loaded", Color.green);

            if (_refFuncToCallOnComplete != null)
            {
                _refFuncToCallOnComplete.Invoke(_lastSceneAssetUsed);
            }
            _refFuncToCallOnComplete = null;
            _lastSceneAssetUsed = null;

            if (Application.isPlaying)
            {
                PhilaeLinker.Instance.InitFromPlay();
            }
        }

        //end of class
    }

    //end of nameSpace
}