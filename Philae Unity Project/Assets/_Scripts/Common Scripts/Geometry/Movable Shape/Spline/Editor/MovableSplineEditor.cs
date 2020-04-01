using extUnityComponents;
using hedCommon.extension.editor;
using hedCommon.geometry.shape2d;
using hedCommon.geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    [CustomEditor(typeof(MovableSpline), true)]
    public class MovableSplineEditor : MovableShapeEditor
    {
        protected const string PROPEPRTY_EXT_SPLINE = "_spline";
        private const string PROPERTY_MATRIX = "_splinesMatrix";
        private const string PROPERTY_LIST_POINTS = "_listPoints";
        private const string PROPERTY_POINT_LOCAL = "PointLocal";
        private const string PROPERTY_POINT_GLOBAL = "PointGlobal";
        private MovableSpline _movableSpline;
        private MovableSplineArrayEditor _movableSplineGenericEditor = new MovableSplineArrayEditor();


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public MovableSplineEditor(bool showExtension, string tinyEditorName)
            : base(showExtension, tinyEditorName)
        {

        }
        public MovableSplineEditor()
            : base(false, "Spline")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _movableSpline = (MovableSpline)GetTarget<MovableShape>();

            _movableSplineGenericEditor.OnCustomEnable(
                this,
                _movableSpline.gameObject,
                ConstructLines,
                LineHasBeenAdded,
                LineHasBeenDeleted);
        }


        protected void ConstructLines()
        {
            SerializedProperty spline = this.GetPropertie(PROPEPRTY_EXT_SPLINE);
            SerializedProperty matrix = spline.GetPropertie(PROPERTY_MATRIX);
            SerializedProperty listPointsOnSpline = spline.GetPropertie(PROPERTY_LIST_POINTS);

            List<PointInSplines> points = new List<PointInSplines>(listPointsOnSpline.arraySize);
            for (int i = 0; i < listPointsOnSpline.arraySize; i++)
            {
                SerializedProperty point = listPointsOnSpline.GetArrayElementAtIndex(i);
                points.Add(new PointInSplines(i, point.GetPropertie(PROPERTY_POINT_LOCAL), point.GetPropertie(PROPERTY_POINT_GLOBAL)));
            }

            _movableSplineGenericEditor.ConstructLines(matrix, points, spline, listPointsOnSpline);
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
            _movableSplineGenericEditor.OnCustomDisable();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _movableSplineGenericEditor.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);
            _movableSplineGenericEditor.CustomOnSceneGUI(sceneview);
        }
    }
}