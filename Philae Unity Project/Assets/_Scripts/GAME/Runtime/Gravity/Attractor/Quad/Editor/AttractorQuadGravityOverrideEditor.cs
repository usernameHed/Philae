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
    [CustomEditor(typeof(AttractorQuadGravityOverride), true)]
    public class AttractorQuadGravityOverrideEditor : AttractorEditor
    {
        protected AttractorQuadGravityOverride _attractorQuadOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorQuadGravityOverrideEditor()
            : base()
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorQuadOverride = GetTarget<AttractorQuadGravityOverride>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorQuadOverride.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();
            SerializedObject movableQuad = this.GetPropertie(ExtQuadProperty.PROPERTY_MOVABLE_QUAD).ToSerializeObject<MovableQuad>();
            SerializedProperty propertyQuad = movableQuad.GetPropertie(ExtQuadProperty.PROPERTY_EXT_QUAD);
            ExtQuad quad = propertyQuad.GetValue<ExtQuad>();
            GravityOverrideQuad gravityQuad = ExtGravityOverrideEditor.DrawQuadWithBorders(quad, _attractorQuadOverride.GravityOverride, Color.red, out bool hasChanged);

            if (hasChanged)
            {
                gravityQuad.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToQuad(this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE), gravityQuad);
                this.ApplyModification();
            }
            _attractorOverrideGeneric.LockEditor();
        }
    }
}