using ExtUnityComponents;
using hedCommon.extension.editor;
using hedCommon.geometry.shape2d;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.line;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor
{
    [CustomEditor(typeof(AttractorDonut), true)]
    public class AttractorDonutEditor : AttractorEditor
    {
        private const string PROPERTY_EXT_DONUT = "_donut";
        private const string PROPERTY_THICKNESS = "_thickNess";
        private const string PROPERTY_REAL_THICKNESS = "_realThickNess";


        protected AttractorDonut _attractorDonut;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorDonutEditor(bool showExtension, string tinyEditorName)
            : base(showExtension, tinyEditorName)
        {

        }
        public AttractorDonutEditor()
            : base(false, "Donus")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorDonut = (AttractorDonut)GetTarget<Attractor>();
        }

        public override void ShowTinyEditorContent()
        {
            this.UpdateEditor();
            base.ShowTinyEditorContent();

            float thickNess = this.GetPropertie(PROPERTY_EXT_DONUT).GetPropertie(PROPERTY_THICKNESS).floatValue;
            ExtDonut donut = this.GetPropertie(PROPERTY_EXT_DONUT).GetValue<ExtDonut>();

            float newThickNess = ExtGUIFloatFields.FloatFieldWithSlider(thickNess, "ThickNess:", "Donut's ThickNess", 0, donut.Radius, false, 40, GUILayout.Width(80));
            if (newThickNess != thickNess)
            {
                this.GetPropertie(PROPERTY_EXT_DONUT).GetPropertie(PROPERTY_THICKNESS).floatValue = newThickNess;
                this.GetPropertie(PROPERTY_EXT_DONUT).GetPropertie(PROPERTY_REAL_THICKNESS).floatValue = newThickNess * donut.MaxXY(donut.LocalScale);
                this.ApplyModification();
            }
        }
    }
}