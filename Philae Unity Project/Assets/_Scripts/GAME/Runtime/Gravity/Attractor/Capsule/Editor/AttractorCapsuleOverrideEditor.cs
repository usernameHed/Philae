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
    [CustomEditor(typeof(AttractorCapsuleOverride), true)]
    public class AttractorCapsuleOverrideEditor : AttractorEditor
    {
        private const string PROPERTY_CAPSULE = "_capsule";
        protected AttractorCapsuleOverride _attractorCapsuleOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorCapsuleOverrideEditor()
            : base(false, "Capsule")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorCapsuleOverride = (AttractorCapsuleOverride)GetTarget<Attractor>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorCapsuleOverride.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();

            ExtCapsule capsule = this.GetPropertie(PROPERTY_CAPSULE).GetValue<ExtCapsule>();
            GravityOverrideLineTopDown gravityCapsule = ExtGravityOverrideEditor.DrawCapsule(capsule, _attractorCapsuleOverride.GravityOverride, Color.red, out bool hasChanged);

            if (hasChanged)
            {
                gravityCapsule.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToCapsuleOrLine(this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE), gravityCapsule);
                this.ApplyModification();
            }
            _attractorOverrideGeneric.LockEditor();
        }
    }
}