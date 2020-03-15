using ExtUnityComponents;
using feerik.editor.utils;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.movable;
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
    [CustomEditor(typeof(AttractorPolyLinesGravityOverride), true)]
    public class AttractorPolyLinesOverrideEditor : AttractorEditor
    {
        private AttractorPolyLinesGravityOverride _attractorPolyLineOverride;
        private SerializedProperty _gravityOverride;

        private AttractorOverrideGenericEditor _attractorOverrideGeneric = new AttractorOverrideGenericEditor();

        private MovablePolyLinesEditor _moveLineReference;

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
            _attractorPolyLineOverride = (AttractorPolyLinesGravityOverride)GetTarget<Attractor>();
            this.UpdateEditor();
            _gravityOverride = this.GetPropertie(AttractorOverrideGenericEditor.PROPERTY_GRAVITY_OVERRIDE);

            MovablePolyLinesEditor[] editors = (MovablePolyLinesEditor[])Resources.FindObjectsOfTypeAll(typeof(MovablePolyLinesEditor));
            if (editors.Length > 0)
            {
                _moveLineReference = editors[0];
                _moveLineReference.LineAdded += LineHasBeenAdded;
                _moveLineReference.LineDeleteAt += LineHasBeenDeleted;
            }
        }

        public override void OnCustomDisable()
        {
            base.OnCustomDisable();
            if (_moveLineReference)
            {
                _moveLineReference.LineAdded -= LineHasBeenAdded;
                _moveLineReference.LineDeleteAt -= LineHasBeenDeleted;
            }
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

            SerializedObject movablePolyLine = this.GetPropertie(ExtPolyLineProperty.PROPEPRTY_MOVABLE_POLY_LINE).ToSerializeObject<MovablePolyLines>();
            SerializedProperty extPolyLine = movablePolyLine.GetPropertie(ExtPolyLineProperty.PROPEPRTY_POLY_EXT_LINE_3D);

            ExtPolyLines polyLine = extPolyLine.GetValue<ExtPolyLines>();
            int countLines = extPolyLine.GetPropertie(ExtPolyLineProperty.PROPERTY_LIST_LINES_GLOBAL).arraySize;
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

        protected void LineHasBeenAdded()
        {
            this.UpdateEditor();

            SerializedObject movablePolyLine = this.GetPropertie(ExtPolyLineProperty.PROPEPRTY_MOVABLE_POLY_LINE).ToSerializeObject<MovablePolyLines>();
            SerializedProperty extPolyLine = movablePolyLine.GetPropertie(ExtPolyLineProperty.PROPEPRTY_POLY_EXT_LINE_3D);


            int countLines = extPolyLine.GetPropertie(ExtPolyLineProperty.PROPERTY_LIST_LINES_GLOBAL).arraySize;
            _gravityOverride.arraySize = countLines;
            SerializedProperty lastGravityoverrideCreated = _gravityOverride.GetArrayElementAtIndex(countLines - 1);

            GravityOverrideLineTopDown full = new GravityOverrideLineTopDown(true, true, true);
            ExtGravityOverrideEditor.ApplyModificationToCapsuleOrLine(lastGravityoverrideCreated, full);
            this.ApplyModification();
        }

        protected void LineHasBeenDeleted(int index)
        {
            this.UpdateEditor();
            _gravityOverride.DeleteArrayElementAtIndex(index);
            this.ApplyModification();
        }
    }
}