using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor
{
    [CustomEditor(typeof(SelectOtherOnEnable))]
    public class SelectOtherOnEnableEditor : Editor
    {
        private SelectOtherOnEnable _target;

        private void OnEnable()
        {
            _target = (SelectOtherOnEnable)target;
            Transform otherTarget = serializedObject.FindProperty("_otherTarget").GetValue<Transform>();
            bool isActive = serializedObject.FindProperty("_isActive").GetValue<bool>();
            if (ExtPrefabs.IsEditingInPrefabMode(_target.gameObject) || !isActive)
            {
                return;
            }
            Selection.activeGameObject = otherTarget.gameObject;
            Repaint();
        }
    }
}