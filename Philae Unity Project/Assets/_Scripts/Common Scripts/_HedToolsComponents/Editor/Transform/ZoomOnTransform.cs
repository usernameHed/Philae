using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.time;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace extUnityComponents.transform
{
    public class ZoomOnTransform
    {
        private Transform[] _currentTargets = null;
        private float _zoomMagnitude = 5f;
        private Editor _currentEditor;
        private EditorChronoWithNoTimeEditor _coolDown = new EditorChronoWithNoTimeEditor();
        private readonly string KEY_EDITOR_PREF_ZOOM_MAGNITUDE = "KEY_EDITOR_PREF_ZOOM_MAGNITUDE";
        private AnimationCurve _easeZoom = new AnimationCurve();

        public void Init(Transform[] targets, Editor currentEditor)
        {
            _easeZoom = AnimationCurve.EaseInOut(0, 0, 1, 1);
            _currentEditor = currentEditor;
            _currentTargets = targets;
            if (EditorPrefs.HasKey(KEY_EDITOR_PREF_ZOOM_MAGNITUDE))
            {
                _zoomMagnitude = EditorPrefs.GetFloat(KEY_EDITOR_PREF_ZOOM_MAGNITUDE);
            }
            else
            {
                EditorPrefs.SetFloat(KEY_EDITOR_PREF_ZOOM_MAGNITUDE, _zoomMagnitude);
            }
        }

        public void CustomDisable()
        {

        }

        public static void ViewPortZoomIn(float zoom)
        {
            SceneView.lastActiveSceneView.size = zoom;
            SceneView.lastActiveSceneView.Repaint();
        }

        public void FocusOnSelection(Transform[] objToFocus, float zoom = -1)
        {
            Vector3 positionToFocus = ExtVector3.GetMeanOfXPoints(objToFocus, out Vector3 sizeBoundingBox);
            SceneView.lastActiveSceneView.LookAt(positionToFocus, SceneView.lastActiveSceneView.camera.transform.rotation, zoom);
        }

        public void CustomOnSceneGUI()
        {
            if ((Event.current.keyCode == KeyCode.J && Event.current.type == EventType.KeyUp))
            {
                FocusOnSelection(_currentTargets, _zoomMagnitude);
            }
        }

        public void CustomOnInspectorGUI()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Focus [J]:");
                _zoomMagnitude = ExtGUIFloatFields.FloatField(_zoomMagnitude, null, out bool valueHasChanged, "amount:", "amount of zoom", 0.01f);
                if (valueHasChanged)
                {
                    EditorPrefs.SetFloat(KEY_EDITOR_PREF_ZOOM_MAGNITUDE, _zoomMagnitude);
                }

                if (GUILayout.Button("Zoom", EditorStyles.miniButton))
                {
                    FocusOnSelection(_currentTargets, _zoomMagnitude);
                }
            }
        }
    }
}