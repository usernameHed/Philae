using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.runtime.animate
{
    [CustomEditor(typeof(AnimateTextAmount))]
    public class AnimateTextAmountEditor : Editor
    {
        private AnimateTextAmount _animate;

        private void OnEnable()
        {
            _animate = (AnimateTextAmount)target;
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