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
        private static List<int> _allGameObjectInFrameWork = new List<int>(500);
        private static List<System.Type> _allNonPersistentTypeComponents = new List<System.Type>()
        {
            typeof(Transform),
            typeof(GameObject),
            typeof(TransformHiddedTools),
            typeof(AnimatorHiddedTools),
            typeof(MeshFilterHiddedTools),
            typeof(RectTransformHiddedTools),
            typeof(RectTransformHiddedTools),
            typeof(RigidBodyAdditionalMonobehaviourSettings),
        };

        private static List<string> _allForbiddenPropertyName = new List<string>()
        {
            "material",
            "materials",
            "mesh"
        };

        private static List<System.Type> _allForbiddenPropertyType = new List<Type>()
        {
            typeof(Transform),
            typeof(GameObject)
        };

        private static bool _needToSetupListOfGameObject = true;
        private const BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        private static EditorChronoWithNoTimeEditor _editorChronoWithNoTimeEditor = new EditorChronoWithNoTimeEditor();
        private const float LIMIT_BETWEEN_TWO_LOADING = 1f;

        /// <summary>
        /// Static constructor subscribes to projectWindowItemOnGUI delegate.
        /// </summary>
        static SymLinksOnHierarchyItemGUI()
		{
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;

            EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
            EditorApplication.hierarchyChanged -= OnHierarchyWindowChanged;

            _needToSetupListOfGameObject = true;
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
                bool isSomethingInsideFrameWork = IsPrefabsAndInSymLink(g) || HasComponentInSymLink(g) || HasSymLinkAssetInsideComponent(g);
                if (isSomethingInsideFrameWork)
                {
                    _allGameObjectInFrameWork.AddIfNotContain(g.GetInstanceID());
                }
            }
        }

        private static bool IsPrefabsAndInSymLink(GameObject g)
        {
            bool isPrefab = ExtPrefabsEditor.IsPrefab(g, out GameObject prefab);
            if (!isPrefab)
            {
                return (false);
            }
            UnityEngine.Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
            string path = AssetDatabase.GetAssetPath(parentObject);
            ExtSymLinks.UpdateSymLinksParent(path);
            FileAttributes attribs = File.GetAttributes(path);
            return (ExtSymLinks.IsAttributeAFileInsideASymLink(path, attribs));
        }

        /// <summary>
        /// return true if this gameObject has at least one component inside a symLink
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private static bool HasComponentInSymLink(GameObject g)
        {
            MonoBehaviour[] monoBehaviours = g.GetComponents<MonoBehaviour>();
            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                MonoBehaviour mono = monoBehaviours[i];
                if (mono != null && mono.hideFlags == HideFlags.None && !_allNonPersistentTypeComponents.Contains(mono.GetType()))
                {
                    MonoScript script = MonoScript.FromMonoBehaviour(mono); // gets script as an asset
                    string path = script.GetPath();
                    ExtSymLinks.UpdateSymLinksParent(path);
                    FileAttributes attribs = File.GetAttributes(path);
                    if (ExtSymLinks.IsAttributeAFileInsideASymLink(path, attribs))
                    {
                        return (true);
                    }
                }
            }
            return (false);
        }

        private static bool HasSymLinkAssetInsideComponent(GameObject g)
        {
            Component[] components = g.GetComponents<Component>();
            if (components == null || components.Length == 0)
            {
                return (false);
            }

            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                
                Type componentType = component.GetType();

                if (_allNonPersistentTypeComponents.Contains(componentType))
                {
                    continue;
                }
                try
                {
                    PropertyInfo[] properties = componentType.GetProperties(_flags);
                    foreach (PropertyInfo property in properties)
                    {

                        if (_allForbiddenPropertyName.Contains(property.Name))
                        {
                            continue;
                        }
                        if (_allForbiddenPropertyType.Contains(property.PropertyType))
                        {
                            continue;
                        }
                        if (property.IsDefined(typeof(ObsoleteAttribute), true))
                        {
                            continue;
                        }
                        UnityEngine.Object propertyValue = property.GetValue(component, null) as UnityEngine.Object;

                        if (propertyValue is UnityEngine.Object && !propertyValue.IsTruelyNull())
                        {
                            string path = propertyValue.GetPath();
                            ExtSymLinks.UpdateSymLinksParent(path);
                            FileAttributes attribs = File.GetAttributes(path);
                            if (ExtSymLinks.IsAttributeAFileInsideASymLink(path, attribs))
                            {
                                return (true);
                            }
                        }
                    }
                    
                    FieldInfo[] fields = componentType.GetFields(_flags);
                    foreach (FieldInfo field in fields)
                    {
                        if (field.IsDefined(typeof(ObsoleteAttribute), true))
                        {
                            continue;
                        }
                        UnityEngine.Object fieldValue = field.GetValue(component) as UnityEngine.Object;

                        if (fieldValue is UnityEngine.Object && !fieldValue.IsTruelyNull())
                        {
                            string path = fieldValue.GetPath();
                            ExtSymLinks.UpdateSymLinksParent(path);
                            FileAttributes attribs = File.GetAttributes(path);
                            if (ExtSymLinks.IsAttributeAFileInsideASymLink(path, attribs))
                            {
                                return (true);
                            }
                        }
                    }
                    
                }
                catch { }
            }
            
            return (false);
        }

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
