using extUnityComponents;
using extUnityComponents.transform;
using hedCommon.extension.runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace hedCommon.symlinks
{
    public static class DetermineIfGameObjectIsInSymLink
    {
        private const BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

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

        /// <summary>
        /// Determine if a gameObject is a prefab, if it is, determine if it's inside a symLink
        /// </summary>
        /// <param name="g">possible prefabs to test</param>
        /// <returns>true if gameObject IS a prefabs, AND inside a symLink folder</returns>
        public static bool IsPrefabsAndInSymLink(GameObject g, ref List<string> allSymLinksAssetPathSaved, ref string toolTipInfo, ref string path)
        {
            bool isPrefab = ExtPrefabsEditor.IsPrefab(g, out GameObject prefab);
            if (!isPrefab)
            {
                return (false);
            }

            UnityEngine.Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
            path = AssetDatabase.GetAssetPath(parentObject);
            DetermineIfAssetIsOrIsInSymLink.UpdateSymLinksParent(path, ref allSymLinksAssetPathSaved);
            FileAttributes attribs = File.GetAttributes(path);

            bool isInPrefab = DetermineIfAssetIsOrIsInSymLink.IsAttributeAFileInsideASymLink(path, attribs, ref allSymLinksAssetPathSaved);
            if (isInPrefab)
            {
                toolTipInfo += "\n - Prefab root";
            }

            return (isInPrefab);
        }

        /// <summary>
        /// return true if this gameObject has at least one component inside a symLink
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool HasComponentInSymLink(GameObject g, ref List<string> allSymLinksAssetPathSaved, ref string toolTip, ref string path)
        {
            bool hasComponent = false;
            MonoBehaviour[] monoBehaviours = g.GetComponents<MonoBehaviour>();
            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                MonoBehaviour mono = monoBehaviours[i];
                if (mono != null && mono.hideFlags == HideFlags.None && !_allNonPersistentTypeComponents.Contains(mono.GetType()))
                {
                    MonoScript script = MonoScript.FromMonoBehaviour(mono); // gets script as an asset
                    path = script.GetPath();
                    DetermineIfAssetIsOrIsInSymLink.UpdateSymLinksParent(path, ref allSymLinksAssetPathSaved);
                    FileAttributes attribs = File.GetAttributes(path);
                    if (DetermineIfAssetIsOrIsInSymLink.IsAttributeAFileInsideASymLink(path, attribs, ref allSymLinksAssetPathSaved))
                    {
                        toolTip += "\n - " + script.name + " c#";
                        hasComponent = true;
                    }
                }
            }
            return (hasComponent);
        }

        public static bool HasSymLinkAssetInsideComponent(GameObject g, ref List<string> allSymLinksAssetPathSaved, ref string toolTip, ref string path)
        {
            bool hasAsset = false;

            Component[] components = g.GetComponents<Component>();
            if (components == null || components.Length == 0)
            {
                return (false);
            }

            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null)
                {
                    continue;
                }
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
                            path = propertyValue.GetPath();
                            DetermineIfAssetIsOrIsInSymLink.UpdateSymLinksParent(path, ref allSymLinksAssetPathSaved);
                            FileAttributes attribs = File.GetAttributes(path);
                            if (DetermineIfAssetIsOrIsInSymLink.IsAttributeAFileInsideASymLink(path, attribs, ref allSymLinksAssetPathSaved))
                            {
                                toolTip += "\n - " + propertyValue.name + " in " + ShortType(component.GetType().ToString());
                                hasAsset = true;
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
                            path = fieldValue.GetPath();
                            DetermineIfAssetIsOrIsInSymLink.UpdateSymLinksParent(path, ref allSymLinksAssetPathSaved);
                            FileAttributes attribs = File.GetAttributes(path);
                            if (DetermineIfAssetIsOrIsInSymLink.IsAttributeAFileInsideASymLink(path, attribs, ref allSymLinksAssetPathSaved))
                            {
                                toolTip += "\n - " + fieldValue.name + " in " + ShortType(component.GetType().ToString());
                                hasAsset = true;
                            }
                        }
                    }

                }
                catch { }
            }

            return (hasAsset);
        }

        private static string ShortType(string fullType)
        {
            return (Path.GetExtension(fullType).Replace(".", ""));
        }


        //end of class
    }
}
