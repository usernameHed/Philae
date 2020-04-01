using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace extUnityComponents
{
    public static class ExtComponentAddition
    {
        public static void AddComponentsExtension<T>(string nameExtension, GameObject[] locationExtension) where T : Component
        {
            for (int i = 0; i < locationExtension.Length; i++)
            {
                AddComponentsExtension<T>(nameExtension, locationExtension[i], out bool justCreated, out bool justDestroyed);
            }
        }

        /// <summary>
        /// return true if the component is present
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameExtension"></param>
        /// <param name="locationExtension"></param>
        /// <returns>true if component is present</returns>
        public static bool AddComponentsExtension<T>(string nameExtension, GameObject locationExtension, out bool justCreated, out bool justDestroyed) where T : Component
        {
            T current = locationExtension.GetComponent<T>();
            justCreated = false;
            justDestroyed = false;

            bool canActiveInternalPropertie = current != null;

            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                GUI.changed = false;
                bool toggle = GUILayout.Toggle(canActiveInternalPropertie, nameExtension, EditorStyles.miniButton);
                if (canActiveInternalPropertie)
                {
                    if (ExtGUIButtons.ButtonAskBefore("x", Color.white, "Warning", "Remove this components ?", EditorStyles.miniButton, GUILayout.Width(20)))
                    {
                        GameObject.DestroyImmediate(current);
                        justDestroyed = true;
                        return (true);
                    }
                }
                else if (GUI.changed)
                {
                    current = locationExtension.GetOrAddComponent<T>();
                    canActiveInternalPropertie = current != null;
                    justCreated = canActiveInternalPropertie;
                }
            }
            return (canActiveInternalPropertie);
        }
    }
}