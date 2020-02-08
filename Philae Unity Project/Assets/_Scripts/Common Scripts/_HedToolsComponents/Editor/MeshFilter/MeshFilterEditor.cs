using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

/*
/// <summary>
/// Custom Transform editor.
/// </summary>
namespace ExtUnityComponents
{
    [CustomEditor(typeof(MeshFilter))]
    public class MeshFilterEditor : DecoratorComponentsEditor
    {
        //I choose to get both target & targets here to show both methods with single and multiple objects selected
        //private MeshFilter _concretTarget;                //targets we inspect
        private MeshFilterHiddedTools _meshFilterHiddedTools;

        //private MovePivot _movePivot = new MovePivot();

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// </summary>
        public MeshFilterEditor()
            : base()
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
            //_movePivot.CustomDisable(true);
        }

        /// <summary>
        /// init the targets inspected and store them.
        /// This is called at the first OnInspectorGUI()
        /// Not in the constructor ! we don't have targets information in the constructor
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
        {
            _meshFilterHiddedTools = ConcretTarget.GetOrAddComponent<MeshFilterHiddedTools>();

            //_movePivot.Init(ConcretTarget.transform, (MeshFilter)ConcretTarget, _meshFilterHiddedTools);
        }

        /// <summary>
        /// This function is called at each OnInspectorGUI
        /// </summary>
        protected override void OnCustomInspectorGUI()
        {
            if (ConcretTargets.Length < 2)
            {
                //_movePivot.CustomOnInspectorGUI();
            }
        }

        /// <summary>
        /// this function is called on the first OnSceneGUI
        /// </summary>
        /// <param name="sceneview"></param>
        protected override void InitOnFirstOnSceneGUI(SceneView sceneview)
        {
            //_movePivot.InitOnFirstOnSceneGUI();
        }

        /// <summary>
        /// this function is called at each sceneGUI draw
        /// </summary>
        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            //_movePivot.CustomOnSceneGUI();
        }

        /// <summary>
        /// get called at the first editorUpdate
        /// </summary>
        protected override void InitOnFirstEditorUpdate()
        {

        }

        /// <summary>
        /// this function is called at each EditorUpdate() when we need it
        /// </summary>
        protected override void OnEditorUpdate()
        {

            //_movePivot.CustomOnEditorApplicationUpdate();
        }
    }
}


    */