using ExtUnityComponents;
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

namespace philae.gravity.attractor
{
    [CustomEditor(typeof(AttractorPolyLines), true)]
    public class AttractorPolyLinesEditor : AttractorEditor
    {
        protected new AttractorPolyLines _attractor;
        private TransformHiddedTools _transformHiddedTools;

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
            SetupLines();
        }

        private void SetupLines()
        {
            this.UpdateEditor();

            SerializedProperty extPolyLine = this.GetPropertie("_polyLines");
            SerializedProperty listLinesGlobal = extPolyLine.GetPropertie("_listLines");
            SerializedProperty listLinesLocal = extPolyLine.GetPropertie("_listLinesLocal");
            SerializedProperty polyLineMatrixPropertie = extPolyLine.GetPropertie("_polyLinesMatrix");

            Matrix4x4 polyLineMatrixInverse = polyLineMatrixPropertie.GetValue<Matrix4x4>().inverse;

            int countLines = listLinesGlobal.arraySize;
            bool changed = false;
            
            for (int i = 0; i < countLines; i++)
            {
                SerializedProperty extLineFromGlobal = listLinesGlobal.GetArrayElementAtIndex(i);
                SerializedProperty p1Propertie = extLineFromGlobal.GetPropertie("_p1");
                SerializedProperty p2Propertie = extLineFromGlobal.GetPropertie("_p2");
                Vector3 p1 = p1Propertie.vector3Value;
                Vector3 p2 = p2Propertie.vector3Value;

                Vector3 point1 = ExtHandle.DoHandleMove(p1, _attractor.Rotation, out bool hasChangedPoint1);
                Vector3 point2 = ExtHandle.DoHandleMove(p2, _attractor.Rotation, out bool hasChangedPoint2);
                if (hasChangedPoint1 || hasChangedPoint2)
                {
                    Debug.Log(point1);
                    SerializedProperty extLineFromLocal = listLinesLocal.GetArrayElementAtIndex(i);
                    changed = true;
                    Vector3 inverseP1 = polyLineMatrixInverse.MultiplyPoint3x4(point1);
                    Vector3 inverseP2 = polyLineMatrixInverse.MultiplyPoint3x4(point2);

                    ExtShapeSerializeProperty.MoveLineFromSerializeProperties(extLineFromLocal, inverseP1, inverseP2);
                    ExtShapeSerializeProperty.MoveLineFromSerializeProperties(extLineFromGlobal, point1, point2);
                }
            }
            if (changed)
            {
                this.ApplyModification();
            }
            /*
            ExtConeSphereBase capsule = this.GetPropertie("_cone").GetValue<ExtConeSphereBase>();
            GravityOverrideConeSphereBase gravitySphereBase = ExtGravityOverrideEditor.DrawConeSphereBase(capsule, _attractor.GravityOverride, 0.5f, out bool hasChanged);

            if (hasChanged)
            {
                gravitySphereBase.SetupGravity();
                ExtGravityOverrideEditor.ApplyModificationToConeSphereBase(this.GetPropertie("GravityOverride"), gravitySphereBase);
                this.ApplyModification();
            }
            */
        }
    }
}