using extUnityComponents;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.splines
{
    [CustomEditor(typeof(CustomSplineController))]
    public class CustomSplineControllerEditor : DecoratorComponentsEditor
    {
        private CustomSplineController _customSplineController;
        private PositionHandleChildsSplineController PositionHandleChildsSplineController;

        public CustomSplineControllerEditor()
            : base(showExtension: false, tinyEditorName: "Custom Controller")
        {

        }
        public override void OnCustomEnable()
        {
            _customSplineController = GetTarget<CustomSplineController>();
            if (_customSplineController == null)
            {
                return;
            }
            PositionHandleChildsSplineController = _customSplineController.transform.GetComponent<PositionHandleChildsSplineController>();
        }

        public override void ShowTinyEditorContent()
        {
            GUILayout.Label("Hold shift to change offset");
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                if (GUILayout.Button("Play/Pause"))
                {
                    _customSplineController.AutomaticMove = !_customSplineController.AutomaticMove;
                }

                if (GUILayout.Button("Reset Offset"))
                {
                    _customSplineController.SetOffsetAngle(0, false);
                    _customSplineController.SetOffsetRadius(0, true);
                }
            }
            _customSplineController.LockToCurrentWorldPosition = GUILayout.Toggle(_customSplineController.LockToCurrentWorldPosition, "Lock To Position");
        }

        /// <summary>
        /// set IsMovingEditor to false if mouse is'nt dragging the object
        /// </summary>
        private void TrySetIsMovingToFalse()
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            EventType eventType = Event.current.GetTypeForControl(controlID);

            if (_customSplineController.IsMovingInEditor && eventType == EventType.MouseUp)
            {
                _customSplineController.IsMovingInEditor = false;
            }
        }

        /// <summary>
        /// apply the handle of the CustomSplineController
        /// </summary>
        /// <param name="modifiedSplineController"></param>
        /// <param name="i"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static bool DoHandle(CustomSplineController customSplineController)
        {
            CustomSplineControllerEditor editorSpline = (CustomSplineControllerEditor)CustomSplineControllerEditor.CreateEditor(customSplineController);
            bool modifiedSplineController = editorSpline.HandleController();
            DestroyImmediate(editorSpline);
            return modifiedSplineController;
        }

        /// <summary>
        /// manage the handle, and return true if modified
        /// </summary>
        /// <returns></returns>
        public bool HandleController(bool canApplyOffset = true)
        {
            bool modifiedSplineController = false;

            TrySetIsMovingToFalse();

            Transform child = _customSplineController.transform;
            if (!child)
            {
                return (modifiedSplineController);
            }
            if (child.gameObject.activeInHierarchy)
            {
                Undo.RecordObject(child.gameObject.transform, "handle camPoint move");
                Vector3 newPosition;

                if (Tools.pivotRotation == PivotRotation.Local)
                {
                    newPosition = Handles.PositionHandle(child.position, child.rotation);
                }
                else
                {
                    newPosition = Handles.PositionHandle(child.position, Quaternion.identity);
                }

                if (newPosition != child.position)
                {
                    modifiedSplineController = true;

                    if (Event.current != null && Event.current.shift && canApplyOffset)
                    {
                        ApplyOffsetHandle(child, newPosition);
                    }
                    else
                    {
                        if (_customSplineController.CurvySpline == null)
                        {
                            return (false);
                        }
                        _customSplineController.IsMovingInEditor = true;
                        _customSplineController.AutomaticMove = false;

                        Vector3 localPositionFromSpline = newPosition;

                        if (_customSplineController.PositionLocal)
                        {
                            //localPositionFromSpline = newPosition - _customSplineController.CurvySpline.transform.position;
                        }

                        _customSplineController.SetPercent(_customSplineController.CurvySpline.GetNearestPointTF(localPositionFromSpline), true, FluffyUnderware.Curvy.CurvyClamping.Loop);
                    }
                }
            }
            return (modifiedSplineController);
        }

        /// <summary>
        /// apply only the offset of the Handle
        /// </summary>
        /// <param name="child"></param>
        /// <param name="newPosition"></param>
        public void ApplyOffsetHandle(Transform child, Vector3 newPosition)
        {
            Vector3 closestPosInSpline = _customSplineController.CurvySpline.InterpolateFast(_customSplineController.GetPercent());
            float diff = (newPosition - closestPosInSpline).magnitude;

            Vector3 B = (newPosition - closestPosInSpline).normalized;

            float dot = Vector3.Dot(B, child.right);
            float angle = 0;
            if (dot == 0)
            {
                angle = 0;

            }
            else
            {
                float sign = Mathf.Sign(dot);
                angle = Mathf.Acos(Vector3.Dot(B, child.up));
                angle = -sign * angle;
                angle = Mathf.Rad2Deg * angle;
            }

            if (_customSplineController.SnapToAngle.Snap)
            {
                if (angle < 0)
                {
                    diff *= -1;
                }
                angle = _customSplineController.SnapToAngle.Angle;
            }

            _customSplineController.SetOffsetAngle(angle, false);
            _customSplineController.SetOffsetRadius(diff, true);
        }
    }
}