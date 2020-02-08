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
            CreateZone(ZoneSettingsLocal.Shape.SPHERE);
        }
        [MenuItem("GameObject/Philae/Zone/Cube", false, -1)]
        private static void ZoneCube()
        {
            CreateZone(ZoneSettingsLocal.Shape.CUBE);
        }
        [MenuItem("GameObject/Philae/Zone/Cylinder", false, -1)]
        private static void ZoneCylinder()
        {
            CreateZone(ZoneSettingsLocal.Shape.CYLINDER);
        }
        [MenuItem("GameObject/Philae/Zone/Capsule", false, -1)]
        private static void ZoneCapsule()
        {
            CreateZone(ZoneSettingsLocal.Shape.CAPSULE);
        }

        private static void CreateZone(ZoneSettingsLocal.Shape shape)
        {
            GameObject instance = ExtPrefabsEditor.InstantiatePrefabsFromAssetPrefabPath("Zone/Zone Gravity");

            GravityAttractorZone zone = instance.GetComponent<GravityAttractorZone>();
            zone.GetScalerZoneReference.gameObject.hideFlags = HideFlags.NotEditable;

            TransformHiddedTools zoneTool = zone.GetOrAddComponent<TransformHiddedTools>();
            zoneTool.HideHandle = true;

            TransformHiddedTools hiddedTools = zone.GetScalerZoneReference.GetOrAddComponent<TransformHiddedTools>();
            hiddedTools.ShowName = true;
            hiddedTools.ColorText = Color.yellow;
            hiddedTools.HideHandle = true;

            zone.Init(ZonesLister.Instance);
            zone.ShapeZone = shape;


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