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
        private const string SPHERE = "GameObject/Philae/Zone/Sphere";
        private const string CUBE = "GameObject/Philae/Zone/Cube";
        private const string CYLINDER = "GameObject/Philae/Zone/Cylinder";
        private const string CAPSULE = "GameObject/Philae/Zone/Capsule";
        private const string CAPSULE_HALF = "GameObject/Philae/Zone/Capsule Half";
        private const string CONE_SPHERE_BASE = "GameObject/Philae/Zone/Cone Sphere Base";

        [MenuItem(SPHERE, false, -1)]
        private static void ZoneSphere()
        {
            CreateZone("Zone Sphere");
        }
        [MenuItem(CUBE, false, -1)]
        private static void ZoneCube()
        {
            CreateZone("Zone Cube");
        }
        [MenuItem(CYLINDER, false, -1)]
        private static void ZoneCylinder()
        {
            CreateZone("Zone Cylinder");
        }
        [MenuItem(CAPSULE, false, -1)]
        private static void ZoneCapsule()
        {
            CreateZone("Zone Capsule");
        }
        [MenuItem(CAPSULE_HALF, false, -1)]
        private static void ZoneCapsuleHalf()
        {
            CreateZone("Zone Capsule Half");
        }
        [MenuItem(CONE_SPHERE_BASE, false, -1)]
        private static void ZoneConeSphereBase()
        {
            CreateZone("Zone Cone Sphere Base");
        }

        private static void CreateZone(string zoneName)
        {
            GameObject instance = ExtPrefabsEditor.InstantiatePrefabsWithLinkFromAssetPrefabPath("Zone/" + zoneName);

            GravityAttractorZone zone = instance.GetComponent<GravityAttractorZone>();


            ZonesLister.Instance.Init();
        }
        
        // Validate the menu item defined by the function above.
        // The menu item will be disabled if this function returns false.
        [MenuItem(SPHERE, true)]
        [MenuItem(CUBE, true)]
        [MenuItem(CYLINDER, true)]
        [MenuItem(CAPSULE, true)]
        [MenuItem(CAPSULE_HALF, true)]
        [MenuItem(CONE_SPHERE_BASE, true)]
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