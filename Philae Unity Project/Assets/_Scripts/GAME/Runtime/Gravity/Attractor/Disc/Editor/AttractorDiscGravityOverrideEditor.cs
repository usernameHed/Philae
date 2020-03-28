﻿using ExtUnityComponents;
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
    [CustomEditor(typeof(AttractorDiscGravityOverride), true)]
    public class AttractorDiscGravityOverrideEditor : AttractorEditor
    {
        protected AttractorDiscGravityOverride _attractorDiscOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorDiscGravityOverrideEditor()
            : base()
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorDiscOverride = GetTarget<AttractorDiscGravityOverride>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorDiscOverride.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();
            SerializedObject movableDisc = this.GetPropertie(ExtDiscProperty.PROPERTY_MOVABLE_DISC).ToSerializeObject<MovableDisc>();
            SerializedProperty propertyDisc = movableDisc.GetPropertie(ExtDiscProperty.PROPERTY_EXT_DISC);
            ExtCircle circle = propertyDisc.GetPropertie(ExtDiscProperty.PROPERTY_EXT_CIRCLE).GetValue<ExtCircle>();
            GravityOverrideDisc gravityDisc = ExtGravityOverrideEditor.DrawDisc(circle, _attractorDiscOverride.GravityOverride, new Color(1, 0, 0, 0.5f), allowBottom: circle.AllowBottom, out bool hasChanged);

            if (hasChanged)
            {
                gravityDisc.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToDisc(this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE), gravityDisc);
                this.ApplyModification();
            }
            _attractorOverrideGeneric.LockEditor();
        }
    }
}