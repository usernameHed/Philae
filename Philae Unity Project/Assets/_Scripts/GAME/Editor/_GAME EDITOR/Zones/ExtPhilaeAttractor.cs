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
        private const string SPHERE = "GameObject/Philae/Attractor/Sphere";
        private const string CUBE = "GameObject/Philae/Attractor/Cube/Cube";
        private const string CUBE_OVERRIDE = "GameObject/Philae/Attractor/Cube/Cube With gravity override";
        private const string DISC = "GameObject/Philae/Attractor/Disc/Disc";
        private const string DISC_OVERRIDE = "GameObject/Philae/Attractor/Disc/Disc with gravity override";
        private const string CYLINDER = "GameObject/Philae/Attractor/Cylinder/Cylinder";
        private const string CYLINDER_OVERRIDE = "GameObject/Philae/Attractor/Cylinder/Cylinder with gravity override";
        private const string CAPSULE = "GameObject/Philae/Attractor/Capsule/Capsule";
        private const string CAPSULE_OVERRIDE = "GameObject/Philae/Attractor/Capsule/Capsule with gravity override";
        private const string CAPSULE_HALF = "GameObject/Philae/Attractor/Capsule/Capsule Half";
        private const string CAPSULE_HALF_OVERRIDE = "GameObject/Philae/Attractor/Capsule/Capsule Half with gravity override";
        private const string MESH_CONCAVE = "GameObject/Philae/Attractor/Mesh/Concave Mesh";
        private const string MESH_CONVEXE = "GameObject/Philae/Attractor/Mesh/Convexe Mesh";
        private const string LINE = "GameObject/Philae/Attractor/Line";
        private const string QUAD = "GameObject/Philae/Attractor/Quad/Quad";
        private const string QUAD_OVERRIDE = "GameObject/Philae/Attractor/Quad/Quad with gravity override";
        private const string SPLINE = "GameObject/Philae/Attractor/Spline";
        private const string TRIANGLE = "GameObject/Philae/Attractor/Triangle";
        private const string CONE_SPHERE_BASE = "GameObject/Philae/Attractor/Cone/Cone Sphere Base";

        [MenuItem(SPHERE, false, -1)]
        private static void AttractorSphere()
        {
            CreateAttractor("Sphere");
        }

        [MenuItem(CUBE, false, -1)]
        private static void AttractorCube()
        {
            CreateAttractor("Cube");
        }
        [MenuItem(CUBE_OVERRIDE, false, -1)]
        private static void AttractorCubeOverride()
        {
            CreateAttractor("Cube Override");
        }
        [MenuItem(DISC, false, -1)]
        private static void AttractorDisc()
        {
            CreateAttractor("Disc");
        }
        [MenuItem(DISC_OVERRIDE, false, -1)]
        private static void AttractorDiscOverride()
        {
            CreateAttractor("Disc Override");
        }
        [MenuItem(CYLINDER, false, -1)]
        private static void AttractorCylinder()
        {
            CreateAttractor("Cylinder");
        }
        [MenuItem(CYLINDER_OVERRIDE, false, -1)]
        private static void AttractorCylinderOverride()
        {
            CreateAttractor("Cylinder Override");
        }

        [MenuItem(CAPSULE, false, -1)]
        private static void AttractorCapsule()
        {
            CreateAttractor("Capsule");
        }
        [MenuItem(CAPSULE_OVERRIDE, false, -1)]
        private static void AttractorCapsuleOverride()
        {
            CreateAttractor("Capsule Override");
        }

        [MenuItem(CAPSULE_HALF, false, -1)]
        private static void AttractorCapsuleHalf()
        {
            CreateAttractor("Capsule Half");
        }
        [MenuItem(CAPSULE_HALF_OVERRIDE, false, -1)]
        private static void AttractorCapsuleHalfOverride()
        {
            CreateAttractor("Capsule Half Override");
        }


        [MenuItem(MESH_CONCAVE, false, -1)]
        private static void AttractorConcaveMesh()
        {
            CreateAttractor("Concave Mesh");
        }
        [MenuItem(MESH_CONVEXE, false, -1)]
        private static void AttractorConvexeMesh()
        {
            CreateAttractor("Convexe Mesh");
        }
        [MenuItem(LINE, false, -1)]
        private static void AttractorLine()
        {
            CreateAttractor("Line");
        }
        [MenuItem(QUAD, false, -1)]
        private static void AttractorQuad()
        {
            CreateAttractor("Quad");
        }
        [MenuItem(QUAD_OVERRIDE, false, -1)]
        private static void AttractorQuadOverride()
        {
            CreateAttractor("Quad Override");
        }
        [MenuItem(SPLINE, false, -1)]
        private static void AttractorSpline()
        {
            CreateAttractor("Spline");
        }
        [MenuItem(TRIANGLE, false, -1)]
        private static void AttractorTriangle()
        {
            CreateAttractor("Triangle");
        }
        [MenuItem(CONE_SPHERE_BASE, false, -1)]
        private static void AttractorConeSphereBase()
        {
            CreateAttractor("Cone Sphere Base");
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
        [MenuItem(SPHERE, true)]
        [MenuItem(CUBE, true)]
        [MenuItem(CUBE_OVERRIDE, true)]
        [MenuItem(DISC, true)]
        [MenuItem(DISC_OVERRIDE, true)]
        [MenuItem(CYLINDER, true)]
        [MenuItem(CYLINDER_OVERRIDE, true)]
        [MenuItem(CAPSULE, true)]
        [MenuItem(CAPSULE_OVERRIDE, true)]
        [MenuItem(CAPSULE_HALF, true)]
        [MenuItem(CAPSULE_HALF_OVERRIDE, true)]
        [MenuItem(MESH_CONCAVE, true)]
        [MenuItem(MESH_CONVEXE, true)]
        [MenuItem(LINE, true)]
        [MenuItem(QUAD, true)]
        [MenuItem(QUAD_OVERRIDE, true)]
        [MenuItem(SPLINE, true)]
        [MenuItem(TRIANGLE, true)]
        [MenuItem(CONE_SPHERE_BASE, true)]
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