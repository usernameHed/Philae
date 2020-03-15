﻿using ExtUnityComponents;
using hedCommon.extension.editor;
using hedCommon.geometry.movable;
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

            SerializedProperty donut = this.GetPropertie(ExtDonutProperty.PROPERTY_EXT_DONUT);

            float thickNess = donut.GetThickNess();
            float radius = donut.GetRadius();

            float newThickNess = ExtGUIFloatFields.FloatFieldWithSlider(thickNess, "ThickNess:", "Donut's ThickNess", 0, radius, false, 40, GUILayout.Width(80));
            if (newThickNess != thickNess)
            {
                donut.SetThickNess(newThickNess);
                this.ApplyModification();
            }
            /*
            float newRadius = ExtGUIFloatFields.FloatFieldWithSlider(radius, "Radius:", "Donut's Radius", 0, 999, false, 40, GUILayout.Width(80));
            if (newRadius != radius)
            {
                donut.SetRadius(newRadius);
                this.ApplyModification();
            }
            */
        }
    }
}