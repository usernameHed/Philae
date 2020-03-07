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
    [CustomEditor(typeof(AttractorDiscOverride), true)]
    public class AttractorDiscOverrideEditor : AttractorDiscEditor
    {
        private const string PROPERTY_DISC = "_disc";
        private const string PROPERTY_CIRCLE = "_circle";
        private  AttractorDiscOverride _attractorDiscOverride;
        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorDiscOverrideEditor()
            : base(false, "Disc Override")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorDiscOverride = (AttractorDiscOverride)GetTarget<Attractor>(); 
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
            ExtCircle circle = this.GetPropertie(PROPERTY_DISC).GetPropertie(PROPERTY_CIRCLE).GetValue<ExtCircle>();

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