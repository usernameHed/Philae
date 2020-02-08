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
namespace ExtUnityComponents
{
    [CustomEditor(typeof(BoxCollider))]
    [CanEditMultipleObjects]
    public class BoxColliderEditor : DecoratorComponentsEditor
    {
        private BoxColliderHiddedTools _boxColliderHiddedTools;
        private FitMesh _fitMesh = new FitMesh();

        public BoxColliderEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.BoxColliderEditor)
        {

        }

        /// <summary>
        /// init the targets
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
        {
           _boxColliderHiddedTools = ConcretTarget.GetOrAddComponent<BoxColliderHiddedTools>();

            _fitMesh.Init(GetTargets<BoxCollider>(), this);
        }

        protected override void OnCustomInspectorGUI()
        {
            _fitMesh.DisplayFitMesh();
        }
    }
}
