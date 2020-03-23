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
    [CustomEditor(typeof(AttractorCylinderGravityOverride), true)]
    public class AttractorCylinderGravityOverrideEditor : AttractorEditor
    {
        protected AttractorCylinderGravityOverride _attractorCylinderOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorCylinderGravityOverrideEditor()
            : base()
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorCylinderOverride = GetTarget<AttractorCylinderGravityOverride>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorCylinderOverride.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();
            SerializedObject movableCylinder = this.GetPropertie(ExtCylinderProperty.PROPERTY_MOVABLE_CYLINDER).ToSerializeObject<MovableCylinder>();

            SerializedProperty propertyCylinder = movableCylinder.GetPropertie(ExtCylinderProperty.PROPERTY_EXT_CYLINDER);

            ExtCylinder cylinder = propertyCylinder.GetValue<ExtCylinder>();
            ExtCircle circle1 = propertyCylinder.GetPropertie(ExtCylinderProperty.PROPERTY_CIRCLE_1).GetValue<ExtCircle>();
            ExtCircle circle2 = propertyCylinder.GetPropertie(ExtCylinderProperty.PROPERTY_CIRCLE_2).GetValue<ExtCircle>();


            GravityOverrideCylinder gravityCylinder = ExtGravityOverrideEditor.DrawCylinder(cylinder, circle1, circle2, _attractorCylinderOverride.GravityOverride, Color.red, out bool hasChanged);

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