using ExtUnityComponents.transform;
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
    public static class ExtPhilaeZone
    {
        [MenuItem("GameObject/Philae/Zone/Sphere", false, -1)]
        private static void ZoneSphere()
        {
            CreateZone("Zone Sphere");
        }
        [MenuItem("GameObject/Philae/Zone/Cube", false, -1)]
        private static void ZoneCube()
        {
            CreateZone("Zone Cube");
        }
        [MenuItem("GameObject/Philae/Zone/Cylinder", false, -1)]
        private static void ZoneCylinder()
        {
            CreateZone("Zone Cylinder");
        }
        [MenuItem("GameObject/Philae/Zone/Capsule", false, -1)]
        private static void ZoneCapsule()
        {
            CreateZone("Zone Capsule");
        }

        private static void CreateZone(string zoneName)
        {
            GameObject instance = ExtPrefabsEditor.InstantiatePrefabsWithLinkFromAssetPrefabPath("Zone/" + zoneName);

            GravityAttractorZone zone = instance.GetComponent<GravityAttractorZone>();


            ZonesLister.Instance.Init();
        }
        
        // Validate the menu item defined by the function above.
        // The menu item will be disabled if this function returns false.
        [MenuItem("GameObject/Philae/Zone/Sphere", true)]
        [MenuItem("GameObject/Philae/Zone/Cube", true)]
        [MenuItem("GameObject/Philae/Zone/Cylinder", true)]
        [MenuItem("GameObject/Philae/Zone/Capsule", true)]
        private static bool ValidateCreateZone()
        {
            if (Selection.activeTransform == null)
            {
                return (true);
            }
            if (Selection.activeTransform.gameObject.GetExtComponentInParents<GravityAttractorZone>(99, true) != null)
            {
                Debug.LogError("Can't create Zone inside another zone");
                return (false);
            }
            return (true);
        }
    }
}