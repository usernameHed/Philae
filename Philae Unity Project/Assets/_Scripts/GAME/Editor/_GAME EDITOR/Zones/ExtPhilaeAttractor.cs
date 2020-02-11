using ExtUnityComponents.transform;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using philae.gravity.attractor;
using philae.gravity.attractor.logic;
using philae.gravity.zones;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.editor.extension.attractor
{
    public static class ExtPhilaeAttractor
    {
        [MenuItem("GameObject/Philae/Attractor/Sphere", false, -1)]
        private static void AttractorSphere()
        {
            CreateAttractor("Sphere");
        }

        [MenuItem("GameObject/Philae/Attractor/Cube", false, -1)]
        private static void AttractorCube()
        {
            CreateAttractor("Cube");
        }
        [MenuItem("GameObject/Philae/Attractor/Cylinder", false, -1)]
        private static void AttractorCylinder()
        {
            CreateAttractor("Cylinder");
        }

        [MenuItem("GameObject/Philae/Attractor/Cylinder Override", false, -1)]
        private static void AttractorCylinderOverride()
        {
            CreateAttractor("Cylinder Override");
        }
        [MenuItem("GameObject/Philae/Attractor/Capsule", false, -1)]
        private static void AttractorCapsule()
        {
            CreateAttractor("Capsule");
        }

        [MenuItem("GameObject/Philae/Attractor/Concave Mesh", false, -1)]
        private static void AttractorConcaveMesh()
        {
            CreateAttractor("Concave Mesh");
        }
        [MenuItem("GameObject/Philae/Attractor/Convexe Mesh", false, -1)]
        private static void AttractorConvexeMesh()
        {
            CreateAttractor("Convexe Mesh");
        }
        [MenuItem("GameObject/Philae/Attractor/Line", false, -1)]
        private static void AttractorLine()
        {
            CreateAttractor("Line");
        }
        [MenuItem("GameObject/Philae/Attractor/Quad", false, -1)]
        private static void AttractorQuad()
        {
            CreateAttractor("Quad");
        }
        [MenuItem("GameObject/Philae/Attractor/Spline", false, -1)]
        private static void AttractorSpline()
        {
            CreateAttractor("Spline");
        }
        [MenuItem("GameObject/Philae/Attractor/Triangle", false, -1)]
        private static void AttractorTriangle()
        {
            CreateAttractor("Triangle");
        }

        private static void CreateAttractor(string nameAttractor)
        {
            GameObject instanceShere = ExtPrefabsEditor.InstantiatePrefabsWithLinkFromAssetPrefabPath("Attractor/" + nameAttractor);

            Attractor attractor = instanceShere.GetComponent<Attractor>();
            List<AttractorListerLogic> lister = attractor.AutomaticlySetupZone();
            attractor.InitOnCreation(lister);
        }

        // Validate the menu item defined by the function above.
        // The menu item will be disabled if this function returns false.
        [MenuItem("GameObject/Philae/Attractor/Sphere", true)]
        [MenuItem("GameObject/Philae/Attractor/Cube", true)]
        [MenuItem("GameObject/Philae/Attractor/Cylinder", true)]
        [MenuItem("GameObject/Philae/Attractor/Cylinder Override", true)]
        [MenuItem("GameObject/Philae/Attractor/Capsule", true)]
        [MenuItem("GameObject/Philae/Attractor/Concave Mesh", true)]
        [MenuItem("GameObject/Philae/Attractor/Concave Mesh", true)]
        [MenuItem("GameObject/Philae/Attractor/Line", true)]
        [MenuItem("GameObject/Philae/Attractor/Quad", true)]
        [MenuItem("GameObject/Philae/Attractor/Spline", true)]
        [MenuItem("GameObject/Philae/Attractor/Triangle", true)]
        private static bool ValidateCreateZone()
        {            
            if (Selection.activeTransform == null)
            {
                return (true);
            }
            
            if (Selection.activeTransform.gameObject.GetExtComponentInParents<Attractor>(99, true) != null)
            {
                throw new System.Exception("Can't create Attractor inside another Attractor");
            }
            /*
            if (Selection.activeTransform.gameObject.GetExtComponentInParents<AttractorListerLogic>(99, true) == null)
            {
                throw new System.Exception("Can't create Attractor outside a zone !");
            }
            */
            return (true);
        }
    }
}