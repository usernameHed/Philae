using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace extUnityComponents.transform
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

        private ShowInfoGameObject _showInfoGameObject = new ShowInfoGameObject();



        private float _saveDeltaSizeRatio;
        private static bool _homothety;
        private bool _isCanvas;
        private TransformHiddedTools[] _rectTransformHiddedTools;


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
            _showInfoGameObject.CustomDisable();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            _showInfoGameObject.CustomOnSceneGUI();
        }

        /// <summary>
        /// This is called at the first OnInspectorGUI()
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
        {
            Transform[] tmpTargets = GetTargets<Transform>();
            _rectTransformHiddedTools = new TransformHiddedTools[tmpTargets.Length];
            for (int i = 0; i < tmpTargets.Length; i++)
            {
                _rectTransformHiddedTools[i] = tmpTargets[i].GetOrAddComponent<TransformHiddedTools>();
            }
            _canvasRef = ConcretTarget.gameObject.GetComponent<Canvas>();
            _showInfoGameObject.Init(GetTargets<Transform>(), this, _rectTransformHiddedTools);
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
            _showInfoGameObject.CustomOnInspectorGUI();
        }

        /// <summary>
        /// called if this rectTransform is a canvas.
        /// </summary>
        private void DisplayCanvasGUI()
        {

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
        }
    }
}
