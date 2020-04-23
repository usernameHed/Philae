using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.runtime.animate.shake
{
    [CustomEditor(typeof(AnimateShake))]
    public class AnimateShakeEditor : Editor
    {
        private AnimateShake _animate;

        private void OnEnable()
        {
            _animate = (AnimateShake)target;
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