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
    [CustomEditor(typeof(AttractorDisc), true)]
    public class AttractorDiscEditor : AttractorEditor
    {
        protected new AttractorDisc _attractor;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorDiscEditor(bool showExtension, string tinyEditorName)
            : base(showExtension, tinyEditorName)
        {

        }
        public AttractorDiscEditor()
            : base(false, "Disc")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractor = (AttractorDisc)GetTarget<Attractor>();
        }

        public override void ShowTinyEditorContent()
        {
            this.UpdateEditor();
            base.ShowTinyEditorContent();
            bool allowDown = this.GetPropertie("_disc").GetPropertie("_circle").GetPropertie("_allowBottom").boolValue;

            bool allowDownChange = GUILayout.Toggle(allowDown, "Allow Down", EditorStyles.miniButton);
            if (allowDownChange != allowDown)
            {
                Debug.Log("ici !");
                SerializedProperty disc = this.GetPropertie("_disc");
                SerializedProperty circle = disc.GetPropertie("_circle");
                SerializedProperty allowBottom = circle.GetPropertie("_allowBottom");
                allowBottom.boolValue = allowDownChange;
                this.ApplyModification();
            }

        }
    }
}