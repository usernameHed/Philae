using hedCommon.extension.runtime;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Custom Transform editor.
/// </summary>
namespace extUnityComponents.transform
{
    [CustomEditor(typeof(Transform))]
    [CanEditMultipleObjects]
    public class TransformEditor : DecoratorComponentsEditor
    {
        private ResetTransformGUI _resetTransformGUI = new ResetTransformGUI(); //ref to script
        private LockChildren _lockChildren = new LockChildren();    //ref to script
        private ShowInfoGameObject _showInfoGameObject = new ShowInfoGameObject();

        private TransformHiddedTools[] _transformHiddedTools;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// </summary>
        public TransformEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.TransformInspector)
        {

        }

        /// <summary>
        /// need to clean when quitting
        /// </summary>
        public override void OnCustomDisable()
        {
            _resetTransformGUI.CustomDisable();
            _lockChildren.CustomDisable();
            _showInfoGameObject.CustomDisable();
        }

        /// <summary>
        /// this function is called on the first OnSceneGUI
        /// </summary>
        /// <param name="sceneview"></param>
        protected override void InitOnFirstOnSceneGUI(SceneView sceneview)
        {
            _lockChildren.InitOnFirstOnSceneGUI();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            _lockChildren.CustomOnSceneGUI();
            _showInfoGameObject.CustomOnSceneGUI();
        }

        /// <summary>
        /// init the targets inspected and store them.
        /// This is called at the first OnInspectorGUI()
        /// Not in the constructor ! we don't have targets information in the constructor
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
        {
            Transform current = GetTarget<Transform>();
            if (current == null || ExtPrefabs.IsEditingInPrefabMode(current.gameObject))
            {
                return;
            }

            Transform[] tmpTargets = GetTargets<Transform>();
            _transformHiddedTools = new TransformHiddedTools[tmpTargets.Length];
            for (int i = 0; i < tmpTargets.Length; i++)
            {
                _transformHiddedTools[i] = tmpTargets[i].GetOrAddComponent<TransformHiddedTools>();
            }

            _resetTransformGUI.Init(GetTargets<Transform>());
            _lockChildren.Init(current, this);
            _showInfoGameObject.Init(GetTargets<Transform>(), this, _transformHiddedTools);
        }

        /// <summary>
        /// This function is called at each OnInspectorGUI
        /// </summary>
        protected override void OnCustomInspectorGUI()
        {
            _resetTransformGUI.CustomOnInspectorGUI();
            if (targets.Length < 2)
            {
                _lockChildren.CustomOnInspectorGUI();
            }
            _showInfoGameObject.CustomOnInspectorGUI();
        }

        /// <summary>
        /// this function is called at each EditorUpdate() when we need it
        /// </summary>
        protected override void OnEditorUpdate()
        {
            _lockChildren.OnEditorUpdate();
        }
    }
}


