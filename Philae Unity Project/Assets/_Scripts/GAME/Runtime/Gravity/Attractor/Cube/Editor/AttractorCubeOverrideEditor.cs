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
        private const string PROPERTY_CUBE = "_cube";
        private AttractorCubeOverride _attractorCube;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


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
            _attractorCube = (AttractorCubeOverride)GetTarget<Attractor>(); 
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorCube.gameObject.activeInHierarchy)
            {
                return;
            }
            this.UpdateEditor();
            ExtCube cube = this.GetPropertie(PROPERTY_CUBE).GetValue<ExtCube>();
            GravityOverrideCube gravityCube = ExtGravityOverrideEditor.DrawCube(cube, _attractorCube.GravityOverride, Color.red, out bool hasChanged);
            if (hasChanged)
            {
                gravityCube.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToCube(this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE), gravityCube);
                this.ApplyModification();

            }

            _attractorOverrideGeneric.LockEditor();
        }
    }
}