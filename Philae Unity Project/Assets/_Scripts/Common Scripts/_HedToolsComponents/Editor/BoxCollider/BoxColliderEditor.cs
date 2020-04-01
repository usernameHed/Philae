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
    [CustomEditor(typeof(BoxCollider))]
    [CanEditMultipleObjects]
    public class BoxColliderEditor : DecoratorComponentsEditor
    {
        private FitBox _fitBox = new FitBox();

        public BoxColliderEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.BoxColliderEditor)
        {

        }

        /// <summary>
        /// init the targets
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
        {
            _fitBox.Init(GetTargets<BoxCollider>(), this);
        }

        protected override void OnCustomInspectorGUI()
        {
            _fitBox.DisplayFitMesh();
        }
    }
}
