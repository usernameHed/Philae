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
    [CustomEditor(typeof(CapsuleCollider))]
    [CanEditMultipleObjects]
    public class CapsuleColliderEditor : DecoratorComponentsEditor
    {
        private FitCapsule _fitCapsule = new FitCapsule();

        public CapsuleColliderEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.CapsuleColliderEditor)
        {

        }

        /// <summary>
        /// init the targets
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
        {
            _fitCapsule.Init(GetTargets<CapsuleCollider>(), this);
        }

        protected override void OnCustomInspectorGUI()
        {
            _fitCapsule.DisplayFitMesh();
        }
    }
}
