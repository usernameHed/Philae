using ExtUnityComponents;
using feerik.editor.utils;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.movable;
using hedCommon.geometry.shape2d;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.gravityOverride;
using philae.gravity.attractor.line;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace philae.gravity.attractor
{
    [CustomEditor(typeof(AttractorCubeGravityOverride))]
    public class AttractorCubeGravityOverrideEditor : AttractorEditor
    {
        private AttractorCubeGravityOverride _attractorCubeOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorCubeGravityOverrideEditor()
            : base()
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorCubeOverride = GetTarget<AttractorCubeGravityOverride>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorCubeOverride.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();
            SerializedObject movableCube = this.GetPropertie(ExtCubeProperty.PROPERTY_MOVABLE_CUBE).ToSerializeObject<MovableCube>();
            ExtCube cube = movableCube.GetPropertie(ExtCubeProperty.PROPERTY_EXT_CUBE).GetValue<ExtCube>();
            GravityOverrideCube gravityCube = ExtGravityOverrideEditor.DrawCube(cube, _attractorCubeOverride.GravityOverride, Color.red, out bool hasChanged);
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