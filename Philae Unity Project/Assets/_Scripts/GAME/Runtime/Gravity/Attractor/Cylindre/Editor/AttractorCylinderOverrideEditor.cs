﻿using ExtUnityComponents;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.gravityOverride;
using philae.gravity.attractor.line;
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
        protected new AttractorCylinderOverride _attractor;

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
            _attractor = (AttractorCylinderOverride)GetTarget<Attractor>(); 
        }

        /// <summary>
        /// this function is called on the first OnSceneGUI()
        /// usefull to initialize scene GUI
        /// </summary>
        /// <param name='sceneview'>current drawing scene view</param>
        protected override void InitOnFirstOnSceneGUI(SceneView sceneview)
        {
            //initialise scene GUI
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            EditorOptions.ShowGravityOverride = GUILayout.Toggle(EditorOptions.ShowGravityOverride, "Setup Gravity", EditorStyles.miniButton);

        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (!EditorOptions.ShowGravityOverride || !_attractor.gameObject.activeInHierarchy)
            {
                return;
            }

            ExtCircle circle1 = this.GetPropertie("_cylinder").GetPropertie("_circle1").GetValue<ExtCircle>();
            ExtCircle circle2 = this.GetPropertie("_cylinder").GetPropertie("_circle2").GetValue<ExtCircle>();

            ExtCylinder cylinder = this.GetPropertie("_cylinder").GetValue<ExtCylinder>();

            GravityOverrideCylinder gravityCylinder = ExtGravityOverrideEditor.DrawCylinder(cylinder, circle1, circle2, _attractor.GravityOverride, out bool hasChanged);
            if (hasChanged)
            {
                _attractor.GravityOverride = gravityCylinder;
                _attractor.GravityOverride.SetupGravity();
                EditorUtility.SetDirty(_attractor);
            }
        }
    }
}