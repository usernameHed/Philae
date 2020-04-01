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
    [CustomEditor(typeof(AttractorTriangleGravityOverride), true)]
    public class AttractorTriangleGravityOverrideEditor : AttractorEditor
    {
        protected AttractorTriangleGravityOverride _attractorTriangleOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorTriangleGravityOverrideEditor()
            : base()
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorTriangleOverride = GetTarget<AttractorTriangleGravityOverride>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorTriangleOverride.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();
            SerializedObject movableTriangle = this.GetPropertie(ExtTriangleProperty.PROPERTY_MOVABLE_TRIANGLE).ToSerializeObject<MovableTriangle>();
            
            /*SerializedProperty propertyQuad = movableTriangle.GetPropertie(ExtQuadProperty.PROPERTY_EXT_QUAD);
            ExtQuad quad = propertyQuad.GetValue<ExtQuad>();
            GravityOverrideQuad gravityQuad = ExtGravityOverrideEditor.DrawQuadWithBorders(quad, _attractorTriangleOverride.GravityOverride, Color.red, out bool hasChanged);

            if (hasChanged)
            {
                gravityQuad.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToQuad(this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE), gravityQuad);
                this.ApplyModification();
            }
            */
            _attractorOverrideGeneric.LockEditor();
        }
    }
}