using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor.Experimental.SceneManagement;
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif


namespace hedCommon.extension.runtime
{
    public static class ExtPrefabs
    {
#if UNITY_EDITOR
        /// <summary>
        /// Are we in prefabs mode ?
        /// </summary>
        /// <returns></returns>
        public static bool IsInPrefabStage()
        {
#if UNITY_2018_3_OR_NEWER
            var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            return (stage != null);
#else
    return false;
#endif
        }

        public static bool IsEditingInPrefabMode(GameObject gameObject)
        {
            if (EditorUtility.IsPersistent(gameObject))
            {
                // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
                return true;
            }
            else
            {
                // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
                var mainStage = StageUtility.GetMainStageHandle();
                var currentStage = StageUtility.GetStageHandle(gameObject);
                if (currentStage != mainStage)
                {
                    var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                    if (prefabStage != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsAssetOnDisk(GameObject gameObject)
        {
            return PrefabUtility.IsPartOfPrefabAsset(gameObject) || IsEditingInPrefabMode(gameObject);
        }
#endif

#if UNITY_EDITOR
        private static string SaveAsset(string nameMesh, string extention = "asset")
        {
            return string.Format("{0}_{1}.{2}",
                                nameMesh,
                                 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                                 extention);
        }
#endif

#if UNITY_EDITOR

        /// <param name="objectToPrefabs"></param>
        /// <param name="localPath"></param>
        /// <param name="name"></param>
        public static void CreateNewPrefabs(GameObject objectToPrefabs, string _localPathAndName)
        {
            //Check if the Prefab and/or name already exists at the path
            if (AssetDatabase.LoadAssetAtPath(_localPathAndName, typeof(GameObject)))
            {
                //Create dialog to ask if User is sure they want to overwrite existing Prefab
                CreateNewLD(objectToPrefabs, _localPathAndName);
            }
            //If the name doesn't exist, create the new Prefab
            else
            {
                Debug.Log(objectToPrefabs.name + " is not a Prefab, will convert");
                CreateNewLD(objectToPrefabs, _localPathAndName);
            }
        }

        public static void ChangeNamePrefab(string _localPathAndName, string newName)
        {
            if (AssetDatabase.LoadAssetAtPath(_localPathAndName, typeof(GameObject)))
            {
                Debug.Log("change: " + _localPathAndName + " to " + newName);
                AssetDatabase.RenameAsset(_localPathAndName, newName);

            }
            else
            {
                Debug.Log("can't find " + _localPathAndName);
            }
            AssetDatabase.Refresh();
        }

        public static void DeletePrefab(string _localPathAndName)
        {
            if (AssetDatabase.LoadAssetAtPath(_localPathAndName, typeof(GameObject)))
            {
                AssetDatabase.MoveAssetToTrash(_localPathAndName);
            }
        }

        private static void CreateNewLD(GameObject obj, string localPath)
        {
            //Create a new Prefab at the path given
            UnityEngine.Object prefab = PrefabUtility.SaveAsPrefabAsset(obj, localPath);
            //PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }

        public static GameObject GetPrefabsFromLocalPath(string _localPath, string name)
        {
            string localPath = _localPath + name + ".prefab";
            GameObject obj = AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)) as GameObject;
            return (obj);
        }

        /// <summary>
        /// Duplicates the GameObject of a component, returning the component
        /// </summary>
        /// <param name="source">a component being part of the source GameObject</param>
        /// <returns>the component from the cloned GameObject</returns>
        public static T DuplicateGameObject<T>(this Component source, Transform newParent, bool keepPrefabConnection = false) where T : Component
        {
            if (!source || !source.gameObject)
                return null;

            List<Component> cmps = new List<Component>(source.gameObject.GetComponents<Component>());
            int sourceIdx = cmps.IndexOf(source);
            GameObject newGO;
#if UNITY_EDITOR
#if UNITY_2018_2_OR_NEWER
            UnityEngine.Object prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(source.gameObject);
#else
            UnityEngine.Object prefabRoot = PrefabUtility.GetPrefabParent(source.gameObject);
#endif


            if (prefabRoot != null && keepPrefabConnection)
                newGO = PrefabUtility.InstantiatePrefab(prefabRoot) as GameObject;
            else
#endif
                newGO = GameObject.Instantiate<GameObject>(source.gameObject);

            if (newGO)
            {
                newGO.transform.SetParent(newParent, false);
                Component[] newCmps = newGO.GetComponents<Component>();
                return newCmps[sourceIdx] as T;
            }
            else
                return null;
        }

        /// <summary>
        /// Duplicates the GameObject of a component, returning the component
        /// </summary>
        /// <param name="source">a component being part of the source GameObject</param>
        /// <returns>the component from the cloned GameObject</returns>
        public static Component DuplicateGameObject(this Component source, Transform newParent, bool keepPrefabConnection = false)
        {
            if (!source || !source.gameObject || !newParent)
                return null;

            List<Component> cmps = new List<Component>(source.gameObject.GetComponents<Component>());
            int sourceIdx = cmps.IndexOf(source);
            GameObject newGO;
#if UNITY_EDITOR
#if UNITY_2018_2_OR_NEWER
            UnityEngine.Object prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(source.gameObject);
#else
            UnityEngine.Object prefabRoot = PrefabUtility.GetPrefabParent(source.gameObject);
#endif


            if (prefabRoot != null && keepPrefabConnection)
                newGO = PrefabUtility.InstantiatePrefab(prefabRoot) as GameObject;
            else
#endif
                newGO = GameObject.Instantiate<GameObject>(source.gameObject);

            if (newGO)
            {
                newGO.transform.SetParent(newParent, false);
                Component[] newCmps = newGO.GetComponents<Component>();
                return newCmps[sourceIdx];
            }
            else
                return null;
        }
#endif

    }
}