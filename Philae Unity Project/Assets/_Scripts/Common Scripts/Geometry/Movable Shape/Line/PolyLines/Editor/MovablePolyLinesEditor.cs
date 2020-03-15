using ExtUnityComponents.transform;
using hedCommon.extension.editor;
using hedCommon.geometry.movable;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor.line
{
    [CustomEditor(typeof(MovablePolyLines), true)]
    public class MovablePolyLinesEditor : MovableShapeEditor
    {
        private MovablePolyLines _movablePolyLine;
        private MovablePolyLinesGenericEditor _movablePolyLinesGeneric;
        
        public delegate void LineAdd();
        public delegate void LineDeletedAt(int index);
        public LineAdd LineAdded;
        public LineDeletedAt LineDeleteAt;


        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public MovablePolyLinesEditor()
            : base(false, "PolyLine")
        {
            _movablePolyLinesGeneric = new MovablePolyLinesGenericEditor();
        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _movablePolyLine = (MovablePolyLines)GetTarget<MovableShape>();

            _movablePolyLinesGeneric.OnCustomEnable(
                this,
                _movablePolyLine.gameObject,
                LinesHasBeenUpdated,
                ConstructLines,
                LineHasBeenAdded,
                LineHasBeenDeleted);
        }

        protected void ConstructLines()
        {
            SerializedProperty polyLine = this.GetPropertie(ExtPolyLineProperty.PROPEPRTY_POLY_EXT_LINE_3D);
            SerializedProperty matrix = polyLine.GetPropertie(ExtPolyLineProperty.PROPERTY_POLY_LINE_MATRIX);
            SerializedProperty listLinesLocal = polyLine.GetPropertie(ExtPolyLineProperty.PROPERTY_LIST_LINES_LOCAL);
            SerializedProperty listLines = polyLine.GetPropertie(ExtPolyLineProperty.PROPERTY_LIST_LINES_GLOBAL);

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
                points.Add(new PointInLines(i, 0, lineLocal.GetPropertie(ExtPolyLineProperty.PROPERTY_P1), lineLocal.GetPropertie(ExtPolyLineProperty.PROPERTY_P2), line.GetPropertie(ExtPolyLineProperty.PROPERTY_P1), line.GetPropertie(ExtPolyLineProperty.PROPERTY_P2)));
                points.Add(new PointInLines(i, 1, lineLocal.GetPropertie(ExtPolyLineProperty.PROPERTY_P1), lineLocal.GetPropertie(ExtPolyLineProperty.PROPERTY_P2), line.GetPropertie(ExtPolyLineProperty.PROPERTY_P1), line.GetPropertie(ExtPolyLineProperty.PROPERTY_P2)));
            }

            _movablePolyLinesGeneric.ConstructLines(matrix, points, listLines, listLinesLocal);
        }

        /// <summary>
        /// called when lines points has been updated.
        /// determine what to do (update delta & deltaSquared cached ?)
        /// REMEMBER TO APPLY MODIFICATION AFTER !!!
        /// </summary>
        private void LinesHasBeenUpdated()
        {
            SerializedProperty polyLine = this.GetPropertie(ExtPolyLineProperty.PROPEPRTY_POLY_EXT_LINE_3D);
            SerializedProperty listLinesLocal = polyLine.GetPropertie(ExtPolyLineProperty.PROPERTY_LIST_LINES_LOCAL);
            SerializedProperty listLines = polyLine.GetPropertie(ExtPolyLineProperty.PROPERTY_LIST_LINES_GLOBAL);

            for (int i = 0; i < listLinesLocal.arraySize; i++)
            {
                SerializedProperty lineLocal = listLinesLocal.GetArrayElementAtIndex(i);
                SerializedProperty line = listLines.GetArrayElementAtIndex(i);

                ExtPolyLineProperty.UpdateLineFromSerializeProperties(line);
                ExtPolyLineProperty.UpdateLineFromSerializeProperties(lineLocal);
            }
            this.ApplyModification();
        }

        public virtual void LineHasBeenAdded()
        {
            Debug.Log("add line");
            LineAdded?.Invoke();
        }

        protected virtual void LineHasBeenDeleted(int index)
        {
            Debug.Log("delete line " + index);
            LineDeleteAt?.Invoke(index);
        }

        public override void OnCustomDisable()
        {
            base.OnCustomDisable();
            _movablePolyLinesGeneric.OnCustomDisable();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _movablePolyLinesGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);
            _movablePolyLinesGeneric.CustomOnSceneGUI(sceneview);
        }
    }
}