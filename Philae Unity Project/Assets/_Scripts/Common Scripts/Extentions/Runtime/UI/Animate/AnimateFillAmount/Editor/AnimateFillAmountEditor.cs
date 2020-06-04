using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.runtime.animate.fillamount
{
    [CustomEditor(typeof(AnimateFillAmount))]
    public class AnimateFillAmountEditor : Editor
    {
        private AnimateFillAmount _animate;

        private void OnEnable()
        {
            _animate = (AnimateFillAmount)target;
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