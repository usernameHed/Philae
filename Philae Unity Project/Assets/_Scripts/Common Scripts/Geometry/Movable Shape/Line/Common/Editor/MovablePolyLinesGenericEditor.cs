using extUnityComponents.transform;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.movable;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    public class MovablePolyLinesGenericEditor : MovableLineGenericEditor
    {
        public delegate void DeleteLineAtIndex(int index);
        public delegate void AddLineAtTheEnd();
        public DeleteLineAtIndex DeleteLineByIndex;
        public AddLineAtTheEnd AddedLine;

        private SerializedProperty _listLinesLocal;
        private SerializedProperty _listLinesGlobal;

        public virtual void OnCustomEnable(
            Editor targetEditor,
            GameObject targetGameObject,
            NeedToUpdateLines needToUpdateLines,
            NeedToReConstructLines needToReConstructLines,
            AddLineAtTheEnd addLine,
            DeleteLineAtIndex deletedLine)
        {
            base.OnCustomEnable(targetEditor, targetGameObject, needToUpdateLines, needToReConstructLines);
            AddedLine = addLine;
            DeleteLineByIndex = deletedLine;

            //Undo.undoRedoPerformed += MyUndoCallback;
        }

        

        public virtual void ConstructLines(
            SerializedProperty matrix,
            List<PointInLines> pointInLines,
            SerializedProperty arrayLine,
            SerializedProperty arrayLocal)
        {
            base.ConstructLines(matrix, pointInLines);
            _listLinesLocal = arrayLocal;
            _listLinesGlobal = arrayLine;
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
                if (GUILayout.Button("Add Line"))
                {
                    AddLine(false);
                }
                if (CanJonction() && GUILayout.Button("Add Jonction"))
                {
                    AddLine(true);
                }
            }
        }

        private bool CanJonction()
        {
            if (PointsSelected.Count < 2)
            {
                return (false);
            }
            Vector3 point = PointsSelected[0].GetGlobalPointPosition();
            for (int i = 1; i < PointsSelected.Count; i++)
            {
                if (PointsSelected[i].GetGlobalPointPosition() != point)
                {
                    return (true);
                }
            }
            return (false);
        }

        private void AddLine(bool canJonction)
        {
            _targetEditor.UpdateEditor();

            Vector3 p1 = GetAddPointLocalPosition();
            Vector3 p2 = p1 + new Vector3(0, 0, 0.2f);

            if (canJonction)
            {
                TestToLinkTwoLines(ref p1, ref p2);
            }

            AddLineLocal(p1, p2);

            _targetEditor.ApplyModification();

            AddedLine?.Invoke();
            _needToReConstructLines?.Invoke();

            SelectPointAtIndex((_listLinesLocal.arraySize * 2) - 1);
        }

        /*
        void MyUndoCallback()
        {
            // code for the action to take on Undo
            //Debug.Log("reconstruct ??");
            _needToReConstructLines?.Invoke();
        }
        */

        /// <summary>
        /// search if there is 2 selected points not linked. If so, link them
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private void TestToLinkTwoLines(ref Vector3 p1, ref Vector3 p2)
        {
            if (PointsSelected.Count < 2)
            {
                return;
            }
            Vector3 point1 = PointsSelected[0].GetLocalPosition();
            Vector3 point2 = PointsSelected[0].GetOtherLocalPoint();
            
            for (int i = 1; i < PointsSelected.Count; i++)
            {
                if (IsPointIsConnectedToOtherPoint(point1, PointsSelected[i].GetLocalPosition()))
                {
                    continue;
                }
                Debug.Log("create junction ?");
                Debug.Log(point1 + ", " + point2 + ", other: " + PointsSelected[i].GetLocalPosition() + ", " + PointsSelected[i].GetOtherLocalPoint());
                p1 = point1;
                p2 = PointsSelected[i].GetLocalPosition();
                return;
            }
        }

        private bool IsPointIsConnectedToOtherPoint(Vector3 point1, Vector3 point2)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].GetLocalPosition() == point1 && Points[i].GetOtherLocalPoint() == point2)
                {
                    return (true);
                }
                if (Points[i].GetLocalPosition() == point2 && Points[i].GetOtherLocalPoint() == point1)
                {
                    return (true);
                }
            }
            return (false);
        }

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

        public void AddLineLocal(Vector3 p1, Vector3 p2)
        {
            _listLinesLocal.arraySize = _listLinesLocal.arraySize + 1;
            _listLinesGlobal.arraySize = _listLinesGlobal.arraySize + 1;
            SerializedProperty lastLineLocal = _listLinesLocal.GetArrayElementAtIndex(_listLinesLocal.arraySize - 1);
            ExtPolyLineProperty.MoveLineFromSerializeProperties(lastLineLocal, p1, p2);

            SerializedProperty lastLineGlobal = _listLinesGlobal.GetArrayElementAtIndex(_listLinesGlobal.arraySize - 1);
            ExtPolyLineProperty.MoveLineFromSerializeProperties(lastLineGlobal, _polyLineMatrix.MultiplyPoint3x4(p1), _polyLineMatrix.MultiplyPoint3x4(p2));
        }

        private void DeleteSelectedPoints()
        {
            _targetEditor.UpdateEditor();

            List<int> deletedLines = new List<int>(_listLinesLocal.arraySize);
            deletedLines.Clear();
            for (int i = 0; i < PointsSelected.Count; i++)
            {
                //don't delete a line already deleted
                if (deletedLines.Contains(PointsSelected[i].IndexLine))
                {
                    continue;
                }
                int indexInArray = PointsSelected[i].IndexLine - deletedLines.Count;
                DeleteLine(indexInArray);
                deletedLines.Add(PointsSelected[i].IndexLine);
            }

            _targetEditor.ApplyModification();

            _needToReConstructLines?.Invoke();
        }

        private void DeleteLine(int indexLine)
        {
            _listLinesLocal.DeleteArrayElementAtIndex(indexLine);
            _listLinesGlobal.DeleteArrayElementAtIndex(indexLine);
            DeleteLineByIndex?.Invoke(indexLine);
        }
        
    }
}