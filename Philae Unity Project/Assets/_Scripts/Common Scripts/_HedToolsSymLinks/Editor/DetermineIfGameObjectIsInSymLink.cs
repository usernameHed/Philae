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
using UnityEngine.AI;
using UnityEngine.UI;

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
            typeof(NavMeshAgent)
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
            string newPath = AssetDatabase.GetAssetPath(parentObject);
            if (newPath.IsNullOrEmpty())
            {
                return (false);
            }
            DetermineIfAssetIsOrIsInSymLink.UpdateSymLinksParent(newPath, ref allSymLinksAssetPathSaved);
            FileAttributes attribs = File.GetAttributes(newPath);

            bool isInPrefab = DetermineIfAssetIsOrIsInSymLink.IsAttributeAFileInsideASymLink(newPath, attribs, ref allSymLinksAssetPathSaved);
            if (isInPrefab)
            {
                toolTipInfo += "\n - Prefab root";
                path = newPath;
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
                    string newPath = script.GetPath();
                    if (newPath.IsNullOrEmpty())
                    {
                        continue;
                    }
                    DetermineIfAssetIsOrIsInSymLink.UpdateSymLinksParent(newPath, ref allSymLinksAssetPathSaved);
                    FileAttributes attribs = File.GetAttributes(newPath);
                    if (DetermineIfAssetIsOrIsInSymLink.IsAttributeAFileInsideASymLink(newPath, attribs, ref allSymLinksAssetPathSaved))
                    {
                        toolTip += "\n - " + script.name + " c#";
                        hasComponent = true;
                        path = newPath;
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
                        if (IsPropertyAnException(component, property))
                        {
                            continue;
                        }
                        UnityEngine.Object propertyValue = property.GetValue(component, null) as UnityEngine.Object;

                        if (propertyValue is UnityEngine.Object && !propertyValue.IsTruelyNull())
                        {
                            string newPath = propertyValue.GetPath();
                            if (newPath.IsNullOrEmpty())
                            {
                                continue;
                            }
                            DetermineIfAssetIsOrIsInSymLink.UpdateSymLinksParent(newPath, ref allSymLinksAssetPathSaved);
                            FileAttributes attribs = File.GetAttributes(newPath);
                            if (DetermineIfAssetIsOrIsInSymLink.IsAttributeAFileInsideASymLink(newPath, attribs, ref allSymLinksAssetPathSaved))
                            {
                                toolTip += "\n - " + propertyValue.name + " in " + ShortType(component.GetType().ToString());
                                hasAsset = true;
                                path = newPath;
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
                            string newPath = fieldValue.GetPath();
                            if (newPath.IsNullOrEmpty())
                            {
                                continue;
                            }
                            DetermineIfAssetIsOrIsInSymLink.UpdateSymLinksParent(newPath, ref allSymLinksAssetPathSaved);
                            FileAttributes attribs = File.GetAttributes(newPath);
                            if (DetermineIfAssetIsOrIsInSymLink.IsAttributeAFileInsideASymLink(newPath, attribs, ref allSymLinksAssetPathSaved))
                            {
                                toolTip += "\n - " + fieldValue.name + " in " + ShortType(component.GetType().ToString());
                                hasAsset = true;
                                path = newPath;
                            }
                        }
                    }

                }
                catch { }
            }

            return (hasAsset);
        }

        /// <summary>
        /// ignore some property
        /// </summary>
        /// <param name="component"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool IsPropertyAnException(Component component, PropertyInfo property)
        {
            if (_allForbiddenPropertyName.Contains(property.Name))
            {
                return (true);
            }
            if (_allForbiddenPropertyType.Contains(property.PropertyType))
            {
                return (true);
            }
            if (property.IsDefined(typeof(ObsoleteAttribute), true))
            {
                return (true);
            }
            if (component.GetType() == typeof(Animator))
            {
                if (property.Name.Equals("runtimeAnimatorController") || property.Name.Equals("avatar"))
                {
                    return (false);
                }
                return (true);
            }
            return (false);
        }

        private static string ShortType(string fullType)
        {
            return (Path.GetExtension(fullType).Replace(".", ""));
        }


        //end of class
    }
}
