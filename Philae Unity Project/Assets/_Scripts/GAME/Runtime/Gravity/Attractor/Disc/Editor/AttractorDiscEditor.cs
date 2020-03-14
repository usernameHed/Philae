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
        private const string PROPERTY_EXT_DISC = "_disc";
        private const string PROPETY_EXT_CIRCLE = "_circle";
        private const string PROPERTY_EXT_PLANE = "_plane";
        private const string PROPERTY_ALLOW_BOTTOM = "_allowBottom";
        protected AttractorDisc _attractorDisc;

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
            _attractorDisc = (AttractorDisc)GetTarget<Attractor>();
        }

        public override void ShowTinyEditorContent()
        {
            this.UpdateEditor();
            base.ShowTinyEditorContent();
            bool allowDown = this.GetPropertie(PROPERTY_EXT_DISC).GetPropertie(PROPETY_EXT_CIRCLE).GetPropertie(PROPERTY_EXT_PLANE).GetPropertie(PROPERTY_ALLOW_BOTTOM).boolValue;

            bool allowDownChange = GUILayout.Toggle(allowDown, "Allow Down", EditorStyles.miniButton);
            if (allowDownChange != allowDown)
            {
                Debug.Log("ici !");
                SerializedProperty disc = this.GetPropertie(PROPERTY_EXT_DISC);
                SerializedProperty circle = disc.GetPropertie(PROPETY_EXT_CIRCLE);
                SerializedProperty plane = circle.GetPropertie(PROPERTY_EXT_PLANE);
                SerializedProperty allowBottom = plane.GetPropertie(PROPERTY_ALLOW_BOTTOM);
                allowBottom.boolValue = allowDownChange;
                this.ApplyModification();
            }

        }
    }
}