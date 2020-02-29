﻿using ExtUnityComponents;
using ExtUnityComponents.transform;
using feerik.editor.utils;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.editor;
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
using static hedCommon.geometry.shape3d.ExtPolyLines;

namespace philae.gravity.attractor
{
    [CustomEditor(typeof(AttractorPolyLines), true)]
    public class AttractorPolyLinesEditor : AttractorEditor
    {
        protected new AttractorPolyLines _attractor;
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
        private Matrix4x4 _polyLineMatrixInverse;
        private Matrix4x4 _multiHandleMatrix;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorPolyLinesEditor()
            : base(false, "Line")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractor = (AttractorPolyLines)GetTarget<Attractor>();
            _transformHiddedTools = _attractor.gameObject.GetComponent<TransformHiddedTools>();
            Tools.hidden = EditorOptions.Instance.SetupLinesOfSphape;
            _transformHiddedTools.HideHandle = EditorOptions.Instance.SetupLinesOfSphape;

            ConstructPoints();
            UnSelectPoints();
        }

        public void ConstructPoints()
        {
            this.UpdateEditor();
            SetupAllSerializeObject();

            int countLines = _listLinesGlobal.arraySize * 2;
            if (_pointInfosArray.arraySize != countLines)
            {
                _pointInfosArray.arraySize = countLines;
            }
            this.ApplyModification();
            SetupMultiHandleMatrixFromAttractor();
        }

        
        private void SetupMultiHandleMatrixFromAttractor()
        {
            _multiHandleMatrix = Matrix4x4.TRS(_attractor.Position, _attractor.Rotation, _attractor.LocalScale);
        }
        

        private void UnSelectPoints()
        {
            int countLines = _listLinesGlobal.arraySize;

            for (int i = 0; i < countLines; i++)
            {
                PointsInfo pointInfo1 = GetPointInfoOfPointLine(i, 0);
                pointInfo1.IsSelected = false;
                SetPointsInfoOfLine(pointInfo1, i, 0);

                PointsInfo pointInfo2 = GetPointInfoOfPointLine(i, 1);
                pointInfo2.IsSelected = false;
                SetPointsInfoOfLine(pointInfo2, i, 1);
            }
            this.ApplyModification();
        }

        public override void OnCustomDisable()
        {
            base.OnCustomDisable();
            Tools.hidden = false;
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();

            EditorGUI.BeginChangeCheck();
            {
                EditorOptions.Instance.SetupLinesOfSphape = GUILayout.Toggle(EditorOptions.Instance.SetupLinesOfSphape, "Setup Lines", EditorStyles.miniButton);

                if (EditorGUI.EndChangeCheck())
                {
                    Tools.hidden = EditorOptions.Instance.SetupLinesOfSphape;
                    _transformHiddedTools.HideHandle = EditorOptions.Instance.SetupLinesOfSphape;
                }
            }
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (!EditorOptions.Instance.SetupLinesOfSphape || !_attractor.gameObject.activeInHierarchy)
            {
                return;
            }

            this.UpdateEditor();
            SetupAllSerializeObject();

            int countLines = _listLinesGlobal.arraySize * 2;
            if (_pointInfosArray.arraySize != countLines)
            {
                _pointInfosArray.arraySize = countLines;
                return;
            }

            //try to move multiple lines at once, if we can't, move only one at a time
            if (CanMoveMultiplePoints(out Vector3 middle))
            {
                MoveMultiplePoints(middle);
            }
            else
            {
                _isMovingMultiplePoints = false;
                //SetupMultiHandleMatrixFromAttractor();
            }
            MovePointsOfLineOneAtATime();

            FramePointsWithF();
            ManageDragRect();
            AttemptToUselectPoints();

            LockEditor();
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
                GetAllPointInsideSelection();
                _isDragging = false;
            }

            Handles.BeginGUI();
            if (_isDragging)
            {
                float width =  Event.current.mousePosition.x - _initialPositionDrag.x;
                float height = Event.current.mousePosition.y - _initialPositionDrag.y;
                //Debug.Log(width + ", " + height);
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
        private void GetAllPointInsideSelection()
        {
            float width = Event.current.mousePosition.x - _initialPositionDrag.x;
            float height = Event.current.mousePosition.y - _initialPositionDrag.y;
            Rect rect = new Rect(_initialPositionDrag.x, _initialPositionDrag.y, width, height);
            rect = ExtRect.ReverseRectIfNeeded(rect);

            int countLines = _listLinesGlobal.arraySize;

            bool hasChanged = false;

            for (int i = 0; i < countLines; i++)
            {
                SerializedProperty extLineFromGlobal = _listLinesGlobal.GetArrayElementAtIndex(i);
                SerializedProperty p1Propertie = extLineFromGlobal.GetPropertie("_p1");
                SerializedProperty p2Propertie = extLineFromGlobal.GetPropertie("_p2");
                Vector3 p1 = p1Propertie.vector3Value;
                Vector3 p2 = p2Propertie.vector3Value;

                if (ExtRect.Is3dPointInside2dRectInScreenSpace(ExtSceneView.Camera(), rect, p1))
                {
                    PointsInfo pointInfo1 = GetPointInfoOfPointLine(i, 0);
                    pointInfo1.IsSelected = true;
                    SetPointsInfoOfLine(pointInfo1, i, 0);
                    hasChanged = true;
                }
                if (ExtRect.Is3dPointInside2dRectInScreenSpace(ExtSceneView.Camera(), rect, p2))
                {
                    PointsInfo pointInfo2 = GetPointInfoOfPointLine(i, 1);
                    pointInfo2.IsSelected = true;
                    SetPointsInfoOfLine(pointInfo2, i, 1);
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                this.ApplyModification();
            }
        }

        /// <summary>
        /// focus the current selected point moved
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
        private static void LockEditor()
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
        }

        /// <summary>
        /// if clickUp, and mouse position is close to the initial drag position, unselect all points !
        /// </summary>
        private void AttemptToUselectPoints()
        {
            if (ExtEventEditor.IsLeftMouseUp(Event.current) && ExtVector3.IsClose(_initialPositionDrag, _endDragPosition, 0.01f))
            {
                Debug.Log("Unselect all !");
                UnSelectPoints();
            }
        }

        private void SetupAllSerializeObject()
        {
            _extPolyLine = this.GetPropertie("_polyLines");
            _listLinesGlobal = _extPolyLine.GetPropertie("_listLines");
            _listLinesLocal = _extPolyLine.GetPropertie("_listLinesLocal");
            _polyLineMatrixPropertie = _extPolyLine.GetPropertie("_polyLinesMatrix");
            _pointInfosArray = _extPolyLine.GetPropertie("_pointsInfos");
        }


        private bool CanMoveMultiplePoints(out Vector3 middle)
        {
            int countLines = _listLinesGlobal.arraySize;
            middle = Vector3.zero;
            List<Vector3> positions = new List<Vector3>(countLines * 2);
            for (int i = 0; i < countLines; i++)
            {
                SerializedProperty extLineFromGlobal = _listLinesGlobal.GetArrayElementAtIndex(i);

                PointsInfo pointInfo1 = GetPointInfoOfPointLine(i, 0);
                if (pointInfo1.IsSelected)
                {
                    SerializedProperty p1Propertie = extLineFromGlobal.GetPropertie("_p1");
                    Vector3 p1 = p1Propertie.vector3Value;
                    positions.Add(p1);
                }
                PointsInfo pointInfo2 = GetPointInfoOfPointLine(i, 1);
                if (pointInfo2.IsSelected)
                {
                    SerializedProperty p2Propertie = extLineFromGlobal.GetPropertie("_p2");
                    Vector3 p2 = p2Propertie.vector3Value;
                    positions.Add(p2);
                }
            }
            if (positions.Count > 1)
            {
                middle = ExtVector3.GetMeanOfXPoints(positions.ToArray());
                return (true);
            }
            return (false);
        }

        private void MovePointsOfLineOneAtATime()
        {
            SetupMatrixInverse();

            int countLines = _listLinesGlobal.arraySize;
            bool changed = false;

            for (int i = 0; i < countLines; i++)
            {
                SerializedProperty extLineFromGlobal = _listLinesGlobal.GetArrayElementAtIndex(i);
                SerializedProperty p1Propertie = extLineFromGlobal.GetPropertie("_p1");
                SerializedProperty p2Propertie = extLineFromGlobal.GetPropertie("_p2");
                Vector3 p1 = p1Propertie.vector3Value;
                Vector3 p2 = p2Propertie.vector3Value;
                changed = ShowPointHandle(_polyLineMatrixInverse, changed, i, p1, p2, 0);
                changed = ShowPointHandle(_polyLineMatrixInverse, changed, i, p1, p2, 1);
            }
            if (changed)
            {
                this.ApplyModification();
            }
        }

        private void SetupMatrixInverse()
        {
            _polyLineMatrixInverse = _polyLineMatrixPropertie.GetValue<Matrix4x4>().inverse;
        }

        private void MoveMultiplePoints(Vector3 middle)
        {
            
            if (_isMovingMultiplePoints)
            {
                _multiHandleMatrix = Matrix4x4.TRS(middle, _attractor.Rotation, _attractor.LocalScale);
            }
            
            _isMovingMultiplePoints = true;

            ExtHandle.DoMultiHandle(ref _multiHandleMatrix, out bool hasChanged);

            //Vector3 newMiddle = ExtHandle.DoHandleMove(middle, _attractor.Rotation, out bool hasChangedPoint);
            if (!hasChanged)
            {
                return;
            }
            Vector3 newMiddle = _multiHandleMatrix.ExtractPosition();

            Debug.Log("middle: " + newMiddle);
            SetupMatrixInverse();
            Vector3 offsetFromOld = newMiddle - middle;
            Vector3 offsetFromAttractor = newMiddle - _attractor.Position;

            int countLines = _listLinesGlobal.arraySize;

            for (int i = 0; i < countLines; i++)
            {
                //_multiHandleMatrix.SetPosition(_attractor.Position);

                SerializedProperty extLineFromLocal = _listLinesLocal.GetArrayElementAtIndex(i);
                SerializedProperty p1PropertieLocal = extLineFromLocal.GetPropertie("_p1");
                SerializedProperty p2PropertieLocal = extLineFromLocal.GetPropertie("_p2");
                Vector3 p1Local = p1PropertieLocal.vector3Value;
                Vector3 p2Local = p2PropertieLocal.vector3Value;

                SerializedProperty extLineFromGlobal = _listLinesGlobal.GetArrayElementAtIndex(i);
                SerializedProperty p1Propertie = extLineFromGlobal.GetPropertie("_p1");
                SerializedProperty p2Propertie = extLineFromGlobal.GetPropertie("_p2");
                Vector3 p1 = p1Propertie.vector3Value;
                Vector3 p2 = p2Propertie.vector3Value;

                //_multiHandleMatrix.SetPosition(_attractor.Position);

                Vector3 p1Moved = _multiHandleMatrix.MultiplyPoint3x4(p1Local);
                Vector3 p2Moved = _multiHandleMatrix.MultiplyPoint3x4(p2Local);
                ExtDrawGuizmos.DebugWireSphere(p1Moved - offsetFromAttractor, Color.white, 0.1f, 1f);
                ExtDrawGuizmos.DebugWireSphere(p2Moved - offsetFromAttractor, Color.white, 0.1f, 1f);

                //_multiHandleMatrix.SetPosition(newMiddle);
                /*
                p1Local += offsetFromAttractor;
                p2Local += offsetFromAttractor;
                */



                PointsInfo pointInfo = GetPointInfoOfPointLine(i, 0);
                if (pointInfo.IsSelected)
                {
                    Debug.Log("p1: " + p1);
                    p1 = p1 + offsetFromOld;

                    //here change offset of p1
                }
                PointsInfo pointInfo2 = GetPointInfoOfPointLine(i, 1);
                if (pointInfo2.IsSelected)
                {
                    Debug.Log("p2: " + p2);
                    p2 = p2 + offsetFromOld;
                }
                if (pointInfo.IsSelected || pointInfo2.IsSelected)
                {
                    MovePointsOfLine(_polyLineMatrixInverse, p1, p2, i);
                }

                //changed = ShowPointHandle(_polyLineMatrixInverse, changed, i, p1, p2, 0);
                //changed = ShowPointHandle(_polyLineMatrixInverse, changed, i, p1, p2, 1);
            }


            this.ApplyModification();


            //_multiHandleMatrix = Matrix4x4.TRS(middle, _attractor.Rotation, _attractor.LocalScale);
        }

        private bool MovePointBasedOnMatrixOffset()
        {

            return (false);
        }

        private bool ShowPointHandle(Matrix4x4 polyLineMatrixInverse, bool changed, int i, Vector3 p1, Vector3 p2, int indexPoint)
        {
            PointsInfo pointInfo = GetPointInfoOfPointLine(i, indexPoint);
            if (!pointInfo.IsSelected)
            {
                ExtGravityOverrideEditor.DrawPoint(false, (indexPoint == 0) ? p1 : p2, Color.red, 0.5f, out bool hasChanged);
                if (hasChanged)
                {
                    pointInfo.IsSelected = true;
                    if (_lastPositionMoved == Vector3.zero)
                    {
                        _lastPositionMoved = (indexPoint == 0) ? p1 : p2;
                    }
                    SetPointsInfoOfLine(pointInfo, i, indexPoint);
                    changed = true;
                }
            }
            else
            {
                if (_isMovingMultiplePoints)
                {
                    //here don't draw, because we have selected multiple points;
                    Handles.color = Color.green;
                    Handles.SphereHandleCap(0, (indexPoint == 0) ? p1 : p2, Quaternion.identity, 0.5f, EventType.Repaint);
                }
                else
                {
                    bool hasChanged = MovePointHandle(polyLineMatrixInverse, p1, p2, indexPoint, i);
                    changed = (!changed) ? hasChanged : changed;
                }
            }

            return changed;
        }

        private bool MovePointHandle(Matrix4x4 inverse, Vector3 p1, Vector3 p2, int indexPoint, int indexLine)
        {
            bool hasChangedPoint = false;
            if (indexPoint == 0)
            {
                p1 = ExtHandle.DoHandleMove(p1, _attractor.Rotation, out hasChangedPoint);
                _lastPositionMoved = hasChangedPoint ? p1 : _lastPositionMoved;
            }
            else
            {
                p2 = ExtHandle.DoHandleMove(p2, _attractor.Rotation, out hasChangedPoint);
                _lastPositionMoved = hasChangedPoint ? p2 : _lastPositionMoved;
            }

            if (hasChangedPoint)
            {
                MovePointsOfLine(inverse, p1, p2, indexLine);
                return (true);
            }
            return (false);
        }

        private void MovePointsOfLine(Matrix4x4 inverse, Vector3 p1, Vector3 p2, int indexLine)
        {
            Debug.Log("here changed for line: " + indexLine);
            SerializedProperty extLineFromLocal = _listLinesLocal.GetArrayElementAtIndex(indexLine);
            SerializedProperty extLineFromGlobal = _listLinesGlobal.GetArrayElementAtIndex(indexLine);
            Vector3 inverseP1 = inverse.MultiplyPoint3x4(p1);
            Vector3 inverseP2 = inverse.MultiplyPoint3x4(p2);

            ExtShapeSerializeProperty.MoveLineFromSerializeProperties(extLineFromLocal, inverseP1, inverseP2);
            ExtShapeSerializeProperty.MoveLineFromSerializeProperties(extLineFromGlobal, p1, p2);
        }

        private PointsInfo GetPointInfoOfPointLine(int indexLine, int indexPoint)
        {
            PointsInfo pointInfo = new PointsInfo();
            SerializedProperty pointInfoLines = _pointInfosArray.GetArrayElementAtIndex(indexLine * 2 + indexPoint);

            pointInfo.IsAttached = pointInfoLines.GetPropertie("IsAttached").boolValue;
            pointInfo.IsSelected = pointInfoLines.GetPropertie("IsSelected").boolValue;
            pointInfo.AttachedIndex = ExtSerializedProperties.GetListInt(pointInfoLines.GetPropertie("AttachedIndex"));
            return (pointInfo);
        }

        private void SetPointsInfoOfLine(PointsInfo pointInfo, int indexLine, int indexPoint)
        {
            SerializedProperty pointInfoLines = _pointInfosArray.GetArrayElementAtIndex(indexLine * 2 + indexPoint);
            pointInfoLines.GetPropertie("IsAttached").boolValue = pointInfo.IsAttached;
            pointInfoLines.GetPropertie("IsSelected").boolValue = pointInfo.IsSelected;
            ExtSerializedProperties.SetListInt(pointInfoLines.GetPropertie("AttachedIndex"), pointInfo.AttachedIndex);
        }
    }
}