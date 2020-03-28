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
namespace ExtUnityComponents.collider
{
    [CustomEditor(typeof(BoxCollider2D))]
    [CanEditMultipleObjects]
    public class BoxCollider2DEditor : DecoratorComponentsEditor
    {
        private FitBox2D _fitBox = new FitBox2D();

        public BoxCollider2DEditor()
            : base(BUILT_IN_EDITOR_COMPONENTS.BoxCollider2DEditor)
        {

        }

        /// <summary>
        /// init the targets
        /// </summary>
        protected override void InitOnFirstInspectorGUI()
        {
            _fitBox.Init(GetTargets<BoxCollider2D>(), this);
        }

        protected override void OnCustomInspectorGUI()
        {
            _fitBox.DisplayFitMesh();
        }
    }
}
