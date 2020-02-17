using ExtUnityComponents;
using hedCommon.extension.editor;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.line;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor
{
    [CustomEditor(typeof(AttractorQuad), true)]
    public class AttractorQuadEditor : AttractorEditor
    {
        protected new AttractorQuad _attractor;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorQuadEditor(bool showExtension, string tinyEditorName)
            : base(showExtension, tinyEditorName)
        {

        }
        public AttractorQuadEditor()
            : base(false, "Quad")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractor = (AttractorQuad)GetTarget<Attractor>();
        }

        public override void ShowTinyEditorContent()
        {
            this.UpdateEditor();
            base.ShowTinyEditorContent();
            bool allowDown = this.GetPropertie("_quad").GetPropertie("_plane").GetPropertie("_allowBottom").boolValue;

            bool allowDownChange = GUILayout.Toggle(allowDown, "Allow Down", EditorStyles.miniButton);
            if (allowDownChange != allowDown)
            {
                SerializedProperty quad = this.GetPropertie("_quad");
                SerializedProperty plane = quad.GetPropertie("_plane");
                SerializedProperty allowBottom = plane.GetPropertie("_allowBottom");
                allowBottom.boolValue = allowDownChange;
                this.ApplyModification();
            }

        }
    }
}