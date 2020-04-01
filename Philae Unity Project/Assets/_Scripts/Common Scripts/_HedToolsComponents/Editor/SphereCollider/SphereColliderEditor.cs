using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;


/// <summary>
///
/// </summary>
namespace extUnityComponents.collider
{
    [CustomEditor(typeof(SphereCollider))]
    [CanEditMultipleObjects]
    public class SphereColliderEditor : DecoratorComponentsEditor
    {
        private FitSphere _fitSphere = new FitSphere();

        public SphereColliderEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.SphereColliderEditor)
        {

        }

        /// <summary>
        /// init the targets
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
        {
            _fitSphere.Init(GetTargets<SphereCollider>(), this);
        }

        protected override void OnCustomInspectorGUI()
        {
            _fitSphere.DisplayFitMesh();
        }
    }
}
