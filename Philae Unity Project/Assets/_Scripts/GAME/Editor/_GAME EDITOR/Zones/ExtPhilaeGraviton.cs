using extUnityComponents.transform;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using philae.data.gravity;
using philae.gravity.graviton;
using philae.gravity.zones;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.editor.extension.zone
{
    public static class ExtPhilaeGraviton
    {
        [MenuItem("GameObject/Philae/Graviton/Base", false, -1)]
        private static void BaseGraviton()
        {
            CreateGaviton("GRAVITON");
        }
        
        private static void CreateGaviton(string nameGraviton)
        {
            GameObject instance = ExtPrefabsEditor.InstantiatePrefabsWithLinkFromAssetPrefabPath("Graviton/" + nameGraviton);

        }
        
        // Validate the menu item defined by the function above.
        // The menu item will be disabled if this function returns false.
        [MenuItem("GameObject/Philae/Graviton/Base", true)]
        private static bool ValidateCreateZone()
        {
            
            return (true);
        }
    }
}