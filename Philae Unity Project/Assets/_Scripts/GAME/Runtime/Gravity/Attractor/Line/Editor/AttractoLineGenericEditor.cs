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
    public class AttractoLineGenericEditor
    {
        private GameObject _targetGameObject;
        private Editor _targetEditor;

        private TransformHiddedTools _transformHiddedTools;

        private Vector3 _lastPositionMoved;

        private SerializedProperty _matrixPropertie;
        public List<PointInLines> Points = new List<PointInLines>(20);
        public List<PointInLines> PointsSelected = new List<PointInLines>(20);
        public delegate void NeedToUpdateLines();
        private NeedToUpdateLines _linesPointsHasBeenUpdated;


        private Vector2 _initialPositionDrag;
        private Vector2 _endDragPosition;
        private bool _isDragging = false;
        private Matrix4x4 _polyLineMatrix;
        private Matrix4x4 _polyLineMatrixInverse;

        private Vector3 _currentHandlePosition;

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public void OnCustomEnable(Editor targetEditor, GameObject targetGameObject, NeedToUpdateLines needToUpdateLines)
        {
            _targetEditor = targetEditor;
            _targetGameObject = targetGameObject;
            _linesPointsHasBeenUpdated = needToUpdateLines;

            _transformHiddedTools = _targetGameObject.GetComponent<TransformHiddedTools>();
            Tools.hidden = EditorOptions.Instance.SetupLinesOfSphape;
            _transformHiddedTools.HideHandle = EditorOptions.Instance.SetupLinesOfSphape;
        }

        public void ConstructLines(SerializedProperty matrix, List<PointInLines> pointInLines)
        {
            _matrixPropertie = matrix;

            Points = pointInLines;
            PointsSelected.Clear();

            _polyLineMatrix = _matrixPropertie.GetValue<Matrix4x4>();
            _polyLineMatrixInverse = _polyLineMatrix.inverse;
        }

        /*
        public void UpdateLines(SerializedProperty matrix)
        {
            _matrixPropertie = matrix;
            _polyLineMatrix = _matrixPropertie.GetValue<Matrix4x4>();
            _polyLineMatrixInverse = _polyLineMatrix.inverse;
        }
        */

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

            _currentHandlePosition = GetMiddleOfAllSelectedPoints();
            HandlePoints(_currentHandlePosition);

            HandleSpherePoints();

            FramePointsWithF();
            ManageDragRect();
            AttemptToUselectPoints();
            LockEditor();
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
                    Debug.Log("exeption");
                    continue;
                }
                Debug.Log("unselect point " + PointsSelected[i]);
                PointsSelected[i].SetSelected(false);
            }
            FillSelectedPoints();
        }

        /// <summary>
        /// called on a Tiny Editor Content
        /// </summary>
        public void ShowTinyEditorContent()
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorOptions.Instance.SetupLinesOfSphape = GUILayout.Toggle(EditorOptions.Instance.SetupLinesOfSphape, "Setup Lines", EditorStyles.miniButton);

                if (EditorGUI.EndChangeCheck())
                {
                    SetupLineChanged();
                }
            }
        }

        private void SetupLineChanged()
        {
            Tools.hidden = EditorOptions.Instance.SetupLinesOfSphape;
            _lastPositionMoved = _targetGameObject.transform.position;
            _transformHiddedTools.HideHandle = EditorOptions.Instance.SetupLinesOfSphape;
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
                _initialPositionDrag = Event.current.mousePosition;
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
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                FillSelectedPoints();
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
                EditorOptions.Instance.SetupLinesOfSphape = false;
                SetupLineChanged();
            }

            if (EditorOptions.Instance.SetupLinesOfSphape && ExtEventEditor.IsKeyDown(KeyCode.Delete))
            {
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
                Debug.Log("unselect ?");
                UnSelectPoints();
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
            //bool changed = false;
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
                    SelectOtherIfThereAreAtExactSamePosition(Points[i], selectionSetting);
                    FillSelectedPoints();
                }
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
                }
            }
        }

        private void FillSelectedPoints()
        {
            PointsSelected.Clear();
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].IsSelected())
                {
                    PointsSelected.Add(Points[i]);
                }
            }
        }

        private void HandlePoints(Vector3 middle)
        {
            if (PointsSelected.Count < 1)
            {
                return;
            }

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
                PointsSelected[i].UpdateLocalPositionFromGlobalPosition(_polyLineMatrixInverse);
            }
            _linesPointsHasBeenUpdated?.Invoke();
            _targetEditor.ApplyModification();
        }
    }
}