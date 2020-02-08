using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;


namespace hedCommon.splines
{
    [CustomEditor(typeof(PositionHandleChildsSplineController))]
    public class PositionHandleChildsSplineControllerEditor : OdinEditor
    {
        private PositionHandleChildsSplineController _positionHandle;
        private Tool LastTool = Tool.None;
        private ExtUtilityEditor.HitSceneView hitScene;

        private new void OnEnable()
        {
            _positionHandle = (PositionHandleChildsSplineController)target;
            LastTool = Tools.current;
            Tools.current = Tool.None;
        }

        private void LoopThoughtMove()
        {
            if (_positionHandle._allChildToMove == null)
                return;

            for (int i = 0; i < _positionHandle._allChildToMove.Count; i++)
            {
                if (_positionHandle._allChildToMove[i] == null)
                {
                    continue;
                }

                Transform child = _positionHandle._allChildToMove[i].transform;
                if (!child)
                    continue;

                if (child.gameObject.activeInHierarchy)
                {
                    Undo.RecordObject(child.gameObject.transform, "handle camPoint move");

                    if (Tools.pivotRotation == PivotRotation.Local)
                    {
                        child.position = Handles.PositionHandle(child.position, child.rotation);
                    }
                    else
                    {
                        child.position = Handles.PositionHandle(child.position, Quaternion.identity);
                    }
                }
            }
        }

        private bool LoopThoughtSplineControllers()
        {
            if (_positionHandle.AllSplineControllerMove == null)
                return (false);

            bool modifiedSplineController = false;

            for (int i = 0; i < _positionHandle.AllSplineControllerMove.Count; i++)
            {
                if (_positionHandle.AllSplineControllerMove[i] == null)
                {
                    continue;
                }

                Transform child = _positionHandle.AllSplineControllerMove[i].transform;
                if (!child)
                    continue;

                if (child.gameObject.activeInHierarchy)
                {
                    modifiedSplineController = CustomSplineControllerEditor.DoHandle(_positionHandle.AllSplineControllerMove[i]);
                }
            }
            return (modifiedSplineController);
        }

        private void LoopThoughtRotate()
        {
            if (_positionHandle._allChildToRotate == null)
                return;

            for (int i = 0; i < _positionHandle._allChildToRotate.Count; i++)
            {
                Transform child = _positionHandle._allChildToRotate[i];
                if (!child)
                    continue;

                if (child.gameObject.activeInHierarchy)
                {
                    Undo.RecordObject(child.gameObject.transform, "handle camPoint rotation");
                    child.rotation = Handles.RotationHandle(child.rotation, child.position);
                }
            }
        }

        private void HandleChild()
        {
            if (Event.current.alt)
                return;
            if (!_positionHandle.ShowHandler)
                return;


            if (!LoopThoughtSplineControllers())
            {
                LoopThoughtMove();
                LoopThoughtRotate();
            }

        }

        /// <summary>
        /// all event of the RaceScene UI 
        /// </summary>
        private void EventInput(SceneView view, Event e)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    break;
            }

            /*
            //clic on gost
            //click on spline: move player
            if (hitScene.objHit != null && e.control && e.alt && e.button == 1 && e.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseDown)
            {
                DeleteChild(hitScene.objHit);
            }
            if (e.control && e.alt && e.button == 0 && e.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseDown)
            {
                CreateChild(hitScene.pointHit, hitScene.normal);
            }
            */
        }

        /// <summary>
        /// try to duplicate one from array
        /// </summary>
        /// <param name="position"></param>
        /// <param name="normal"></param>
        private void CreateChild(Vector3 position, Vector3 normal)
        {
            //defaultParent
            if (_positionHandle._allChildToMove.Count <= 0)
                return;

            Instantiate(_positionHandle._allChildToMove[0], position, ExtQuaternion.DirToQuaternion(normal), _positionHandle.defaultParent);
        }

        private void OnSceneGUI()
        {
            if (_positionHandle.canDelete)
            {
                SceneView view = SceneView.currentDrawingSceneView;
                Event e = Event.current;

                hitScene = ExtUtilityEditor.SetCurrentOverObject(hitScene, true);

                EventInput(view, e);
            }
            HandleChild();
        }

        private new void OnDisable()
        {
            Tools.current = LastTool;
        }
    }
}