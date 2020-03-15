using ExtUnityComponents.transform;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.movable;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    public class MovableSplineArrayEditor : MovableSplinePointsEditor
    {
        public delegate void DeleteLineAtIndex(int index);
        public delegate void AddLineAtTheEnd();
        public DeleteLineAtIndex DeleteLineByIndex;
        public AddLineAtTheEnd AddedLine;

        private SerializedProperty _listPointsOnSpline;
        private SerializedProperty _spline;

        public virtual void OnCustomEnable(
            Editor targetEditor,
            GameObject targetGameObject,
            NeedToReConstructLines needToReConstructLines,
            AddLineAtTheEnd addLine,
            DeleteLineAtIndex deletedLine)
        {
            base.OnCustomEnable(targetEditor, targetGameObject, needToReConstructLines);
            AddedLine = addLine;
            DeleteLineByIndex = deletedLine;
        }

        public virtual void ConstructLines(
            SerializedProperty matrix,
            List<PointInSplines> pointInSplines,
            SerializedProperty spline,
            SerializedProperty listPointsOnSpline)
        {
            base.ConstructLines(matrix, pointInSplines);
            _spline = spline;
            _listPointsOnSpline = listPointsOnSpline;
        }

        public override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);
        }

        /// <summary>
        /// called on a Tiny Editor Content
        /// </summary>
        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();

            if (EditorOptions.Instance.SetupLinesOfSphape)
            {
                ButtonsSetupsLinesTools();
            }
        }

        protected override void PreventDelete()
        {
            if (EditorOptions.Instance.SetupLinesOfSphape && ExtEventEditor.IsKeyDown(KeyCode.Delete))
            {
                ExtEventEditor.Use();
                DeleteSelectedPoints();
            }
        }

        private void ButtonsSetupsLinesTools()
        {
            using (ExtGUIScopes.Horiz())
            {
                if (GUILayout.Button("Add Point"))
                {
                    AddPoint();
                }
            }
        }

        private void AddPoint()
        {
            _targetEditor.UpdateEditor();

            Vector3 p1 = /*GetAddPointLocalPosition() + */new Vector3(0, 0, 0.2f);

            AddPointLocal(p1);

            _targetEditor.ApplyModification();

            AddedLine?.Invoke();
            _needToReConstructLines?.Invoke();

            SelectPointAtIndex(_listPointsOnSpline.arraySize - 1);
        }

        

        /*
        private Vector3 GetAddPointLocalPosition()
        {
            if (Points.Count == 0)
            {
                return (Vector3.zero);
            }

            Vector3 globalPosition = GetAddPointPosition();
            return (_polyLineMatrixInverse.MultiplyPoint3x4(globalPosition));
        }

        private Vector3 GetAddPointPosition()
        {
            if (PointsSelected.Count == 0)
            {
                return (GetClosestPointFromCamera());
            }
            return (PointsSelected[PointsSelected.Count - 1].GetGlobalPointPosition());
        }

        private Vector3 GetClosestPointFromCamera()
        {
            List<Vector3> points = new List<Vector3>(Points.Count);
            for (int i = 0; i < Points.Count; i++)
            {
                points.Add(Points[i].GetGlobalPointPosition());
            }
            return (ExtMathf.GetClosestPoint(ExtSceneView.GetSceneViewCameraTransform().position, points, out int indexFound));
        }
        */

        public void AddPointLocal(Vector3 p1)
        {
            ExtShapeSerializeProperty.MoveSplinePointFromSerializePropertie(_spline, p1);
        }

        private void DeleteSelectedPoints()
        {
            _targetEditor.UpdateEditor();

            List<int> deletedPoints = new List<int>(_listPointsOnSpline.arraySize);
            deletedPoints.Clear();
            for (int i = 0; i < PointsSelected.Count; i++)
            {
                DeleteLine(PointsSelected[i].IndexPoint);
                deletedPoints.Add(PointsSelected[i].IndexPoint);
            }

            _targetEditor.ApplyModification();

            _needToReConstructLines?.Invoke();
        }

        private void DeleteLine(int indexLine)
        {
            _listPointsOnSpline.DeleteArrayElementAtIndex(indexLine);
            DeleteLineByIndex?.Invoke(indexLine);
        }
        
    }
}