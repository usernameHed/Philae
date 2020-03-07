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
    [CustomEditor(typeof(AttractorCylinderOverride), true)]
    public class AttractorCylinderOverrideEditor : AttractorEditor
    {
        private const string PROPERTY_CYLINDER = "_cylinder";
        private const string PROPERTY_CIRCLE_1 = "_circle1";
        private const string PROPERTY_CIRCLE_2 = "_circle2";
        private AttractorCylinderOverride _attractorCylinder;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorCylinderOverrideEditor()
            : base(false, "Cylinder")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorCylinder = (AttractorCylinderOverride)GetTarget<Attractor>(); 
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorCylinder.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();
            SerializedProperty propertyCylinder = this.GetPropertie(PROPERTY_CYLINDER);
            ExtCircle circle1 = propertyCylinder.GetPropertie(PROPERTY_CIRCLE_1).GetValue<ExtCircle>();
            ExtCircle circle2 = propertyCylinder.GetPropertie(PROPERTY_CIRCLE_2).GetValue<ExtCircle>();

            ExtCylinder cylinder = propertyCylinder.GetValue<ExtCylinder>();

            GravityOverrideCylinder gravityCylinder = ExtGravityOverrideEditor.DrawCylinder(cylinder, circle1, circle2, _attractorCylinder.GravityOverride, Color.red, out bool hasChanged);

            if (hasChanged)
            {
                gravityCylinder.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToCylinder(this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE), gravityCylinder);
                this.ApplyModification();
            }
            _attractorOverrideGeneric.LockEditor();
        }
    }
}