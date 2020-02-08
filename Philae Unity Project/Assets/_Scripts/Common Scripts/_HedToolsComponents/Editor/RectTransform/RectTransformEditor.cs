using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace ExtUnityComponents.transform
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(RectTransform))]
    public class RectTransformEditor : DecoratorComponentsEditor
    {
        /// <summary>
        /// we are not selecting multiple targets, because one of them could
        /// be the canvas, and we don't want to apply the same behavior to the canvas
        /// </summary>
        private Canvas _canvasRef;

        
        private float _saveDeltaSizeRatio;
        private static bool _homothety;
        private bool _isCanvas;
        private RectTransformHiddedTools _rectTransformHiddedTools;


        public RectTransformEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.RectTransformEditor)
        {

        }

        /// <summary>
        /// called on enable
        /// </summary>
        public override void OnCustomEnable()
        {

        }

        /// <summary>
        /// need to clean when quitting
        /// </summary>
        public override void OnCustomDisable()
        {

        }

        /// <summary>
        /// This is called at the first OnInspectorGUI()
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
        {
            _rectTransformHiddedTools = ConcretTarget.GetOrAddComponent<RectTransformHiddedTools>();
            _canvasRef = ConcretTarget.gameObject.GetComponent<Canvas>();
        }

        /// <summary>
        /// This function is called at each OnInspectorGUI
        /// </summary>
        protected override void OnCustomInspectorGUI()
        {
            if (_canvasRef != null)
            {
                DisplayCanvasGUI();
            }
            else
            {
                DisplayNotCanvaGUI();
            }
        }

        /// <summary>
        /// called if this rectTransform is a canvas.
        /// </summary>
        private void DisplayCanvasGUI()
        {

        }

        /// <summary>
        /// this option allow to scale the object and keep his aspect ratio.
        /// </summary>
        private void ApplyLockHomotetie()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Locked Scaling Ratio");
                //_homothety = GUILayout.Toggle(_homothety, (_homothety) ? "Homothetic" : "Free", EditorStyles.miniButton);
                _homothety = ExtGUIToggles.Toggle(_homothety, null, (_homothety) ? "Homothetic" : "Free", out bool valueHasChanged, EditorStyles.miniButton);
                if (valueHasChanged && _homothety)
                {
                    _saveDeltaSizeRatio = (((RectTransform)ConcretTarget).sizeDelta.y / ((RectTransform)ConcretTarget).sizeDelta.x + 0.00001f);
                }
            }
        }

        /// <summary>
        /// called if this rectTransform is NOT a canvas
        /// </summary>
        private void DisplayNotCanvaGUI()
        {
            RectTransform rectTarget = (RectTransform)ConcretTarget;

            using (HorizontalScope horizontalScope = new HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Reset");
                if (GUILayout.Button("Position"))
                {
                    Undo.RecordObject(rectTarget.transform, GetType().FullName);

                    rectTarget.localPosition = Vector3.zero;
                }
                if (GUILayout.Button("Delta"))
                {
                    Undo.RecordObject(rectTarget.transform, GetType().FullName);

                    rectTarget.sizeDelta = new Vector2(100, 100);
                }
                if (GUILayout.Button("Rotation"))
                {
                    Undo.RecordObject(rectTarget.transform, GetType().FullName);

                    rectTarget.localRotation = Quaternion.identity;

                }
                if (GUILayout.Button("Scale"))
                {
                    Undo.RecordObject(rectTarget.transform, GetType().FullName);

                    rectTarget.localScale = Vector3.one;
                }
            }

            ApplyLockHomotetie();
        }

        protected override void OnEditorUpdate()
        {
            float y = ((RectTransform)ConcretTarget).sizeDelta.x * _saveDeltaSizeRatio;
            ((RectTransform)ConcretTarget).sizeDelta = new Vector2(((RectTransform)ConcretTarget).sizeDelta.x, y);
        }
    }
}
