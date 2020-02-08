using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


namespace hedCommon.tools
{
    [CustomEditor(typeof(FOVVisualize))]
    public class FOVVisualizeEditor : OdinEditor
    {
        FOVVisualize _fOVVisualize;
        ArcHandle arcHandle;

        private new void OnEnable()
        {
            _fOVVisualize = (FOVVisualize)target;
            arcHandle = new ArcHandle();
            arcHandle.SetColorWithRadiusHandle(Color.red, 0.2f);
        }

        private void OnSceneGUI()
        {
            if (!_fOVVisualize.EnableVisual)
            {
                return;
            }

            Quaternion rotation = _fOVVisualize.transform.rotation * Quaternion.Euler(_fOVVisualize.DefaultUp.x, _fOVVisualize.DefaultUp.y, _fOVVisualize.DefaultUp.z);

            Matrix4x4 handleMatrix = Matrix4x4.TRS(_fOVVisualize.transform.position, rotation, Vector3.one);
            arcHandle.angle = _fOVVisualize.angle;
            arcHandle.radius = _fOVVisualize.radius;

            Handles.color = _fOVVisualize.colorFov;
            using (new Handles.DrawingScope(handleMatrix))
            {
                Handles.DrawLine(Vector3.zero, Vector3.up * arcHandle.radius);
                arcHandle.DrawHandle();
            }
            _fOVVisualize.radius = arcHandle.radius;
            _fOVVisualize.angle = arcHandle.angle;
        }
    }
}