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
    [CustomEditor(typeof(Rigidbody))]
    public class RigidBodyEditor : DecoratorComponentsEditor
    {
        //private PreviewRenderUtility m_PreviewUtility;
        private RigidBodyInternalProperties _internalProperties = new RigidBodyInternalProperties();
        private RigidBodyCenterOfMass _centerOfMass = new RigidBodyCenterOfMass();

        public RigidBodyEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.RigidbodyEditor)
        {

        }

        /// <summary>
        /// need to clean when quitting
        /// </summary>
        public override void OnCustomDisable()
        {
            _centerOfMass.CustomDisable();
        }

        /// <summary>
        /// this function is called on the first OnSceneGUI
        /// </summary>
        /// <param name="sceneview"></param>
        protected override void InitOnFirstOnSceneGUI(SceneView sceneview)
        {
            _centerOfMass.InitOnFirstOnSceneGUI();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            _centerOfMass.CustomOnSceneGUI();
        }

        /// <summary>
        /// This is called at the first OnInspectorGUI()
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
        {
            Rigidbody current = GetTarget<Rigidbody>();
            if (current == null || ExtPrefabs.IsEditingInPrefabMode(current.gameObject))
            {
                return;
            }

            _internalProperties.Init(GetTarget<Rigidbody>(), this);
            _centerOfMass.Init(current, this);
        }

        /// <summary>
        /// This function is called at each OnInspectorGUI
        /// </summary>
        protected override void OnCustomInspectorGUI()
        {
            _internalProperties.DisplayInternalProperties();
            if (targets.Length < 2)
            {
                _centerOfMass.CustomOnInspectorGUI();
            }
        }

        
    }
}
