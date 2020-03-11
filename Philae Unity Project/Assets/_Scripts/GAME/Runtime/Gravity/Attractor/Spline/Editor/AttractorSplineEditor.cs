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

namespace philae.gravity.attractor.line
{
    [CustomEditor(typeof(AttractorSpline), true)]
    public class AttractorSplineEditor : AttractorEditor
    {
        protected const string PROPEPRTY_EXT_SPLINE = "_spline";
        private const string PROPERTY_MATRIX = "_splinesMatrix";
        private const string PROPERTY_LIST_POINTS = "_listPoints";
        private const string PROPERTY_POINT_LOCAL = "PointLocal";
        private const string PROPERTY_POINT_GLOBAL = "PointGlobal";
        private AttractorSpline _attractorSpline;
        private AttractorSplineGenericEditor _attractoSplineGenericEditor;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorSplineEditor()
            : base(false, "Spline")
        {
            _attractoSplineGenericEditor = new AttractorSplineGenericEditor();
        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorSpline = (AttractorSpline)GetTarget<Attractor>();


            _attractoSplineGenericEditor.OnCustomEnable(
                this,
                _attractorSpline.gameObject,
                LinesHasBeenUpdated,
                ConstructLines,
                LineHasBeenAdded,
                LineHasBeenDeleted);
        }

        
        protected void ConstructLines()
        {
            SerializedProperty spline = this.GetPropertie(PROPEPRTY_EXT_SPLINE);
            SerializedProperty matrix = spline.GetPropertie(PROPERTY_MATRIX);
            SerializedProperty listLinesLocal = spline.GetPropertie(PROPERTY_LIST_POINTS);

            List<PointInLines> points = new List<PointInLines>(listLinesLocal.arraySize);
            for (int i = 1; i < listLinesLocal.arraySize; i++)
            {
                SerializedProperty point1 = listLinesLocal.GetArrayElementAtIndex(i - 1);
                SerializedProperty point2 = listLinesLocal.GetArrayElementAtIndex(i);
                points.Add(new PointInLines(i, 0, point1.GetPropertie(PROPERTY_POINT_LOCAL), point2.GetPropertie(PROPERTY_POINT_LOCAL), point1.GetPropertie(PROPERTY_POINT_GLOBAL), point2.GetPropertie(PROPERTY_POINT_GLOBAL)));
                if (i == listLinesLocal.arraySize - 1)
                {
                    points.Add(new PointInLines(i, 1, point1.GetPropertie(PROPERTY_POINT_LOCAL), point2.GetPropertie(PROPERTY_POINT_LOCAL), point1.GetPropertie(PROPERTY_POINT_GLOBAL), point2.GetPropertie(PROPERTY_POINT_GLOBAL)));
                }
            }
            //_attractoSplineGenericEditor.ConstructLines(matrix, points, listLines, listLinesLocal);
        }


        /// <summary>
        /// called when lines points has been updated.
        /// determine what to do (update delta & deltaSquared cached ?)
        /// REMEMBER TO APPLY MODIFICATION AFTER !!!
        /// </summary>
        private void LinesHasBeenUpdated()
        {
            /*
            SerializedProperty polyLine = this.GetPropertie(PROPEPRTY_POLY_EXT_LINE_3D);
            SerializedProperty listLinesLocal = polyLine.GetPropertie(PROPERTY_LIST_LINES_LOCAL);
            SerializedProperty listLines = polyLine.GetPropertie(PROPERTY_LIST_LINES_GLOBAL);

            for (int i = 0; i < listLinesLocal.arraySize; i++)
            {
                SerializedProperty lineLocal = listLinesLocal.GetArrayElementAtIndex(i);
                SerializedProperty line = listLines.GetArrayElementAtIndex(i);

                ExtShapeSerializeProperty.UpdateLineFromSerializeProperties(line);
                ExtShapeSerializeProperty.UpdateLineFromSerializeProperties(lineLocal);
            }
            this.ApplyModification();
            */
        }

        protected virtual void LineHasBeenAdded()
        {
            Debug.Log("add line");
        }

        protected virtual void LineHasBeenDeleted(int index)
        {
            Debug.Log("delete line " + index);
        }


        public override void OnCustomDisable()
        {
            base.OnCustomDisable();
            _attractoSplineGenericEditor.OnCustomDisable();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractoSplineGenericEditor.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);
            _attractoSplineGenericEditor.CustomOnSceneGUI(sceneview);
        }
    }
}