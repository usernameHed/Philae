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

        //Don't use dictionnary ! it crash !!
        private static List<int> _gameObjectsId = new List<int>(300);
        private static List<UnityEngine.Object> _gameObjectsList = new List<UnityEngine.Object>(300);
        private static List<bool> _gameObjectsHasBeenSetup = new List<bool>(300);
        private static List<bool> _gameObjectsIsInFrameWork = new List<bool>(300);
        private static List<string> _toolTipInfo = new List<string>(300);
        private static bool _needToSetup = false;

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
        }

        public static void ResetSymLinksDatas()
        {
            _gameObjectsId.Clear();
            _gameObjectsList.Clear();
            _toolTipInfo.Clear();
            _gameObjectsHasBeenSetup.Clear();
            _gameObjectsIsInFrameWork.Clear();
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
                    index = _gameObjectsId.Count - 1;
                    SetupGameObject(index);
                }

                if (!_gameObjectsHasBeenSetup[index])
                {
                    SetupGameObject(index);
                }

                if (_gameObjectsIsInFrameWork[index])
                {
                    DisplayMarker(selectionRect, _toolTipInfo[index]);
                }
            }
            catch (Exception e) { Debug.Log(e); }
        }

        private static void ResetHasBeenSetupOfGameObjects()
        {
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
            string toolTip = _toolTipInfo[index];
            bool prefab = DetermineIfGameObjectIsInSymLink.IsPrefabsAndInSymLink(_gameObjectsList[index] as GameObject, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved, ref toolTip);
            bool component = DetermineIfGameObjectIsInSymLink.HasComponentInSymLink(_gameObjectsList[index] as GameObject, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved, ref toolTip);
            bool assets = DetermineIfGameObjectIsInSymLink.HasSymLinkAssetInsideComponent(_gameObjectsList[index] as GameObject, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved, ref toolTip);
            bool isSomethingInsideFrameWork = prefab || component || assets;

            _toolTipInfo[index] = toolTip;

            _gameObjectsHasBeenSetup[index] = true;
            _gameObjectsIsInFrameWork[index] = isSomethingInsideFrameWork;
        }

        /// <summary>
        /// display the marker at the given position
        /// </summary>
        /// <param name="selectionRect"></param>
        private static void DisplayMarker(Rect selectionRect, string toolTip)
        {
            Rect r = new Rect(selectionRect);
            r.x = r.width - 20;
            r.width = 18;
            ExtSymLinks.DisplayTinyMarker(selectionRect, toolTip);
        }
    }
}
