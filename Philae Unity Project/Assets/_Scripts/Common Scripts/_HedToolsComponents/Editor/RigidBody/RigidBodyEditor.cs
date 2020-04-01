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
    [CustomEditor(typeof(Rigidbody))]
    public class RigidBodyEditor : DecoratorComponentsEditor
    {
        private RigidBodyInternalProperties _internalProperties = new RigidBodyInternalProperties();

        public RigidBodyEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.RigidbodyEditor)
        {

        }

        /// <summary>
        /// need to clean when quitting
        /// </summary>
        public override void OnCustomDisable()
        {
            _internalProperties.CustomDisable();
        }

        /// <summary>
        /// this function is called on the first OnSceneGUI
        /// </summary>
        /// <param name="sceneview"></param>
        protected override void InitOnFirstOnSceneGUI(SceneView sceneview)
        {
            _internalProperties.InitOnFirstOnSceneGUI();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (_internalProperties.IsSpecialSettingsActive())
            {
                _internalProperties.CustomOnSceneGUI();
            }
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
        }

        /// <summary>
        /// This function is called at each OnInspectorGUI
        /// </summary>
        protected override void OnCustomInspectorGUI()
        {
            GUILayout.Label("Related RigidBody Extensions:");
            if (ExtComponentAddition.AddComponentsExtension<RigidBodyAdditionalMonobehaviourSettings>("Internal Variable", this.GetGameObject(), out bool justCreated, out bool justDestroyed))
            {
                _internalProperties.DisplayInternalProperties(justCreated, justDestroyed);
            }
            ExtComponentAddition.AddComponentsExtension<SleepyBody>("Fall asleep", this.GetGameObject(), out justCreated, out justDestroyed);
        }
    }
}
