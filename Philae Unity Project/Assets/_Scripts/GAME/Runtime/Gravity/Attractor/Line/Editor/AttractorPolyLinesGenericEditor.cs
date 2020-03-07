﻿using ExtUnityComponents.transform;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.editor;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.gravityOverride;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor.line
{
    /*
    public class AttractorPolyLinesGenericEditor
    {
        private GameObject _targetGameObject;
        private Editor _targetEditor;

        private TransformHiddedTools _transformHiddedTools;

        private Vector3 _lastPositionMoved;
        private SerializedProperty _extPolyLine;
        private SerializedProperty _listLinesGlobal;
        private SerializedProperty _pointInfosArray;
        private SerializedProperty _listLinesLocal;
        private SerializedProperty _polyLineMatrixPropertie;



        private Vector2 _initialPositionDrag;
        private Vector2 _currentDragPosition;
        private Vector2 _endDragPosition;
        private bool _isDragging = false;
        private bool _isMovingMultiplePoints = false;
        private Matrix4x4 _polyLineMatrix;
        private Matrix4x4 _polyLineMatrixInverse;

        private Vector3 _currentHandlePosition;

        public List<PointInLines> Points = new List<PointInLines>(50);
        public List<PointInLines> PointsSelected = new List<PointInLines>(50);

        public delegate void DeleteLineAtIndex(int index);
        public DeleteLineAtIndex DeleteLineByIndex;

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public void OnCustomEnable(Editor targetEditor, GameObject targetGameObject)
        {
            _targetEditor = targetEditor;
            _targetGameObject = targetGameObject;


            _transformHiddedTools = _targetGameObject.GetComponent<TransformHiddedTools>();
            Tools.hidden = EditorOptions.Instance.SetupLinesOfSphape;
            _transformHiddedTools.HideHandle = EditorOptions.Instance.SetupLinesOfSphape;

            _targetEditor.UpdateEditor();
            ConstructPoints();
            SetupPointInLines();
            UnSelectPoints();
        }

        public void OnCustomDisable()
        {
            Tools.hidden = false;
        }

        public void CustomOnSceneGUI(SceneView sceneview)
        {
            if (!EditorOptions.Instance.SetupLinesOfSphape || !_targetGameObject.activeInHierarchy)
            {
                return;
            }

            _targetEditor.UpdateEditor();
            SetupAllSerializeObject();

            _isMovingMultiplePoints = PointsSelected.Count > 1;
            _currentHandlePosition = GetMiddleOfAllSelectedPoints();

            HandlePoints(_currentHandlePosition);

            HandleSpherePoints();

            FramePointsWithF();
            ManageDragRect();
            AttemptToUselectPoints();
            ShowPointsSelectedText();
            LockEditor();
        }

        /// <summary>
        /// setup serialized properties
        /// </summary>
        private void SetupAllSerializeObject()
        {
            _extPolyLine = _targetEditor.GetPropertie("_polyLines");
            _listLinesGlobal = _extPolyLine.GetPropertie("_listLines");
            _listLinesLocal = _extPolyLine.GetPropertie("_listLinesLocal");
            _polyLineMatrixPropertie = _extPolyLine.GetPropertie("_polyLinesMatrix");
            _pointInfosArray = _extPolyLine.GetPropertie("_pointsInfos");

            //if a change occured in lnie array, update this array size
            int countLines = _listLinesGlobal.arraySize * 2;
            if (_pointInfosArray.arraySize != countLines)
            {
                _pointInfosArray.arraySize = countLines;
            }

            _polyLineMatrix = _polyLineMatrixPropertie.GetValue<Matrix4x4>();
            _polyLineMatrixInverse = _polyLineMatrix.inverse;
        }

        /// <summary>
        /// from pointsInfo save, construct array of points (useful for edior purpose)
        /// </summary>
        public void ConstructPoints()
        {
            SetupAllSerializeObject();

            int countLines = _listLinesGlobal.arraySize * 2;
            if (_pointInfosArray.arraySize != countLines)
            {
                _pointInfosArray.arraySize = countLines;
                _targetEditor.ApplyModification();
            }
        }

        /// <summary>
        /// unselect all current selected points
        /// </summary>
        private void UnSelectPoints(params PointInLines[] exception)
        {
            for (int i = 0; i < PointsSelected.Count; i++)
            {
                if (exception.Contains(PointsSelected[i]))
                {
                    continue;
                }
                PointsSelected[i].SetSelected(false);
                SetPointsInfoOfLine(PointsSelected[i], false);
            }
            SetupPointInLines();
            _targetEditor.ApplyModification();
        }

        /// <summary>
        /// called on a Tiny Editor Content
        /// </summary>
        public void ShowTinyEditorContent()
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorOptions.Instance.SetupLinesOfSphape = GUILayout.Toggle(EditorOptions.Instance.SetupLinesOfSphape, "Setup Lines", EditorStyles.miniButton);

                if (EditorOptions.Instance.SetupLinesOfSphape)
                {
                    ButtonsSetupsLinesTools();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    Tools.hidden = EditorOptions.Instance.SetupLinesOfSphape;
                    _lastPositionMoved = _targetGameObject.transform.position;
                    _transformHiddedTools.HideHandle = EditorOptions.Instance.SetupLinesOfSphape;
                }
            }
        }

        private void ButtonsSetupsLinesTools()
        {
            if (GUILayout.Button("Add Line"))
            {
                AddLine();
            }
            if (_isMovingMultiplePoints && PointsSelected.Count >= 2)
            {
                ShowMergeSelectedPointsButton();
                UnMergeSelectedPoint();
            }
        }

        public void ShowMergeSelectedPointsButton()
        {
            bool pointAreAtSamePlace = true;
            Vector3 pointPosition = PointsSelected[0].GetGlobalPointPosition();
            for (int i = 1; i < PointsSelected.Count; i++)
            {
                if (pointPosition != PointsSelected[i].GetGlobalPointPosition())
                {
                    pointAreAtSamePlace = false;
                    break;
                }
            }
            if (pointAreAtSamePlace)
            {
                return;
            }

            if (GUILayout.Button("Merge Points"))
            {
                MergeSelectedPoints();
            }
        }

        public void MergeSelectedPoints()
        {
            _targetEditor.UpdateEditor();
            for (int i = 0; i < PointsSelected.Count; i++)
            {
                PointsSelected[i].SetGlobalPointPosition(_currentHandlePosition);
                MovePoints(_polyLineMatrixInverse, PointsSelected[i]);
            }
            _targetEditor.ApplyModification();
        }

        public void UnMergeSelectedPoint()
        {
            using (ExtGUIScopes.Horiz())
            {
                for (int i = 0; i < PointsSelected.Count; i++)
                {
                    if (GUILayout.Button(i.ToString()))
                    {
                        _targetEditor.UpdateEditor();
                        UnSelectPoints(PointsSelected[i]);
                    }
                }
            }
        }

        private void ShowPointsSelectedText()
        {
            if (PointsSelected.Count < 2)
            {
                return;
            }
            for (int i = 0; i < PointsSelected.Count; i++)
            {
                Vector3 middleLine = ExtVector3.GetMeanOfXPoints(PointsSelected[i].GetGlobalPointPosition(), PointsSelected[i].GetMiddleLine());
                ExtSceneView.DisplayStringIn3D(middleLine, i.ToString(), Color.black);
            }
        }

        public void AddLine()
        {
            _targetEditor.UpdateEditor();
            SetupAllSerializeObject();

            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(0, 0, 0.3f);
            AddLineLocal(p1, p2);

            _targetEditor.ApplyModification();

            ConstructPoints();
            SetupPointInLines();
            UnSelectPoints();
        }

        public void AddLineLocal(Vector3 p1, Vector3 p2)
        {
            _listLinesLocal.arraySize = _listLinesLocal.arraySize + 1;
            _listLinesGlobal.arraySize = _listLinesGlobal.arraySize + 1;
            SerializedProperty lastLineLocal = _listLinesLocal.GetArrayElementAtIndex(_listLinesLocal.arraySize - 1);
            ExtShapeSerializeProperty.MoveLineFromSerializeProperties(lastLineLocal, p1, p2);

            SerializedProperty lastLineGlobal = _listLinesGlobal.GetArrayElementAtIndex(_listLinesGlobal.arraySize - 1);
            ExtShapeSerializeProperty.MoveLineFromSerializeProperties(lastLineGlobal, _polyLineMatrix.MultiplyPoint3x4(p1), _polyLineMatrix.MultiplyPoint3x4(p2));
        }

        private void DeleteSelectedPoints()
        {
            _targetEditor.UpdateEditor();
            SetupAllSerializeObject();

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
                if (DeleteLineByIndex != null)
                {
                    DeleteLineByIndex.Invoke(indexInArray);
                }
                deletedLines.Add(PointsSelected[i].IndexLine);
            }

            _targetEditor.ApplyModification();

            ConstructPoints();
            SetupPointInLines();
            UnSelectPoints();
        }

        private void DeleteLine(int indexLine)
        {
            _listLinesLocal.DeleteArrayElementAtIndex(indexLine);
            _listLinesGlobal.DeleteArrayElementAtIndex(indexLine);
        }

        /// <summary>
        /// create a rectangle of selection
        /// (event drag & release)
        /// 
        ///     +---------+
        ///     |         |
        ///     |         |
        ///     |         |
        ///     +--------- mouse
        ///     
        ///  if points are inside, select them
        /// </summary>
        private void ManageDragRect()
        {
            if (Event.current.alt || Event.current.control || Event.current.shift)
            {
                return;
            }

            if (ExtEventEditor.IsLeftMouseDown(Event.current) && !_isDragging)
            {
                _isDragging = true;
                _currentDragPosition = _initialPositionDrag = Event.current.mousePosition;
            }
            if (ExtEventEditor.IsLeftMouseUp(Event.current))
            {
                _endDragPosition = Event.current.mousePosition;
                ChangePointSelectionStateInsideMouseRectangleSelection();
                _isDragging = false;
            }

            Handles.BeginGUI();
            if (_isDragging)
            {
                float width =  Event.current.mousePosition.x - _initialPositionDrag.x;
                float height = Event.current.mousePosition.y - _initialPositionDrag.y;
                ExtSceneView.GUIDrawRect(new Rect(_initialPositionDrag.x, _initialPositionDrag.y, width, height), new Color(0.1f, 0.1f, 0.1f, 0.4f));
            }
            Handles.EndGUI();
        }

        /// <summary>
        /// 
        ///     +---------+
        ///     |         |
        ///     |         |
        ///     |         |
        ///     +--------- mouse
        ///     
        ///  if points are inside, select them
        /// </summary>
        private void ChangePointSelectionStateInsideMouseRectangleSelection()
        {
            float width = Event.current.mousePosition.x - _initialPositionDrag.x;
            float height = Event.current.mousePosition.y - _initialPositionDrag.y;
            Rect rect = new Rect(_initialPositionDrag.x, _initialPositionDrag.y, width, height);
            rect = ExtRect.ReverseRectIfNeeded(rect);

            bool hasChanged = false;

            for (int i = 0; i < Points.Count; i++)
            {
                if (ExtRect.Is3dPointInside2dRectInScreenSpace(ExtSceneView.Camera(), rect, Points[i].GetGlobalPointPosition()))
                {
                    Points[i].SetSelected(true);
                    SetPointsInfoOfLine(Points[i], false);
                    hasChanged = true;
                }
            }
            
            if (hasChanged)
            {
                SetupPointInLines();
                _targetEditor.ApplyModification();
            }
        }

        /// <summary>
        /// focus the current selected point moved, OR the last selected point (if nothing was moved)
        /// </summary>
        private void FramePointsWithF()
        {
            if (ExtEventEditor.IsKeyDown(KeyCode.F) && _lastPositionMoved != Vector3.zero)
            {
                if (Event.current.type != EventType.Layout)
                {
                    Event.current.Use();
                }
                ExtSceneView.Frame(_lastPositionMoved);
            }
        }

        /// <summary>
        /// need to be called at the end of the editor, lock the editor from deselecting the gameObject from the sceneView
        /// </summary>
        private void LockEditor()
        {
            //if nothing else, lock editor !
            if (EditorOptions.Instance.SetupLinesOfSphape)
            {
                ExtSceneView.LockFromUnselect();
            }
            if (ExtEventEditor.IsKeyDown(KeyCode.Escape))
            {
                Selection.activeObject = null;
            }

            if (EditorOptions.Instance.SetupLinesOfSphape && ExtEventEditor.IsKeyDown(KeyCode.Delete))
            {
                DeleteSelectedPoints();
                ExtEventEditor.Use();
            }
        }

        /// <summary>
        /// if clickUp, and mouse position is close to the initial drag position, unselect all points !
        /// </summary>
        private void AttemptToUselectPoints()
        {
            if (ExtEventEditor.IsLeftMouseUp(Event.current) && ExtVector3.IsClose(_initialPositionDrag, _endDragPosition, 0.01f))
            {
                SetupPointInLines();
                UnSelectPoints();
            }
        }

        /// <summary>
        /// the most important function, it save every state of points inside arrays
        /// MUST BE UPDATED every time a change occure in datas
        /// </summary>
        private void SetupPointInLines()
        {
            Points.Clear();
            PointsSelected.Clear();
            int countLines = _listLinesGlobal.arraySize;

            for (int i = 0; i < countLines; i++)
            {
                SerializedProperty extLineFromGlobal = _listLinesGlobal.GetArrayElementAtIndex(i);
                SerializedProperty extLineFromLocal = _listLinesLocal.GetArrayElementAtIndex(i);
                PointsInfo pointInfo1 = GetPointInfoOfPointLine(i, 0);
                PointsInfo pointInfo2 = GetPointInfoOfPointLine(i, 1);

                Points.Add(new PointInLines(i, 0, pointInfo1, extLineFromGlobal, extLineFromLocal));
                Points.Add(new PointInLines(i, 1, pointInfo2, extLineFromGlobal, extLineFromLocal));

                if (pointInfo1.IsSelected)
                {
                    PointsSelected.Add(new PointInLines(i, 0, pointInfo1, extLineFromGlobal, extLineFromLocal));
                }
                if (pointInfo2.IsSelected)
                {
                    PointsSelected.Add(new PointInLines(i, 1, pointInfo2, extLineFromGlobal, extLineFromLocal));
                }
            }
        }

        /// <summary>
        /// return the middle of all selected points
        /// if there is no point selected, return Vector3.zero;
        /// </summary>
        /// <returns></returns>
        private Vector3 GetMiddleOfAllSelectedPoints()
        {
            List<Vector3> positions = new List<Vector3>(PointsSelected.Count);
            for (int i = 0; i < PointsSelected.Count; i++)
            {
                positions.Add(PointsSelected[i].GetGlobalPointPosition());
            }
            Vector3 middle = ExtVector3.GetMeanOfXPoints(positions.ToArray());
            return (middle);
        }

        /// <summary>
        /// for all unselected points, draw a handle sphere: we can clic on it, and therefore select it
        /// </summary>
        private void HandleSpherePoints()
        {
            bool changed = false;
            Color colorSelected = Color.green;
            Color colorUnselected = Color.red;

            for (int i = 0; i < Points.Count; i++)
            {
                //determine the color of the point
                Color color = Points[i].IsSelected() ? colorSelected : colorUnselected;

                //if there is only one point selected, show only the visual of the point, not the handleSphere
                if (ExtVector3.IsClose(_currentHandlePosition, Points[i].GetGlobalPointPosition(), 0.3f))
                {
                    if (Points[i].IsSelected())
                    {
                        ExtGravityOverrideEditor.DrawPoint(Points[i].GetGlobalPointPosition(), EditorOptions.Instance.SizeLinesPoints * 0.5f, colorSelected);
                    }
                    else
                    {
                        ExtGravityOverrideEditor.DrawPoint(Points[i].GetGlobalPointPosition(), EditorOptions.Instance.SizeLinesPoints, colorUnselected);
                    }
                    continue;
                }

                ExtGravityOverrideEditor.DrawPoint(false, Points[i].GetGlobalPointPosition(), color, EditorOptions.Instance.SizeLinesPoints, out bool hasChanged);
                if (hasChanged)
                {
                    if (!Event.current.control)
                    {
                        UnSelectPoints();
                    }
                    _lastPositionMoved = Points[i].GetGlobalPointPosition();
                    bool selectionSetting = !Points[i].IsSelected();
                    Points[i].SetSelected(selectionSetting);
                    SetPointsInfoOfLine(Points[i], false);

                    SelectOtherIfThereAreAtExactSamePosition(Points[i], selectionSetting);

                    SetupPointInLines();
                    changed = true;
                }
            }
            if (changed)
            {
                _targetEditor.ApplyModification();
            }
        }

        private void SelectOtherIfThereAreAtExactSamePosition(PointInLines pointClicked, bool selectionSetting)
        {
            Vector3 positionPointClicked = pointClicked.GetGlobalPointPosition();
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i] == pointClicked)
                {
                    continue;
                }
                if (Points[i].GetGlobalPointPosition() == positionPointClicked)
                {
                    Points[i].SetSelected(selectionSetting);
                    SetPointsInfoOfLine(Points[i], false);
                }
            }
        }

        private void HandlePoints(Vector3 middle)
        {
            Vector3 newMiddle = ExtHandle.DoHandleMove(middle, _targetGameObject.transform.rotation, out bool hasChangedPoint);
            if (!hasChangedPoint)
            {
                return;
            }
            _lastPositionMoved = newMiddle;

            Vector3 offsetFromOld = newMiddle - middle;
            for (int i = 0; i < PointsSelected.Count; i++)
            {
                PointsSelected[i].SetGlobalPointPosition(PointsSelected[i].GetGlobalPointPosition() + offsetFromOld);
                MovePoints(_polyLineMatrixInverse, PointsSelected[i]);
            }

            _targetEditor.ApplyModification();
        }

        private void MovePoints(Matrix4x4 inverse, PointInLines point)
        {
            Vector3 p1 = point.P1PropertieGlobal.vector3Value;
            Vector3 p2 = point.P2PropertieGlobal.vector3Value;

            Vector3 inverseP1 = inverse.MultiplyPoint3x4(p1);
            Vector3 inverseP2 = inverse.MultiplyPoint3x4(p2);

            ExtShapeSerializeProperty.MoveLineFromSerializeProperties(point.ExtLineFromLocal, inverseP1, inverseP2);
            ExtShapeSerializeProperty.MoveLineFromSerializeProperties(point.ExtLineFromGlobal, p1, p2);
        }

        private PointsInfo GetPointInfoOfPointLine(int indexLine, int indexPoint)
        {
            PointsInfo pointInfo = new PointsInfo();
            SerializedProperty pointInfoLines = _pointInfosArray.GetArrayElementAtIndex(indexLine * 2 + indexPoint);

            pointInfo.IsAttached = pointInfoLines.GetPropertie("IsAttached").boolValue;
            pointInfo.IsSelected = pointInfoLines.GetPropertie("IsSelected").boolValue;
            return (pointInfo);
        }

        private void SetPointsInfoOfLine(PointInLines point, bool updatePoints)
        {
            SerializedProperty pointInfoLines = _pointInfosArray.GetArrayElementAtIndex(point.IndexLine * 2 + point.IndexPoint);
            pointInfoLines.GetPropertie("IsAttached").boolValue = point.PointInfo.IsAttached;
            pointInfoLines.GetPropertie("IsSelected").boolValue = point.PointInfo.IsSelected;

            if (updatePoints)
            {
                SetupPointInLines();
            }
        }
    }
    */
}