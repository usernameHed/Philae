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
    [CustomEditor(typeof(AttractorCapsuleGravityOverride), true)]
    public class AttractorCapsuleGravityOverrideEditor : AttractorEditor
    {
        protected AttractorCapsuleGravityOverride _attractorCapsuleOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorCapsuleGravityOverrideEditor()
            : base()
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorCapsuleOverride = GetTarget<AttractorCapsuleGravityOverride>();
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
            SerializedObject movableCapsule = this.GetPropertie(ExtCapsuleProperty.PROPERTY_MOVABLE_CAPSULE).ToSerializeObject<MovableCapsule>();

            ExtCapsule capsule = movableCapsule.GetPropertie(ExtCapsuleProperty.PROPERTY_EXT_CAPSULE).GetValue<ExtCapsule>();
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