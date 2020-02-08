using ExtUnityComponents;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.line;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor
{
    [CustomEditor(typeof(AttractorLine), true)]
    public class AttractorLineEditor : AttractorEditor
    {
        protected new AttractorLine _attractor;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorLineEditor()
            : base(false, "Line")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            _attractor = (AttractorLine)GetTarget<Attractor>();

            //initialiee basic stuff
            if (!_attractor.Line.IsInit())
            {
                _attractor.Line = new ExtLine(new Vector3(1, 0, 0),
                                                    new Vector3(1, 0, 0));
            }
            
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
            GUILayout.Button("Line !");
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {

            /*
            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(0);
            }
            */
        }
    }
}