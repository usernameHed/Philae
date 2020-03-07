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
        private const string PROPERTY_QUAD = "_quad";
        private AttractorQuadOverride _attractorQuadOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();

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
            _attractorQuadOverride = (AttractorQuadOverride)GetTarget<Attractor>(); 
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

            ExtQuad quad = this.GetPropertie(PROPERTY_QUAD).GetValue<ExtQuad>();

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