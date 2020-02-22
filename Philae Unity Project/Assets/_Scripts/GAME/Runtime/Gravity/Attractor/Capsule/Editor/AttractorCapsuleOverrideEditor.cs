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
        protected new AttractorCapsuleOverride _attractor;

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
            _attractor = (AttractorCapsuleOverride)GetTarget<Attractor>();
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

            this.UpdateEditor();

            ExtCapsule capsule = this.GetPropertie("_capsule").GetValue<ExtCapsule>();
            GravityOverrideCapsule gravityCapsule = ExtGravityOverrideEditor.DrawCapsule(capsule, _attractor.GravityOverride, 0.5f, out bool hasChanged);

            if (hasChanged)
            {
                gravityCapsule.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToCapsule(this.GetPropertie("GravityOverride"), gravityCapsule);
                this.ApplyModification();
            }

        }
    }
}