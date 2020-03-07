using ExtUnityComponents.transform;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.editor;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.gravityOverride;
using philae.gravity.attractor.line;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor
{
    [CustomEditor(typeof(AttractorPolyLines), true)]
    public class AttractorPolyLinesEditor : AttractorEditor
    {
        protected const string PROPEPRTY_POLY_EXT_LINE_3D = "_polyLines";

        private AttractorPolyLines _attractorPolyLine;
        private AttractoLineGenericEditor _attractorLinesGeneric;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorPolyLinesEditor()
            : base(false, "PolyLine")
        {
            _attractorLinesGeneric = new AttractoLineGenericEditor();
        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorPolyLine = (AttractorPolyLines)GetTarget<Attractor>();

            _attractorLinesGeneric.OnCustomEnable(this, _attractorPolyLine.gameObject, LinesHasBeenUpdated, ConstructLines);
        }

        private void ConstructLines()
        {
            SerializedProperty polyLine = this.GetPropertie(PROPEPRTY_POLY_EXT_LINE_3D);
            SerializedProperty matrix = polyLine.GetPropertie("_polyLinesMatrix");
            SerializedProperty listLinesLocal = polyLine.GetPropertie("_listLinesLocal");
            SerializedProperty listLines = polyLine.GetPropertie("_listLines");

            if (listLines.arraySize != listLinesLocal.arraySize)
            {
                listLines.arraySize = listLinesLocal.arraySize;
                this.ApplyModification();
            }
            List<PointInLines> points = new List<PointInLines>(listLinesLocal.arraySize * 2);
            for (int i = 0; i < listLinesLocal.arraySize; i++)
            {
                SerializedProperty lineLocal = listLinesLocal.GetArrayElementAtIndex(i);
                SerializedProperty line = listLines.GetArrayElementAtIndex(i);
                points.Add(new PointInLines(i, 0, lineLocal.GetPropertie("_p1"), lineLocal.GetPropertie("_p2"), line.GetPropertie("_p1"), line.GetPropertie("_p2")));
                points.Add(new PointInLines(i, 1, lineLocal.GetPropertie("_p1"), lineLocal.GetPropertie("_p2"), line.GetPropertie("_p1"), line.GetPropertie("_p2")));
            }

            _attractorLinesGeneric.ConstructLines(matrix, points);
        }

        /// <summary>
        /// called when lines points has been updated.
        /// determine what to do (update delta & deltaSquared cached ?)
        /// REMEMBER TO APPLY MODIFICATION AFTER !!!
        /// </summary>
        private void LinesHasBeenUpdated()
        {
            SerializedProperty polyLine = this.GetPropertie(PROPEPRTY_POLY_EXT_LINE_3D);
            SerializedProperty listLinesLocal = polyLine.GetPropertie("_listLinesLocal");
            SerializedProperty listLines = polyLine.GetPropertie("_listLines");

            for (int i = 0; i < listLinesLocal.arraySize; i++)
            {
                SerializedProperty lineLocal = listLinesLocal.GetArrayElementAtIndex(i);
                SerializedProperty line = listLines.GetArrayElementAtIndex(i);

                ExtShapeSerializeProperty.UpdateLineFromSerializeProperties(line);
                ExtShapeSerializeProperty.UpdateLineFromSerializeProperties(lineLocal);
            }
            this.ApplyModification();
        }

        public override void OnCustomDisable()
        {
            base.OnCustomDisable();
            _attractorLinesGeneric.OnCustomDisable();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorLinesGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);
            _attractorLinesGeneric.CustomOnSceneGUI(sceneview);
        }
    }
}