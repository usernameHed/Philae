using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.runtime.animate.scale
{
    [CustomEditor(typeof(AnimateScale))]
    public class AnimateScaleEditor : Editor
    {
        private AnimateScale _animate;

        private void OnEnable()
        {
            _animate = (AnimateScale)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            OnCustomInspectorGUI();
        }

        private void OnCustomInspectorGUI()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                if (GUILayout.Button("Play"))
                {
                    _animate.Animate();
                }
            }
        }
    }
}