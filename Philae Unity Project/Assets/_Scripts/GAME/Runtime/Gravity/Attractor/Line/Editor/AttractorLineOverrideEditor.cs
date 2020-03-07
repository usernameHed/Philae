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
    [CustomEditor(typeof(AttractorLineOverride))]
    public class AttractorLineOverrideEditor : AttractorLineEditor
    {
        private const string PROPERTY_GRAVITY_OVERRIDE = "GravityOverride";
        protected AttractorLineOverride _attractorLineOverride;

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
            _attractorLineOverride = (AttractorLineOverride)GetTarget<Attractor>();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            EditorOptions.Instance.ShowGravityOverride = GUILayout.Toggle(EditorOptions.Instance.ShowGravityOverride, "Setup Gravity", EditorStyles.miniButton);
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!EditorOptions.Instance.ShowGravityOverride || !_attractorLineOverride.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();

            ExtLine3d line = this.GetPropertie(PROPERTY_EXT_LINE_3D).GetValue<ExtLine3d>();
            GravityOverrideLineTopDown gravityLine = ExtGravityOverrideEditor.DrawLine3d(line, _attractorLineOverride.GravityOverride, Color.red, out bool hasChanged);

            if (hasChanged)
            {
                gravityLine.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToCapsuleOrLine(this.GetPropertie(PROPERTY_GRAVITY_OVERRIDE), gravityLine);
                this.ApplyModification();
            }

            LockEditor();
        }

        /// <summary>
        /// need to be called at the end of the editor, lock the editor from deselecting the gameObject from the sceneView
        /// </summary>
        private void LockEditor()
        {
            //if nothing else, lock editor !
            if (EditorOptions.Instance.ShowGravityOverride)
            {
                ExtSceneView.LockFromUnselect();
            }
            if (ExtEventEditor.IsKeyDown(KeyCode.Escape))
            {
                EditorOptions.Instance.ShowGravityOverride = false;
            }

            if (EditorOptions.Instance.ShowGravityOverride && ExtEventEditor.IsKeyDown(KeyCode.Delete))
            {
                ExtEventEditor.Use();
            }
        }
    }
}