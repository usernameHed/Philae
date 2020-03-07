using ExtUnityComponents;
using feerik.editor.utils;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
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
    public class AttractorConeSphereBaseOverrideEditor : AttractorEditor
    {
        private const string PROPERTY_CONE = "_cone";
        protected AttractorConeSphereBaseOverride _attractorCone;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorConeSphereBaseOverrideEditor()
            : base(false, "Cone")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorCone = (AttractorConeSphereBaseOverride)GetTarget<Attractor>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorCone.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();

            ExtConeSphereBase capsule = this.GetPropertie(PROPERTY_CONE).GetValue<ExtConeSphereBase>();
            GravityOverrideConeSphereBase gravitySphereBase = ExtGravityOverrideEditor.DrawConeSphereBase(capsule, _attractorCone.GravityOverride, Color.red, out bool hasChanged);

            if (hasChanged)
            {
                gravitySphereBase.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToConeSphereBase(this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE), gravitySphereBase);
                this.ApplyModification();
            }
            _attractorOverrideGeneric.LockEditor();
        }
    }
}