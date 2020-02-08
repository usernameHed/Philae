using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.splines;
using philae.gravity.zones;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.editor.editorGlobal
{
    public class OrderRightClick
    {
        public void RightClick(ref ExtUtilityEditor.HitSceneView overObjectData)
        {
            GravityAttractorZone zone = overObjectData.objHit.GetExtComponentInParents<GravityAttractorZone>(99, true);
            CustomSplineController splineController = overObjectData.objHit.GetExtComponentInParents<CustomSplineController>(99, true);

            if (zone)
            {
                Selection.activeGameObject = zone.gameObject;
            }
            else if (splineController)
            {
                Selection.activeGameObject = splineController.gameObject;
            }
        }

        public void RightControlClick(ref ExtUtilityEditor.HitSceneView overObjectData)
        {
            Debug.Log("CONTROL + RIGHT clic: " + overObjectData.objHit);
        }

        public void RightAltControl(ref ExtUtilityEditor.HitSceneView overObjectData)
        {
            Debug.Log("CTRL + ALT + RIGHT clic: " + overObjectData.objHit);
        }
    }
}