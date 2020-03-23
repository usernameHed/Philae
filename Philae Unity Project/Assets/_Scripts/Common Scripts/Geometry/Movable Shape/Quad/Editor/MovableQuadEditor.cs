using ExtUnityComponents;
using hedCommon.extension.editor;
using hedCommon.geometry.shape2d;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.line;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    [CustomEditor(typeof(MovableQuad), true)]
    public class MovableQuadEditor : MovableShapeEditor
    {
        protected MovableQuad _attractorQuad;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public MovableQuadEditor(bool showExtension, string tinyEditorName)
            : base(showExtension, tinyEditorName)
        {

        }
        public MovableQuadEditor()
            : base(false, "Quad")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorQuad = (MovableQuad)GetTarget<MovableShape>();
        }

        public override void ShowTinyEditorContent()
        {
            this.UpdateEditor();
            base.ShowTinyEditorContent();

            SerializedProperty quad = this.GetPropertie(ExtQuadProperty.PROPERTY_EXT_QUAD);

            bool allowDown = ExtQuadProperty.GetAllowDown(quad);

            bool allowDownChange = GUILayout.Toggle(allowDown, "Allow Down", EditorStyles.miniButton);
            if (allowDownChange != allowDown)
            {
                ExtQuadProperty.SetAllowDown(quad, allowDownChange);
                this.ApplyModification();
            }
        }
    }
}