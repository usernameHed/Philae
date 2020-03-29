using hedCommon.extension.editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.procedural
{
    public class ExtProceduralShape : MonoBehaviour
    {
        private const string PLANE = "GameObject/3D Procedural Object/Plane";
        private const string SPHERE = "GameObject/3D Procedural Object/Sphere";
        private const string CONE = "GameObject/3D Procedural Object/Cone";
        private const string CYLINDER = "GameObject/3D Procedural Object/Cylinder";
        private const string CUBE = "GameObject/3D Procedural Object/Cube";
        private const string TORUS = "GameObject/3D Procedural Object/Torus";

        [MenuItem(PLANE, false, -1)]
        private static void GeneratePlane()
        {
            GenerateProceduralShape<ProceduralPlane>("Plane");
        }
        [MenuItem(SPHERE, false, -1)]
        private static void GenerateSphere()
        {
            GenerateProceduralShape<ProceduralSphere>("Sphere");
        }
        [MenuItem(CONE, false, -1)]
        private static void GenerateCone()
        {
            GenerateProceduralShape<ProceduralCone>("Cone");
        }
        [MenuItem(CYLINDER, false, -1)]
        private static void GenerateCylinder()
        {
            GenerateProceduralShape<ProceduralCylinder>("Cylinder");
        }
        [MenuItem(CUBE, false, -1)]
        private static void GenerateCube()
        {
            GenerateProceduralShape<ProceduralCube>("Cube");
        }
        [MenuItem(TORUS, false, -1)]
        private static void GenerateTorus()
        {
            GenerateProceduralShape<ProceduralTorus>("Torus");
        }

        private static void GenerateProceduralShape<T>(string nameShape) where T : ProceduralShape
        {
            GameObject plane = new GameObject(nameShape);
            plane.transform.SetParent(Selection.activeTransform);
            if (Selection.activeTransform != null)
            {
                plane.transform.localPosition = Vector3.zero;
            }
            Selection.activeGameObject = plane;
            ExtSceneView.PlaceGameObjectInFrontOfSceneView(plane);
            GameObjectUtility.EnsureUniqueNameForSibling(plane);

            T proceduralPlane = plane.AddComponent<T>();
            GenerateEditor generateEditor = Editor.CreateEditor(proceduralPlane) as GenerateEditor;
            generateEditor.Construct();
        }
    }
}