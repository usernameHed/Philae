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
    /*
    [CustomEditor(typeof(AttractorPolyLinesOverride), true)]
    public class AttractorPolyLinesOverrideEditor : AttractorPolyLinesEditor
    {
        private AttractorPolyLinesOverride _attractorPolyLineOverride;

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
            DeleteLineByIndex = DeleteGravityOverrideLineAtIndex;
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            EditorOptions.Instance.ShowGravityOverride = GUILayout.Toggle(EditorOptions.Instance.ShowGravityOverride, "Setup Gravity", EditorStyles.miniButton);
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);

            if (!EditorOptions.Instance.ShowGravityOverride || !_attractorPolyLineOverride.gameObject.activeInHierarchy)
            {
                return;
            }
            this.UpdateEditor();

            ExtPolyLines polyLine = this.GetPropertie("_polyLines").GetValue<ExtPolyLines>();
            int countLines = this.GetPropertie("_polyLines").GetPropertie("_listLines").arraySize;
            if (countLines != this.GetPropertie("GravityOverride").arraySize)
            {
                this.GetPropertie("GravityOverride").arraySize = countLines;
                this.ApplyModification();
            }

            GravityOverrideLineTopDown[] gravityLine = ExtGravityOverrideEditor.DrawPolyLines(polyLine, _attractorPolyLineOverride.GravityOverride, Color.red, out bool hasChanged);

            if (hasChanged)
            {
                for (int i = 0; i < gravityLine.Length; i++)
                {
                    gravityLine[i].SetupGravity();
                }
                ExtGravityOverrideEditor.ApplyModificationOfExtPolyLine(this.GetPropertie("GravityOverride"), gravityLine);
                this.ApplyModification();
            }
        }

        private void DeleteGravityOverrideLineAtIndex(int index)
        {
            this.GetPropertie("GravityOverride").DeleteArrayElementAtIndex(index);
        }
    }
    */
}