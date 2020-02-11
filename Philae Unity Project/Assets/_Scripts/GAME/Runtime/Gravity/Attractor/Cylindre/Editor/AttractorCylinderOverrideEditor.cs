using ExtUnityComponents;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.gravityOverride;
using philae.gravity.attractor.line;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace philae.gravity.attractor
{
    [CustomEditor(typeof(AttractorCylinderOverride), true)]
    public class AttractorCylinderOverrideEditor : AttractorEditor
    {
        protected new AttractorCylinderOverride _attractor;

        public Rect MyRect;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorCylinderOverrideEditor()
            : base(false, "Cylinder")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractor = (AttractorCylinderOverride)GetTarget<Attractor>(); 
        }

        /*
        // This function is called for each instance of "spawnPoint" in the scene. 
        // Make sure to pass the correct class in the first argument. In this case ItemSpawnPoint
        // Make sure it is a "static" function
        // name it whatever you want
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        public static void DrawHandles(AttractorLine spawnPoint, GizmoType gizmoType)
        {
            bool selected = gizmoType == (GizmoType.Active | GizmoType.InSelectionHierarchy | GizmoType.Selected);

            GUIStyle style = new GUIStyle(); // This is optional
            style.normal.textColor = Color.yellow;
            Handles.Label(spawnPoint.transform.position, spawnPoint.name, style); // you can remove the "style" if you don't want it
            Debug.DrawLine(spawnPoint.transform.position + spawnPoint.Line.A, spawnPoint.transform.position + spawnPoint.Line.B, Color.blue);
        }
        */

        /// <summary>
        /// this function is called on the first OnSceneGUI()
        /// usefull to initialize scene GUI
        /// </summary>
        /// <param name='sceneview'>current drawing scene view</param>
        protected override void InitOnFirstOnSceneGUI(SceneView sceneview)
        {
            //initialise scene GUI
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            EditorOptions.ShowGravityOverride = GUILayout.Toggle(EditorOptions.ShowGravityOverride, "Setup Gravity", EditorStyles.miniButton);

        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (!EditorOptions.ShowGravityOverride)
            {
                return;
            }

            ExtCircle circle1 = this.GetPropertie("_cylinder").GetPropertie("_circle1").GetValue<ExtCircle>();
            ExtCircle circle2 = this.GetPropertie("_cylinder").GetPropertie("_circle2").GetValue<ExtCircle>();

            ExtCylinder cylinder = this.GetPropertie("_cylinder").GetValue<ExtCylinder>();

            _attractor.GravityOverride = ExtGravityOverrideEditor.DrawCylinder(cylinder, _attractor.GravityOverride, out bool hasChanged);
            //_attractor.GravityOverride.Disc1 = ExtGravityOverrideEditor.DrawDisc(circle1, _attractor.GravityOverride.Disc1, _attractor.Rotation, out bool hasChanged);
            //_attractor.GravityOverride.Disc2 = ExtGravityOverrideEditor.DrawDisc(circle2, _attractor.GravityOverride.Disc2, _attractor.Rotation, out hasChanged);


            /*
            Vector3[] arrowHead = new Vector3[3];
            Vector3[] arrowLine = new Vector3[2];
            Vector3 start = circle1.Point;
            Vector3 end = circle2.Point;

            Vector3 forward = (end - start).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            float size = HandleUtility.GetHandleSize(end);
            float width = size * 0.1f;
            float height = size * 0.3f;

            arrowHead[0] = end;
            arrowHead[1] = end - forward * height + right * width;
            arrowHead[2] = end - forward * height - right * width;

            arrowLine[0] = start;
            arrowLine[1] = end - forward * height;

            Handles.color = Color.red;
            Handles.DrawAAPolyLine(arrowLine);
            Handles.DrawAAConvexPolygon(arrowHead);
            */
        }
    }
}