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
    [CustomEditor(typeof(AttractorPolyLinesOverride), true)]
    public class AttractorPolyLinesOverrideEditor : AttractorPolyLinesEditor
    {
        private const string PROPERTY_LIST_LINE_GLOBAL = "_listLines";
        private AttractorPolyLinesOverride _attractorPolyLineOverride;
        private SerializedProperty _gravityOverride;

        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorPolyLinesOverrideEditor()
            : base()
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorPolyLineOverride = (AttractorPolyLinesOverride)GetTarget<Attractor>();
            this.UpdateEditor();
            _gravityOverride = this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE);
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorOverrideGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!_attractorOverrideGeneric.CanSetupGravity() || !_attractorPolyLineOverride.gameObject.activeInHierarchy)
            {
                return;
            }
            this.UpdateEditor();

            

            ExtPolyLines polyLine = this.GetPropertie(PROPEPRTY_POLY_EXT_LINE_3D).GetValue<ExtPolyLines>();
            int countLines = this.GetPropertie(PROPEPRTY_POLY_EXT_LINE_3D).GetPropertie(PROPERTY_LIST_LINE_GLOBAL).arraySize;
            if (countLines != _gravityOverride.arraySize)
            {
                _gravityOverride.arraySize = countLines;
                this.ApplyModification();
            }

            GravityOverrideLineTopDown[] gravityLine = ExtGravityOverrideEditor.DrawPolyLines(polyLine, _attractorPolyLineOverride.GravityOverride, Color.red, out bool hasChanged);

            if (hasChanged)
            {
                for (int i = 0; i < gravityLine.Length; i++)
                {
                    gravityLine[i].SetupGravity();
                }
                ExtGravityOverrideEditor.ApplyModificationOfExtPolyLine(_gravityOverride, gravityLine);
                this.ApplyModification();
            }

            _attractorOverrideGeneric.LockEditor();
        }

        protected override void LineHasBeenAdded()
        {
            int countLines = this.GetPropertie(PROPEPRTY_POLY_EXT_LINE_3D).GetPropertie(PROPERTY_LIST_LINE_GLOBAL).arraySize;
            _gravityOverride.arraySize = countLines;
            this.ApplyModification();
        }

        protected override void LineHasBeenDeleted(int index)
        {
            _gravityOverride.DeleteArrayElementAtIndex(index);
            this.ApplyModification();
        }
    }
}