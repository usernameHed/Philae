using ExtUnityComponents;
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
    [CustomEditor(typeof(AttractorQuadOverride), true)]
    public class AttractorQuadOverrideEditor : AttractorQuadEditor
    {
        protected new AttractorQuadOverride _attractor;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorQuadOverrideEditor()
            : base(false, "Quad Override")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractor = (AttractorQuadOverride)GetTarget<Attractor>(); 
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
            this.UpdateEditor();

            ExtQuad quad = this.GetPropertie("_quad").GetValue<ExtQuad>();

            GravityOverrideQuad gravityQuad = ExtGravityOverrideEditor.DrawQuadWithBorders(quad, _attractor.GravityOverride, out bool hasChanged);
            if (hasChanged)
            {
                gravityQuad.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToQuad(this.GetPropertie("GravityOverride"), gravityQuad);
                this.ApplyModification();
            }
        }
    }
}