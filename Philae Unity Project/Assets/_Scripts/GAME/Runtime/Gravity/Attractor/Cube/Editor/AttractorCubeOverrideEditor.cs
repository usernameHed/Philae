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
    [CustomEditor(typeof(AttractorCubeOverride), true)]
    public class AttractorCubeOverrideEditor : AttractorEditor
    {
        protected new AttractorCubeOverride _attractor;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorCubeOverrideEditor()
            : base(false, "Cube")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractor = (AttractorCubeOverride)GetTarget<Attractor>(); 
        }

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
            EditorOptions.Instance.ShowGravityOverride = GUILayout.Toggle(EditorOptions.Instance.ShowGravityOverride, "Setup Gravity", EditorStyles.miniButton);

        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (!EditorOptions.Instance.ShowGravityOverride || !_attractor.gameObject.activeInHierarchy)
            {
                return;
            }
            this.UpdateEditor();
            ExtCube cube = this.GetPropertie("_cube").GetValue<ExtCube>();
            GravityOverrideCube gravityCube = ExtGravityOverrideEditor.DrawCube(cube, _attractor.GravityOverride, Color.red, out bool hasChanged);
            if (hasChanged)
            {
                gravityCube.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToCube(this.GetPropertie("GravityOverride"), gravityCube);
                this.ApplyModification();

            }
        }
    }
}