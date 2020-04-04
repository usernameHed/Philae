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

namespace hedCommon.sceneWorkflow
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
        private bool _isActivatingScene = false;

        /// <summary>
        /// Load a list of scene: ALL scene present in game
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScenesByIndex(int index, bool activeAfterLoaded, FuncToCallOnComplete funcToCallOnComplete, bool hardReload)
        {
            if (_isActivatingScene && !hardReload)
            {
                throw new System.Exception("can't load twice in a row...");
            }
            LoadSceneFromLister(_listSceneToLoad[index], activeAfterLoaded, funcToCallOnComplete);
        }

        private void LoadSceneFromLister(SceneAssetLister lister, bool activeAfterLoaded, FuncToCallOnComplete funcToCallOnComplete)
        {
            UnloadCurrentLoadingScenes();

            _isActivatingScene = true;
            _activeAfterLoaded = activeAfterLoaded;
            _lastSceneAssetUsed = lister;
            _refFuncToCallOnComplete = funcToCallOnComplete;

            if (Application.isPlaying)
            {
                _asyncOperations.Clear();
                for (int i = 0; i < _lastSceneAssetUsed.SceneToLoad.Count; i++)
                {
                    _asyncOperations.Add(null);
                }

                for (int i = 0; i < _lastSceneAssetUsed.SceneToLoad.Count; i++)
                {
                    if (i == 0)
                    {
                        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_lastSceneAssetUsed.SceneToLoad[0].ScenePath, LoadSceneMode.Single);
                        asyncOperation.allowSceneActivation = true;
                        asyncOperation.completed += FirstSceneLoaded;
                        _asyncOperations[i] = asyncOperation;
                    }
                    else
                    {
                        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_lastSceneAssetUsed.SceneToLoad[i].ScenePath, LoadSceneMode.Additive);
                        asyncOperation.allowSceneActivation = false;
                        asyncOperation.completed += OnSecondarySceneCompleted;
                        _asyncOperations[i] = asyncOperation;
                    }
                }
            }
#if UNITY_EDITOR
            else
            {
                for (int i = 0; i < _lastSceneAssetUsed.SceneToLoad.Count; i++)
                {
                    EditorSceneManager.OpenScene(_lastSceneAssetUsed.SceneToLoad[i].ScenePath,
                        (i == 0) ? OpenSceneMode.Single : OpenSceneMode.Additive);
                }
                CalledWhenAllSceneAreLoaded();
            }
#endif
        }

        private void UnloadCurrentLoadingScenes()
        {
            if (_lastSceneAssetUsed == null)
            {
                return;
            }
            for (int i = 0; i < _lastSceneAssetUsed.SceneToLoad.Count; i++)
            {
                SceneManager.UnloadSceneAsync(_lastSceneAssetUsed.SceneToLoad[i].ScenePath);
            }
        }
        
        /// <summary>
        /// called when ONE scene is loaded (we need to wait for all others scene...)
        /// </summary>
        /// <param name="obj"></param>
        private void FirstSceneLoaded(AsyncOperation obj)
        {
            _asyncOperations.Remove(obj);
            if (_asyncOperations.Count == 0)
            {
                CalledWhenAllSceneAreLoaded();
            }
            else
            {
                ExtCoroutineWithoutMonoBehaviour.StartUniqueCoroutine(WaitForLoading());
            }
        }

        
        private IEnumerator WaitForLoading()
        {
            yield return new WaitUntil(() => AllAditiveSceneAreReady());
            ActiveAllScenes();
        }

        
        /// <summary>
        /// test if ALL scenes are loaded
        /// </summary>
        /// <returns></returns>
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
        
        private void ActiveAllScenes()
        {
            for (int i = 0; i < _asyncOperations.Count; i++)
            {
                _asyncOperations[i].allowSceneActivation = true;
            }
        }

        private void OnSecondarySceneCompleted(AsyncOperation obj)
        {
            _asyncOperations.Remove(obj);
            if (_asyncOperations.Count == 0)
            {
                ExtCoroutineWithoutMonoBehaviour.CleanUp();
                CalledWhenAllSceneAreLoaded();
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
            _isActivatingScene = false;
        }

        //end of class
    }

    //end of nameSpace
}