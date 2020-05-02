using UnityEditor;
using UnityEngine;
using System.IO;
using hedCommon.extension.editor;
using System.Collections.Generic;
using hedCommon.extension.runtime;
using extUnityComponents.transform;
using extUnityComponents;
using System;
using System.Reflection;
using hedCommon.time;

namespace hedCommon.symlinks
{
    /// <summary>
    /// An editor utility for easily creating symlinks in your project.
    /// 
    /// Adds a Menu item under `Assets/Create/Folder(Symlink)`, and
    /// draws a small indicator in the Project view for folders that are
    /// symlinks.
    /// </summary>
    [InitializeOnLoad]
    public static class SymLinksOnHierarchyItemGUI
    {
        private const string SYMLINK_TOOLTIP = "In Symlink: ";
        private const int MAX_SETUP_IN_ONE_FRAME = 10;
        private const float TIME_BETWEEN_2_CHUNK_SETUP = 0.1f;

        //Don't use dictionnary ! it crash !!
        private static List<int> _gameObjectsId = new List<int>(300);
        private static List<UnityEngine.Object> _gameObjectsList = new List<UnityEngine.Object>(300);
        private static List<bool> _gameObjectsHasBeenSetup = new List<bool>(300);
        private static List<bool> _gameObjectsIsInFrameWork = new List<bool>(300);
        private static List<string> _toolTipInfo = new List<string>(300);
        private static List<string> _pathSymLinksObjects = new List<string>(300);
        private static bool _needToSetup = false;
        private static int _settupedGameObject = 0;
        private static EditorChronoWithNoTimeEditor _timerBetween2ChunkSetup = new EditorChronoWithNoTimeEditor();


        /// <summary>
        /// Static constructor subscribes to projectWindowItemOnGUI delegate.
        /// </summary>
        static SymLinksOnHierarchyItemGUI()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;

            EditorApplication.hierarchyChanged -= OnHierarchyWindowChanged;
            EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;

            _needToSetup = false;
            _settupedGameObject = 0;
        }

        public static void ResetSymLinksDatas()
        {
            _gameObjectsId.Clear();
            _gameObjectsList.Clear();
            _toolTipInfo.Clear();
            _gameObjectsHasBeenSetup.Clear();
            _gameObjectsIsInFrameWork.Clear();
            _pathSymLinksObjects.Clear();
            _settupedGameObject = 0;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            _needToSetup = true;
        }

        private static void OnHierarchyWindowChanged()
        {
            _needToSetup = true;
        }

        /// <summary>
        /// called at every frameGUI for every gaemObjects inside hierarchy
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="selectionRect"></param>
        private static void OnHierarchyItemGUI(int instanceId, Rect selectionRect)
        {
            try
            {
                if (_needToSetup)
                {
                    ResetHasBeenSetupOfGameObjects();
                    _needToSetup = false;
                }

                bool succes = _gameObjectsId.ContainIndex(instanceId, out int index);
                if (!succes)
                {
                    UnityEngine.Object targetGameObject = EditorUtility.InstanceIDToObject(instanceId);
                    if (targetGameObject == null)
                    {
                        return;
                    }
                    _gameObjectsId.Add(instanceId);
                    _gameObjectsList.Add(targetGameObject);
                    _toolTipInfo.Add(SYMLINK_TOOLTIP);
                    _gameObjectsHasBeenSetup.Add(false);
                    _gameObjectsIsInFrameWork.Add(false);
                    _pathSymLinksObjects.Add("");
                    index = _gameObjectsId.Count - 1;
                    SetupGameObject(index);
                }

                if (!_gameObjectsHasBeenSetup[index])
                {
                    SetupGameObject(index);
                }

                if (_gameObjectsIsInFrameWork[index])
                {
                    DisplayMarker(selectionRect, _toolTipInfo[index], SymLinksColorChoice.ChooseColorFromPath(_pathSymLinksObjects[index]));
                }
            }
            catch (Exception e) { Debug.Log(e); }
        }

        private static void ResetHasBeenSetupOfGameObjects()
        {
            _settupedGameObject = 0;
            for (int i = 0; i < _gameObjectsHasBeenSetup.Count; i++)
            {
                _gameObjectsHasBeenSetup[i] = false;
                _toolTipInfo[i] = SYMLINK_TOOLTIP;
            }
        }

        /// <summary>
        /// From an instance id, setup the info:
        /// HasBennSetup at true
        /// determine if gameObject is inside framework or not !
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="info"></param>
        private static void SetupGameObject(int index)
        {
            if (_settupedGameObject > MAX_SETUP_IN_ONE_FRAME)
            {
                _timerBetween2ChunkSetup.StartChrono(TIME_BETWEEN_2_CHUNK_SETUP);
                _settupedGameObject = 0;
                return;
            }
            if (_timerBetween2ChunkSetup.IsRunning())
            {
                return;
            }

            string toolTip = _toolTipInfo[index];
            string path = "";
            bool prefab = DetermineIfGameObjectIsInSymLink.IsPrefabsAndInSymLink(_gameObjectsList[index] as GameObject, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved, ref toolTip, ref path);
            bool component = DetermineIfGameObjectIsInSymLink.HasComponentInSymLink(_gameObjectsList[index] as GameObject, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved, ref toolTip, ref path);
            bool assets = DetermineIfGameObjectIsInSymLink.HasSymLinkAssetInsideComponent(_gameObjectsList[index] as GameObject, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved, ref toolTip, ref path);
            bool isSomethingInsideFrameWork = prefab || component || assets;

            _pathSymLinksObjects[index] = path;
            _toolTipInfo[index] = toolTip;
            _gameObjectsHasBeenSetup[index] = true;
            _gameObjectsIsInFrameWork[index] = isSomethingInsideFrameWork;
            _settupedGameObject++;
        }

        /// <summary>
        /// display the marker at the given position
        /// </summary>
        /// <param name="selectionRect"></param>
        private static void DisplayMarker(Rect selectionRect, string toolTip, Color color)
        {
            Rect r = new Rect(selectionRect);
            r.x = r.width - 20;
            r.width = 18;
            ExtSymLinks.DisplayTinyMarker(selectionRect, toolTip, color);
        }
    }
}
