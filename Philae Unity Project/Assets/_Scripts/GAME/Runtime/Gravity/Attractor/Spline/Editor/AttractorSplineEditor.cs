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
        private AttractoLineGenericEditor _attractoLineGenericEditor;

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
            _attractoLineGenericEditor = new AttractoLineGenericEditor();
        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorSpline = (AttractorSpline)GetTarget<Attractor>();


            _attractoLineGenericEditor.OnCustomEnable(
                this,
                _attractorSpline.gameObject,
                LinesHasBeenUpdated,
                ConstructLines);
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
            _attractoLineGenericEditor.ConstructLines(matrix, points);
        }
        

        /// <summary>
        /// called when lines points has been updated.
        /// determine what to do (update delta & deltaSquared cached ?)
        /// REMEMBER TO APPLY MODIFICATION AFTER !!!
        /// </summary>
        private void LinesHasBeenUpdated()
        {
            /*
            SerializedProperty polyLine = this.GetPropertie(PROPEPRTY_EXT_SPLINE);
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
            */
        }
        

        public override void OnCustomDisable()
        {
            base.OnCustomDisable();
            _attractoLineGenericEditor.OnCustomDisable();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractoLineGenericEditor.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);
            _attractoLineGenericEditor.CustomOnSceneGUI(sceneview);
        }
    }
}