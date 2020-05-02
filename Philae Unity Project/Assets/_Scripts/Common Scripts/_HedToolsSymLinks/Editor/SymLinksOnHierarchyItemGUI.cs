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
        public struct GameObjectsInfo
        {
            public int Id;
            public bool HasBeeSetupped;
            public bool IsInFrameWork;
        }

        private static Dictionary<int, GameObjectsInfo> _gameObjectsInfo = new Dictionary<int, GameObjectsInfo>(300);


        private static List<int> _allGameObjectInFrameWork = new List<int>(500);

        private static bool _needToSetupListOfGameObject = true;
        private static EditorChronoWithNoTimeEditor _editorChronoWithNoTimeEditor = new EditorChronoWithNoTimeEditor();
        private const float LIMIT_BETWEEN_TWO_LOADING = 1f;

        /// <summary>
        /// Static constructor subscribes to projectWindowItemOnGUI delegate.
        /// </summary>
        static SymLinksOnHierarchyItemGUI()
		{
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;

            EditorApplication.hierarchyChanged -= OnHierarchyWindowChanged;
            EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;

            _needToSetupListOfGameObject = true;
        }

        public static void ResetSymLinksDatas()
        {
            
        }


        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            _needToSetupListOfGameObject = true;
        }

        private static void OnHierarchyWindowChanged()
        {
            _needToSetupListOfGameObject = true;
        }

        private static void UpdateListGameObjects()
        {
            _needToSetupListOfGameObject = false;
            _editorChronoWithNoTimeEditor.StartChrono(LIMIT_BETWEEN_TWO_LOADING);

            // Check here
            GameObject[] allGameObjects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];

            _allGameObjectInFrameWork.Clear();
            foreach (GameObject g in allGameObjects)
            {
                bool isSomethingInsideFrameWork = DetermineIfGameObjectIsInSymLink.IsPrefabsAndInSymLink(g, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved)
                    || DetermineIfGameObjectIsInSymLink.HasComponentInSymLink(g, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved)
                    || DetermineIfGameObjectIsInSymLink.HasSymLinkAssetInsideComponent(g, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved);

                if (isSomethingInsideFrameWork)
                {
                    _allGameObjectInFrameWork.AddIfNotContain(g.GetInstanceID());
                }
            }
        }

        /// <summary>
        /// called at every frameGUI for every gaemObjects inside hierarchy
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="selectionRect"></param>
        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            try
            {
                if (_needToSetupListOfGameObject && _editorChronoWithNoTimeEditor.IsNotRunning())
                {
                    UpdateListGameObjects();
                }

                // place the icoon to the right of the list:
                Rect r = new Rect(selectionRect);
                r.x = r.width - 20;
                r.width = 18;

                if (_allGameObjectInFrameWork.Contains(instanceID))
                {
                    ExtSymLinks.DisplayTinyMarker(selectionRect);
                }
            }
            catch { }
        }
    }
}
