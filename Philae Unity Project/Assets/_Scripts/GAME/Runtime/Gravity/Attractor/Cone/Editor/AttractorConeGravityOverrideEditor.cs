using extUnityComponents;
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
    [CustomEditor(typeof(AttractorConeSphereBaseOverride), true)]
    public class AttractorConeGravityOverrideEditor : AttractorEditor
    {
        protected AttractorConeSphereBaseOverride _attractorConeOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorConeGravityOverrideEditor()
            : base()
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorConeOverride = GetTarget<AttractorConeSphereBaseOverride>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorConeOverride.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();
            SerializedObject movableCone = this.GetPropertie(ExtConeProperty.PROPERTY_MOVABLE_CONE).ToSerializeObject<MovableConeSphereBase>();

            ExtConeSphereBase cone = movableCone.GetPropertie(ExtConeProperty.PROPERTY_EXT_CONE).GetValue<ExtConeSphereBase>();
            GravityOverrideConeSphereBase gravityCone = ExtGravityOverrideEditor.DrawConeSphereBase(cone, _attractorConeOverride.GravityOverride, Color.red, out bool hasChanged);

            if (hasChanged)
            {
                gravityCone.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToConeSphereBase(this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE), gravityCone);
                this.ApplyModification();
            }
            _attractorOverrideGeneric.LockEditor();
        }
    }
}