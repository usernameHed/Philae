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
    [CustomEditor(typeof(AttractorCapsuleHalfGravityOverride), true)]
    public class AttractorCapsuleHalfGravityOverrideEditor : AttractorEditor
    {
        protected AttractorCapsuleHalfGravityOverride _attractorCapsuleHalfOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorCapsuleHalfGravityOverrideEditor()
            : base()
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorCapsuleHalfOverride = GetTarget<AttractorCapsuleHalfGravityOverride>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorCapsuleHalfOverride.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();
            SerializedObject movableCapsuleHalf = this.GetPropertie(ExtCapsuleProperty.PROPERTY_MOVABLE_CAPSULE_HALF).ToSerializeObject<MovableCapsuleHalf>();

            ExtHalfCapsule capsuleHalf = movableCapsuleHalf.GetPropertie(ExtCapsuleProperty.PROPERTY_EXT_CAPSULE_HALF).GetValue<ExtHalfCapsule>();
            GravityOverrideLineTopDown gravityCapsule = ExtGravityOverrideEditor.DrawCapsuleHalf(capsuleHalf, _attractorCapsuleHalfOverride.GravityOverride, Color.red, out bool hasChanged);

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