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
    [CustomEditor(typeof(AttractorLineGravityOverride))]
    public class AttractorLineOverrideEditor : AttractorEditor
    {
        private AttractorLineGravityOverride _attractorLineOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorLineOverrideEditor()
            : base()
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorLineOverride = GetTarget<AttractorLineGravityOverride>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorLineOverride.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();

            SerializedObject movablePolyLine = this.GetPropertie(ExtLineProperty.PROPERTY_MOVABLE_LINE).ToSerializeObject<MovableLine>();
            ExtLine3d line = movablePolyLine.GetPropertie(ExtLineProperty.PROPERTY_EXT_LINE_3D).GetValue<ExtLine3d>();
            GravityOverrideLineTopDown gravityLine = ExtGravityOverrideEditor.DrawLine3d(line, _attractorLineOverride.GravityOverride, Color.red, out bool hasChanged);

            if (hasChanged)
            {
                gravityLine.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToCapsuleOrLine(this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE), gravityLine);
                this.ApplyModification();
            }

            _attractorOverrideGeneric.LockEditor();
        }

        
    }
}